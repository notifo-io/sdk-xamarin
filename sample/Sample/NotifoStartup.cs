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
                .UseFirebasePluginEventsProvider();

            notifo.OnNotificationOpened += Current_OnNotificationOpened;
            notifo.OnLog += JustLog;

            return notifo;
        }

        private static void JustLog(object source, NotificationLogEventArgs e)
        {
            Console.WriteLine($"DEBUG: Log {e.Message} Message Args: {e.MessageArgs}", e.Message, e.MessageArgs);
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
