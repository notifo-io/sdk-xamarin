// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Notifo.SDK
{
#pragma warning disable RECS0001 // Class is declared partial but has only one part
    public static partial class NotifoIO
#pragma warning restore RECS0001 // Class is declared partial but has only one part
    {
        private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);
        public static INotifoMobilePush Current => Instance.Value;

        private static INotifoMobilePush SetupNotifoMobilePush()
        {
            Log.Logger = ConfigureLogger();

            var httpClient = ConfigureHttpClient();
            var settings = new Settings();

            return new NotifoMobilePush(httpClient, settings);
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

        private static HttpClient ConfigureHttpClient()
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));

            var handler = new PolicyHttpMessageHandler(retryPolicy)
            {
                InnerHandler = new HttpClientHandler()
            };

            return new HttpClient(handler);
        }
    }
}
