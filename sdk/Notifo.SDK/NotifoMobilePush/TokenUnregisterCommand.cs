// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed class TokenUnregisterCommand : ICommand
    {
        public string Token { get; set; }

        public async ValueTask ExecuteAsync()
        {
            await NotifoIO.Current.MobilePush.DeleteMyTokenAsync(Token);
        }

        public bool Merge(ICommand other)
        {
            return other is TokenUnregisterCommand;
        }
    }
}
