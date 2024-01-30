using Contents.GeneratedServices;
using EasyMicroservices.ContentsMicroservice.Clients.Helpers;
using EasyMicroservices.Cores.AspCoreApi.Authorizations;
using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Middlewares;
using EasyMicroservices.Cores.Clients;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class StartUpExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static WebApplicationBuilder Create<TContext>(string[] args)
        {
            return WebApplication.CreateBuilder(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="yourMicroserviceName"></param>
        /// <param name="swaggerOptions"></param>
        /// <returns></returns>
        public static IServiceCollection Builder<TContext>(this IServiceCollection services, string yourMicroserviceName, Action<SwaggerGenOptions> swaggerOptions = default)
            where TContext : RelationalCoreContext
        {
            MicroserviceName = yourMicroserviceName;
            // Add services to the container.
            //builder.Services.AddAuthorization();
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                swaggerOptions?.Invoke(options);
                options.SchemaFilter<GenericFilter>();
                options.SchemaFilter<XEnumNamesSchemaFilter>();
                options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork>(service => new UnitOfWork(service));
            services.AddScoped<IBaseUnitOfWork, UnitOfWork>();
            services.AddSingleton(service => new WhiteLabelManager(service));
            services.AddSingleton<IUniqueIdentityManager, DefaultUniqueIdentityManager>((provider) =>
            {
                return new DefaultUniqueIdentityManager(provider.GetService<WhiteLabelManager>().CurrentWhiteLabel);
            });
            services.AddScoped(service => new UnitOfWork(service).GetMapper());
            services.AddTransient<RelationalCoreContext>(serviceProvider => serviceProvider.GetService<TContext>());
            services.AddTransient<TContext>(serviceProvider => serviceProvider.GetService<TContext>());
            services.AddExceptionHandler((option) =>
            {
                option.ExceptionHandler = AppAuthorizationMiddleware.ExceptionHandler;
            });
            services.AddScoped<Contents.GeneratedServices.ContentClient>(service => GetContentClient(service));
            services.AddScoped<ContentLanguageHelper>();
            services.AddScoped<IContentResolver, InternalContentResolver>();
            return services;
        }

        static HttpClient ContentClientCttpClient = new HttpClient();
        static T SetToken<T>(IServiceProvider serviceProvider, T coreSwaggerClientBase)
                where T : CoreSwaggerClientBase
        {
            var _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            if (_httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                coreSwaggerClientBase.SetBearerToken(authorizationHeader.ToString().Replace("Bearer ", ""));
            }
            return coreSwaggerClientBase;
        }

        static ContentClient GetContentClient(IServiceProvider serviceProvider)
        {
            return SetToken(serviceProvider, new Contents.GeneratedServices.ContentClient(GetContentAddress(serviceProvider)?.Address, ContentClientCttpClient));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseDefaultSwaggerOptions(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            //Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseActivityChangeLog(this IServiceCollection services)
        {
            ActivityChangeLogLogic.UseActivityChangeLog = true;
            return services;
        }


        static void UseSwaggerUI(IConfiguration config, Func<Action<SwaggerUIOptions>, IApplicationBuilder> swagger)
        {
            var ui = config.GetSection("Swagger:SwaggerUI").Get<SwaggerUIConfigInfo>();
            swagger(so =>
            {
                if (ui.Endpoints?.Length > 0)
                {
                    foreach (var item in ui.Endpoints)
                    {
                        so.SwaggerEndpoint(item.Url, item.Name);
                    }
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task Build<TContext>(this IApplicationBuilder app)
            where TContext : RelationalCoreContext
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
            app.UseDeveloperExceptionPage();

            var useSwagger = config["Swagger:IsUse"];
            var doUseSwagger = useSwagger.HasValue() && useSwagger.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (doUseSwagger)
            {
                app.UseSwagger();
                UseSwaggerUI(config, app.UseSwaggerUI);
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            //app.MapControllers();
            //app.Run(build);
            using (var scope = app.ApplicationServices.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
                var whiteLabel = GetWhiteLabelAddress(scope.ServiceProvider);
                if (whiteLabel != null)
                {
                    using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>() as UnitOfWork;
                    await uow.InitializeWhiteLabel(MicroserviceName, whiteLabel.Address, typeof(TContext)).ConfigureAwait(false);
                }
            }
            var build = app.Build();
            app.Run(build);
        }

        private static string MicroserviceName = default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication Build(this WebApplicationBuilder app)
        {
            return app.Build();
        }

        static bool PreBuild(WebApplicationBuilder app)
        {
            var useAuth = app.Configuration["Authorization:IsUse"];
            var useAuthorization = useAuth.HasValue() && useAuth.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (useAuthorization)
            {
                app.Services.AddScoped<IAuthorization, AspCoreAuthorization>();
                app.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = app.Configuration["Authorization:JWT:Issuer"],
                            ValidAudience = app.Configuration["Authorization:JWT:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(app.Configuration["Authorization:Jwt:Key"]))
                        };
                    });
            }
            return useAuthorization;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <param name="useGlobalExceptionHandling"></param>
        /// <returns></returns>
        public static async Task<WebApplication> Build<TContext>(this WebApplicationBuilder app, bool useGlobalExceptionHandling = false)
            where TContext : RelationalCoreContext
        {
            var useAuthorization = PreBuild(app);
            var build = app.Build();
            var webApp = await Use(build, useGlobalExceptionHandling, useAuthorization);
            return await Use<TContext>(webApp, useGlobalExceptionHandling, useAuthorization);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <param name="corsSettings"></param>
        /// <param name="useGlobalExceptionHandling"></param>
        /// <returns></returns>
        public static async Task<WebApplication> BuildWithUseCors<TContext>(this WebApplicationBuilder app, Action<CorsPolicyBuilder> corsSettings, bool useGlobalExceptionHandling = false)
            where TContext : RelationalCoreContext
        {
            var useAuthorization = PreBuild(app);
            var build = app.Build();
            build.UseCors(options =>
            {
                if (corsSettings != null)
                    corsSettings(options);
                else
                {
                    options.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
            });
            var webApp = await Use(build, useGlobalExceptionHandling, useAuthorization);
            return await Use<TContext>(webApp, useGlobalExceptionHandling, useAuthorization);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webApplication"></param>
        /// <param name="useGlobalExceptionHandling"></param>
        /// <param name="useAuthorization"></param>
        /// <returns></returns>
        public static Task<WebApplication> Use(this WebApplication webApplication, bool useGlobalExceptionHandling = false, bool useAuthorization = false)
        {
            webApplication.UseRouting();
            if (useAuthorization)
                webApplication.UseAuthentication();
            if (useGlobalExceptionHandling)
            {
                webApplication.UseExceptionHandler();
            }

            return Task.FromResult(webApplication);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="webApplication"></param>
        /// <param name="useGlobalExceptionHandling"></param>
        /// <param name="useAuthorization"></param>
        /// <returns></returns>
        public static async Task<WebApplication> Use<TContext>(this WebApplication webApplication, bool useGlobalExceptionHandling = false, bool useAuthorization = false)
            where TContext : RelationalCoreContext
        {
            webApplication.UseMiddleware<AppAuthorizationMiddleware>();

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            var useSwagger = config["Swagger:IsUse"];
            var doUseSwagger = useSwagger.HasValue() && useSwagger.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (doUseSwagger)
            {
                webApplication.UseSwagger();
                UseSwaggerUI(config, webApplication.UseSwaggerUI);
            }

            webApplication.UseHttpsRedirection();
            webApplication.UseAuthorization();

            using (var scope = webApplication.Services.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
                using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>() as UnitOfWork;
                var whiteLabel = GetWhiteLabelAddress(scope.ServiceProvider);
                if (whiteLabel != null)
                    await uow.InitializeWhiteLabel(MicroserviceName, whiteLabel.Address, typeof(TContext)).ConfigureAwait(false);
            }
            return webApplication;
        }

        static ServiceAddressInfo GetWhiteLabelAddress(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork>();
            return unitOfWork.GetServiceAddress("WhiteLabel");
        }

        static ServiceAddressInfo GetContentAddress(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork>();
            return unitOfWork.GetServiceAddress("Content");
        }
    }
}

internal class GenericFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsGenericType == false)
            return;

        schema.Title = $"{type.Name[0..^2]}<{type.GenericTypeArguments[0].Name}>";
    }
}

internal class XEnumNamesSchemaFilter : ISchemaFilter
{
    private const string NAME = "x-enumNames";
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        var typeInfo = context.Type;
        // Chances are something in the pipeline might generate this automatically at some point in the future
        // therefore it's best to check if it exists.
        if (typeInfo.IsEnum && !model.Extensions.ContainsKey(NAME))
        {
            var names = Enum.GetNames(context.Type);
            var arr = new OpenApiArray();
            arr.AddRange(names.Select(name => new OpenApiString(name)));
            model.Extensions.Add(NAME, arr);
        }
    }
}
