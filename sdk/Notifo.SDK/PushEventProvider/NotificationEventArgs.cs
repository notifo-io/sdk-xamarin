// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Notifo.SDK.Extensions;

namespace Notifo.SDK.PushEventProvider
{
    public class NotificationEventArgs
    {
        public Guid Id { get; }
        public string Subject { get; }
        public string? Body { get; }
        public string? ConfirmText { get; }
        public string? ConfirmUrl { get; }
        public bool IsConfirmed { get; }
        public string? ImageSmall { get; }
        public string? ImageLarge { get; }
        public string? TrackingUrl { get; }
        public string? LinkUrl { get; }
        public string? LinkText { get; }

        public NotificationEventArgs(Dictionary<string, object> data)
        {
            var notification = new NotificationDto().FromDictionary(data);

            Id = notification.Id;
            Subject = notification.Subject;
            Body = notification.Body;
            ConfirmText = notification.ConfirmText;
            ConfirmUrl = notification.ConfirmUrl;
            IsConfirmed = notification.IsConfirmed;
            ImageSmall = notification.ImageSmall;
            ImageLarge = notification.ImageLarge;
            TrackingUrl = notification.TrackingUrl;
            LinkUrl = notification.LinkUrl;
            LinkText = notification.LinkText;
        }
    }
}
