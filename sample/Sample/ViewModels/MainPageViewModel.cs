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
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;

namespace Sample.ViewModels
{
    public class MainPageViewModel : BindableBase, IPageLifecycleAware
    {
        public ObservableRangeCollection<UserNotificationDto> Notifications { get; private set; } = new ObservableRangeCollection<UserNotificationDto>() { };

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public DelegateCommand RefreshCommand { get; set; }

        private readonly INotifoMobilePush notifoService;
        private readonly IDeviceService deviceService;

        public MainPageViewModel(INotifoMobilePush notifoService, IDeviceService deviceService)
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
                    var notifications = await notifoService.Notifications.GetMyNotificationsAsync();
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
