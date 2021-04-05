// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Diagnostics;
using MvvmHelpers;
using Notifo.SDK;
using Notifo.SDK.PushEventProvider;
using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public ObservableRangeCollection<NotificationDto> Notifications { get; private set; } = new ObservableRangeCollection<NotificationDto>() { };

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NotifoIO.Current.OnNotificationReceived += Current_OnNotificationReceived;

            RefreshEventsAsync();
        }

        protected override void OnDisappearing()
        {
            NotifoIO.Current.OnNotificationReceived -= Current_OnNotificationReceived;

            base.OnDisappearing();
        }

        private void Current_OnNotificationReceived(object sender, NotificationDataEventArgs e)
        {
            RefreshEventsAsync();
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            RefreshEventsAsync();
        }

        private void RefreshEventsAsync()
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var notifications = await NotifoIO.Current.Notifications.GetNotificationsAsync();
                    Notifications.ReplaceRange(notifications.Items);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    RefreshView.IsRefreshing = false;
                }
            });
        }
    }
}
