using Wharf.Api.Tasks;

namespace Wharf.Api.Endpoints;

public static class SupervisorEndpoints
{
    /// <summary>
    /// Registers the Supervisor health route for a Kubernetes context.
    /// </summary>
    public static IEndpointRouteBuilder MapSupervisorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clusters/{clusterContext}/health", GetSupervisorHealthAsync);
        return app;
    }

    /// <summary>
    /// Returns HTTP 200 with the Supervisor probe result, including an unhealthy result.
    /// </summary>
    private static async Task<IResult> GetSupervisorHealthAsync(
        string clusterContext,
        IClusterAccess clusterAccess,
        CancellationToken cancellationToken)
    {
        var health = await clusterAccess.GetHealthAsync(clusterContext, cancellationToken);
        return Results.Ok(health);
    }
}
