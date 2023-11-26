using EasyMicroservices.Cores.AspCoreApi.Authorizations;
using EasyMicroservices.Cores.AspCoreApi.Managers;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Middlewares;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="swaggerOptions"></param>
        /// <returns></returns>
        public static IServiceCollection Builder<TContext>(this IServiceCollection services, Action<SwaggerGenOptions> swaggerOptions = default)
            where TContext : RelationalCoreContext
        {

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
            services.AddExceptionHandler((option) =>
            {
                option.ExceptionHandler = AppAuthorizationMiddleware.ExceptionHandler;
            });
            return services;
        }

        //static string GetUniqueIdentityFromHttpContext(HttpContext httpContext)
        //{
        //    httpContext.ThrowIfNull(nameof(httpContext));
        //    var uniqueIdentity = httpContext.User.FindFirst(nameof(IUniqueIdentitySchema.UniqueIdentity));
        //    if (uniqueIdentity == null || uniqueIdentity.Value.IsNullOrEmpty())
        //        return UnitOfWork.DefaultUniqueIdentity;
        //    return uniqueIdentity.Value;
        //}

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
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task Build<TContext>(this IApplicationBuilder app)
            where TContext : RelationalCoreContext
        {
            app.UseDeveloperExceptionPage();
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
            //app.MapControllers();
            //app.Run(build);
            using (var scope = app.ApplicationServices.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
                if (WhiteLabelRoute.HasValue() || WhiteLabelConfigName.HasValue())
                {
                    using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>() as UnitOfWork;
                    await uow.InitializeWhiteLabel(MicroserviceName, config.GetValue<string>(WhiteLabelConfigName), typeof(TContext)).ConfigureAwait(false);
                }
                if (AuthenticationConfigName.HasValue())
                    AspCoreAuthorization.AuthenticationRouteAddress = config.GetValue<string>(AuthenticationConfigName);
            }
            var build = app.Build();
            app.Run(build);
        }

        private static string MicroserviceName = default;
        private static string WhiteLabelConfigName = default;
        private static string WhiteLabelRoute = default;
        private static string AuthenticationConfigName = default;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="configName"></param>
        public static void AddWhiteLabel(string microserviceName, string configName)
        {
            MicroserviceName = microserviceName;
            WhiteLabelConfigName = configName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        public static void AddAuthentication(string configName)
        {
            AuthenticationConfigName = configName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="whiteLabelRoute"></param>
        public static void AddWhiteLabelRoute(string microserviceName, string whiteLabelRoute)
        {
            MicroserviceName = microserviceName;
            WhiteLabelRoute = whiteLabelRoute;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication Build(this WebApplicationBuilder app)
        {
            return app.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <param name="useGlobalExceptionHandling"></param>
        /// <param name="useAuthorization"></param>
        /// <returns></returns>
        public static async Task<WebApplication> Build<TContext>(this WebApplicationBuilder app, bool useGlobalExceptionHandling = false, bool useAuthorization = false)
            where TContext : RelationalCoreContext
        {
            var build = app.Build();
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

            // Configure the HTTP request pipeline.
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();

            webApplication.UseHttpsRedirection();
            webApplication.UseAuthorization();
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            using (var scope = webApplication.Services.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
                using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>() as UnitOfWork;
                if (WhiteLabelRoute.HasValue() || WhiteLabelConfigName.HasValue())
                {
                    var value = WhiteLabelRoute ?? config.GetValue<string>(WhiteLabelConfigName);
                    await uow.InitializeWhiteLabel(MicroserviceName, WhiteLabelRoute ?? config.GetValue<string>(WhiteLabelConfigName), typeof(TContext)).ConfigureAwait(false);
                }
                if (AuthenticationConfigName.HasValue())
                    AspCoreAuthorization.AuthenticationRouteAddress = config.GetValue<string>(AuthenticationConfigName);
            }
            return webApplication;
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
