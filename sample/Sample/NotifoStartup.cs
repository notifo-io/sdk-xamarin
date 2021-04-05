// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using Notifo.SDK.FirebasePlugin;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.PushEventProvider;
using Xamarin.Forms;

namespace Sample
{
    public class NotifoStartup : INotifoStartup
    {
        public void ConfigureService(INotifoMobilePush notifo)
        {
            notifo
                .SetBaseUrl(Constants.ApiUrl)
                .SetApiKey(Constants.UserApiKey)
                .UseFirebasePluginEventsProvider()
                .Register();

            notifo.OnNotificationOpened += Current_OnNotificationOpened;
        }

        private void Current_OnNotificationOpened(object source, NotificationEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.MainPage?.DisplayAlert("Notification opened", e.Subject, "OK");
            });
        }
    }
}
