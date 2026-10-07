using Wharf.Api.K8s;
using Wharf.Api.Models;

public class FakeClusterAccess : IClusterAccess
{
    public bool Healthy { get; set; }

    public Task<ClusterHealth> GetHealthAsync(string context, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ClusterHealth(Healthy, ""));
    }
}
