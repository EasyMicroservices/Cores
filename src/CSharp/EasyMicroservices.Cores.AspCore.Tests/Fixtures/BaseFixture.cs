using EasyMicroservices.Cores.AspCore.Tests.Controllers;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.Fixtures;
using EasyMicroservices.Cores.Widgets;

namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class BaseFixture
{
    public static async Task<IServiceProvider> Init(int port, int? whiteLabelPort, Action<IServiceCollection> changeCollection = null, string microserviceName = "TestExample")
    {
        WebApplicationBuilder app = StartUpExtensions.Create<MyTestContext>(null);
        app.Services.Builder<MyTestContext>(microserviceName);
        //app.Services.AddTransient<IUnitOfWork>((serviceProvider) => new UnitOfWork(serviceProvider));
        app.Services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
        app.Services.AddTransient<IEntityFrameworkCoreDatabaseBuilder, DatabaseBuilder>();
        app.Services.AddSingleton(service => new WhiteLabelManager(service));
        app.Services.AddSingleton<IWidgetBuilder, EmptyWidgetBuilder>(); 
        app.Services.AddSingleton<IDatabaseWidgetManager, DatabaseWidgetManager>();
        app.Services.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
        {
            return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
        });
        changeCollection?.Invoke(app.Services);
        if (whiteLabelPort.HasValue)
            await BaseWhiteLabelFixture.Run(whiteLabelPort.Value);
        whiteLabelPort ??= 1041;
        UnitOfWork.ManualServiceAddresses = new List<ServiceAddressInfo>()
            {
                new ServiceAddressInfo()
                {
                    Name = "WhiteLabel",
                    Address = $"http://localhost:{whiteLabelPort}"
                }
            };
        app.Services.AddControllers().AddApplicationPart(typeof(UserController).Assembly);
        app.WebHost.UseUrls($"http://localhost:{port}");
        var build = await app.Build<MyTestContext>(true);
        build.MapControllers();
        _ = build.RunAsync();
        //await Task.Delay(TimeSpan.FromSeconds(2));
        return build.Services;
    }
}
