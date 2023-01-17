// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Notifo.SDK;
using Notifo.SDK.FirebasePlugin;
using Xamarin.Forms;

namespace Sample
{
    public static class NotifoStartup
    {
        public static INotifoMobilePush UseDefaults(this INotifoMobilePush notifo)
        {
            notifo
                .SetSharedName("group.io.notifo.xamarin.sample")
                .SetBaseUrl(Constants.ApiUrl)
                .SetApiVersion(ApiVersion.Version_1_5)
                .UseFirebasePluginEventsProvider();

            notifo.OnNotificationOpened += Current_OnNotificationOpened;
            notifo.OnNotificationReceived += Notifo_OnNotificationReceived;
            notifo.OnLog += Notifo_OnLog;

            return notifo;
        }

        private static void Notifo_OnLog(object sender, NotificationLogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Notification log message: " + e.Message);
        }

        private static void Notifo_OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Notification received: " + e.Notification.Subject);
        }

        private static void Current_OnNotificationOpened(object source, NotificationEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.MainPage?.DisplayAlert("Notification opened", e.Notification.Subject, "OK");
            });
        }
    }
}
