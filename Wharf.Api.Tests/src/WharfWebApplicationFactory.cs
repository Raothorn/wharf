using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Wharf.Api.Data;
using Wharf.Api.K8s;

public class WharfWebApplicationFactory
    : WebApplicationFactory<Program>
{
    // Keep the name stable across requests, but isolate parallel test classes.
    private readonly string _databaseName = $"WharfTests-{Guid.NewGuid()}";

    public FakeClusterAccess ClusterAccess { get; } = new();

    public async Task ResetAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<WharfDbContext>();

        await database.Database.EnsureDeletedAsync();
        await database.Database.EnsureCreatedAsync();
        ClusterAccess.Healthy = false;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {        
            
            // Remove the existing IClusterAccess registration and replace it with the fake implementation
            services.RemoveAll<IClusterAccess>();
            services.AddSingleton<IClusterAccess>(ClusterAccess);


            services.RemoveAll<WharfDbContext>();
            services.RemoveAll<DbContextOptions<WharfDbContext>>();
            services.RemoveAll<
                IDbContextOptionsConfiguration<WharfDbContext>
            >();

            services.AddDbContext<WharfDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    _databaseName
                );
            });
            
        });
    }
}
