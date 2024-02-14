using EasyMicroservices.Cores.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class RelationalCoreContext : CoreContext
    {
        readonly IEntityFrameworkCoreDatabaseBuilder _builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public RelationalCoreContext(IEntityFrameworkCoreDatabaseBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// 
        /// </summary>
        public RelationalCoreContext()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _builder?.OnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _builder?.OnWidgetBuilder(modelBuilder);
            new RelationalCoreModelBuilder().OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual StringBuilder AutoModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _builder?.OnWidgetBuilder(modelBuilder);
            return new RelationalCoreModelBuilder().AutoModelCreating(modelBuilder);
        }
    }
}