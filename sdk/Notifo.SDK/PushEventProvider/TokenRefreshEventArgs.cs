// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK
{
    public class TokenRefreshEventArgs : EventArgs
    {
        public string Token { get; set; }

        public TokenRefreshEventArgs(string token)
        {
            Token = token;
        }
    }
}
