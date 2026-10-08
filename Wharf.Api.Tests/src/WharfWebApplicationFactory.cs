using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wharf.Api.Data;
using Wharf.Api.K8s;
using Wharf.Api.Tests.Fakes;

namespace Wharf.Api.Tests;

public class WharfWebApplicationFactory
    : WebApplicationFactory<Program>
{
    // Keep the name stable across requests, but isolate parallel test classes.
    private readonly string _databaseName = $"WharfTests-{Guid.NewGuid()}";

    public FakeClusterAccess ClusterAccess { get; } = new();

    /// <summary>
    /// Recreates the fixture's in-memory database and restores the fake cluster's default health value.
    /// </summary>
    public async Task ResetAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<WharfDbContext>();

        await database.Database.EnsureDeletedAsync();
        await database.Database.EnsureCreatedAsync();
        ClusterAccess.Healthy = false;
    }

    /// <summary>
    /// Replaces Kubernetes access and PostgreSQL registrations with isolated test services.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IClusterAccess>();
            services.AddSingleton<IClusterAccess>(ClusterAccess);

            services.RemoveAll<WharfDbContext>();
            services.RemoveAll<DbContextOptions<WharfDbContext>>();
            // Remove the provider configuration too, so EF does not register both PostgreSQL and InMemory.
            services.RemoveAll<IDbContextOptionsConfiguration<WharfDbContext>>();

            services.AddDbContext<WharfDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
