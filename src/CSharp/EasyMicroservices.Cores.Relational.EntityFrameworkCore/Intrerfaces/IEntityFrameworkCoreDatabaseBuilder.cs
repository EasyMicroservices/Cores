using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityFrameworkCoreDatabaseBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        void OnWidgetBuilder(ModelBuilder modelBuilder);
    }
}
