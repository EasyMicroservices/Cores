using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class LongIdDatabaseLogicBase<TEntity> : DatabaseLogicBase<TEntity, long>, IReadableLogic<TEntity, long>, IWritableLogic<TEntity, long>
        where TEntity : class, IIdSchema<long>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="mapperProvider"></param>
        public LongIdDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider) : base(easyReadableQueryable, mapperProvider)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        public LongIdDatabaseLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider) : base(easyWriteableQueryable, mapperProvider)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="mapperProvider"></param>
        public LongIdDatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IMapperProvider mapperProvider) : base(easyReadableQueryable, easyWriteableQueryable, mapperProvider)
        {
        }
    }
}
