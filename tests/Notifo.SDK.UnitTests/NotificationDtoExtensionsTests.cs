// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Notifo.SDK.Extensions;
using Xunit;

namespace Notifo.SDK.UnitTests
{
    public class NotificationDtoExtensionsTests
    {
        [Fact]
        public void ToDictionary_ShouldPopulateDictionary()
        {
            var id = Guid.NewGuid();
            var body = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr";
            var confirmText = "Got It!";
            var confirmUrl = "https://confirm.notifo.com";
            var imageLarge = "https://via.placeholder.com/600";
            var imageSmall = "https://via.placeholder.com/100";
            var isConfirmed = true;
            var linkText = "Go to link";
            var linkUrl = "https://app.notifo.io";
            var silent = true;
            var subject = "subject1";
            var trackingToken = "tracking/token";
            var trackingUrl = "https://track.notifo.com/seen";

            var notification = new UserNotificationDto
            {
                Id = id,
                Body = body,
                ConfirmText = confirmText,
                ConfirmUrl = confirmUrl,
                ImageLarge = imageLarge,
                ImageSmall = imageSmall,
                IsConfirmed = isConfirmed,
                LinkText = linkText,
                LinkUrl = linkUrl,
                Silent = silent,
                Subject = subject,
                TrackingToken = trackingToken,
                TrackSeenUrl = trackingUrl
            };

            var dictionary = notification.ToDictionary();

            Assert.Equal(id.ToString(), dictionary[nameof(id)]);
            Assert.Equal(body, dictionary[nameof(body)]);
            Assert.Equal(confirmText, dictionary[nameof(confirmText)]);
            Assert.Equal(confirmUrl, dictionary[nameof(confirmUrl)]);
            Assert.Equal(imageLarge, dictionary[nameof(imageLarge)]);
            Assert.Equal(imageSmall, dictionary[nameof(imageSmall)]);
            Assert.Equal(isConfirmed.ToString(), dictionary[nameof(isConfirmed)]);
            Assert.Equal(linkText, dictionary[nameof(linkText)]);
            Assert.Equal(linkUrl, dictionary[nameof(linkUrl)]);
            Assert.Equal(silent.ToString(), dictionary[nameof(silent)]);
            Assert.Equal(subject, dictionary[nameof(subject)]);
            Assert.Equal(trackingToken, dictionary[nameof(trackingToken)]);
            Assert.Equal(trackingUrl, dictionary[nameof(trackingUrl)]);
        }

        [Fact]
        public void FromDictionary_ShouldPopulateDto()
        {
            var id = Guid.NewGuid();
            var body = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr";
            var confirmText = "Got It!";
            var confirmUrl = "https://confirm.notifo.com";
            var imageLarge = "https://via.placeholder.com/600";
            var imageSmall = "https://via.placeholder.com/100";
            var isConfirmed = true;
            var linkText = "Go to link";
            var linkUrl = "https://app.notifo.io";
            var silent = true;
            var subject = "subject1";
            var trackingToken = "tracking/token";
            var trackingUrl = "https://track.notifo.com/seen";

            var dictionary = new Dictionary<string, object>
            {
                [nameof(id)] = id.ToString(),
                [nameof(body)] = body,
                [nameof(confirmText)] = confirmText,
                [nameof(confirmUrl)] = confirmUrl,
                [nameof(imageLarge)] = imageLarge,
                [nameof(imageSmall)] = imageSmall,
                [nameof(isConfirmed)] = isConfirmed.ToString(),
                [nameof(linkText)] = linkText,
                [nameof(linkUrl)] = linkUrl,
                [nameof(silent)] = silent,
                [nameof(subject)] = subject,
                [nameof(trackingToken)] = trackingToken,
                [nameof(trackingUrl)] = trackingUrl
            };

            var notification = new UserNotificationDto().FromDictionary(dictionary);

            Assert.Equal(id, notification.Id);
            Assert.Equal(body, notification.Body);
            Assert.Equal(confirmText, notification.ConfirmText);
            Assert.Equal(confirmUrl, notification.ConfirmUrl);
            Assert.Equal(imageLarge, notification.ImageLarge);
            Assert.Equal(imageSmall, notification.ImageSmall);
            Assert.Equal(isConfirmed, notification.IsConfirmed);
            Assert.Equal(linkText, notification.LinkText);
            Assert.Equal(linkUrl, notification.LinkUrl);
            Assert.Equal(silent, notification.Silent);
            Assert.Equal(subject, notification.Subject);
            Assert.Equal(trackingToken, notification.TrackingToken);
            Assert.Equal(trackingUrl, notification.TrackSeenUrl);
        }
    }
}