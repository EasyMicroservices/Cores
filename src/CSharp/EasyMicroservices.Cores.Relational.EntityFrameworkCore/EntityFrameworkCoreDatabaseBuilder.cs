using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Database.Widgets;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore;
/// <summary>
/// 
/// </summary>
public abstract class EntityFrameworkCoreDatabaseBuilder : IEntityFrameworkCoreDatabaseBuilder
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly IConfiguration Configuration;
    /// <summary>
    /// 
    /// </summary>
    protected readonly IDatabaseWidgetManager WidgetManager;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="widgetManager"></param>
    public EntityFrameworkCoreDatabaseBuilder(IConfiguration configuration, IDatabaseWidgetManager widgetManager)
    {
        Configuration = configuration;
        WidgetManager = widgetManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public EntityFrameworkCoreDatabaseBuilder(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    public abstract void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public DatabaseConfig GetEntity()
    {
        return GetDatabases()?
            .Where(x => x.Name.HasValue())
            .FirstOrDefault(x => x.Name.Equals("Entity", System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual List<DatabaseConfig> GetDatabases()
    {
        return Configuration?.GetSection("Databases")?.Get<List<DatabaseConfig>>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    public virtual void OnWidgetBuilder(ModelBuilder modelBuilder)
    {
        if (WidgetManager is null)
            return;
        foreach (var widget in WidgetManager.GetWidgets<DatabaseBuilderWidget<ModelBuilder>>())
        {
            widget.OnModelCreating(modelBuilder);
        }
    }
}