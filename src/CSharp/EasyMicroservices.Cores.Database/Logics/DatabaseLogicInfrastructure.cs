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
        /// <summary>
        /// 
        /// </summary>
        internal protected readonly IMapperProvider _mapperProvider;
        readonly IUniqueIdentityManager _uniqueIdentityManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapperProvider"></param>
        /// <param name="uniqueIdentityManager"></param>
        public DatabaseLogicInfrastructure(IMapperProvider mapperProvider, IUniqueIdentityManager uniqueIdentityManager)
        {
            _mapperProvider = mapperProvider;
            _uniqueIdentityManager = uniqueIdentityManager;
        }

        /// <summary>
        /// 
        /// </summary>
        public DatabaseLogicInfrastructure(IUniqueIdentityManager uniqueIdentityManager)
        {
            _uniqueIdentityManager = uniqueIdentityManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual void ValidateMappedResult<T>(ref T value)
        {
            if (value == null || value.Equals(default(T)))
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
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TEntity>> GetById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, TId id, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);
            var result = await queryable.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
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
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);
            var result = await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
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
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> GetBy<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetBy(easyReadableQueryable, predicate, query, cancellationToken);
            if (entityResult == null)
                return entityResult.ToContract<TContract>();
            var result = await _mapperProvider.MapAsync<TContract>(entityResult.Result);
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
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TContract>> GetById<TEntity, TContract, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, TId id, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
            where TContract : class
        {
            var entityResult = await GetById(easyReadableQueryable, id, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = await _mapperProvider.MapAsync<TContract>(entityResult.Result);
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
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<List<TEntity>>> GetAll<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);
            return await queryable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// get all items mapped
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<List<TContract>>> GetAll<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            var entityResult = await GetAll(easyReadableQueryable, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<List<TContract>>();
            var result = await _mapperProvider.MapToListAsync<TContract>(entityResult.Result);
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TUpdateContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> Update<TEntity, TUpdateContract, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TUpdateContract contract, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entity = await _mapperProvider.MapAsync<TEntity>(contract);
            ValidateMappedResult(ref entity);
            var result = await easyWritableQueryable.Update(entity, cancellationToken);
            var mappedResult = await _mapperProvider.MapAsync<TContract>(result.Entity);
            ValidateMappedResult(ref mappedResult);
            return mappedResult;
        }

        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> HardDeleteById<TEntity, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TId id, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            var result = await easyWritableQueryable.RemoveAllAsync(x => x.Id.Equals(id), cancellationToken);
            return result.Entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> HardDeleteById<TEntity, TContract, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TId id, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            var result = await easyWritableQueryable.RemoveAllAsync(x => x.Id.Equals(id), cancellationToken);
            var mappedResult = await _mapperProvider.MapAsync<TContract>(result.Entity);
            ValidateMappedResult(ref mappedResult);
            return mappedResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> HardDeleteBy<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            return result.Entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TContract>> HardDeleteBy<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
           where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            var mappedResult = await _mapperProvider.MapAsync<TContract>(result.Entity);
            ValidateMappedResult(ref mappedResult);
            return mappedResult;
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
            if (_uniqueIdentityManager.UpdateUniqueIdentity(result.Entity))
            {
                await Update(easyWritableQueryable, result.Entity, cancellationToken);
            }
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
            var entity = await _mapperProvider.MapAsync<TEntity>(contract);
            ValidateMappedResult(ref entity);
            var result = await Add<TEntity>(easyWritableQueryable, entity, cancellationToken);
            return result;
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
