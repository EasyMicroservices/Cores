using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.DataTypes;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
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
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class DatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract> : DatabaseLogicInfrastructure, IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TResponseContract>
        where TEntity : class
        where TResponseContract : class
    {
        readonly IEasyReadableQueryableAsync<TEntity> _easyReadableQueryable;
        readonly IEasyWritableQueryableAsync<TEntity> _easyWriteableQueryable;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseMappedLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        public DatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork) : base(baseUnitOfWork)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
            _easyReadableQueryable = easyReadableQueryable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ListMessageContract<TResponseContract>> GetAll(CancellationToken cancellationToken = default)
        {
            return await GetAll<TEntity, TResponseContract>(_easyReadableQueryable, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<MessageContract<TResponseContract>> GetById(GetByIdRequestContract<TResponseContract> contract, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            throw new Exception("GetById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this GetById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TResponseContract>> GetBy(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetBy<TEntity, TResponseContract>(_easyReadableQueryable, predicate, func, cancellationToken);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TResponseContract>> Add(TCreateRequestContract contract, CancellationToken cancellationToken = default)
        {
            var result = await Add(_easyWriteableQueryable, contract, cancellationToken);
            if (!result)
                return result.ToContract<TResponseContract>();
            var mapped = await MapAsync<TResponseContract, TEntity>(result.Result);
            ValidateMappedResult(ref mapped);
            return mapped;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ListMessageContract<TResponseContract>> AddBulk(CreateBulkRequestContract<TCreateRequestContract> request, CancellationToken cancellationToken = default)
        {
            var result = await AddBulk(_easyWriteableQueryable, request, cancellationToken);
            if (!result)
                return result.ToListContract<TResponseContract>();
            var mapped = await MapAsync<List<TResponseContract>, List<TEntity>>(result.Result);
            ValidateMappedResult(ref mapped);
            return mapped;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract<TEntity>> AddEntity(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Add<TEntity>(_easyWriteableQueryable, entity, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ListMessageContract<TResponseContract>> Filter(FilterRequestContract filterRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return Filter<TEntity, TResponseContract>(filterRequest, _easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ListMessageContract<TResponseContract>> Filter(FilterRequestContract filterRequest, CancellationToken cancellationToken = default)
        {
            return Filter<TEntity, TResponseContract>(filterRequest, _easyReadableQueryable, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ListMessageContract<TResponseContract>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return await GetAll<TEntity, TResponseContract>(_easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetById(GetByIdRequestContract<TResponseContract> contract, CancellationToken cancellationToken = default)
        {
            throw new Exception("GetById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this GetById method");
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
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteBy(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return HardDeleteBy<TEntity, TResponseContract>(_easyWriteableQueryable, predicate, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isDelete"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> SoftDeleteBy(Expression<Func<TEntity, bool>> predicate, bool isDelete, CancellationToken cancellationToken = default)
        {
            return SoftDeleteBy(_easyReadableQueryable, _easyWriteableQueryable, predicate, isDelete, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract schema, CancellationToken cancellationToken = default)
        {
            return Update<TEntity, TUpdateRequestContract, TResponseContract>(_easyWriteableQueryable, schema, false, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> UpdateChangedValuesOnly(TUpdateRequestContract schema, CancellationToken cancellationToken = default)
        {
            return Update<TEntity, TUpdateRequestContract, TResponseContract>(_easyWriteableQueryable, schema, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> UpdateBulk(UpdateBulkRequestContract<TUpdateRequestContract> schema, CancellationToken cancellationToken = default)
        {
            return UpdateBulk(_easyWriteableQueryable, schema, false, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> UpdateBulkChangedValuesOnly(UpdateBulkRequestContract<TUpdateRequestContract> schema, CancellationToken cancellationToken = default)
        {
            return UpdateBulk(_easyWriteableQueryable, schema, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteById(DeleteRequestContract<TResponseContract> contract, CancellationToken cancellationToken = default)
        {
            throw new Exception("HardDeleteById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this HardDeleteById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteBulkByIds(DeleteBulkRequestContract<TResponseContract> request, CancellationToken cancellationToken = default)
        {
            throw new Exception("HardDeleteBulkByIds is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this HardDeleteBulkByIds method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetByUniqueIdentity(IUniqueIdentitySchema request, GetUniqueIdentityType type = GetUniqueIdentityType.All, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return base.GetByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, type, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListMessageContract<TResponseContract>> GetAllByUniqueIdentity(IUniqueIdentitySchema request, GetUniqueIdentityType type = GetUniqueIdentityType.All, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = null;
            if (query != null)
                func = (q) => _easyReadableQueryable.ConvertToReadable(query(_easyReadableQueryable));
            return base.GetAllByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, type, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract> SoftDeleteById(SoftDeleteRequestContract<TResponseContract> deleteRequest, CancellationToken cancellationToken = default)
        {
            throw new Exception("SoftDeleteById is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this SoftDeleteById method");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<MessageContract> SoftDeleteBulkByIds(SoftDeleteBulkRequestContract<TResponseContract> deleteRequest, CancellationToken cancellationToken = default)
        {
            throw new Exception("SoftDeleteBulkByIds is not supported in DatabaseMappedLogicBase, you can use IdSchemaDatabaseMappedLogicBase or override this SoftDeleteBulkByIds method");
        }
    }
}
