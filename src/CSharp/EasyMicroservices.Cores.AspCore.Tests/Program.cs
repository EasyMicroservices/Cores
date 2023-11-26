using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string microserviceName = "TestExample";
            var app = StartUpExtensions.Create<MyTestContext>(args);
            app.Services.Builder<MyTestContext>();
            app.Services.AddScoped((serviceProvider) => new UnitOfWork(serviceProvider).GetLongContractLogic<UserEntity, UserEntity, UserEntity, UserEntity>());
            app.Services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
            app.Services.AddScoped<IEntityFrameworkCoreDatabaseBuilder>(serviceProvider => new DatabaseBuilder());
            app.Services.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
            {
                return new DefaultUniqueIdentityManager(WhiteLabelManager.CurrentWhiteLabel);
            });
            StartUpExtensions.AddWhiteLabel(microserviceName, "RootAddresses:WhiteLabel");
            var build = await app.Build<MyTestContext>();
            build.MapControllers();
            build.Run();
        }
    }

    public class DatabaseBuilder : IEntityFrameworkCoreDatabaseBuilder
    {
        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Test DB");
        }
    }
}
