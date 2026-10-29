using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Klaviyo.HttpClients.Abstract;

namespace Soenneker.Klaviyo.OpenApiClientUtil.Tests;

internal sealed class TestHttpClient(HttpClient client) : IKlaviyoOpenApiHttpClient
{
    public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(client);
    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
