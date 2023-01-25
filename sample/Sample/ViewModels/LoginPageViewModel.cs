// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using Prism.Commands;
using Prism.Mvvm;

namespace Sample.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly INotifoMobilePush notifoService;
        private string apiKey = Constants.UserApiKey;

        public string ApiKey
        {
            get => apiKey;
            set => SetProperty(ref apiKey, value);
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand LogoutCommand { get; }

        public LoginPageViewModel(INotifoMobilePush notifoService)
        {
            this.notifoService = notifoService;

            LoginCommand = new DelegateCommand(Login, () => !string.IsNullOrWhiteSpace(ApiKey)).ObservesProperty(() => ApiKey);
            LogoutCommand = new DelegateCommand(Logout, () => !string.IsNullOrWhiteSpace(ApiKey)).ObservesProperty(() => ApiKey);
        }

        private void Login()
        {
            notifoService.SetApiKey(ApiKey);
            notifoService.Register();
        }

        private void Logout()
        {
            notifoService.Unregister();
            notifoService.WaitForBackgroundTasksAsync();
            notifoService.ClearAllSettings();
        }
    }
}
