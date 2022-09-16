// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Resources;
using Serilog;
using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed class TokenRegisterCommand : ICommand
    {
        private static int refreshCount;

        public string Token { get; set; }

        public async ValueTask ExecuteAsync(
            CancellationToken ct)
        {
            refreshCount++;

            try
            {
                Log.Debug(Strings.TokenRefreshStartExecutingCount, refreshCount);

                var request = new RegisterMobileTokenDto
                {
                    Token = Token
                };

                var platform = DeviceInfo.Platform;

                if (platform == DevicePlatform.Android)
                {
                    request.DeviceType = MobileDeviceType.Android;
                }

                if (platform == DevicePlatform.iOS)
                {
                    request.DeviceType = MobileDeviceType.IOS;
                }

                await NotifoIO.Current.MobilePush.PostMyTokenAsync(request, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Strings.TokenRefreshFailException);
                throw ex;
            }
            finally
            {
                Log.Debug(Strings.TokenRefreshEndExecutingCount, refreshCount);
            }
        }

        public bool Merge(ICommand other)
        {
            if (other is TokenRegisterCommand registerCommand)
            {
                Token = registerCommand.Token;
                return true;
            }

            return false;
        }
    }
}
