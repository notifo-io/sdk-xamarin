// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Foundation;
using UIKit;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace Sample.iOS
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
