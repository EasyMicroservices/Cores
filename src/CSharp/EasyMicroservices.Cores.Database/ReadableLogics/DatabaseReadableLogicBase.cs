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
    public class DatabaseReadableLogicBase
    {
        IMapperProvider _mapperProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapperProvider"></param>
        public DatabaseReadableLogicBase(IMapperProvider mapperProvider)
        {
            _mapperProvider = mapperProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        public DatabaseReadableLogicBase()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="System.NullReferenceException"></exception>
        protected virtual void ValidateMappedResult<T>(ref T value)
            where T : class
        {
            if (value == default(T))
                throw new System.NullReferenceException("the result was null when we mapped it to contract! something went wrong!");
        }

        #region Get one

        /// <summary>
        /// get an item by an id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TEntity>> GetById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, TId id, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            var result = await easyReadableQueryable.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by id {id} not found!");
            return result;
        }

        /// <summary>
        /// get an item by an id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TContract>> GetById<TEntity, TContract, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, TId id, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
            where TContract : class
        {
            var entityResult = await GetById(easyReadableQueryable, id, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = _mapperProvider.Map<TContract>(entityResult.Result);
            ValidateMappedResult(ref result);
            return result;
        }

        #endregion

        #region get list

        /// <summary>
        /// get all items
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<List<TEntity>>> GetAll<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await easyReadableQueryable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// get all items mapped
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<List<TContract>>> GetAll<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetAll(easyReadableQueryable, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<List<TContract>>();
            var result = _mapperProvider.Map<List<TContract>>(entityResult.Result);
            ValidateMappedResult(ref result);
            return result;
        }

        #endregion
    }
}
