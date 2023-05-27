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
    public class DatabaseLogicInfrastructure : IDisposable
#if (!NETSTANDARD2_0 && !NET45)
        , IAsyncDisposable
#endif
    {
        IMapperProvider _mapperProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapperProvider"></param>
        public DatabaseLogicInfrastructure(IMapperProvider mapperProvider)
        {
            _mapperProvider = mapperProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        public DatabaseLogicInfrastructure()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual void ValidateMappedResult<T>(ref T value)
            where T : class
        {
            if (value == default(T))
                throw new NullReferenceException("the result was null when we mapped it to contract! something went wrong!");
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
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyReadableQueryable.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by predicate not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> GetBy<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetBy(easyReadableQueryable, predicate, cancellationToken);
            if (entityResult == null)
                return entityResult.ToContract<TContract>();
            var result = _mapperProvider.Map<TContract>(entityResult.Result);
            ValidateMappedResult(ref result);
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

        #region Get list

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

        #region Update


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> Update<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.Update(entity, cancellationToken);
            return result.Entity;
        }

        #endregion

        #region Add

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> Add<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> Add<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TContract contract, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entity = _mapperProvider.Map<TEntity>(contract);
            ValidateMappedResult(ref entity);
            var result = await easyWritableQueryable.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {

        }

#if (!NETSTANDARD2_0 && !NET45)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
#endif
    }
}
