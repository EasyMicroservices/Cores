using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using EasyMicroservices.Payments.VirtualServerForTests.TestResources;
using EasyMicroservices.WhiteLabelsMicroservice.VirtualServerForTests;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            StartUpExtensions.Builder<MyTestContext>(services);
            services.AddScoped((serviceProvider) => new UnitOfWork(serviceProvider).GetLongContractLogic<UserEntity, UserEntity, UserEntity, UserEntity>());
            services.AddTransient(serviceProvider => new MyTestContext(serviceProvider.GetService<IEntityFrameworkCoreDatabaseBuilder>()));
            services.AddScoped<IEntityFrameworkCoreDatabaseBuilder>(serviceProvider => new DatabaseBuilder());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            StartUpExtensions.AddWhiteLabel("TestExample", "RootAddresses:WhiteLabel");

            app.Build<MyTestContext>().Wait();
        }
    }
}
