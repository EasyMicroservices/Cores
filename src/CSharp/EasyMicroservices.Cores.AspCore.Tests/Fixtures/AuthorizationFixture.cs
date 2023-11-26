using EasyMicroservices.Cores.AspCoreApi.Interfaces;

namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class AuthorizationFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; }
    public Task InitializeAsync()
    {
        return BaseFixture.Init(4565, (sc) =>
        {
            sc.AddScoped<IAuthorization, AppAuthorization>();
        });
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}