using Microsoft.EntityFrameworkCore;
using Wharf.Api.Models;

namespace Wharf.Api.Data;

/// <summary>
/// Provides access to stored deployment environments using the supplied EF Core configuration.
/// </summary>
/// <param name="options">The database provider and context configuration.</param>
public sealed class WharfDbContext(DbContextOptions<WharfDbContext> options)
    : DbContext(options)
{
    public DbSet<DeployEnvironment> Environments => Set<DeployEnvironment>();

    /// <summary>
    /// Configures the environment table, primary key, and unique name index.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var environmentEntity = modelBuilder.Entity<DeployEnvironment>();

        environmentEntity.ToTable("Environments");
        environmentEntity.HasKey(environment => environment.Id);
        environmentEntity.HasIndex(environment => environment.Name).IsUnique();
    }
}
