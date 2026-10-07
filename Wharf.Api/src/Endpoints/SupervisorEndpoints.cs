using Wharf.Api.K8s;

namespace Wharf.Api.Endpoints;

public static class ClusterEndpoints
{
    public static IEndpointRouteBuilder MapClusterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clusters/{clusterCtx}/health", GetSupervisorHealth);
        return app;
    }

    private static async Task<IResult> GetSupervisorHealth(
        string clusterCtx,
        IClusterAccess clusterAccess,
        CancellationToken cancellationToken)
    {
        var health = await clusterAccess.GetHealthAsync(clusterCtx, cancellationToken);
        return Results.Ok(health);
    }
}
