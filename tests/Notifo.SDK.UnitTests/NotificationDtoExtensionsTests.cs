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
                TrackSeenUrl = trackingUrl
            };

            var dictionary = notification.ToDictionary();

            Assert.Equal(dictionary[nameof(id)], id.ToString());
            Assert.Equal(dictionary[nameof(body)], body);
            Assert.Equal(dictionary[nameof(confirmText)], confirmText);
            Assert.Equal(dictionary[nameof(confirmUrl)], confirmUrl);
            Assert.Equal(dictionary[nameof(imageLarge)], imageLarge);
            Assert.Equal(dictionary[nameof(imageSmall)], imageSmall);
            Assert.Equal(dictionary[nameof(isConfirmed)], isConfirmed.ToString());
            Assert.Equal(dictionary[nameof(linkText)], linkText);
            Assert.Equal(dictionary[nameof(linkUrl)], linkUrl);
            Assert.Equal(dictionary[nameof(silent)], silent.ToString());
            Assert.Equal(dictionary[nameof(subject)], subject);
            Assert.Equal(dictionary[nameof(trackingUrl)], trackingUrl);
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
            var trackingUrl = "https://track.notifo.com/seen";

            var dictionary = new Dictionary<string, object>
            {
                [nameof(id)] = id.ToString(),
                [nameof(body)] = body,
                [nameof(confirmText)] = confirmText,
                [nameof(confirmUrl)] = confirmUrl,
                [nameof(isConfirmed)] = isConfirmed.ToString(),
                [nameof(imageSmall)] = imageSmall,
                [nameof(imageLarge)] = imageLarge,
                [nameof(linkText)] = linkText,
                [nameof(linkUrl)] = linkUrl,
                [nameof(trackingUrl)] = trackingUrl,
                [nameof(subject)] = subject,
                [nameof(silent)] = silent
            };

            var notification = new UserNotificationDto().FromDictionary(dictionary);

            Assert.Equal(notification.Id, id);
            Assert.Equal(notification.Body, body);
            Assert.Equal(notification.ConfirmText, confirmText);
            Assert.Equal(notification.ConfirmUrl, confirmUrl);
            Assert.Equal(notification.ImageLarge, imageLarge);
            Assert.Equal(notification.ImageSmall, imageSmall);
            Assert.Equal(notification.IsConfirmed, isConfirmed);
            Assert.Equal(notification.LinkText, linkText);
            Assert.Equal(notification.LinkUrl, linkUrl);
            Assert.Equal(notification.Silent, silent);
            Assert.Equal(notification.Subject, subject);
            Assert.Equal(notification.TrackSeenUrl, trackingUrl);
        }
    }
}
