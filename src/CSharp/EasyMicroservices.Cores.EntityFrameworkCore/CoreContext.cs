using EasyMicroservices.Cores.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EasyMicroservices.Cores.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class CoreContext : DbContext
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
                    modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(IUniqueIdentitySchema.UniqueIdentity));
                }

                if (typeof(IDateTimeSchema).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(IDateTimeSchema.CreationDateTime));
                    modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(IDateTimeSchema.ModificationDateTime));
                }

                if (typeof(ISoftDeleteSchema).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(ISoftDeleteSchema.IsDeleted));
                    modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(ISoftDeleteSchema.DeletedDateTime));
                }

                foreach (var property in entityType.ClrType.GetProperties())
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTimeKind dateTimeKind = DateTimeKind.Local;
                        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                           v => v, v => DateTime.SpecifyKind(v, dateTimeKind));
                        modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasConversion(dateTimeConverter);
                    }
                }
            }
        }
    }
}
