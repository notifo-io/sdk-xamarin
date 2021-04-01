// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Notifo.SDK.Extensions
{
    public static class NotificationDtoExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NotificationDto notification)
        {
            var data = new Dictionary<string, string>();

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

            return data;
        }
    }
}
