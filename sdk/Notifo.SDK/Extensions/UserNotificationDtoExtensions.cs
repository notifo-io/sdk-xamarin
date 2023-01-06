// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.Extensions;

internal static class UserNotificationDtoExtensions
{
    public static Dictionary<string, string> ToDictionary(this UserNotificationDto notification)
    {
        var data = new Dictionary<string, string>
        {
            [Constants.IdKey] = notification.Id.ToString()
        };

        if (!string.IsNullOrWhiteSpace(notification.Subject))
        {
            data[Constants.SubjectKey] = notification.Subject;
        }

        if (!string.IsNullOrWhiteSpace(notification.Body))
        {
            data[Constants.BodyKey] = notification.Body;
        }

        if (!string.IsNullOrWhiteSpace(notification.ImageSmall))
        {
            data[Constants.ImageSmallKey] = notification.ImageSmall;
        }

        if (!string.IsNullOrWhiteSpace(notification.ImageLarge))
        {
            data[Constants.ImageLargeKey] = notification.ImageLarge;
        }

        if (!string.IsNullOrWhiteSpace(notification.ConfirmUrl))
        {
            data[Constants.ConfirmUrlKey] = notification.ConfirmUrl;
        }

        if (!string.IsNullOrWhiteSpace(notification.ConfirmText))
        {
            data[Constants.ConfirmTextKey] = notification.ConfirmText;
        }

        data[Constants.IsConfirmedKey] = notification.IsConfirmed.ToString();

        if (!string.IsNullOrWhiteSpace(notification.LinkUrl))
        {
            data[Constants.LinkUrlKey] = notification.LinkUrl;
        }

        if (!string.IsNullOrWhiteSpace(notification.LinkText))
        {
            data[Constants.LinkTextKey] = notification.LinkText;
        }

        data[Constants.SilentKey] = notification.Silent.ToString();

        if (!string.IsNullOrWhiteSpace(notification.TrackingToken))
        {
            data[Constants.TrackingTokenKey] = notification.TrackingToken;
        }

        if (!string.IsNullOrWhiteSpace(notification.TrackSeenUrl))
        {
            data[Constants.TrackSeenUrlKey] = notification.TrackSeenUrl;
        }

        if (!string.IsNullOrWhiteSpace(notification.Data))
        {
            data[Constants.DataKey] = notification.Data;
        }

        return data;
    }

    public static UserNotificationDto FromDictionary(this UserNotificationDto notification, Dictionary<string, object> data)
    {
        if (data.TryGetValue(Constants.IdKey, out var id) && Guid.TryParse(id.ToString(), out var guid))
        {
            notification.Id = guid;
        }

        if (data.TryGetValue(Constants.SubjectKey, out var subject))
        {
            notification.Subject = subject.ToString();
        }
        else if (data.TryGetValue(Constants.ApsAlertTitleKey, out subject))
        {
            notification.Subject = subject.ToString();
        }

        if (data.TryGetValue(Constants.BodyKey, out var body))
        {
            notification.Body = body.ToString();
        }
        else if (data.TryGetValue(Constants.ApsAlertBodyKey, out body))
        {
            notification.Body = body.ToString();
        }

        if (data.TryGetValue(Constants.ConfirmUrlKey, out var confirmUrl))
        {
            notification.ConfirmUrl = confirmUrl.ToString();
        }

        if (data.TryGetValue(Constants.ConfirmTextKey, out var confirmText))
        {
            notification.ConfirmText = confirmText.ToString();
        }

        if (data.TryGetValue(Constants.IsConfirmedKey, out var isConfirmed))
        {
            notification.IsConfirmed = Convert.ToBoolean(isConfirmed.ToString());
        }

        if (data.TryGetValue(Constants.ImageSmallKey, out var imageSmall))
        {
            notification.ImageSmall = imageSmall.ToString();
        }

        if (data.TryGetValue(Constants.ImageLargeKey, out var imageLarge))
        {
            notification.ImageLarge = imageLarge.ToString();
        }

        if (data.TryGetValue(Constants.LinkUrlKey, out var linkUrl))
        {
            notification.LinkUrl = linkUrl.ToString();
        }

        if (data.TryGetValue(Constants.LinkTextKey, out var linkText))
        {
            notification.LinkText = linkText.ToString();
        }

        if (data.TryGetValue(Constants.SilentKey, out var silent))
        {
            notification.Silent = Convert.ToBoolean(silent.ToString());
        }

        if (data.TryGetValue(Constants.TrackingTokenKey, out var trackingToken))
        {
            notification.TrackingToken = trackingToken.ToString();
        }

        if (data.TryGetValue(Constants.TrackSeenUrlKey, out var trackingUrl))
        {
            notification.TrackSeenUrl = trackingUrl.ToString();
        }

        if (data.TryGetValue(Constants.DataKey, out var notificationData))
        {
            notification.Data = notificationData.ToString();
        }

        return notification;
    }
}
