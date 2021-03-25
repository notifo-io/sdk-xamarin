// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace Notifo.SDK
{
    internal class HttpService : IHttpService
    {
        private readonly HttpClient httpClient;
        private readonly AsyncRetryPolicy retryPolicy;

        public HttpService()
        {
            httpClient = new HttpClient();

            retryPolicy = Policy.Handle<WebException>()
               .Or<HttpRequestException>()
               .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string apiKey)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            return retryPolicy.ExecuteAsync(() => httpClient.PostAsync(requestUri, content));
        }
    }
}
