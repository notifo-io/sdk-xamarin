// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Notifo.SDK
{
    public class NotificationDataEventArgs
    {
        public IDictionary<string, object> Data { get; }

        public NotificationDataEventArgs(IDictionary<string, object> data)
        {
            Data = data;
        }
    }
}
