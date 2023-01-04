// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Notifo.SDK.Helpers
{
    internal sealed class NotifoHttpMessageHandler : PolicyHttpMessageHandler
    {
        private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
            HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));

        public NotifoHttpMessageHandler()
            : base(RetryPolicy)
        {
            InnerHandler = new HttpClientHandler();
        }
    }
}
