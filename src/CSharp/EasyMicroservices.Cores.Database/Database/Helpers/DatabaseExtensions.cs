using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyMicroservices.Cores.Database.Helpers;
/// <summary>
/// 
/// </summary>
internal static class DatabaseExtensions
{
    public static object GetRecordId<TRecordEntity>(TRecordEntity entity)
    {
        var idProperty = typeof(TRecordEntity)
            .GetProperty(nameof(IRecordIdSchema<string>.RecordId), BindingFlags.Public | BindingFlags.Instance);

        if (idProperty == null)
            throw new Exception($"I cannot find RecordId in your {typeof(TRecordEntity).Name}, Did you inherit from IRecordIdSchema<T>?");

        return idProperty.GetValue(entity);
    }

    public static object[] GetId<TEntity>(IContext context, TEntity entity)
    {
        var ids = context.GetPrimaryKeyValues(entity);
        if (!ids.HasAny())
            throw new Exception($"I cannot find any primary key in your {typeof(TEntity).Name}!");
        return ids;
    }

    public static void SetIdToRecordId<TEntity, TRecordEntity>(IContext context, TEntity entity, TRecordEntity recordEntity)
    {
        var idProperty = typeof(TRecordEntity)
            .GetProperty(nameof(IRecordIdSchema<string>.RecordId), BindingFlags.Public | BindingFlags.Instance);
        if (idProperty == null)
            throw new Exception($"I cannot find RecordId in your {typeof(TRecordEntity).Name}, Did you inherit from IRecordIdSchema<T>?");

        var ids = GetId(context, entity);
        idProperty.SetValue(recordEntity, ids.First());
    }

    public static void SetIdToRecordId<TEntity, TRecordEntity>(IContext context, Dictionary<TEntity, TRecordEntity> records)
    {
        foreach (var item in records)
        {
            SetIdToRecordId(context, item.Key, item.Value);
        }
    }
}
