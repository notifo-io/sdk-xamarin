// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush;

internal static class Device
{
    private static string value;

    public static string DeviceIdentifier
    {
        get
        {
            if (value != null)
            {
                return value;
            }

            value = Guid.NewGuid().ToString();

            Preferences.Set(nameof(DeviceIdentifier), value);
            return value;
        }
    }
}
