using EasyMicroservices.Cores.EntityFrameworkCore;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyMicroservices.Cores.Relational.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class RelationalCoreContext : CoreContext
    {
        IEntityFrameworkCoreDatabaseBuilder _builder;
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
            if (_builder != null)
                _builder.OnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

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

        /// <summary>
        /// 
        /// </summary>
        protected virtual StringBuilder AutoModelCreating(ModelBuilder modelBuilder)
        {
            StringBuilder stringBuilder = new StringBuilder();
            base.OnModelCreating(modelBuilder);
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

        internal string DoRelationToForeignKey(ModelBuilder modelBuilder, EntityTypeBuilder entityTypeBuilder, Type entityType)
        {
            StringBuilder stringBuilder = new StringBuilder();
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
                            var findCollection = GetColectionProperty(entityType, property.PropertyType);
                            if (findCollection != null)
                            {
                                entityTypeBuilder.HasOne(property.Name)
                                    .WithMany(findCollection.Name)
                                    .HasForeignKey(findForeignKey.Name);
                                stringBuilder.AppendLine($"{property.Name}-{findCollection.Name}-{findForeignKey.Name}");
                            }
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

        bool IsCollection(Type type)
        {
            return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
        }

        PropertyInfo GetColectionProperty(Type entityType, Type propertyType)
        {
            var findProperties = propertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 .Where(x => GetSimplifyPropertyName(x.Name).Any(i => i.StartsWith(GetSimplifyClassName(entityType.Name)))).ToList();
            return findProperties.FirstOrDefault(x => IsCollection(x.PropertyType));
        }

        bool HasProperty(Type entityType, string name)
        {
            return entityType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }

        string[] GetSimplifyPropertyName(string name)
        {
            string entity = "es";
            if (name.EndsWith(entity))
                return GetEndOfCollectionNames(name[..^entity.Length]);
            else if (name.EndsWith("s"))
                return new string[] { name[..^1] };
            return new string[] { name };
        }

        string[] GetEndOfCollectionNames(string name)
        {
            List<string> names = new List<string>
            {
                name
            };
            if (name.Length < 2)
                return names.ToArray();
            if (name.EndsWith('i'))
                names.Add(name[..^1] + "y");
            if (name.EndsWith('v'))
            {
                names.Add(name[..^1] + "f");
                names.Add(name[..^1] + "fe");
            }
            return names.ToArray();
        }

        string GetSimplifyClassName(string name)
        {
            string entity = "Entity";
            if (name.EndsWith(entity))
                return name[..^entity.Length];
            return name;
        }
    }
}