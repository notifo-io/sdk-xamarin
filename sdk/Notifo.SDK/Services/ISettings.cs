// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace NotifoIO.SDK
{
    internal interface ISettings
    {
        string Token { get; set; }

        bool IsTokenRefreshed { get; set; }
    }
}
