// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK
{
    /// <summary>
    /// Event arguments containing a device token.
    /// </summary>
    public class TokenRefreshEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the device token.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRefreshEventArgs"/> class.
        /// </summary>
        /// <param name="token">The device token.</param>
        public TokenRefreshEventArgs(string token)
        {
            Token = token;
        }
    }
}