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
            var isConfirmed = true;
            var imageLarge = "https://via.placeholder.com/600";
            var imageSmall = "https://via.placeholder.com/100";
            var linkUrl = "https://app.notifo.io";
            var linkText = "Go to link";
            var subject = "subject1";
            var silent = true;
            var trackingUrl = "https://track.notifo.com";

            var notification = new NotificationDto
            {
                Id = id,
                ConfirmUrl = confirmUrl,
                Body = body,
                ConfirmText = confirmText,
                ImageLarge = imageLarge,
                ImageSmall = imageSmall,
                LinkText = linkText,
                LinkUrl = linkUrl,
                Subject = subject,
                Silent = silent,
                IsConfirmed = isConfirmed,
                TrackingUrl = trackingUrl
            };

            var dictionary = notification.ToDictionary();

            Assert.Equal(dictionary[nameof(id)], id.ToString());
            Assert.Equal(dictionary[nameof(body)], body);
            Assert.Equal(dictionary[nameof(confirmText)], confirmText);
            Assert.Equal(dictionary[nameof(confirmUrl)], confirmUrl);
            Assert.Equal(dictionary[nameof(isConfirmed)], isConfirmed.ToString());
            Assert.Equal(dictionary[nameof(imageSmall)], imageSmall);
            Assert.Equal(dictionary[nameof(imageLarge)], imageLarge);
            Assert.Equal(dictionary[nameof(linkText)], linkText);
            Assert.Equal(dictionary[nameof(linkUrl)], linkUrl);
            Assert.Equal(dictionary[nameof(trackingUrl)], trackingUrl);
            Assert.Equal(dictionary[nameof(subject)], subject);
            Assert.Equal(dictionary[nameof(silent)], silent.ToString());
        }

        [Fact]
        public void FromDictionary_ShouldPopulateDto()
        {
            var id = Guid.NewGuid();
            var body = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr";
            var confirmText = "Got It!";
            var confirmUrl = "https://confirm.notifo.com";
            var isConfirmed = true;
            var imageLarge = "https://via.placeholder.com/600";
            var imageSmall = "https://via.placeholder.com/100";
            var linkUrl = "https://app.notifo.io";
            var linkText = "Go to link";
            var subject = "subject1";
            var silent = true;
            var trackingUrl = "https://track.notifo.com";

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

            var notification = new NotificationDto().FromDictionary(dictionary);

            Assert.Equal(notification.Id, id);
            Assert.Equal(notification.Body, body);
            Assert.Equal(notification.ConfirmText, confirmText);
            Assert.Equal(notification.ConfirmUrl, confirmUrl);
            Assert.Equal(notification.IsConfirmed, isConfirmed);
            Assert.Equal(notification.ImageSmall, imageSmall);
            Assert.Equal(notification.ImageLarge, imageLarge);
            Assert.Equal(notification.LinkText, linkText);
            Assert.Equal(notification.LinkUrl, linkUrl);
            Assert.Equal(notification.TrackingUrl, trackingUrl);
            Assert.Equal(notification.Subject, subject);
            Assert.Equal(notification.Silent, silent);
        }
    }
}
