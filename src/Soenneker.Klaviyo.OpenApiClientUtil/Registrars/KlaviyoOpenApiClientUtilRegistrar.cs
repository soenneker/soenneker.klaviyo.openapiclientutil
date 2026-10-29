using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Klaviyo.HttpClients.Registrars;
using Soenneker.Klaviyo.OpenApiClientUtil.Abstract;

namespace Soenneker.Klaviyo.OpenApiClientUtil.Registrars;

/// <summary>
/// Registers the OpenAPI client utility for dependency injection.
/// </summary>
public static class KlaviyoOpenApiClientUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="KlaviyoOpenApiClientUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddKlaviyoOpenApiClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddKlaviyoOpenApiHttpClientAsSingleton()
                .TryAddSingleton<IKlaviyoOpenApiClientUtil, KlaviyoOpenApiClientUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="KlaviyoOpenApiClientUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddKlaviyoOpenApiClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddKlaviyoOpenApiHttpClientAsSingleton()
                .TryAddScoped<IKlaviyoOpenApiClientUtil, KlaviyoOpenApiClientUtil>();

        return services;
    }
}
