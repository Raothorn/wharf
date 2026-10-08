namespace Wharf.Api.Tests;

public abstract class ApiTestBase
    : IClassFixture<WharfWebApplicationFactory>, IAsyncLifetime
{
    protected WharfWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    /// <summary>
    /// Creates a test-owned HTTP client using the application factory shared by the test class.
    /// </summary>
    protected ApiTestBase(WharfWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Resets shared database and fake cluster state before each test.
    /// </summary>
    /// <remarks>
    /// xUnit invokes this on each test instance; initialization on the class fixture would run only once.
    /// </remarks>
    public Task InitializeAsync() => Factory.ResetAsync();

    /// <summary>
    /// Disposes this test's client, leaving the shared application factory available for subsequent tests.
    /// </summary>
    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}
