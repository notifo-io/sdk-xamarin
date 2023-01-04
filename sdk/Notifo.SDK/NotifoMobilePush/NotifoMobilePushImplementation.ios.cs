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
using Serilog;
using UserNotifications;
using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : NSObject
    {
        private INotificationHandler? notificationHandler;

        public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler)
        {
            this.notificationHandler = notificationHandler;
            return this;
        }

        public async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            Log.Debug(Strings.ReceivedNotification, request.Content.UserInfo);

            var notification = new UserNotificationDto().FromDictionary(request.Content.UserInfo.ToDictionary());

            await EnrichNotificationContentAsync(bestAttemptContent, notification);

            // Always track our notifications as seen.
            await TrackNotificationsAsync(notification);
        }

        public async Task DidReceivePullRefreshRequestAsync(PullRefreshOptions? options = null)
        {
            options ??= new PullRefreshOptions();

            // iOS does not maintain a queue of undelivered notifications, therefore we have to query here.
            var notifications = await GetPendingNotificationsAsync(options.Take, options.Period, default);

            foreach (var notification in notifications)
            {
                if (options.RaiseEvent)
                {
                    OnReceived(new NotificationEventArgs(notification));
                }

                if (notification.Silent)
                {
                    continue;
                }

                if (options.PresentNotification)
                {
                    await ShowLocalNotificationAsync(notification);
                }
            }

            await TrackNotificationsAsync(notifications.ToArray());
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

                Log.Debug(Strings.PendingNotificationsCount, notificationsPending.Length);

                return notificationsPending;
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.NotificationsRetrieveException, ex, this);
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
            var content = new UNMutableNotificationContent();

            content = await EnrichNotificationContentAsync(content, notification);
            content.UserInfo = notification.ToDictionary().ToNSDictionary();

            var request = UNNotificationRequest.FromIdentifier(notification.Id.ToString(), content, trigger: null);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    NotifoIO.Current.RaiseError(error.LocalizedDescription, null, this);
                }
            });
        }

        private async Task<UNMutableNotificationContent> EnrichNotificationContentAsync(UNMutableNotificationContent content, UserNotificationDto notification)
        {
            if (!string.IsNullOrWhiteSpace(notification.Subject))
            {
                content.Title = notification.Subject;
            }

            if (!string.IsNullOrWhiteSpace(notification.Body))
            {
                content.Body = notification.Body;
            }

            await AddImageAsync(content, notification);

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
                _ = await UNUserNotificationCenter.Current.GetNotificationCategoriesAsync();

                content.CategoryIdentifier = categoryId;
            }

            content.Sound ??= UNNotificationSound.Default;

            notificationHandler?.OnBuildNotification(content, notification);

            return content;
        }

        private async Task AddImageAsync(UNMutableNotificationContent content, UserNotificationDto notification)
        {
            var image = string.IsNullOrWhiteSpace(notification.ImageLarge) ? notification.ImageSmall : notification.ImageLarge;

            if (string.IsNullOrWhiteSpace(image))
            {
                return;
            }

            var imagePath = await GetImageAsync(image);

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            // The system needs a file extension here, but it actually does not matter if the extension matchs to the actual format.
            var attachmentName = $"{Guid.NewGuid()}.png";
            var attachmentUrl = new NSUrl(attachmentName, NSFileManager.DefaultManager.GetTemporaryDirectory());

            // The cache directory cannot be used.
            NSFileManager.DefaultManager.Copy(NSUrl.FromFilename(imagePath!), attachmentUrl, out var error);

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

        public void DidReceiveNotificationResponse(UNNotificationResponse response)
        {
            var userInfo = response.Notification.Request.Content.UserInfo.ToDictionary();

            object? value = default;

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

        private async Task<string?> GetImageAsync(string imageUrl)
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
                        using (var imageStream = await httpClient.GetStreamAsync(imageUrl))
                        {
                            await imageStream.CopyToAsync(fileStream);
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
    }
}
