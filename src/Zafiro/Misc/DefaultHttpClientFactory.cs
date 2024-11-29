using System;
using System.Net.Http;

namespace Zafiro.Misc;

public sealed class DefaultHttpClientFactory : IHttpClientFactory, IDisposable
{
    private readonly Lazy<HttpMessageHandler> handlerLazy = new(() => new HttpClientHandler());

    public HttpClient CreateClient(string name) => new(handlerLazy.Value, false);

    public void Dispose()
    {
        if (!handlerLazy.IsValueCreated)
        {
            return;
        }
        
        handlerLazy.Value.Dispose();
    }
}