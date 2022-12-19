// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Extensions;
using Notifo.SDK.Helpers;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.Services;
using Plugin.Connectivity;
using Serilog;

namespace Notifo.SDK
{
    /// <summary>
    /// Notifo service implementation.
    /// </summary>
    public static partial class NotifoIO
    {
        /// <summary>
        /// Current service implementation to use.
        /// </summary>
        public static INotifoMobilePush Current { get; } = SetupNotifoMobilePush();

        private static INotifoMobilePush SetupNotifoMobilePush()
        {
            Log.Logger = ConfigureLogger();

            var settings = new Settings();

            var commandQueue = new DefaultCommandQueue(
                settings,
                new ICommandTrigger[]
                {
                    new TriggerOnStart(TimeSpan.FromSeconds(10)),
                    new TriggerPeriodically(TimeSpan.FromMinutes(10), CrossConnectivity.Current),
                    new TriggerWhenConnected(CrossConnectivity.Current)
                },
                10, TimeSpan.FromSeconds(5));

            return new NotifoMobilePushImplementation(HttpClientFactory, settings, commandQueue, settings, settings);
        }

        private static ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.PlatformSink()
                .CreateLogger();
        }

        private static HttpClient HttpClientFactory()
        {
            return new HttpClient(new NotifoHttpMessageHandler());
        }
    }
}
