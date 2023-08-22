using EasyMicroservices.Cores.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseCreator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Initialize(CoreContext context)
        {
            if (context.Database.EnsureCreated())
            {
                if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                    return;
                //auto migration when database created first time

                //add migration history table

                string createEFMigrationsHistoryCommand = $@"
USE [{context.Database.GetDbConnection().Database}];
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
    [MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
";
                context.Database.ExecuteSqlRaw(createEFMigrationsHistoryCommand);

                //insert all of migrations
                var dbAssebmly = context.GetType().Assembly;
                foreach (var item in dbAssebmly.GetTypes())
                {
                    if (item.BaseType == typeof(Migration))
                    {
                        string migrationName = item.GetCustomAttributes<MigrationAttribute>().First().Id;
                        var version = typeof(Migration).Assembly.GetName().Version;
                        string efVersion = $"{version.Major}.{version.Minor}.{version.Build}";
                        context.Database.ExecuteSqlRaw("INSERT INTO __EFMigrationsHistory(MigrationId,ProductVersion) VALUES ({0},{1})", migrationName, efVersion);
                    }
                }
            }
            context.Database.Migrate();
        }
    }
}
