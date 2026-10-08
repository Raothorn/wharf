using Wharf.Api.Models;
using Wharf.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;

namespace Wharf.Api.Endpoints;

public static class EnvironmentEndpoints
{
    public static IEndpointRouteBuilder MapEnvironmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/environments");
        group.MapGet("/{environmentName}", GetEnvironment);
        group.MapGet("/", ListEnvironments);
        group.MapPost("/", CreateCustomEnvironment);
        group.MapPatch("/{environmentName}", PatchEnvironment);
        return app;
    }

    private static async Task<IResult> GetEnvironment(
        string environmentName,
        WharfDbContext db,
        CancellationToken cancellationToken
    )
    {
        var env = await db.Environments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == environmentName, cancellationToken);

        if (env is null)
            return Results.NotFound();

        return Results.Ok(env);
    }

    private static async Task<IResult> ListEnvironments(
        WharfDbContext db,
        CancellationToken cancellationToken
    )
    {
        var envs = await db.Environments
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        return Results.Ok(envs);
    }

    private static async Task<IResult> CreateCustomEnvironment(
        CreateCustomEnvironmentRequest request,
        WharfDbContext db,
        CancellationToken cancellationToken)
    {
        var exists = await db.Environments.AnyAsync(x => x.Name == request.Name);

        if (exists)
        {
            return Results.Conflict(new { detail = "An environment with that name already exists" });
        }

        var environment = request.ToEntity();
        db.Environments.Add(environment);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { detail = ex.ToString() });
        }

        return Results.Json(environment, statusCode: StatusCodes.Status201Created);
    }

    // Method to patch an environment with any changes, indexed by Namme
    private static async Task<IResult> PatchEnvironment(
        string environmentName,
        JsonObject request,
        WharfDbContext db,
        CancellationToken cancellationToken
    )
    {
        var env = await db.Environments
            .SingleOrDefaultAsync(x => x.Name == environmentName, cancellationToken);

        if (env is null)
            return Results.NotFound();

        env.ApplyPatch(request);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(env);
    }
}
