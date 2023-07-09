using EasyMicroservices.Cores.Database.Interfaces;
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
        public DefaultUniqueIdentityManager(string startUniqueIdentity)
        {
            if (startUniqueIdentity.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(startUniqueIdentity));
            StartUniqueIdentity = startUniqueIdentity;
        }

        string StartUniqueIdentity { get; set; }
        Dictionary<string, long> TableIds { get; set; } = new Dictionary<string, long>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool UpdateUniqueIdentity<TEntity>(TEntity entity)
        {
            if (entity is IUniqueIdentitySchema uniqueIdentitySchema)
            {
                if (uniqueIdentitySchema.UniqueIdentity.IsNullOrEmpty())
                    uniqueIdentitySchema.UniqueIdentity = StartUniqueIdentity;
                var ids = DecodeUniqueIdentity(uniqueIdentitySchema.UniqueIdentity);
                if (TableIds.TryGetValue(GetTableName<TEntity>(), out long tableId))
                {
                    if (entity is IIdSchema<long> longIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = GenerateUniqueIdentity(ids, tableId, longIdSchema.Id);
                    }
                    else if (entity is IIdSchema<int> intIdSchema)
                    {
                        uniqueIdentitySchema.UniqueIdentity = GenerateUniqueIdentity(ids, tableId, intIdSchema.Id);
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
            return string.Join("-", parameters.Select(x => StringHelper.EncodeByKey(x)));
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
        public static string GetTableName<TEntity>()
        {
            return typeof(TEntity).Name;
        }
    }
}
