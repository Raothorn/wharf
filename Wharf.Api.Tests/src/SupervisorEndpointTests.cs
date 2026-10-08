using System.Net;
using System.Net.Http.Json;
using Wharf.Api.Models;

namespace Wharf.Api.Tests;

public class SupervisorEndpointTests : ApiTestBase
{
    /// <summary>
    /// Connects the tests to the shared application fixture and per-test reset lifecycle.
    /// </summary>
    public SupervisorEndpointTests(WharfWebApplicationFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Verifies that an unhealthy probe is reported in an HTTP 200 response.
    /// </summary>
    [Fact]
    public async Task UnhealthySupervisorReturnsUnhealthy()
    {
        Factory.ClusterAccess.Healthy = false;

        var response = await Client.GetAsync("/clusters/test-cluster/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var health = await response.Content.ReadFromJsonAsync<ClusterHealth>();

        Assert.NotNull(health);
        Assert.False(health.Healthy);
    }

    /// <summary>
    /// Verifies that a healthy probe is reported in an HTTP 200 response.
    /// </summary>
    [Fact]
    public async Task HealthySupervisorReturnsHealthy()
    {
        Factory.ClusterAccess.Healthy = true;

        var response = await Client.GetAsync("/clusters/test-cluster/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var health = await response.Content.ReadFromJsonAsync<ClusterHealth>();

        Assert.NotNull(health);
        Assert.True(health.Healthy);
    }
}
