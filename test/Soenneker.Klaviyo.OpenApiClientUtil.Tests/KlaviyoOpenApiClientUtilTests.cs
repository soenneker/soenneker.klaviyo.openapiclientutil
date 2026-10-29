using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Soenneker.Klaviyo.OpenApiClientUtil.Tests;

public sealed class KlaviyoOpenApiClientUtilTests
{
    [Test]
    public async Task UsesConfiguredBaseUrlAndCurrentCredentials()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Klaviyo:ApiKey"] = "first"
        }).Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://a.klaviyo.com/") };
        await using var utility = new KlaviyoOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        if (!ReferenceEquals(client, await utility.Get()))
            throw new Exception("The client was not cached.");

        await client.Api.Accounts.GetAsync();
        configuration["Klaviyo:ApiKey"] = "refreshed";
        await client.Api.Accounts.GetAsync();

        if (handler.Urls.Count != 2 || handler.Urls[0] != "https://a.klaviyo.com/api/accounts" ||
            handler.Authorizations[0] != "Klaviyo-API-Key first" || handler.Authorizations[1] != "Klaviyo-API-Key refreshed")
            throw new Exception("The https://a.klaviyo.com/api/accounts or credentials were incorrect.");
        if (handler.Revisions[0] != "2026-10-15" || handler.Accepts[0] != "application/vnd.api+json") throw new Exception("Klaviyo revision or JSON:API headers were missing.");
    }

    [Test]
    public async Task RejectsUntrustedHosts()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Klaviyo:ApiKey"] = "first" }).Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://a.klaviyo.com/") };
        await using var utility = new KlaviyoOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        try { await client.Api.Accounts.WithUrl("https://other.example/resource").GetAsync(); }
        catch (InvalidOperationException)
        {
            if (handler.Urls.Count != 0) throw new Exception("Credentials escaped to another host.");
            return;
        }
        throw new Exception("An untrusted host was accepted.");
    }

    [Test]
    public async Task RejectsMissingCredentialsBeforeSending()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://a.klaviyo.com/") };
        await using var utility = new KlaviyoOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        try { await client.Api.Accounts.GetAsync(); }
        catch (InvalidOperationException)
        {
            if (handler.Urls.Count != 0) throw new Exception("A request was sent without credentials.");
            return;
        }
        throw new Exception("Missing credentials were accepted.");
    }
}
