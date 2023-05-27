using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class LongIdMappedDatabaseLogicBase<TEntity, TRequestContract, TResponseContract> : DatabaseMappedLogicBase<TEntity, long, TRequestContract, TResponseContract>
        where TEntity : class, IIdSchema<long>
        where TResponseContract : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="mapperProvider"></param>
        public LongIdMappedDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider) : base(easyReadableQueryable, mapperProvider)
        {
        }
    }
}