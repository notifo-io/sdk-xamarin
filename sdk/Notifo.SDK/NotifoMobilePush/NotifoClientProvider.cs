// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed class NotifoClientProvider
    {
        private const string CloudUrl = "https://app.notifo.io";
        private readonly Func<HttpClient> httpClientFactory;
        private readonly ICredentialsStore store;
        private readonly NotifoClientBuilder clientBuilder = NotifoClientBuilder.Create();
        private string? apiKey;
        private string apiUrl;
        private INotifoClient? clientInstance;

        public bool IsConfigured
        {
            get => !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiUrl);
        }

        public string? ApiKey
        {
            get => apiKey;
            set
            {
                if (apiKey != value)
                {
                    apiKey = value;

                    clientBuilder.SetApiKey(value);
                    clientInstance = null;

                    store.ApiKey = value;
                }
            }
        }

        public string ApiUrl
        {
            get => apiUrl;
            set
            {
                value = value.TrimEnd('/');

                if (apiUrl != value)
                {
                    apiUrl = value;

                    clientBuilder.SetApiUrl(value);
                    clientInstance = null;

                    store.ApiUrl = value;
                }
            }
        }

        public INotifoClient Client
        {
            get
            {
                if (clientInstance == null)
                {
                    clientBuilder.SetClient(CreateHttpClient());
                    clientInstance = clientBuilder.Build();
                }

                return clientInstance;
            }
        }

        public HttpClient CreateHttpClient()
        {
            return httpClientFactory();
        }

        public NotifoClientProvider(Func<HttpClient> httpClientFactory, ICredentialsStore store)
        {
            this.httpClientFactory = httpClientFactory;

            apiKey = store.ApiKey;
            apiUrl = store.ApiUrl ?? CloudUrl;

            this.store = store;
        }
    }
}
