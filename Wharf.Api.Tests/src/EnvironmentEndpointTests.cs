using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Wharf.Api.Data;
using Wharf.Api.Models;

namespace Wharf.Api.Tests;

public class EnvironmentEndpointTests : ApiTestBase
{
    public EnvironmentEndpointTests(WharfWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetEnvironmentReturnsEnvironment()
    {
        var env = new DeployEnvironment() { Name = "test-env" };
        await AddEnvironmentAsync(env);

        var response = await _client.GetAsync("/environments/test-env");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseEnv = await response.Content.ReadFromJsonAsync<DeployEnvironment>();
        
        Assert.NotNull(responseEnv);
        Assert.Equal("test-env", responseEnv.Name);
    }

    [Fact]
    public async Task GetEnvironmentReturnsErrorIfEnvironmentDoesNotExist() 
    {
        var response = await _client.GetAsync($"/environments/test-env");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateEnvironmentReturnsCreated()
    {
        var request = new CreateCustomEnvironmentRequest() { Name = "new-environment" };

        var response = await _client.PostAsJsonAsync("/environments", request);

        //Try to figure out what the internal server error was, if it happens
        // if (!response.IsSuccessStatusCode)
        // {
        //     var errorContent = await response.Content.ReadAsStringAsync();
        //     throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
        // }


        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseEnv = await response.Content.ReadFromJsonAsync<DeployEnvironment>();

        Assert.NotNull(responseEnv);
        Assert.Equal("new-environment", responseEnv.Name);
    }

    [Fact]
    public async Task CreateEnvironmentReturnsBadRequestForDuplicateName()
    {
        var  request = new CreateCustomEnvironmentRequest{ Name = "test-env" };

        // // First creation should succeed
        var firstResponse = await _client.PostAsJsonAsync(
            "/environments",
            request
        );

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Second creation with the same name should fail
        var secondResponse = await _client.PostAsJsonAsync(
            "/environments",
            request
        );
        
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    // Helpers
    private async Task AddEnvironmentAsync(DeployEnvironment environment)
    {
        using var scope = _factory.Services.CreateScope();

        var database = scope.ServiceProvider
            .GetRequiredService<WharfDbContext>();

        database.Add(environment);

        await database.SaveChangesAsync();
    }
}
