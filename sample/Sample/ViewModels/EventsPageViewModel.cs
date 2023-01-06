// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Diagnostics;
using MvvmHelpers;
using Notifo.SDK;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;

namespace Sample.ViewModels
{
    public class EventsPageViewModel : BindableBase, IPageLifecycleAware
    {
        private readonly INotifoMobilePush notifoService;
        private readonly IDeviceService deviceService;
        private bool isRefreshing;

        public ObservableRangeCollection<UserNotificationDto> Notifications { get; } = new ObservableRangeCollection<UserNotificationDto>();

        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public DelegateCommand RefreshCommand { get; }

        public EventsPageViewModel(INotifoMobilePush notifoService, IDeviceService deviceService)
        {
            this.notifoService = notifoService;
            this.deviceService = deviceService;

            RefreshCommand = new DelegateCommand(RefreshExecute);
        }

        public void OnAppearing()
        {
            notifoService.OnNotificationReceived += Current_OnNotificationReceived;
            RefreshEventsAsync();
        }

        public void OnDisappearing()
        {
            notifoService.OnNotificationReceived -= Current_OnNotificationReceived;
        }

        private void Current_OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            RefreshEventsAsync();
        }

        private void RefreshExecute()
        {
            RefreshEventsAsync();
        }

        private void RefreshEventsAsync()
        {
            deviceService.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var notifications = await notifoService.Client.Notifications.GetMyNotificationsAsync();

                    Notifications.ReplaceRange(notifications.Items);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
    }
}
