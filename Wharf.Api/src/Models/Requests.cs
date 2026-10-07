namespace Wharf.Api.Models;

public interface ICreateEnvironmentRequest 
{
    public DeployEnvironment ToEntity();
}

public class CreateGesEnvironmentRequest : ICreateEnvironmentRequest
{
    public required string SiteCode { get; set; }
    public required string ClassificationCode { get; set; }

    public DeployEnvironment ToEntity()
    {
        var environment = new DeployEnvironment() 
        { 
            SiteCode = SiteCode,
            ClassificationCode = ClassificationCode,
            Name = $"{SiteCode}-{ClassificationCode}",
            HarborUrl = $"harbor.bigsafari.{SiteCode}.usaf"
        };

        return environment;
    }
}

public class CreateCustomEnvironmentRequest : ICreateEnvironmentRequest
{
    public required string Name { get; set; }

    public DeployEnvironment ToEntity()
    {
        var environment = new DeployEnvironment() 
        {
            Name = Name,
        };
        return environment;
    }
}
