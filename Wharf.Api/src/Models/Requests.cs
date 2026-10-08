namespace Wharf.Api.Models;

public interface ICreateEnvironmentRequest
{
    /// <summary>
    /// Creates an unsaved environment from the request values.
    /// </summary>
    DeployEnvironment ToEntity();
}

public class CreateGesEnvironmentRequest : ICreateEnvironmentRequest
{
    public required string SiteCode { get; set; }
    public required string ClassificationCode { get; set; }

    /// <summary>
    /// Creates an unsaved environment with a name and Harbor hostname derived from the site codes.
    /// </summary>
    public DeployEnvironment ToEntity()
    {
        return new DeployEnvironment
        {
            SiteCode = SiteCode,
            ClassificationCode = ClassificationCode,
            Name = $"{SiteCode}-{ClassificationCode}",
            HarborUrl = $"harbor.bigsafari.{SiteCode}.usaf"
        };
    }
}

public class CreateCustomEnvironmentRequest : ICreateEnvironmentRequest
{
    public required string Name { get; set; }

    /// <inheritdoc />
    public DeployEnvironment ToEntity()
    {
        return new DeployEnvironment
        {
            Name = Name,
        };
    }
}
