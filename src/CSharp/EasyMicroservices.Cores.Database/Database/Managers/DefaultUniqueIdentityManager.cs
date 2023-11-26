using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DefaultUniqueIdentityManager(WhiteLabelInfo whiteLabelInfo)
        {
            _whiteLabelInfo = whiteLabelInfo;
        }

        readonly WhiteLabelInfo _whiteLabelInfo;
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
                    uniqueIdentitySchema.UniqueIdentity = _whiteLabelInfo.StartUniqueIdentity;
                var ids = uniqueIdentitySchema.UniqueIdentity.IsNullOrEmpty() ? null : DecodeUniqueIdentity(uniqueIdentitySchema.UniqueIdentity);
                if (TableIds.TryGetValue(GetContextTableName<TEntity>(context.ContextType, _whiteLabelInfo.MicroserviceId), out long tableId))
                {
                    if (entity is IIdSchema<long> longIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = ids.IsEmpty() ? GenerateUniqueIdentity(tableId, longIdSchema.Id) : GenerateUniqueIdentity(ids, tableId, longIdSchema.Id);
                    }
                    else if (entity is IIdSchema<int> intIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = ids.IsEmpty() ? GenerateUniqueIdentity(tableId, intIdSchema.Id) : GenerateUniqueIdentity(ids, tableId, intIdSchema.Id);
                    }
                }
                else
                {

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
            if (parameters.IsEmpty())
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
            if (parameters.IsEmpty())
                throw new Exception($"{nameof(parameters)} cannot be null or empty!");
            if (startParameters.IsEmpty())
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
        public static string CutUniqueIdentity(string uniqueIdentity, int segmentCount)
        {
            if (uniqueIdentity.IsNullOrEmpty())
                throw new Exception($"{nameof(uniqueIdentity)} cannot be null or empty!");

            var keys = uniqueIdentity.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("-", keys.Take(segmentCount));
        }

        /// <summary>
        /// merge multiple unique identities to one
        /// </summary>
        /// <param name="uniqueIdentities"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string MergeUniqueIdentities(params string[] uniqueIdentities)
        {
            return GenerateUniqueIdentity(uniqueIdentities.SelectMany(DecodeUniqueIdentity).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <param name="segmentCount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string CutUniqueIdentityFromEnd(string uniqueIdentity, int segmentCount)
        {
            if (uniqueIdentity.IsNullOrEmpty())
                throw new Exception($"{nameof(uniqueIdentity)} cannot be null or empty!");

            var keys = uniqueIdentity.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("-", keys.Take(keys.Length - segmentCount));
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public bool IsUniqueIdentityForThisTable<TEntity>(IContext context, string uniqueIdentity)
        {
            var decodeIds = DecodeUniqueIdentity(uniqueIdentity);
            if (TableIds.TryGetValue(GetContextTableName(_whiteLabelInfo.MicroserviceId, GetContextName(context.ContextType), GetTableName(typeof(TEntity))), out long tableId))
            {
                return decodeIds.Contains(tableId);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public int GetRepeatCountOfTableId<TEntity>(IContext context, string uniqueIdentity)
        {
            return GetRepeatCountOfTableId<TEntity>(context.ContextType, uniqueIdentity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public int GetRepeatCountOfTableId<TEntity, TContext>(string uniqueIdentity)
        {
            return GetRepeatCountOfTableId<TEntity>(typeof(TContext), uniqueIdentity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="contextType"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public int GetRepeatCountOfTableId<TEntity>(Type contextType, string uniqueIdentity)
        {
            return GetRepeatCountOfTableId(contextType, typeof(TEntity), uniqueIdentity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="tableType"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public int GetRepeatCountOfTableId(Type contextType, Type tableType, string uniqueIdentity)
        {
            var decodeIds = DecodeUniqueIdentity(uniqueIdentity);
            if (TableIds.TryGetValue(GetContextTableName(_whiteLabelInfo.MicroserviceId, GetContextName(contextType), GetTableName(tableType)), out long tableId))
            {
                return decodeIds.Count(x => x == tableId);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetTableUniqueIdentity<TEntity>(IContext context)
        {
            return GetTableUniqueIdentity<TEntity>(context.ContextType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public string GetTableUniqueIdentity<TEntity, TContext>()
        {
            return GetTableUniqueIdentity<TEntity>(typeof(TContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public string GetTableUniqueIdentity<TEntity>(Type contextType)
        {
            return GetTableUniqueIdentity(contextType, typeof(TEntity));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public string GetTableUniqueIdentity(Type contextType, Type tableType)
        {
            if (TableIds.TryGetValue(GetContextTableName(_whiteLabelInfo.MicroserviceId, GetContextName(contextType), GetTableName(tableType)), out long tableId))
            {
                return GenerateUniqueIdentity(tableId);
            }
            throw new Exception($"Type of {tableType} is not table of {contextType}!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public string GetLastTableUniqueIdentity(string uniqueIdentity)
        {
            var decode = DecodeUniqueIdentity(uniqueIdentity);
            return GenerateUniqueIdentity(decode[decode.Length - 2]);
        }
    }
}
