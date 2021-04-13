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
    /// <summary>
    /// Event arguments containing notification data.
    /// </summary>
    public class NotificationEventArgs
    {
        /// <summary>
        /// The id of the notification.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The subject of the notification in the language of the user.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// The optional body text.
        /// </summary>
        public string? Body { get; }

        /// <summary>
        /// The text for the confirm button.
        /// </summary>
        public string? ConfirmText { get; }

        /// <summary>
        /// The tracking url that needs to be invoked to mark the notifiation as confirmed.
        /// </summary>
        public string? ConfirmUrl { get; }

        /// <summary>
        /// True when the notification has been confirmed.
        /// </summary>
        public bool IsConfirmed { get; }

        /// <summary>
        /// The optional link to the small image.
        /// </summary>
        public string? ImageSmall { get; }

        /// <summary>
        /// The optional link to the large image.
        /// </summary>
        public string? ImageLarge { get; }

        /// <summary>
        /// The tracking url that needs to be invoked to mark the notifiation as seen.
        /// </summary>
        public string? TrackingUrl { get; }

        /// <summary>
        /// An optional link.
        /// </summary>
        public string? LinkUrl { get; }

        /// <summary>
        /// The link text.
        /// </summary>
        public string? LinkText { get; }

        /// <summary>
        /// Optional data, usually a json object.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="notification">The notification DTO.</param>
        public NotificationEventArgs(NotificationDto notification)
        {
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
            Data = notification.Data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="data">The notification data dictionary.</param>
        public NotificationEventArgs(Dictionary<string, object> data)
            : this(new NotificationDto().FromDictionary(data))
        {
        }
    }
}
