// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Android.App;
using Android.Runtime;
using NotifoIO.SDK.FirebasePlugin;

namespace Sample.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
#if DEBUG
            NotifoFirebasePlugin.Initialize(this, resetToken: true);
#else
            NotifoFirebasePlugin.Initialize(this,false);
#endif
        }
    }
}