namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class NormalFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; }
    public Task InitializeAsync()
    {
        return BaseFixture.Init(4564,6042);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}