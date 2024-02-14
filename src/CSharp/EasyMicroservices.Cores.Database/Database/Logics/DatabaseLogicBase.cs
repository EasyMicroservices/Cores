using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class DatabaseLogicBase<TEntity, TId> : DatabaseLogicInfrastructure
        where TEntity : class, IIdSchema<TId>
    {
        readonly IEasyReadableQueryableAsync<TEntity> _easyReadableQueryable;
        readonly IEasyWritableQueryableAsync<TEntity> _easyWriteableQueryable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetById(GetByIdRequestContract<TId> idRequest, CancellationToken cancellationToken = default)
        {
            return await GetById(_easyReadableQueryable, idRequest, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetById(GetByIdRequestContract<TId> idRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetById(_easyReadableQueryable, idRequest, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ListMessageContract<TEntity>> GetAll(CancellationToken cancellationToken = default)
        {
            return await GetAll(_easyReadableQueryable, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ListMessageContract<TEntity>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetAll(_easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListMessageContract<TEntity>> Filter(FilterRequestContract filterRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return Filter<TEntity>(filterRequest, _easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListMessageContract<TEntity>> Filter(FilterRequestContract filterRequest, CancellationToken cancellationToken = default)
        {
            return Filter<TEntity>(filterRequest, _easyReadableQueryable, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TEntity>> GetBy(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetBy(_easyReadableQueryable, predicate, func, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TEntity>> Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Update(_easyWriteableQueryable, entity, false, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<MessageContract<TEntity>> UpdateChangedValuesOnly(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Update(_easyWriteableQueryable, entity, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<MessageContract> UpdateBulk(UpdateBulkRequestContract<TEntity> request, CancellationToken cancellationToken = default)
        {
            return UpdateBulk(_easyWriteableQueryable, request, false, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<MessageContract> UpdateBulkChangedValuesOnly(UpdateBulkRequestContract<TEntity> request, CancellationToken cancellationToken = default)
        {
            return UpdateBulk(_easyWriteableQueryable, request, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> HardDeleteById(DeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return HardDeleteById(_easyWriteableQueryable, request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteBulkByIds(DeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return HardDeleteBulkByIds(_easyWriteableQueryable, request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> HardDeleteBy(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return HardDeleteBy<TEntity, TId>(_easyWriteableQueryable, predicate, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> SoftDeleteById(SoftDeleteRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
        {
            return SoftDeleteById(_easyReadableQueryable, _easyWriteableQueryable, deleteRequest, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<MessageContract> SoftDeleteBulkByIds(SoftDeleteBulkRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
        {
            return SoftDeleteBulkByIds(_easyReadableQueryable, _easyWriteableQueryable, deleteRequest, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _easyWriteableQueryable.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IContext GetReadableContext()
        {
            return _easyReadableQueryable.Context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IContext GetWritableContext()
        {
            return _easyWriteableQueryable.Context;
        }
    }
}
