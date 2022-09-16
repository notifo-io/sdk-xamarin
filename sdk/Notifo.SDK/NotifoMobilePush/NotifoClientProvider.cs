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
        private readonly Func<HttpClient> httpClientFactory;
        private readonly NotifoClientBuilder clientBuilder = NotifoClientBuilder.Create();
        private string? apiKey;
        private string apiUrl = "https://app.notifo.io";
        private INotifoClient? clientInstance;

        public string? ApiKey
        {
            get => apiKey;
            set
            {
                if (apiKey != value)
                {
                    apiKey = value;

                    clientBuilder.SetApiKey(apiKey);
                    clientInstance = null;
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

                    clientBuilder.SetApiUrl(apiUrl);
                    clientInstance = null;
                }
            }
        }

        public INotifoClient Client
        {
            get
            {
                if (clientInstance == null)
                {
                    var httpClient = httpClientFactory();

                    clientBuilder.SetClient(httpClient);
                    clientInstance = clientBuilder.Build();
                }

                return clientInstance;
            }
        }

        public NotifoClientProvider(Func<HttpClient> httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
    }
}
