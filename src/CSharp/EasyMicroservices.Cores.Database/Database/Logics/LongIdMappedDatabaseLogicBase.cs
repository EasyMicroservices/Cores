using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class LongIdMappedDatabaseLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract> : IdSchemaDatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, long>
        where TEntity : class
        where TResponseContract : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public LongIdMappedDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(easyReadableQueryable, mapperProvider, uniqueIdentityManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public LongIdMappedDatabaseLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(easyWriteableQueryable, mapperProvider, uniqueIdentityManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public LongIdMappedDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager) : base(easyReadableQueryable, easyWriteableQueryable, mapperProvider, uniqueIdentityManager)
        {

        }

    }
}