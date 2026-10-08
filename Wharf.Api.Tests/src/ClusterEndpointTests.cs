using System.Net;
using System.Net.Http.Json;

using Wharf.Api.Models;

namespace Wharf.Api.Tests;

public class ClusterEndpointTests : ApiTestBase
{
    public ClusterEndpointTests(WharfWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task UnheathyClusterReturnsUnhealthy() 
    {
        _factory.ClusterAccess.Healthy = false;

        var response = await _client.GetAsync(
            "/clusters/test-cluster/health"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var health = await response.Content.ReadFromJsonAsync<ClusterHealth>();

        Assert.NotNull(health);
        Assert.False(health.Healthy);
    }

    [Fact]
    public async Task HealthyClusterReturnsHealthy() 
    {
        _factory.ClusterAccess.Healthy = true;

        var response = await _client.GetAsync(
            "/clusters/test-cluster/health"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var health = await response.Content.ReadFromJsonAsync<ClusterHealth>();

        Assert.NotNull(health);
        Assert.True(health.Healthy);
    }
}
