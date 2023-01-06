// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.NotifoMobilePush;

internal interface ICredentialsStore
{
    string? ApiKey { get; set; }

    string? ApiUrl { get; set; }

    void Clear();
}
