using EasyMicroservices.Cores.Database.Entities;
using EasyMicroservices.Cores.Database.Logics;
using EasyMicroservices.Cores.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore.Builders;
/// <summary>
/// 
/// </summary>
public class RelationalCoreModelBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    void InitActivityLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityChangeLogEntity>(entity =>
        {
            entity.HasIndex(x => x.TableName);
            entity.HasIndex(x => x.RecordIdentity);
            entity.HasIndex(x => x.Type);

            entity.HasIndex(x => x.RequesterUniqueIdentity);
            entity.Property(x => x.RequesterUniqueIdentity)
            .UseCollation("SQL_Latin1_General_CP1_CS_AS");

            entity.ToTable("ActivityChangeLogs");
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        InternalOnModelCreating(modelBuilder);
        if (ActivityChangeLogLogic.UseActivityChangeLog)
            InitActivityLog(modelBuilder);
    }

    void InternalOnModelCreating(ModelBuilder modelBuilder)
    {
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

    /// <summary>
    /// 
    /// </summary>
    public virtual StringBuilder AutoModelCreating(ModelBuilder modelBuilder)
    {
        StringBuilder stringBuilder = new StringBuilder();
        InternalOnModelCreating(modelBuilder);
        foreach (var entityType in GetAllEntities(modelBuilder))
        {
            modelBuilder.Entity(entityType, e =>
            {
                string id = "Id";
                if (HasProperty(entityType, id))
                    e.HasKey(id);
                string userName = "UserName";
                if (HasProperty(entityType, userName))
                    e.HasIndex(id).IsUnique();
                stringBuilder.Append(DoRelationToForeignKey(modelBuilder, e, entityType));
            });
        }
        return stringBuilder;
    }

    IEnumerable<Type> GetAllEntities(ModelBuilder modelBuilder)
    {
        return modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType);
    }

    internal string DoRelationToForeignKey(ModelBuilder modelBuilder, EntityTypeBuilder entityTypeBuilder, Type entityType, bool throwIfHasError = true)
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder errorStringBuilder = new StringBuilder();
        var allEntities = GetAllEntities(modelBuilder).ToList();
        var allProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        foreach (var property in allProperties)
        {
            if (allEntities.Contains(property.PropertyType))
            {
                if (!IsCollection(property.PropertyType))
                {
                    var findForeignKey = allProperties.FirstOrDefault(x => x.Name.StartsWith(property.Name));
                    if (findForeignKey != null)
                    {
                        var findCollection = GetRelationProperty(entityType, property);
                        if (findCollection != null)
                        {
                            entityTypeBuilder.HasOne(property.Name)
                                .WithMany(findCollection.Name)
                                .HasForeignKey(findForeignKey.Name)
                                .OnDelete(DeleteBehavior.Restrict);
                            stringBuilder.AppendLine($"{property.Name}-{findCollection.Name}-{findForeignKey.Name}");
                        }
                    }
                    else
                        errorStringBuilder.AppendLine($"NotFound_{entityType.Name}_{property.Name}");
                }
            }
        }
        if (throwIfHasError && errorStringBuilder.Length > 0)
            throw new Exception($"Some of relations are not determine, please check them: {errorStringBuilder}");
        return stringBuilder.ToString();
    }

    bool IsCollection(Type type)
    {
        return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
    }

    PropertyInfo GetRelationProperty(Type entityType, PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var findProperties = propertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
             .Where(x => x != property && CouldBeARelation(x.PropertyType, entityType)).ToList();
        if (findProperties.Count > 1)
            return null;
        return findProperties.OrderBy(x => IsCollection(x.PropertyType)).FirstOrDefault();
    }

    bool CouldBeARelation(Type type, Type propertyType)
    {
        if (type == propertyType)
            return true;
        return IsCollectionOf(type, propertyType) || IsCollectionOf(propertyType, type);
    }

    bool IsCollectionOf(Type type, Type propertyType)
    {
        return type.IsGenericType && propertyType == type.GetGenericArguments()[0];
    }

    bool HasProperty(Type entityType, string name)
    {
        return entityType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
    }

    Type[] GetAllBases(Type type)
    {
        if (type.BaseType == null)
            return new Type[0];
        var result = new List<Type> { type };
        result.AddRange(GetAllBases(type.BaseType));
        return result.ToArray();
    }
}
