namespace Wharf.Api.Tests;

public abstract class ApiTestBase
    : IClassFixture<WharfWebApplicationFactory>, IAsyncLifetime
{
    protected readonly WharfWebApplicationFactory _factory;
    protected readonly HttpClient _client;

    protected ApiTestBase(WharfWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // xUnit calls this before every test, even though the factory is shared.
    public Task InitializeAsync() => _factory.ResetAsync();

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }
}
