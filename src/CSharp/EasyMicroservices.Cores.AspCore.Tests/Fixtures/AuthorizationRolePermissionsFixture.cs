using EasyMicroservices.Cores.AspCoreApi.Authorizations;
using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyMicroservices.Cores.AspCore.Tests.Fixtures;

public class AuthorizationRolePermissionsFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; }
    public Task InitializeAsync()
    {
        return BaseFixture.Init(4566,6043, (services) =>
        {
            services.AddScoped<IAuthorization, AspCoreAuthorization>();
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
        });
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}