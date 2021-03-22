// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Xamarin.Essentials;

namespace NotifoIO.SDK
{
    internal class Settings : ISettings
    {
        private static readonly string SharedName = $"{AppInfo.PackageName}.notifo";

        public string Token
        {
            get => Preferences.Get(nameof(Token), string.Empty, SharedName);
            set => Preferences.Set(nameof(Token), value, SharedName);
        }

        public bool IsTokenRefreshed
        {
            get => Preferences.Get(nameof(IsTokenRefreshed), false, SharedName);
            set => Preferences.Set(nameof(IsTokenRefreshed), value, SharedName);
        }
    }
}
