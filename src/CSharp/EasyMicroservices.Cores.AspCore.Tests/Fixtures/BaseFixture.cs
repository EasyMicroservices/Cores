﻿using EasyMicroservices.Cores.AspCore.Tests.Controllers;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;

namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class BaseFixture
{
    public static async Task<IServiceProvider> Init(long port, Action<IServiceCollection> changeCollection = null)
    {
        string microserviceName = "TestExample";
        WebApplicationBuilder app = StartUpExtensions.Create<MyTestContext>(null);
        app.Services.Builder<MyTestContext>();
        app.Services.AddTransient<IUnitOfWork>((serviceProvider) => new UnitOfWork(serviceProvider));
        app.Services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
        app.Services.AddTransient<IEntityFrameworkCoreDatabaseBuilder, DatabaseBuilder>();
        app.Services.AddSingleton(service => new WhiteLabelManager(service));
        app.Services.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
        {
            return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
        });
        changeCollection?.Invoke(app.Services);
        StartUpExtensions.AddWhiteLabelRoute(microserviceName, $"http://localhost:6041");
        app.Services.AddControllers().AddApplicationPart(typeof(UserController).Assembly);
        app.WebHost.UseUrls($"http://localhost:{port}");
        var build = await app.Build<MyTestContext>(true);
        build.MapControllers();
        _ = build.RunAsync();
        return build.Services;
    }
}
