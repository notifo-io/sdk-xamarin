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
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Caching.Memory;
using Notifo.SDK.Extensions;
using Notifo.SDK.Resources;
using Serilog;
using UserNotifications;
using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : NSObject
    {
        private readonly IMemoryCache imageCache = new MemoryCache(new MemoryCacheOptions());

        public async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            Log.Debug(Strings.ReceivedNotification, request.Content.UserInfo);

            var userInfo = request.Content.UserInfo.ToDictionary();

            if (userInfo.TryGetValue(Constants.TrackingUrlKey, out var trackingUrl))
            {
                var notificationId = Guid.Empty;
                if (userInfo.TryGetValue(Constants.IdKey, out var id))
                {
                    notificationId = Guid.Parse(id);
                }

                await TrackNotificationAsync(notificationId, trackingUrl);
            }

            await EnrichNotificationContentAsync(bestAttemptContent, userInfo);
        }

        public async Task DidReceivePullRefreshRequestAsync()
        {
            var notifications = await GetPendingNotificationsAsync();
            foreach (var notification in notifications)
            {
                _ = ShowLocalNotificationAsync(notification);
            }

            await TrackNotificationsAsync(notifications);
        }

        private async Task ShowLocalNotificationAsync(NotificationDto notification)
        {
            var content = new UNMutableNotificationContent();
            content = await EnrichNotificationContentAsync(content, notification.ToDictionary());

            var request = UNNotificationRequest.FromIdentifier(notification.Id.ToString(), content, trigger: null);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    Log.Debug(error.LocalizedDescription);
                }
            });
        }

        private async Task<UNMutableNotificationContent> EnrichNotificationContentAsync(UNMutableNotificationContent content, Dictionary<string, string> data)
        {
            if (data.TryGetValue(Constants.SubjectKey, out var subject))
            {
                content.Title = subject;
            }

            if (data.TryGetValue(Constants.BodyKey, out var body))
            {
                content.Body = body;
            }

            if (data.TryGetValue(Constants.ImageLargeKey, out var imageLarge))
            {
                var imagePath = await GetImageAsync(imageLarge);
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    var attachement = UNNotificationAttachment.FromIdentifier(
                        Constants.ImageLargeKey,
                        NSUrl.FromFilename(imagePath),
                        new UNNotificationAttachmentOptions(),
                        out var error);

                    if (error == null)
                    {
                        content.Attachments = new UNNotificationAttachment[] { attachement };
                    }
                    else
                    {
                        Log.Error(error.LocalizedDescription);
                    }
                }
            }

            var actions = new List<UNNotificationAction>();
            var userInfo = new NSMutableDictionary();

            if (data.ContainsKey(Constants.ConfirmUrlKey) && data.ContainsKey(Constants.ConfirmTextKey))
            {
                userInfo.Add(new NSString(Constants.ConfirmUrlKey), new NSString(data[Constants.ConfirmUrlKey]));

                var confirmAction = UNNotificationAction.FromIdentifier(
                    Constants.ConfirmAction,
                    data[Constants.ConfirmTextKey],
                    UNNotificationActionOptions.Foreground);

                actions.Add(confirmAction);
            }

            if (data.ContainsKey(Constants.LinkUrlKey) && data.ContainsKey(Constants.LinkTextKey))
            {
                userInfo.Add(new NSString(Constants.LinkUrlKey), new NSString(data[Constants.LinkUrlKey]));

                var linkAction = UNNotificationAction.FromIdentifier(
                    Constants.LinkAction,
                    data[Constants.LinkTextKey],
                    UNNotificationActionOptions.Foreground);

                actions.Add(linkAction);
            }

            if (actions.Any())
            {
                content.UserInfo = userInfo;

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
                    foreach (UNNotificationCategory category in allCategories)
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

                // without this call action buttons won't be added or updated
                _ = await UNUserNotificationCenter.Current.GetNotificationCategoriesAsync();

                content.CategoryIdentifier = categoryId;
            }

            return content;
        }

        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            string url = string.Empty;

            var userInfo = response.Notification.Request.Content.UserInfo.ToDictionary();

            switch (response.ActionIdentifier)
            {
                case Constants.ConfirmAction:
                    userInfo.TryGetValue(Constants.ConfirmUrlKey, out url);
                    break;
                case Constants.LinkAction:
                    userInfo.TryGetValue(Constants.LinkUrlKey, out url);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }

            completionHandler();
        }

        private async Task<string> GetImageAsync(string imageUrl)
        {
            try
            {
                if (imageCache.TryGetValue(imageUrl, out string imagePath) && File.Exists(imagePath))
                {
                    return imagePath;
                }

                var uri = new Uri(imageUrl);
                var imageByteArray = await httpClient.GetByteArrayAsync(uri);

                imagePath = Path.Combine(FileSystem.CacheDirectory, uri.Segments.Last());
                File.WriteAllBytes(imagePath, imageByteArray);

                imageCache.Set(imageUrl, imagePath);

                return imagePath;
            }
            catch (Exception ex)
            {
                Log.Error(Strings.DownloadImageError, ex);
                return string.Empty;
            }
        }
    }
}
