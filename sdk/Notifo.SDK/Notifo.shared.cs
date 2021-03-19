// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using Serilog;

namespace NotifoIO.SDK
{
    public class Notifo
    {
        private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);
        public static INotifoMobilePush Current => Instance.Value;

        private static INotifoMobilePush SetupNotifoMobilePush()
        {
            ConfigureLogger();

            var httpService = new HttpService();
            return new NotifoMobilePush(httpService);
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
