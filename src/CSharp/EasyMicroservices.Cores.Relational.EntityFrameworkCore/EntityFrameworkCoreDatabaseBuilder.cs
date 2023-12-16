using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
    /// <param name="configuration"></param>
    public EntityFrameworkCoreDatabaseBuilder(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        OnConfiguring(optionsBuilder, Configuration.GetSection("Database:ProviderName").Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <param name="name"></param>
    public abstract void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string name);
}
