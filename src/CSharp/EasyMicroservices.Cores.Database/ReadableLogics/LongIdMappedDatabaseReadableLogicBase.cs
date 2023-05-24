using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using ServiceContracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.ReadableLogics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContract"></typeparam>
    public class LongIdMappedDatabaseReadableLogicBase<TEntity, TContract> : DatabaseReadableLogicBase, IReadableLogic<TContract, long>
        where TEntity : class, IIdSchema<long>
        where TContract : class
    {
        readonly IEasyReadableQueryableAsync<TEntity> _easyReadableQueryable;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="mapperProvider"></param>
        public LongIdMappedDatabaseReadableLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IMapperProvider mapperProvider) : base(mapperProvider)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<List<TContract>>> GetAll(CancellationToken cancellationToken = default)
        {
            return await GetAll<TEntity, TContract>(_easyReadableQueryable, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> GetById(long id, CancellationToken cancellationToken = default)
        {
            return await GetById<TEntity, TContract, long>(_easyReadableQueryable, id, cancellationToken);
        }
    }
}