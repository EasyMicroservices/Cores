using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private IEasyReadableQueryableAsync<TEntity> UniqueIdentityQueryMaker<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, string uniqueIdentity)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (!_uniqueIdentityManager.IsUniqueIdentityForThisTable<TEntity>(easyReadableQueryable.Context, uniqueIdentity))
                uniqueIdentity += "-";
            queryable = queryable.ConvertToReadable(queryable.Where(x => (x as IUniqueIdentitySchema).UniqueIdentity.StartsWith(uniqueIdentity)));
            return queryable;
        }

        #region Get one

        /// <summary>
        /// get an item by an id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TEntity>> GetById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, GetIdRequestContract<TId> idRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.FirstOrDefaultAsync(x => x.Id.Equals(idRequest.Id), cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by id {idRequest.Id} not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="doCheckIsDelete"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Expression<Func<TEntity, bool>> predicate, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, bool doCheckIsDelete = true, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (doCheckIsDelete && typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
                return (FailedReasonType.NotFound, $"Item by predicate not found!");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = easyReadableQueryable;
            if (query != null)
                queryable = query(queryable);

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

            var result = await queryable.FirstOrDefaultAsync(cancellationToken);
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
            var entityResult = await GetBy(easyReadableQueryable, predicate, query, true, cancellationToken);
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
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<MessageContract<TContract>> GetById<TEntity, TContract, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, GetIdRequestContract<TId> idRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
            where TContract : class
        {
            var entityResult = await GetById(easyReadableQueryable, idRequest, query, cancellationToken);
            if (!entityResult)
                return entityResult.ToContract<TContract>();
            var result = await _mapperProvider.MapAsync<TContract>(entityResult.Result);
            ValidateMappedResult(ref result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract<TContract>> GetByUniqueIdentity<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IUniqueIdentitySchema request, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = null, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = UniqueIdentityQueryMaker(easyReadableQueryable, request.UniqueIdentity);
            var entityResult = await GetBy(queryable, query, cancellationToken);
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

            if (typeof(ISoftDeleteSchema).IsAssignableFrom(typeof(TEntity)))
                queryable = queryable.ConvertToReadable(queryable.Where(x => !(x as ISoftDeleteSchema).IsDeleted));

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract<List<TContract>>> GetAllByUniqueIdentity<TEntity, TContract>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IUniqueIdentitySchema request, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = null, CancellationToken cancellationToken = default)
            where TEntity : class
            where TContract : class
        {
            IEasyReadableQueryableAsync<TEntity> queryable = UniqueIdentityQueryMaker(easyReadableQueryable, request.UniqueIdentity);
            var entityResult = await GetAll(queryable, query, cancellationToken);
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
        public Task<MessageContract<TEntity>> Update<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return InternalUpdate(easyWritableQueryable, entity, cancellationToken, false, true);
        }

        private async Task<MessageContract<TEntity>> InternalUpdate<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, TEntity entity, CancellationToken cancellationToken = default, bool doSkipUpdate = true, bool doSkipDelete = true)
            where TEntity : class
        {
            var result = await easyWritableQueryable.Update(entity, cancellationToken);
            if (entity is IDateTimeSchema schema)
            {
                easyWritableQueryable.Context.ChangeModificationPropertyState(result.Entity, nameof(IDateTimeSchema.CreationDateTime), false);
                if (!doSkipUpdate)
                    schema.ModificationDateTime = DateTime.Now;
            }
            if (doSkipDelete && entity is ISoftDeleteSchema)
            {
                easyWritableQueryable.Context.ChangeModificationPropertyState(result.Entity, nameof(ISoftDeleteSchema.DeletedDateTime), false);
                easyWritableQueryable.Context.ChangeModificationPropertyState(result.Entity, nameof(ISoftDeleteSchema.IsDeleted), false);
            }
            await easyWritableQueryable.SaveChangesAsync();
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
            var result = await Update(easyWritableQueryable, entity, cancellationToken);
            if (!result)
                return result.ToContract<TContract>();
            var mappedResult = await _mapperProvider.MapAsync<TContract>(result.Result);
            ValidateMappedResult(ref mappedResult);
            return mappedResult;
        }

        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteById<TEntity, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, DeleteRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            var result = await easyWritableQueryable.RemoveAllAsync(x => x.Id.Equals(deleteRequest.Id), cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteById<TEntity, TContract, TId>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, DeleteRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            var result = await easyWritableQueryable.RemoveAllAsync(x => x.Id.Equals(deleteRequest.Id), cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> HardDeleteBy<TEntity>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
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
        public async Task<MessageContract> HardDeleteBy<TEntity, TContract>(IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
           where TEntity : class
        {
            var result = await easyWritableQueryable.RemoveAllAsync(predicate, cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="deleteRequest"></param>
        /// /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> SoftDeleteById<TEntity, TId>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, SoftDeleteRequestContract<TId> deleteRequest, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class, IIdSchema<TId>
        {
            return SoftDeleteBy(easyReadableQueryable, easyWritableQueryable, x => x.Id.Equals(deleteRequest.Id), deleteRequest.IsDelete, query, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWritableQueryable"></param>
        /// <param name="predicate"></param>
        /// <param name="isDelete"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract> SoftDeleteBy<TEntity>(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWritableQueryable, Expression<Func<TEntity, bool>> predicate, bool isDelete, Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> query = default, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var getResult = await GetBy(easyReadableQueryable, predicate, query, false, cancellationToken);
            if (!getResult)
                return getResult;
            if (getResult.Result is ISoftDeleteSchema softDeleteSchema)
            {
                softDeleteSchema.IsDeleted = isDelete;
                if (isDelete)
                    softDeleteSchema.DeletedDateTime = DateTime.Now;
                else
                    softDeleteSchema.DeletedDateTime = null;
                return await InternalUpdate(easyWritableQueryable, getResult.Result, cancellationToken, true, false);
            }
            else
                return (FailedReasonType.OperationFailed, $"Your entity type {getResult.Result.GetType().FullName} is not inheritance from ISoftDeleteSchema");
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
            if (entity is IDateTimeSchema schema)
                schema.CreationDateTime = DateTime.Now;

            var result = await easyWritableQueryable.AddAsync(entity, cancellationToken);
            await easyWritableQueryable.SaveChangesAsync();
            if (_uniqueIdentityManager.UpdateUniqueIdentity(easyWritableQueryable.Context, result.Entity))
            {
                await InternalUpdate(easyWritableQueryable, result.Entity, cancellationToken);
                await easyWritableQueryable.SaveChangesAsync();
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
