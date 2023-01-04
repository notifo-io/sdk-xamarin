﻿// ==========================================================================
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
                    DeviceIdentifier = Device.DeviceIdentifier,
                    DeviceType = GetDeviceType(),
                    Token = Token
                };

                await NotifoIO.Current.Client.MobilePush.PostMyTokenAsync(request, ct);
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.TokenRefreshFailException, ex, this);
                throw ex;
            }
            finally
            {
                Log.Debug(Strings.TokenRefreshEndExecutingCount, refreshCount);
            }
        }

        private static MobileDeviceType GetDeviceType()
        {
            var platform = DeviceInfo.Platform;

            if (platform == DevicePlatform.Android)
            {
                return MobileDeviceType.Android;
            }
            else if (platform == DevicePlatform.iOS)
            {
                return MobileDeviceType.IOS;
            }
            else
            {
                return MobileDeviceType.Unknown;
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
