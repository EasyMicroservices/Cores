using EasyMicroservices.Cores.EntityFrameworkCore;
using EasyMicroservices.Cores.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class RelationalCoreContext : CoreContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IUniqueIdentitySchema).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IUniqueIdentitySchema.UniqueIdentity))
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");
                }
            }
        }
    }
}