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

namespace Notifo.SDK.Helpers
{
    internal sealed class NotifoOptions : INotifoOptions
    {
        private readonly ICredentialsStore store;

        public bool IsConfigured
        {
            get => !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(ApiUrl);
        }

        public string ApiUrl
        {
            get => store.ApiUrl ?? "https://app.notifo.io";
            set => store.ApiUrl = value;
        }

        public string ApiKey
        {
            get => store.ApiKey;
            set => store.ApiKey = value;
        }

        public string? ClientId => null;

        public string? ClientSecret => null;

        public TimeSpan Timeout => default(TimeSpan);

        public NotifoOptions(ICredentialsStore store)
        {
            this.store = store;
        }

        public void Validate()
        {
        }

        public DelegatingHandler Configure(DelegatingHandler inner)
        {
            var retryTimes = 3;
            var retryTime = TimeSpan.FromMilliseconds(300);

            inner.InnerHandler =
                new PolicyHttpMessageHandler(
                    HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(retryTimes, _ => retryTime))
                {
                    InnerHandler = new HttpClientHandler()
                };

            return inner;
        }

        public void Configure(HttpClient client)
        {
        }
    }
}