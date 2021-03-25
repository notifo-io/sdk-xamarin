// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using MvvmHelpers;
using Notifo.SDK;
using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public ObservableRangeCollection<EventDto> Events { get; private set; } = new ObservableRangeCollection<EventDto>() { };

        private readonly HttpClient httpClient;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("ApiKey", Constants.AdminApiKey);

            NotifoIO.Current.OnNotificationReceived += Current_OnNotificationReceived;
            NotifoIO.Current.OnNotificationOpened += Current_OnNotificationOpened;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RefreshEventsAsync();
        }

        private void Current_OnNotificationReceived(object sender, NotificationDataEventArgs e)
        {
            RefreshEventsAsync();
        }

        private void Current_OnNotificationOpened(object source, NotificationResponseEventArgs e)
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
                    string url = $"{Constants.ApiUrl}/api/apps/{Constants.AppId}/events/?query=users/{Constants.UserId}";
                    var response = await httpClient.GetAsync(url);

                    var eventsJson = await response.Content.ReadAsStringAsync();
                    var eventsList = JsonSerializer.Deserialize<ListResponseDtoOfEventDto>(eventsJson);

                    Events.ReplaceRange(eventsList.Items);
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
