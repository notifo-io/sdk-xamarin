// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Notifo.SDK.PushEventProvider
{
    public class NotificationEventArgs
    {
        public Guid Id { get; }
        public string Subject { get; }
        public string? Body { get; }
        public string? ConfirmText { get; }
        public string? ConfirmUrl { get; }
        public string? ImageSmall { get; }
        public string? ImageLarge { get; }
        public string? TrackingUrl { get; }
        public string? LinkUrl { get; }
        public string? LinkText { get; }

        public NotificationEventArgs(IDictionary<string, object> data)
        {
            if (data.TryGetValue(Constants.IdKey, out var id))
            {
                Id = Guid.Parse(id.ToString());
            }

            if (data.TryGetValue(Constants.SubjectKey, out var subject))
            {
                Subject = subject.ToString();
            }
            else if (data.TryGetValue(Constants.ApsAlertTitleKey, out subject))
            {
                Subject = subject.ToString();
            }

            if (data.TryGetValue(Constants.BodyKey, out var body))
            {
                Body = body.ToString();
            }
            else if (data.TryGetValue(Constants.ApsAlertBodyKey, out body))
            {
                Body = body.ToString();
            }

            if (data.TryGetValue(Constants.ConfirmUrlKey, out var confirmUrl))
            {
                ConfirmUrl = confirmUrl.ToString();
            }

            if (data.TryGetValue(Constants.ConfirmTextKey, out var confirmText))
            {
                ConfirmText = confirmText.ToString();
            }

            if (data.TryGetValue(Constants.ImageSmallKey, out var imageSmall))
            {
                ImageSmall = imageSmall.ToString();
            }

            if (data.TryGetValue(Constants.ImageLargeKey, out var imageLarge))
            {
                ImageLarge = imageLarge.ToString();
            }

            if (data.TryGetValue(Constants.LinkUrlKey, out var linkUrl))
            {
                LinkUrl = linkUrl.ToString();
            }

            if (data.TryGetValue(Constants.LinkTextKey, out var linkText))
            {
                LinkText = linkText.ToString();
            }

            if (data.TryGetValue(Constants.TrackingUrlKey, out var trackingUrl))
            {
                TrackingUrl = trackingUrl.ToString();
            }
        }
    }
}
