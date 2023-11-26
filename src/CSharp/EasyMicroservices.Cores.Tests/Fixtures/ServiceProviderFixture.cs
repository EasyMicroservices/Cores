using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.Database;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace EasyMicroservices.Cores.Tests.Fixtures
{
    public class ServiceProviderFixture : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public ServiceProviderFixture()
        {

        }

        public async Task InitializeAsync()
        {
            var serviceCollection = new ServiceCollection();
            string microserviceName = "TestExample";
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
            serviceCollection.AddTransient<IEntityFrameworkCoreDatabaseBuilder, DatabaseBuilder>();
            serviceCollection.AddSingleton(service => new WhiteLabelManager(service));
            serviceCollection.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
            {
                return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
            });
            StartUpExtensions.AddWhiteLabelRoute(microserviceName, $"http://localhost:6041");
            serviceCollection.AddTransient<IBaseUnitOfWork, UnitOfWork>();
            ServiceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<MyTestContext>();
                dbbuilder.Initialize(context);
                using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>() as UnitOfWork;
                await uow.InitializeWhiteLabel(microserviceName, $"http://localhost:6041", typeof(MyTestContext)).ConfigureAwait(false);
            }
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
