using Microsoft.EntityFrameworkCore;
using Wharf.Api.Models;

namespace Wharf.Api.Data;

public sealed class WharfDbContext(DbContextOptions<WharfDbContext> options)
    : DbContext(options)
{
    public DbSet<DeployEnvironment> Environments => Set<DeployEnvironment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var environment = modelBuilder.Entity<DeployEnvironment>();

        environment.ToTable("Environments");
        environment.HasKey(x => x.Id);
        environment.HasIndex(x => x.Name).IsUnique();
    }

}
