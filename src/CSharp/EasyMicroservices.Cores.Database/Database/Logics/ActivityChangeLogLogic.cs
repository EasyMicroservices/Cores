using EasyMicroservices.Cores.Database.Entities;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.DataTypes;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Serialization.Interfaces;
using EasyMicroservices.ServiceContracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics;
/// <summary>
/// 
/// </summary>
public class ActivityChangeLogLogic
{
    /// <summary>
    /// 
    /// </summary>
    public static bool UseActivityChangeLog = false;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <returns></returns>
    public static async Task AddAsync<TEntity>(TEntity entity, IBaseUnitOfWork baseUnitOfWork)
    {
        if (!UseActivityChangeLog || typeof(TEntity) == typeof(ActivityChangeLogEntity))
            return;

        var logic = baseUnitOfWork.GetLongLogic<ActivityChangeLogEntity>();

        string requesterUniqueIdentity = await baseUnitOfWork.GetCurrentUserUniqueIdentity();
        var log = GetActivityChangeLogEntity(entity, baseUnitOfWork.GetTextSerialization(), baseUnitOfWork.GetUniqueIdentityManager(), requesterUniqueIdentity);

        await logic.Add(log).AsCheckedResult();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="id"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="activityChangeLogType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task ChangeLogAsync<TEntity, TId>(TId id, IBaseUnitOfWork baseUnitOfWork, ActivityChangeLogType activityChangeLogType, CancellationToken cancellationToken)
        where TEntity : class
    {
        return ChangeLogAsync<TEntity, TId>(new List<TId>() { id }, baseUnitOfWork, activityChangeLogType, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="ids"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="activityChangeLogType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ChangeLogAsync<TEntity, TId>(List<TId> ids, IBaseUnitOfWork baseUnitOfWork, ActivityChangeLogType activityChangeLogType, CancellationToken cancellationToken)
        where TEntity : class
    {
        if (!UseActivityChangeLog || typeof(TEntity) == typeof(ActivityChangeLogEntity))
            return;

        var logic = baseUnitOfWork.GetLongLogic<ActivityChangeLogEntity>();
        var entities = await baseUnitOfWork.GetLogic<TEntity, TId>()
            .GetAll(q => q.Where(x => ids.Contains(((IIdSchema<TId>)x).Id)), cancellationToken)
            .AsCheckedResult();
        string requesterUniqueIdentity = await baseUnitOfWork.GetCurrentUserUniqueIdentity();
        List<ActivityChangeLogEntity> logs = new List<ActivityChangeLogEntity>();

        foreach (var entity in entities)
        {
            var log = GetActivityChangeLogEntity(entity,
                       baseUnitOfWork.GetTextSerialization(),
                       baseUnitOfWork.GetUniqueIdentityManager(),
                       requesterUniqueIdentity,
                       activityChangeLogType);
            logs.Add(log);
        }

        await logic.AddBulk(logs).AsCheckedResult();
    }

    static ActivityChangeLogEntity GetActivityChangeLogEntity<TEntity>(TEntity entity,
        ITextSerializationProvider textSerialization,
        IUniqueIdentityManager uniqueIdentityManager,
        string requesterUniqueIdentity,
        ActivityChangeLogType activityChangeLogType = ActivityChangeLogType.Add)
    {
        string uid = null;
        long? id = null;
        if (entity is IIdSchema<long> idSchema)
            id = idSchema.Id;
        if (entity is IUniqueIdentitySchema uniqueIdentitySchema)
            uid = uniqueIdentitySchema.UniqueIdentity;

        return new ActivityChangeLogEntity()
        {
            JsonData = textSerialization.Serialize(entity),
            RecordIdentity = id?.ToString(),
            TableName = uniqueIdentityManager.GetTableName(typeof(TEntity)),
            RequesterUniqueIdentity = requesterUniqueIdentity,
            Type = activityChangeLogType,
            UniqueIdentity = uid
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entities"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <returns></returns>
    public static async Task AddBulkAsync<TEntity>(IEnumerable<TEntity> entities, IBaseUnitOfWork baseUnitOfWork)
    {
        if (!UseActivityChangeLog || typeof(TEntity) == typeof(ActivityChangeLogEntity))
            return;

        var logic = baseUnitOfWork.GetLongLogic<ActivityChangeLogEntity>();

        string requesterUniqueIdentity = await baseUnitOfWork.GetCurrentUserUniqueIdentity();
        var textSerialization = baseUnitOfWork.GetTextSerialization();
        var uniqueIdentityManager = baseUnitOfWork.GetUniqueIdentityManager();

        List<ActivityChangeLogEntity> logs = new List<ActivityChangeLogEntity>();

        foreach (var entity in entities)
        {
            var log = GetActivityChangeLogEntity(entity, textSerialization, uniqueIdentityManager, requesterUniqueIdentity);
            logs.Add(log);
        }

        await logic.AddBulk(logs).AsCheckedResult();
    }

}
