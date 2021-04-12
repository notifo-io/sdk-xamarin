// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using System.Threading;
using Notifo.SDK.Extensions;
using Notifo.SDK.Helpers;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.Services;
using Serilog;

namespace Notifo.SDK
{
    /// <summary>
    /// Notifo service implementation.
    /// </summary>
    public static partial class NotifoIO
    {
        private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current service implementation to use.
        /// </summary>
        public static INotifoMobilePush Current => Instance.Value;

        private static INotifoMobilePush SetupNotifoMobilePush()
        {
            Log.Logger = ConfigureLogger();

            var settings = new Settings();

            return new NotifoMobilePushImplementation(HttpClientFactory, settings);
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

        private static HttpClient HttpClientFactory() => new HttpClient(new NotifoHttpMessageHandler());
    }
}
