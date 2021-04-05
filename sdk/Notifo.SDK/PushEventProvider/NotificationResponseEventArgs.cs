// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Notifo.SDK.PushEventProvider
{
    public class NotificationResponseEventArgs
    {
        public string Subject { get; }
        public string? Body { get; }

        public NotificationResponseEventArgs(IDictionary<string, object> data)
        {
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
        }
    }
}
