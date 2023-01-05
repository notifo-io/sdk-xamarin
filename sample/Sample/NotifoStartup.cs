// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using Notifo.SDK.FirebasePlugin;
using Xamarin.Forms;

namespace Sample
{
    public class NotifoStartup : INotifoStartup
    {
        public void ConfigureService(INotifoMobilePush notifo)
        {
            notifo
                .SetBaseUrl(Constants.ApiUrl)
                .SetApiVersion(ApiVersion.Version_1_5)
                .UseFirebasePluginEventsProvider();

            notifo.OnNotificationOpened += Current_OnNotificationOpened;
            notifo.OnNotificationReceived += Notifo_OnNotificationReceived;
            notifo.OnLog += Notifo_OnLog;
        }

        private void Notifo_OnLog(object sender, NotificationLogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Notification log message: " + e.Message);
        }

        private void Notifo_OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Notification received: " + e.Notification.Subject);
        }

        private void Current_OnNotificationOpened(object source, NotificationEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.MainPage?.DisplayAlert("Notification opened", e.Notification.Subject, "OK");
            });
        }
    }
}
