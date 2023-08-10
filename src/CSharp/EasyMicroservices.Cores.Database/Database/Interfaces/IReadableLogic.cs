﻿using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResultSchema"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IReadableLogic<TResultSchema, TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<TResultSchema>> GetById(GetIdRequestContract<TId> idRequest, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<List<TResultSchema>>> GetAll(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IContractReadableLogic<TEntity, TContract, TId> : IReadableLogic<TContract, TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<TContract>> GetBy(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRequest"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<TContract>> GetById(GetIdRequestContract<TId> idRequest, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<TContract>> GetByUniqueIdentity(IUniqueIdentitySchema request, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<List<TContract>>> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<List<TContract>>> GetAllByUniqueIdentity(IUniqueIdentitySchema request, Func<IQueryable<TEntity>, IQueryable<TEntity>> query = default, CancellationToken cancellationToken = default);
    }
}