using k8s;
using Wharf.Api.Models;

namespace Wharf.Api.Tasks;

public interface IClusterAccess
{
    /// <summary>
    /// Probes the Kubernetes API for the named kubeconfig context and returns its health details.
    /// </summary>
    Task<ClusterHealth> GetHealthAsync(string contextName, CancellationToken cancellationToken);
}

public sealed class ClusterAccess : IClusterAccess
{
    /// <summary>
    /// Checks API reachability by listing at most one namespace; probe exceptions become unhealthy results.
    /// </summary>
    /// <remarks>
    /// This checks API access, including namespace-list permissions, rather than node or workload health.
    /// </remarks>
    public async Task<ClusterHealth> GetHealthAsync(
        string contextName,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = CreateClient(contextName);

            await client.CoreV1.ListNamespaceAsync(
                limit: 1,
                cancellationToken: cancellationToken);

            return new ClusterHealth(true, "Kubernetes API reachable");
        }
        catch (Exception exception)
        {
            return new ClusterHealth(false, exception.Message);
        }
    }

    /// <summary>
    /// Creates a client for a context in KUBECONFIG, falling back to the default kubeconfig path.
    /// The caller owns and must dispose the client.
    /// </summary>
    /// <exception cref="KubernetesException">The requested context is absent from the kubeconfig.</exception>
    private static Kubernetes CreateClient(string contextName)
    {
        var kubeconfigPath = System.Environment.GetEnvironmentVariable("KUBECONFIG")
            ?? KubernetesClientConfiguration.KubeConfigDefaultLocation;
        var kubeconfig = KubernetesClientConfiguration.LoadKubeConfig(kubeconfigPath);
        var contextExists = kubeconfig.Contexts.Any(context =>
            string.Equals(context.Name, contextName, StringComparison.Ordinal));

        if (!contextExists)
        {
            throw new KubernetesException($"The cluster context {contextName} does not exist");
        }

        var configuration = KubernetesClientConfiguration.BuildConfigFromConfigFile(
            kubeconfigPath,
            currentContext: contextName);

        return new Kubernetes(configuration);
    }
}
