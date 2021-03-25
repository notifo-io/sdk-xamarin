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
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            NotifoIO.Current
                .SetBaseUrl(Constants.ApiUrl)
                .SetApiKey(Constants.UserApiKey)
                .UseFirebasePluginEventsProvider()
                .Register();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
