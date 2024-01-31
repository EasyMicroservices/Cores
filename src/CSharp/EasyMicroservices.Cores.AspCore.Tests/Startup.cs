using EasyMicroservices.Cores.AspCore.Tests.Controllers;
using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.ServiceContracts;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class Startup
    {
        static WebApplicationBuilder CreateBuilder(long port)
        {
            string microserviceName = "TestExample";
            var app = StartUpExtensions.Create<MyTestContext>(null);
            app.Services.Builder<MyTestContext>(microserviceName);
            app.Services.AddTransient<IUnitOfWork>((serviceProvider) => new UnitOfWork(serviceProvider));
            app.Services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
            app.Services.AddTransient<IEntityFrameworkCoreDatabaseBuilder, DatabaseBuilder>();
            app.Services.AddTransient(service => new WhiteLabelManager(service));
            app.Services.AddTransient<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
            {
                return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
            });
            UnitOfWork.ManualServiceAddresses = new List<ServiceAddressInfo>()
            {
                new ServiceAddressInfo()
                {
                    Name = "WhiteLabel",
                    Address = $"http://localhost:6041"
                }
            };
            app.Services.AddControllers().AddApplicationPart(typeof(UserController).Assembly);
            app.WebHost.UseUrls($"http://localhost:{port}");
            return app;
        }

        public static async Task Run(long port, Action<IServiceCollection> use, Action<IServiceProvider> serviceProvider)
        {
            var app = CreateBuilder(port);
            use?.Invoke(app.Services);
            var build = await app.Build<MyTestContext>(true);
            serviceProvider?.Invoke(build.Services);
            build.MapControllers();
            _ = build.RunAsync();
        }
    }
    public class AppAuthorization : IAuthorization
    {
        public Task<MessageContract> CheckIsAuthorized(HttpContext httpContext)
        {
            return Task.FromResult<MessageContract>((FailedReasonType.SessionAccessDenied, "AppAuthorization"));
        }

        public Task<bool> HasUnlimitedPermission(HttpContext httpContext)
        {
            return Task.FromResult(false);
        }
    }
}
