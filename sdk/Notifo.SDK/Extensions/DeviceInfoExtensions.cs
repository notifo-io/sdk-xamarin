// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Xamarin.Essentials;

namespace Notifo.SDK.Extensions
{
    internal static class DeviceInfoExtensions
    {
        public static MobileDeviceType ToMobileDeviceType(this DevicePlatform platform)
        {
            if (platform == DevicePlatform.Android)
            {
                return MobileDeviceType.Android;
            }

            if (platform == DevicePlatform.iOS)
            {
                return MobileDeviceType.IOS;
            }

            return MobileDeviceType.Unknown;
        }
    }
}
