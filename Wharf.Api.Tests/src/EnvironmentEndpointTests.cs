using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Wharf.Api.Data;
using Wharf.Api.Models;

namespace Wharf.Api.Tests;

public class EnvironmentEndpointTests : ApiTestBase
{
    /// <summary>
    /// Connects the tests to the shared application fixture and per-test reset lifecycle.
    /// </summary>
    public EnvironmentEndpointTests(WharfWebApplicationFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Verifies that a stored environment can be retrieved by name.
    /// </summary>
    [Fact]
    public async Task GetEnvironmentReturnsEnvironment()
    {
        var environment = new DeployEnvironment { Name = "test-env" };
        await AddEnvironmentAsync(environment);

        var response = await Client.GetAsync("/environments/test-env");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseEnvironment = await response.Content.ReadFromJsonAsync<DeployEnvironment>();

        Assert.NotNull(responseEnvironment);
        Assert.Equal("test-env", responseEnvironment.Name);
    }

    /// <summary>
    /// Verifies that looking up an absent environment returns HTTP 404.
    /// </summary>
    [Fact]
    public async Task GetEnvironmentReturnsNotFoundIfEnvironmentDoesNotExist()
    {
        var response = await Client.GetAsync("/environments/test-env");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Verifies that creating an environment returns HTTP 201 and the saved values.
    /// </summary>
    [Fact]
    public async Task CreateEnvironmentReturnsCreated()
    {
        var request = new CreateCustomEnvironmentRequest { Name = "new-environment" };

        var response = await Client.PostAsJsonAsync("/environments", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseEnvironment = await response.Content.ReadFromJsonAsync<DeployEnvironment>();

        Assert.NotNull(responseEnvironment);
        Assert.Equal("new-environment", responseEnvironment.Name);
    }

    /// <summary>
    /// Verifies that a second creation with the same name returns HTTP 409.
    /// </summary>
    [Fact]
    public async Task CreateEnvironmentReturnsConflictForDuplicateName()
    {
        var request = new CreateCustomEnvironmentRequest { Name = "test-env" };

        var firstResponse = await Client.PostAsJsonAsync("/environments", request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await Client.PostAsJsonAsync("/environments", request);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    /// <summary>
    /// Seeds an environment directly into the fixture database for lookup tests.
    /// </summary>
    private async Task AddEnvironmentAsync(DeployEnvironment environment)
    {
        using var scope = Factory.Services.CreateScope();

        var database = scope.ServiceProvider
            .GetRequiredService<WharfDbContext>();

        database.Add(environment);

        await database.SaveChangesAsync();
    }
}
