using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMicroservices.Cores.Database.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultUniqueIdentityManager : IUniqueIdentityManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startUniqueIdentity"></param>
        /// <param name="microserviceId"></param>
        public DefaultUniqueIdentityManager(string startUniqueIdentity, long microserviceId)
        {
            if (startUniqueIdentity.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(startUniqueIdentity));
            StartUniqueIdentity = startUniqueIdentity;
            MicroserviceId = microserviceId;
        }

        /// <summary>
        /// 
        /// </summary>
        public DefaultUniqueIdentityManager()
        {

        }

        string StartUniqueIdentity { get; set; }
        long MicroserviceId { get; set; }
        Dictionary<string, long> TableIds { get; set; } = new Dictionary<string, long>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool UpdateUniqueIdentity<TEntity>(IContext context, TEntity entity)
        {
            if (entity is IUniqueIdentitySchema uniqueIdentitySchema)
            {
                if (uniqueIdentitySchema.UniqueIdentity.IsNullOrEmpty())
                    uniqueIdentitySchema.UniqueIdentity = StartUniqueIdentity;
                var ids = uniqueIdentitySchema.UniqueIdentity.IsNullOrEmpty() ? null : DecodeUniqueIdentity(uniqueIdentitySchema.UniqueIdentity);
                if (TableIds.TryGetValue(GetContextTableName<TEntity>(context.ContextType, MicroserviceId), out long tableId))
                {
                    if (entity is IIdSchema<long> longIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = ids.IsNullOrEmpty() ? GenerateUniqueIdentity(tableId, longIdSchema.Id) : GenerateUniqueIdentity(ids, tableId, longIdSchema.Id);
                    }
                    else if (entity is IIdSchema<int> intIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = ids.IsNullOrEmpty() ? GenerateUniqueIdentity(tableId, intIdSchema.Id) : GenerateUniqueIdentity(ids, tableId, intIdSchema.Id);
                    }
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateUniqueIdentity(params long[] parameters)
        {
            if (parameters.IsNullOrEmpty())
                throw new Exception($"{nameof(parameters)} cannot be null or empty!");
            return string.Join("-", parameters.Select(x => StringHelper.EncodeByKey(x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateUniqueIdentity(long[] startParameters, params long[] parameters)
        {
            if (parameters.IsNullOrEmpty())
                throw new Exception($"{nameof(parameters)} cannot be null or empty!");
            if (startParameters.IsNullOrEmpty())
                throw new Exception($"{nameof(startParameters)} cannot be null or empty!");
            return string.Join("-", startParameters.Select(x => StringHelper.EncodeByKey(x))) + "-" +
                string.Join("-", parameters.Select(x => StringHelper.EncodeByKey(x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <param name="segmentCount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string CutUniqueIdentityBySegmentCount(string uniqueIdentity, int segmentCount)
        {
            if (uniqueIdentity.IsNullOrEmpty())
                throw new Exception($"{nameof(uniqueIdentity)} cannot be null or empty!");

            var keys = uniqueIdentity.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("-", keys.Take(segmentCount));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static long[] DecodeUniqueIdentity(string uniqueIdentity)
        {
            if (uniqueIdentity.IsNullOrEmpty())
                throw new Exception($"{nameof(uniqueIdentity)} cannot be null or empty!");

            var keys = uniqueIdentity.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return keys.Select(x => StringHelper.DecodeByKey(x)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetContextTableName<TEntity>(Type contextType, long microserviceId)
        {
            return GetContextTableName(microserviceId, GetContextName(contextType), typeof(TEntity).Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public string GetContextName(Type contextType)
        {
            return contextType.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public string GetTableName(Type tableType)
        {
            return tableType.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceId"></param>
        /// <param name="contextName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetContextTableName(long microserviceId, string contextName, string tableName)
        {
            return $"{microserviceId}_{contextName}_{tableName}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceId"></param>
        /// <param name="contextName"></param>
        /// <param name="tableName"></param>
        /// <param name="tableId"></param>
        public void InitializeTables(long microserviceId, string contextName, string tableName, long tableId)
        {
            TableIds[GetContextTableName(microserviceId, contextName, tableName)] = tableId;
        }
    }
}
