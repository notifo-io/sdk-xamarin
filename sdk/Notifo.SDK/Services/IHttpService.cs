// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using System.Threading.Tasks;

namespace NotifoIO.SDK
{
    internal interface IHttpService
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string apiKey);
    }
}
