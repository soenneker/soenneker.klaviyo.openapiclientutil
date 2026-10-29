using Soenneker.Klaviyo.OpenApiClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Klaviyo.OpenApiClientUtil.Abstract;

/// <summary>
/// Exposes a cached OpenAPI client instance.
/// </summary>
public interface IKlaviyoOpenApiClientUtil: IDisposable, IAsyncDisposable
{
    /// <summary>Gets the cached client. Reads Klaviyo:ApiKey for every request and sends revision 2026-10-15 by default. Klaviyo:Revision may override the header; the generated models target 2026-10-15.</summary>
    ValueTask<KlaviyoOpenApiClient> Get(CancellationToken cancellationToken = default);
}
