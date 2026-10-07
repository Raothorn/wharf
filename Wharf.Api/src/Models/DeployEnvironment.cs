using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Wharf.Api.Models;

public sealed class DeployEnvironment
{
    public int Id { get; set; }

    public string? SiteCode { get; set; }
    public string? ClassificationCode { get; set; }

    // Global values
    public string? CaCrt { get; set; }
    public string? NtpServer { get; set; }

    // Kubernetes values
    public string? K8sNamespace { get; set; } = "ges-namespace";
    public string? K8sCluster { get; set; } = "ges-cluster";
    public string? K8sPodCidr { get; set; } = "172.69.0.0/16";
    public string? K8sServiceCidr { get; set; } = "172.169.0.0/16";
    public string? K8sVkrVersion { get; set; } = "1.32.0";
    public string? K8sStorageClass { get; set; } = "vsan_default_storage_class";

    // Control plane
    public int? K8sCpNodes { get; set; } = 3;
    public string? K8sCpVmClass { get; set; } = "guaranteed-large";

    // Workers
    public int? K8sWorkerNodes { get; set; } = 3;
    public string? K8sWorkerVmClass { get; set; } = "guaranteed-large";

    public required string Name { get; set; }

    public string? HarborUrl { get; set; } = null;

    public void ApplyPatch(
        JsonObject updates
    )
    {
        var properties = typeof(DeployEnvironment)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(
                p => p.Name,
                p => p,
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var (name, value) in updates)
        {
            if (!properties.TryGetValue(name, out var property))
                continue;

            if (property.Name == nameof(DeployEnvironment.Id))
                continue;

            var converted = value?.Deserialize(property.PropertyType);

            property.SetValue(this, converted);
        }
    }
}
