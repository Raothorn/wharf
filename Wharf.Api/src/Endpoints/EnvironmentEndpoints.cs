using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Wharf.Api.Data;
using Wharf.Api.Models;

namespace Wharf.Api.Endpoints;

public static class EnvironmentEndpoints
{
    /// <summary>
    /// Registers the environment lookup, listing, creation, and patch routes.
    /// </summary>
    public static IEndpointRouteBuilder MapEnvironmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/environments");
        group.MapGet("/{environmentName}", GetEnvironmentAsync);
        group.MapGet("/", ListEnvironmentsAsync);
        group.MapPost("/", CreateCustomEnvironmentAsync);
        group.MapPatch("/{environmentName}", PatchEnvironmentAsync);
        return app;
    }

    /// <summary>
    /// Returns the environment with the requested name, or HTTP 404 if it does not exist.
    /// </summary>
    private static async Task<IResult> GetEnvironmentAsync(
        string environmentName,
        WharfDbContext database,
        CancellationToken cancellationToken)
    {
        var environment = await database.Environments
            .AsNoTracking()
            .SingleOrDefaultAsync(environment => environment.Name == environmentName, cancellationToken);

        if (environment is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(environment);
    }

    /// <summary>
    /// Returns all environments ordered by their database identifiers.
    /// </summary>
    private static async Task<IResult> ListEnvironmentsAsync(
        WharfDbContext database,
        CancellationToken cancellationToken)
    {
        var environments = await database.Environments
            .AsNoTracking()
            .OrderBy(environment => environment.Id)
            .ToListAsync(cancellationToken);

        return Results.Ok(environments);
    }

    /// <summary>
    /// Creates a named environment, returning HTTP 409 for an existing name
    /// or HTTP 400 if saving fails.
    /// </summary>
    private static async Task<IResult> CreateCustomEnvironmentAsync(
        CreateCustomEnvironmentRequest request,
        WharfDbContext database,
        CancellationToken cancellationToken)
    {
        var environmentExists = await database.Environments
            .AnyAsync(environment => environment.Name == request.Name);

        if (environmentExists)
        {
            return Results.Conflict(new { detail = "An environment with that name already exists" });
        }

        var environment = request.ToEntity();
        database.Environments.Add(environment);

        try
        {
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            return Results.BadRequest(new { detail = exception.ToString() });
        }

        return Results.Json(environment, statusCode: StatusCodes.Status201Created);
    }

    /// <summary>
    /// Applies supplied property values to a named environment, or returns HTTP 404 if absent.
    /// </summary>
    private static async Task<IResult> PatchEnvironmentAsync(
        string environmentName,
        JsonObject request,
        WharfDbContext database,
        CancellationToken cancellationToken)
    {
        var environment = await database.Environments
            .SingleOrDefaultAsync(environment => environment.Name == environmentName, cancellationToken);

        if (environment is null)
        {
            return Results.NotFound();
        }

        environment.ApplyPatch(request);
        await database.SaveChangesAsync(cancellationToken);

        return Results.Ok(environment);
    }
}
