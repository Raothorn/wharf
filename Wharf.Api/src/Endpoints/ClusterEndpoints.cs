using Wharf.Api.K8s;

namespace Wharf.Api.Endpoints;

public static class ClusterEndpoints
{
    /// <summary>
    /// Registers the health route for a Kubernetes context.
    /// </summary>
    public static IEndpointRouteBuilder MapClusterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clusters/{clusterContext}/health", GetClusterHealthAsync);
        return app;
    }

    /// <summary>
    /// Returns HTTP 200 with the cluster probe result, including an unhealthy result.
    /// </summary>
    private static async Task<IResult> GetClusterHealthAsync(
        string clusterContext,
        IClusterAccess clusterAccess,
        CancellationToken cancellationToken)
    {
        var health = await clusterAccess.GetHealthAsync(clusterContext, cancellationToken);
        return Results.Ok(health);
    }
}
