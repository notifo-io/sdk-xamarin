// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Foundation;
using Notifo.SDK.FirebasePlugin;
using Prism;
using Prism.Ioc;
using Sample.iOS.Shared;
using UIKit;
using UserNotifications;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace Sample.iOS
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new iOSInitializer()));

            NotifoFirebasePlugin.Initialize(launchOptions, new NotifoStartup(), new NotificationHandler(), true);
            UNUserNotificationCenter.Current.Delegate = this;

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            NotifoFirebasePlugin.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            NotifoFirebasePlugin.RemoteNotificationRegistrationFailed(error);
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public override async void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            await NotifoFirebasePlugin.DidReceiveMessageAsync(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler) =>
            NotifoFirebasePlugin.DidReceiveNotificationResponse(center, response, completionHandler);

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
        public class iOSInitializer : IPlatformInitializer
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                // Register any platform specific implementations
            }
        }
    }
}
