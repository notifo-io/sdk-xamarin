// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Notifo.SDK.NotifoMobilePush
{
    internal class NotifoClientProvider : INotifoClient
    {
        private bool rebuild;

        private string? apiKey;
        public string? ApiKey
        {
            get => apiKey;
            set
            {
                if (apiKey != value)
                {
                    apiKey = value;
                    clientBuilder.SetApiKey(apiKey);

                    rebuild = true;
                }
            }
        }

        private string apiUrl = "https://app.notifo.io";
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

                    rebuild = true;
                }
            }
        }

        private INotifoClient? client;
        private INotifoClient Client
        {
            get
            {
                if (client == null || rebuild)
                {
                    rebuild = false;

                    httpClient.DefaultRequestHeaders.Clear();
                    client = clientBuilder.Build();
                }

                return client;
            }
        }

        public IAppsClient Apps => Client.Apps;
        public IConfigsClient Configs => Client.Configs;
        public IEventsClient Events => Client.Events;
        public ILogsClient Logs => Client.Logs;
        public IMediaClient Media => Client.Media;
        public IMobilePushClient MobilePush => Client.MobilePush;
        public INotificationsClient Notifications => Client.Notifications;
        public ITemplatesClient Templates => Client.Templates;
        public ITopicsClient Topics => Client.Topics;
        public IUsersClient Users => Client.Users;

        private readonly HttpClient httpClient;
        private readonly NotifoClientBuilder clientBuilder;

        public NotifoClientProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;

            clientBuilder = NotifoClientBuilder
                .Create()
                .SetClient(httpClient);
        }
    }
}
