using Wharf.Api.Tasks;
using Wharf.Api.Models;

namespace Wharf.Api.Tests.Fakes;

public class FakeClusterAccess : IClusterAccess
{
    public bool Healthy { get; set; }

    /// <summary>
    /// Returns the configured health value without contacting Kubernetes.
    /// </summary>
    public Task<ClusterHealth> GetHealthAsync(string contextName, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ClusterHealth(Healthy, string.Empty));
    }
}
