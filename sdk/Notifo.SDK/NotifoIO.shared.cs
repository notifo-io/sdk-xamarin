// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using Serilog;

namespace Notifo.SDK
{
    public static class NotifoIO
    {
        private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);
        public static INotifoMobilePush Current => Instance.Value;

        private static INotifoMobilePush SetupNotifoMobilePush()
        {
            ConfigureLogger();

            var httpService = new HttpService();
            var settings = new Settings();

            return new NotifoMobilePush(httpService, settings);
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.PlatformSink()
                .CreateLogger();
        }
    }
}
