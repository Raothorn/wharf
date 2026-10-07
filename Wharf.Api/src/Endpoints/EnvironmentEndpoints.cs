using Wharf.Api.Models;
using Wharf.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using Npgsql;

namespace Wharf.Api.Endpoints;

public static class EnvironmentEndpoints
{
    public static IEndpointRouteBuilder MapEnvironmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/environments");
        group.MapGet("/{environmentId}", GetEnvironment);
        group.MapGet("/", ListEnvironments);
        group.MapPost("/", CreateCustomEnvironment);
        group.MapPatch("/{environmentId}", PatchEnvironment);
        return app;
    }

    private static async Task<IResult> CreateCustomEnvironment(
        CreateCustomEnvironmentRequest request,
        WharfDbContext db,
        CancellationToken cancellationToken)
    {
        var environment = request.ToEntity();
        db.Environments.Add(environment);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Results.Conflict(new
            {
                detail = "An environment with that name already exists"
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { detail = ex.ToString() });
        }

        return Results.Json(environment, statusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> GetEnvironment(
        int environmentId,
        WharfDbContext db,
        CancellationToken cancellationToken
    )
    {
        var env = await db.Environments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == environmentId, cancellationToken);

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
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        return Results.Ok(envs);
    }

    // Method to patch an environment with any changes, indexed by Namme
    private static async Task<IResult> PatchEnvironment(
        int environmentId,
        JsonObject request,
        WharfDbContext db,
        CancellationToken cancellationToken
    )
    {
        var env = await db.Environments
            .SingleOrDefaultAsync(x => x.Id == environmentId, cancellationToken);

        if (env is null)
            return Results.NotFound();

        env.ApplyPatch(request);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(env);
    }


    // private static async Task<IResult> GetEnvironment(
    //     int environmentId,
    //     WharfDbContext db,
    //     CancellationToken cancellationToken)
    // {
    //     var environment = await db.Environments
    //         .AsNoTracking()
    //         .SingleOrDefaultAsync(x => x.Id == environmentId, cancellationToken);
    //
    //     return environment is null
    //         ? Results.NotFound()
    //         : Results.Ok(environment);
    // }
}
