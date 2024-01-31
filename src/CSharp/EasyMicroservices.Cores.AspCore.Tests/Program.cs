using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string microserviceName = "TestExample";
            var app = StartUpExtensions.Create<MyTestContext>(args);
            app.Services.Builder<MyTestContext>(microserviceName);
            app.Services.AddScoped((serviceProvider) => new UnitOfWork(serviceProvider).GetLongContractLogic<UserEntity, UserEntity, UserEntity, UserEntity>());
            app.Services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
            app.Services.AddScoped<IEntityFrameworkCoreDatabaseBuilder, DatabaseBuilder>();
            app.Services.AddSingleton(service => new WhiteLabelManager(service));
            app.Services.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
            {
                return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
            });
            //StartUpExtensions.AddWhiteLabel(microserviceName, "RootAddresses:WhiteLabel");
            var build = await app.Build<MyTestContext>();
            build.MapControllers();
            build.Run();
        }
    }

    public class DatabaseBuilder : EntityFrameworkCoreDatabaseBuilder
    {
        public DatabaseBuilder(IConfiguration configuration) : base(configuration)
        {
        }

        public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var entity = GetEntity();
            if (entity != null && entity.IsInMemory())
                optionsBuilder.UseInMemoryDatabase("Test DB");
            else
                optionsBuilder.UseInMemoryDatabase("Test DB");
        }
    }
}
