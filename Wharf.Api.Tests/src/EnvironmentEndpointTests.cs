using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Wharf.Api.Data;
using Wharf.Api.Models;

namespace Wharf.Api.Tests;

public class EnvironmentEndpointTests : IClassFixture<WharfWebApplicationFactory>
{
    private readonly WharfWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EnvironmentEndpointTests(WharfWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEnvironmentReturnsEnvironment()
    {
        // var response = await _client.GetAsync(
        //     "/environments/test-env"
        // );
        //
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //
        // var environment = await response.Content.ReadFromJsonAsync<DeployEnvironment>();
        //
        // Assert.NotNull(environment);
        // Assert.Equal("test-env", environment.Name);
    }

    [Fact]
    public async Task CreateEnvironmentReturnsCreated()
    {
        // var request = new CreateCustomEnvironmentRequest()
        // {
        //     Name = "new-environment",
        // };
        //
        // var response = await _client.PostAsJsonAsync(
        //     "/environments",
        //     request
        // );

        //Try to figure out what the internal server error was, if it happens
        // if (!response.IsSuccessStatusCode)
        // {
        //     var errorContent = await response.Content.ReadAsStringAsync();
        //     throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
        // }


        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateEnvironmentReturnsBadRequestForDuplicateName()
    {
        // var request = new CreateGesEnvironmentRequest()
        // {
        //     SiteCode = "mob",
        //     ClassificationCode = "unc"
        // };
        //
        // // First creation should succeed
        // var firstResponse = await _client.PostAsJsonAsync(
        //     "/environments",
        //     request
        // );
        //
        // Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        //
        // // Second creation with the same name should fail
        // var secondResponse = await _client.PostAsJsonAsync(
        //     "/environments",
        //     request
        // );
        //
        // Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
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
