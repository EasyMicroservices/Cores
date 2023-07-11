using EasyMicroservices.Cores.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TCreateRequestContract"></typeparam>
    /// <typeparam name="TUpdateRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SimpleQueryServiceController<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> ContractLogic { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractReadable"></param>
        public SimpleQueryServiceController(IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> contractReadable)
        {
            ContractLogic = contractReadable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual Task<MessageContract<TResponseContract>> GetById(TId id, CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetById(id, OnGetQuery(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<TId>> Add(TCreateRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.Add(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.Update(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<TResponseContract>> HardDeleteById(TId id, CancellationToken cancellationToken = default)
        {
            return ContractLogic.HardDeleteById(id, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual Task<MessageContract<List<TResponseContract>>> GetAll(CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetAll(OnGetAllQuery(), cancellationToken);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetQuery()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> OnGetAllQuery()
        {
            return null;
        }
    }
}