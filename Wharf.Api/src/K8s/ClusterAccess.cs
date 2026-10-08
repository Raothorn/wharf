using k8s;
using Wharf.Api.Models;

namespace Wharf.Api.K8s;

public interface IClusterAccess 
{
    Task<ClusterHealth> GetHealthAsync(string context, CancellationToken cancellationToken);
}


public sealed class ClusterAccess : IClusterAccess
{
    private Kubernetes GetClient(string context)
    {
        var kubeconfigPath = System.Environment.GetEnvironmentVariable("KUBECONFIG")
            ?? KubernetesClientConfiguration.KubeConfigDefaultLocation;

        var kubeconfig = KubernetesClientConfiguration.LoadKubeConfig(kubeconfigPath);

        var exists = kubeconfig.Contexts.Any(ctx =>
            string.Equals(ctx.Name, context, StringComparison.Ordinal));

        if (!exists)
            throw new KubernetesException($"The cluster context {context} does not exist");

        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(
            kubeconfigPath,
            currentContext: context);

        return new Kubernetes(config);
    }

    public async Task<ClusterHealth> GetHealthAsync(
        string context,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = GetClient(context);

            // The Python version calls /healthz?verbose directly. The generated
            // Kubernetes .NET client does not expose that endpoint as a normal
            // typed API, so use a tiny API request as the connectivity/health probe.
            await client.CoreV1.ListNamespaceAsync(
                limit: 1,
                cancellationToken: cancellationToken);

            return new ClusterHealth(true, "Kubernetes API reachable");
        }
        catch (Exception ex)
        {
            return new ClusterHealth(false, ex.Message);
        }
    }
}
