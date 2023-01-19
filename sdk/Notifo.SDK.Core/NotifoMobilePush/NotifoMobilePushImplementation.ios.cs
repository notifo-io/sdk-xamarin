// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Notifo.SDK.Extensions;
using Notifo.SDK.Resources;
using UserNotifications;
using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : NSObject, InternalIOSPushAdapter
    {
        private PullRefreshOptions refreshOptions;
        private INotificationHandler notificationHandler;

        /// <inheritdoc />
        public INotifoMobilePush SetRefreshOptions(PullRefreshOptions refreshOptions)
        {
            this.refreshOptions = refreshOptions ?? new PullRefreshOptions();
            return this;
        }

        /// <inheritdoc />
        public INotifoMobilePush SetNotificationHandler(INotificationHandler notificationHandler)
        {
            this.notificationHandler = notificationHandler;
            return this;
        }

        /// <inheritdoc />
        public async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent content)
        {
            RaiseDebug(Strings.ReceivedNotification, this, request.Content.UserInfo);

            var notification = new UserNotificationDto().FromDictionary(request.Content.UserInfo.ToDictionary());

            // Do the tracking first.
            await TrackNotificationsAsync(notification);

            // We only have 30 seconds in this flow.
            await EnrichAsync(content, notification, TimeSpan.FromSeconds(20));

            // pull + track pending notifications
            await PullPendingNotificationsAndTrack();
        }

        /// <inheritdoc />
        public async Task DidReceivePullRefreshRequestAsync()
        {
            await PullPendingNotificationsAndTrack();
        }

        private async Task PullPendingNotificationsAndTrack()
        {
            // iOS does not maintain a queue of undelivered notifications, therefore we have to query here.
            var notifications = await GetPendingNotificationsAsync(refreshOptions.Take, refreshOptions.Period, default(CancellationToken));

            List<UserNotificationDto> trackImmediatly = null;

            foreach (var notification in notifications)
            {
                if (refreshOptions.RaiseEvent)
                {
                    OnReceived(new NotificationEventArgs(notification));
                }

                if (notification.Silent || !refreshOptions.PresentNotification)
                {
                    trackImmediatly = trackImmediatly == null ? new List<UserNotificationDto>() : trackImmediatly;
                    trackImmediatly.Add(notification);
                    continue;
                }

                ShowLocalNotificationAsync(notification).Forget();
            }

            if (trackImmediatly != null)
            {
                await TrackNotificationsAsync(trackImmediatly.ToArray());
            }
        }

        private async Task<IEnumerable<UserNotificationDto>> GetPendingNotificationsAsync(int take, TimeSpan maxAge,
            CancellationToken ct)
        {
            try
            {
                var after = DateTime.UtcNow.Subtract(maxAge);

                List<UserNotificationDto> notifications;

                if (ApiVersion == ApiVersion.Version_1_0)
                {
                    notifications = await GetPendingNotifications1_0Async(take, after, ct);
                }
                else
                {
                    try
                    {
                        notifications = await GetPendingNotifications1_4Async(take, after, ct);
                    }
                    catch (NotifoException ex) when (ex.StatusCode == 404)
                    {
                        notifications = await GetPendingNotifications1_0Async(take, after, ct);
                    }
                }

                if (notifications.Count == 0)
                {
                    return Enumerable.Empty<UserNotificationDto>();
                }

                var notificationsSeen = await GetSeenNotificationsAsync();
                var notificationsPending = notifications.Where(n => !notificationsSeen.Contains(n.Id)).OrderBy(x => x.Created).ToArray();

                RaiseDebug(Strings.PendingNotificationsCount, this, notificationsPending.Length);

                return notificationsPending;
            }
            catch (Exception ex)
            {
                RaiseError(Strings.NotificationsRetrieveException, ex, this);
            }

            return Array.Empty<UserNotificationDto>();
        }

        private async Task<List<UserNotificationDto>> GetPendingNotifications1_0Async(int take, DateTime after,
            CancellationToken ct)
        {
            var result = await Client.Notifications.GetMyNotificationsAsync(take: take, cancellationToken: ct);

            return result.Items.Where(x => x.Created >= after).ToList();
        }

        private async Task<List<UserNotificationDto>> GetPendingNotifications1_4Async(int take, DateTime after,
            CancellationToken ct)
        {
            var result = await Client.Notifications.GetMyDeviceNotificationsAsync(token, after, true, take * 2, ct);

            return result.Items;
        }

        private void OnReceived(NotificationEventArgs eventArgs)
        {
            OnNotificationReceived?.Invoke(this, eventArgs);
        }

        private async Task ShowLocalNotificationAsync(UserNotificationDto notification)
        {
            var content = new UNMutableNotificationContent
            {
                UserInfo = notification.ToDictionary().ToNSDictionary()
            };

            // We only have 30 seconds in the other flow, so we keep the time for consistency.
            await EnrichAsync(content, notification, TimeSpan.FromSeconds(20));

            var request = UNNotificationRequest.FromIdentifier(notification.Id.ToString(), content, trigger: null);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    NotifoIO.Current.RaiseError(error.LocalizedDescription, null, this);
                }
                else
                {
                    TrackNotificationsAsync(notification).Forget();
                }
            });
        }

        private async Task EnrichAsync(UNMutableNotificationContent content, UserNotificationDto notification, TimeSpan timeout)
        {
            // Give enough time for other operations.
            using (var cts = new CancellationTokenSource(timeout))
            {
                try
                {
                    // We have ony limited time in the notification service, so we dot things in the right order.
                    await EnrichBasicAsync(content, notification);
                    await EnrichImagesAsync(content, notification, cts.Token);
                    await EnrichWithCustomCodeAsync(content, notification, cts.Token);
                }
                catch (Exception ex)
                {
                    RaiseError(Strings.GeneralException, ex, this);
                }
            }
        }

        private async Task EnrichBasicAsync(UNMutableNotificationContent content, UserNotificationDto notification)
        {
            if (!string.IsNullOrWhiteSpace(notification.Subject))
            {
                content.Title = notification.Subject;
            }

            if (!string.IsNullOrWhiteSpace(notification.Body))
            {
                content.Body = notification.Body;
            }

            var actions = new List<UNNotificationAction>();

            if (!string.IsNullOrWhiteSpace(notification.ConfirmUrl) &&
                !string.IsNullOrWhiteSpace(notification.ConfirmText) &&
                !notification.IsConfirmed)
            {
                var confirmAction = UNNotificationAction.FromIdentifier(
                    Constants.ConfirmAction,
                    notification.ConfirmText,
                    UNNotificationActionOptions.Foreground);

                actions.Add(confirmAction);
            }

            if (!string.IsNullOrWhiteSpace(notification.LinkUrl) &&
                !string.IsNullOrWhiteSpace(notification.LinkText))
            {
                var linkAction = UNNotificationAction.FromIdentifier(
                    Constants.LinkAction,
                    notification.LinkText,
                    UNNotificationActionOptions.Foreground);

                actions.Add(linkAction);
            }

            if (actions.Any())
            {
                var categoryId = Guid.NewGuid().ToString();

                var newCategory = UNNotificationCategory.FromIdentifier(
                    categoryId,
                    actions.ToArray(),
                    new string[] { },
                    UNNotificationCategoryOptions.None);

                var categories = new List<UNNotificationCategory>();

                var allCategories = await UNUserNotificationCenter.Current.GetNotificationCategoriesAsync();
                if (allCategories != null)
                {
                    foreach (var category in allCategories.OfType<UNNotificationCategory>())
                    {
                        if (category.Identifier != categoryId)
                        {
                            categories.Add(category);
                        }
                    }

                    categories.Add(newCategory);
                }
                else
                {
                    categories.Add(newCategory);
                }

                UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories.ToArray()));

                // Without this call action buttons will not be added or updated.
                await UNUserNotificationCenter.Current.GetNotificationCategoriesAsync();

                content.CategoryIdentifier = categoryId;
            }

            if (content.Sound == null)
            {
                content.Sound = UNNotificationSound.Default;
            }
        }

        private async Task EnrichImagesAsync(UNMutableNotificationContent content, UserNotificationDto notification,
            CancellationToken ct)
        {
            var image = string.IsNullOrWhiteSpace(notification.ImageLarge) ? notification.ImageSmall : notification.ImageLarge;

            if (string.IsNullOrWhiteSpace(image))
            {
                return;
            }

            var imagePath = await GetImageAsync(image, ct);

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            // The system needs a file extension here, but it actually does not matter if the extension matchs to the actual format.
            var attachmentName = $"{Guid.NewGuid()}.png";
            var attachmentUrl = new NSUrl(attachmentName, NSFileManager.DefaultManager.GetTemporaryDirectory());

            // The cache directory cannot be used.
            NSFileManager.DefaultManager.Copy(NSUrl.FromFilename(imagePath), attachmentUrl, out var error);

            if (error != null)
            {
                NotifoIO.Current.RaiseError(error.LocalizedDescription, null, this);
                return;
            }

            var attachement = UNNotificationAttachment.FromIdentifier(
                Constants.ImageLargeKey,
                attachmentUrl,
                new UNNotificationAttachmentOptions(),
                out error);

            if (error != null || attachement == null)
            {
                NotifoIO.Current.RaiseError(error?.LocalizedDescription ?? "Unknown Error", null, this);
                return;
            }

            content.Attachments = new UNNotificationAttachment[] { attachement };
        }

        private async Task<string> GetImageAsync(string imageUrl,
            CancellationToken ct)
        {
            try
            {
                // Convert the hash of the url to create short files names. Base64 will create longer file names.
                var imagePath = Path.Combine(FileSystem.CacheDirectory, imageUrl.Sha256());

                if (File.Exists(imagePath))
                {
                    return imagePath;
                }

                // Let the server decide how the image should be delivered.
                imageUrl = imageUrl.AppendQueries("preset", "MobileIOS");

                // Copy directly from the web stream to the image stream to reduce memory allocations.
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    var httpClient = Client.CreateHttpClient();
                    try
                    {
                        var response = await httpClient.GetAsync(imageUrl, ct);

                        response.EnsureSuccessStatusCode();

                        using (var imageStream = await response.Content.ReadAsStreamAsync())
                        {
                            await imageStream.CopyToAsync(fileStream, ct);
                        }
                    }
                    finally
                    {
                        Client.ReturnHttpClient(httpClient);
                    }
                }

                return imagePath;
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.DownloadImageError, ex, this);
            }

            return null;
        }

        private async Task EnrichWithCustomCodeAsync(UNMutableNotificationContent content, UserNotificationDto notification,
            CancellationToken ct)
        {
            if (notificationHandler != null)
            {
                await notificationHandler.OnBuildNotificationAsync(content, notification, ct);
            }
        }

        /// <inheritdoc />
        public void DidReceiveNotificationResponse(UNNotificationResponse response)
        {
            var userInfo = response.Notification.Request.Content.UserInfo.ToDictionary();

            object value = null;

            switch (response.ActionIdentifier)
            {
                case Constants.ConfirmAction:
                    userInfo.TryGetValue(Constants.ConfirmUrlKey, out value);
                    break;
                case Constants.LinkAction:
                    userInfo.TryGetValue(Constants.LinkUrlKey, out value);
                    break;
            }

            var url = value?.ToString();

            if (!string.IsNullOrWhiteSpace(url))
            {
                Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}