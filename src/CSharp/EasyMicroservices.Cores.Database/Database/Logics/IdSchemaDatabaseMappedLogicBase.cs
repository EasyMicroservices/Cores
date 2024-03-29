﻿using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.DataTypes;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
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
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class IdSchemaDatabaseMappedLogicBase<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> : DatabaseLogicInfrastructure, IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>
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
        /// <param name="logicOptions"></param>
        public IdSchemaDatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IBaseUnitOfWork baseUnitOfWork, LogicOptions logicOptions = default) : base(baseUnitOfWork, logicOptions)
        {
            _easyReadableQueryable = easyReadableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        /// <param name="logicOptions"></param>
        public IdSchemaDatabaseMappedLogicBase(IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork, LogicOptions logicOptions = default) : base(baseUnitOfWork, logicOptions)
        {
            _easyWriteableQueryable = easyWriteableQueryable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easyReadableQueryable"></param>
        /// <param name="easyWriteableQueryable"></param>
        /// <param name="baseUnitOfWork"></param>
        /// <param name="logicOptions"></param>
        public IdSchemaDatabaseMappedLogicBase(IEasyReadableQueryableAsync<TEntity> easyReadableQueryable, IEasyWritableQueryableAsync<TEntity> easyWriteableQueryable, IBaseUnitOfWork baseUnitOfWork, LogicOptions logicOptions = default) : base(baseUnitOfWork, logicOptions)
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
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TResponseContract>> GetById(GetByIdRequestContract<TId> idRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return await GetById<TEntity, TResponseContract, TId>(_easyReadableQueryable, idRequest, func, cancellationToken);
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
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return await GetBy<TEntity, TResponseContract>(_easyReadableQueryable, predicate, func, cancellationToken);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetBy(GetByRequestContract<TId> request, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return GetBy<TEntity, TResponseContract, TId>(_easyReadableQueryable, request, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetById(GetByIdRequestContract<TId> idRequest, CancellationToken cancellationToken = default)
        {
            return GetById<TEntity, TResponseContract, TId>(_easyReadableQueryable, idRequest, null, cancellationToken);
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
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return await GetAll<TEntity, TResponseContract>(_easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ListMessageContract<TResponseContract>> GetAllByUniqueIdentity(IUniqueIdentitySchema request, GetUniqueIdentityType type = GetUniqueIdentityType.All, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return base.GetAllByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, type, func, cancellationToken);
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
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return base.GetByUniqueIdentity<TEntity, TResponseContract>(_easyReadableQueryable, request, type, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListMessageContract<TResponseContract>> Filter(FilterRequestContract filterRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = null, CancellationToken cancellationToken = default)
        {
            Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> func = UpdateFunctionQuery(query);
            return Filter<TEntity, TResponseContract>(filterRequest, _easyReadableQueryable, func, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListMessageContract<TResponseContract>> Filter(FilterRequestContract filterRequest, CancellationToken cancellationToken = default)
        {
            return Filter<TEntity, TResponseContract>(filterRequest, _easyReadableQueryable, null, cancellationToken);
        }

        TId MapToTId<T>(T result)
        {
            if (result is IIdSchema<TId> schema)
                return schema.Id;
            else if (result is TId tid)
                return tid;
            else if ((typeof(TId) == typeof(long) || typeof(TId) == typeof(int)) && result?.GetType() != typeof(string) && (result?.GetType()?.IsClass).GetValueOrDefault())
                throw new Exception($"I think you cannot convert TId from type {typeof(TId).Name} to type {result?.GetType()?.Name}, maybe you don't need to use LongLogic?");
            return MapperProvider.Map<TId>(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MessageContract<TId>> Add(TCreateRequestContract contract, CancellationToken cancellationToken = default)
        {
            var result = await Add(_easyWriteableQueryable, contract, cancellationToken);
            if (result)
                return MapToTId(result.Result);
            return result.ToContract<TId>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ListMessageContract<TId>> AddBulk(CreateBulkRequestContract<TCreateRequestContract> request, CancellationToken cancellationToken = default)
        {
            var result = await AddBulk(_easyWriteableQueryable, request, cancellationToken);
            if (result)
            {
                List<TId> items = new List<TId>();
                foreach (var item in result.Result)
                {
                    items.Add(MapToTId(item));
                }
                return items;
            }
            return result.ToListContract<TId>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Obsolete]
        public Task<MessageContract<TEntity>> AddEntity(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Add<TEntity, TEntity>(_easyWriteableQueryable, entity, entity, cancellationToken);
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
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteById(DeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return HardDeleteById<TEntity, TResponseContract, TId>(_easyWriteableQueryable, request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> HardDeleteBulkByIds(DeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return HardDeleteBulkByIds(_easyWriteableQueryable, request, cancellationToken);
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
            return SoftDeleteBy<TEntity, TId>(_easyReadableQueryable, _easyWriteableQueryable, predicate, isDelete, null, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> SoftDeleteBulkByIds(SoftDeleteBulkRequestContract<TId> deleteRequest, CancellationToken cancellationToken = default)
        {
            return SoftDeleteBulkByIds(_easyReadableQueryable, _easyWriteableQueryable, deleteRequest, null, cancellationToken);
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
        /// <exception cref="NotImplementedException"></exception>
        public Task<MessageContract> UpdateBulkChangedValuesOnly(UpdateBulkRequestContract<TUpdateRequestContract> schema, CancellationToken cancellationToken = default)
        {
            return UpdateBulk(_easyWriteableQueryable, schema, true, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected Func<IEasyReadableQueryableAsync<TEntity>, IEasyReadableQueryableAsync<TEntity>> UpdateFunctionQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
        {
            if (query != null)
                return (q) => _easyReadableQueryable.ConvertToReadable(query(q));
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IContext GetReadableContext()
        {
            return _easyReadableQueryable.Context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IContext GetWritableContext()
        {
            return _easyWriteableQueryable.Context;
        }
    }
}
