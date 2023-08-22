using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

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
        /// <returns></returns>
        public static IServiceCollection Builder<TContext>(this IServiceCollection services)
            where TContext : RelationalCoreContext
        {

            // Add services to the container.
            //builder.Services.AddAuthorization();
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<GenericFilter>();
                options.SchemaFilter<XEnumNamesSchemaFilter>();
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork>(service => new UnitOfWork(service));
            services.AddScoped(service => new UnitOfWork(service).GetMapper());
            services.AddTransient<RelationalCoreContext>(serviceProvider => serviceProvider.GetService<TContext>());
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void Build<TContext>(this IApplicationBuilder app)
            where TContext : RelationalCoreContext
        {
            var build = app.Build();
            app.UseDeveloperExceptionPage();
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            //app.MapControllers();
            //app.Run(build);
            using (var scope = app.ApplicationServices.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
            }
            app.Run(build);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication Build<TContext>(this WebApplicationBuilder app)
            where TContext : RelationalCoreContext
        {
            var build = app.Build();
            build.UseDeveloperExceptionPage();
            // Configure the HTTP request pipeline.
            build.UseSwagger();
            build.UseSwaggerUI();

            build.UseHttpsRedirection();
            build.UseAuthorization();
            //app.MapControllers();
            //app.Run(build);
            using (var scope = build.Services.CreateAsyncScope())
            {
                var dbbuilder = new DatabaseCreator();
                using var context = scope.ServiceProvider.GetRequiredService<TContext>();
                dbbuilder.Initialize(context);
            }
            return build;
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
