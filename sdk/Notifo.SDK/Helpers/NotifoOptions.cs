// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Notifo.SDK.NotifoMobilePush;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Notifo.SDK.Helpers
{
    internal sealed class NotifoOptions : INotifoOptions
    {
        private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
            HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));

        private readonly ICredentialsStore store;

        public bool IsConfigured
        {
            get => !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(ApiUrl);
        }

        public string? ApiUrl
        {
            get => store.ApiUrl ?? "https://cloud.squidex.io";
            set => store.ApiUrl = value;
        }

        public string? ApiKey
        {
            get => store.ApiKey;
            set => store.ApiKey = value;
        }

        public string? ClientId => default;

        public string? ClientSecret => default;

        public TimeSpan Timeout => default;

        public NotifoOptions(ICredentialsStore store)
        {
            this.store = store;
        }

        public void Validate()
        {
        }

        public HttpClient BuildHttpClient(DelegatingHandler handler)
        {
            handler.InnerHandler = new PolicyHttpMessageHandler(RetryPolicy)
            {
                InnerHandler = new HttpClientHandler()
            };

            return new HttpClient(handler);
        }
    }
}
