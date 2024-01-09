using EasyMicroservices.Cores.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EasyMicroservices.Cores.EntityFrameworkCore.Builders;
/// <summary>
/// 
/// </summary>
public class CoreModelBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
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
                    //Because your server's local times may be different
                    DateTimeKind dateTimeKind = DateTimeKind.Utc;
                    var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                       v => v, v => DateTime.SpecifyKind(v, dateTimeKind));
                    modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasConversion(dateTimeConverter);
                }
            }
        }
    }
}
