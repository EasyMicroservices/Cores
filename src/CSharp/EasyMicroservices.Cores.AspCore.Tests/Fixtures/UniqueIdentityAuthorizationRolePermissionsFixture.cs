using EasyMicroservices.Cores.AspCoreApi.Authorizations;
using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class UniqueIdentityAuthorizationRolePermissionsFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; }
    public Task InitializeAsync()
    {
        return BaseFixture.Init(4567, null, (services) =>
        {
            services.AddScoped<IAuthorization, AspCoreAuthorization>();
            services.AddTransient<IUnitOfWork, UniqueIdentityUnitOfWork>();
            services.AddTransient<IBaseUnitOfWork, UniqueIdentityUnitOfWork>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://github.com/easymicroservices",
                    ValidAudience = "easymicroservices",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VGhpc0lzQVNlY3JldEtleUZvckp3dEF1dGhlbnRpY2F0aW9u="))
                };
            });
        }, "UIDTestExample");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

public class UniqueIdentityUnitOfWork : UnitOfWork
{
    public UniqueIdentityUnitOfWork(IServiceProvider service) : base(service)
    {
    }

    public override ServiceAddressInfo GetServiceAddress(string name)
    {
        if (name == "Authentication")
            return new ServiceAddressInfo()
            {
                Name = name,
                Address = "http://localhost:1044"
            };
        return base.GetServiceAddress(name);
    }
}