using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.ValueTask;
using Soenneker.Klaviyo.HttpClients.Abstract;
using Soenneker.Klaviyo.OpenApiClientUtil.Abstract;
using Soenneker.Klaviyo.OpenApiClient;
using Soenneker.Utils.AsyncSingleton;

namespace Soenneker.Klaviyo.OpenApiClientUtil;

public sealed class KlaviyoOpenApiClientUtil : IKlaviyoOpenApiClientUtil
{
    private readonly AsyncSingleton<KlaviyoOpenApiClient> _client;

    public KlaviyoOpenApiClientUtil(IKlaviyoOpenApiHttpClient httpClientUtil, IConfiguration configuration)
    {
        _client = new AsyncSingleton<KlaviyoOpenApiClient>(async token =>
        {
            HttpClient httpClient = await httpClientUtil.Get(token).NoSync();

            Uri baseAddress = httpClient.BaseAddress ?? throw new InvalidOperationException("The Klaviyo HTTP client requires a base address.");
            var authenticationProvider = new KlaviyoAuthenticationProvider(configuration, baseAddress);
            var requestAdapter = new HttpClientRequestAdapter(authenticationProvider, httpClient: httpClient)
            {
                BaseUrl = baseAddress.AbsoluteUri.TrimEnd('/')
            };
            return new KlaviyoOpenApiClient(requestAdapter);
        });
    }

    public ValueTask<KlaviyoOpenApiClient> Get(CancellationToken cancellationToken = default)
    {
        return _client.Get(cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _client.DisposeAsync();
    }
}

