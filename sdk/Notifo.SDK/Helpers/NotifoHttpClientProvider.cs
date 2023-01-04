// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using Notifo.SDK.Configuration;

namespace Notifo.SDK.Helpers
{
    internal sealed class NotifoHttpClientProvider : DefaultHttpClientProvider
    {
        public NotifoHttpClientProvider(INotifoOptions options)
            : base(options)
        {
        }

        public override HttpClient BuildHttpClient(HttpMessageHandler handler)
        {
            if (handler is DelegatingHandler delegatingHandler)
            {
                delegatingHandler.InnerHandler = new NotifoHttpMessageHandler();
            }

            return base.BuildHttpClient(handler);
        }
    }
}
