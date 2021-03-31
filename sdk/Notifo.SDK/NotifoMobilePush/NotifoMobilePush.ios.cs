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
using Notifo.SDK.Resources;
using Serilog;
using UserNotifications;
using Xamarin.Essentials;

namespace Notifo.SDK
{
    internal partial class NotifoMobilePush : NSObject, IUNUserNotificationCenterDelegate
    {
        private readonly IMemoryCache imageCache = new MemoryCache(new MemoryCacheOptions());

        public async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            var userInfo = request.Content.UserInfo.ToDictionary();

            if (userInfo.TryGetValue(Constants.TrackingUrlKey, out var trackingUrl))
            {
                await TrackNotificationAsync(trackingUrl);
            }

            await EnrichNotificationContentAsync(bestAttemptContent, userInfo);
        }

        public async Task DidReceivePullRefreshRequestAsync()
        {
            UNUserNotificationCenter.Current.Delegate = this;

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

            if (data.TryGetValue(Constants.ConfirmUrlKey, out var confirmUrl))
            {
                content.UserInfo = new NSDictionary(
                    new NSString(Constants.ConfirmUrlKey), new NSString(confirmUrl)
                );

                data.TryGetValue(Constants.ConfirmTextKey, out var confirmText);

                var confirmAction = UNNotificationAction.FromIdentifier(
                    Constants.ConfirmAction,
                    confirmText,
                    UNNotificationActionOptions.Foreground);

                var category = UNNotificationCategory.FromIdentifier(
                    Constants.ConfirmCategory,
                    new UNNotificationAction[] { confirmAction },
                    new string[] { },
                    UNNotificationCategoryOptions.None);

                var categories = new[] { category };
                UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

                // without this call action buttons won't be added or updated
                var registeredCategories = await UNUserNotificationCenter.Current.GetNotificationCategoriesAsync();

                content.CategoryIdentifier = Constants.ConfirmCategory;
            }

            return content;
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (response.ActionIdentifier == Constants.ConfirmAction)
            {
                var confirmUrl = response.Notification.Request.Content.UserInfo[Constants.ConfirmUrlKey].ToString();
                Browser.OpenAsync(confirmUrl, BrowserLaunchMode.SystemPreferred);
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
