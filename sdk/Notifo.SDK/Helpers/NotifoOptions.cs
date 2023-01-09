// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Http;
using Notifo.SDK.NotifoMobilePush;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Notifo.SDK.Helpers;

internal sealed class NotifoOptions : INotifoOptions
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
        HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));

    private readonly ICredentialsStore store;
    private string? apiUrl = "https://cloud.squidex.io";
    private string? apiKey;

    public bool IsConfigured
    {
        get => !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiUrl);
    }

    public string? ApiUrl
    {
        get => apiUrl;
        set
        {
            if (apiUrl != value)
            {
                apiUrl = value;
                store.ApiUrl = value;
            }
        }
    }

    public string? ApiKey
    {
        get => apiKey;
        set
        {
            if (apiKey != value)
            {
                apiKey = value;
                store.ApiKey = value;
            }
        }
    }

    public string? ClientId => default;

    public string? ClientSecret => default;

    public TimeSpan Timeout => default;

    public NotifoOptions(ICredentialsStore store)
    {
        this.store = store;

        apiUrl = store.ApiUrl;
        apiKey = store.ApiKey;
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
