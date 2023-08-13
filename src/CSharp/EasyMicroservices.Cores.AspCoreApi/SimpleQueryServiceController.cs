using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
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
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<TResponseContract>> GetById(GetIdRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetById(request, OnGetQuery(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<TResponseContract>> GetByUniqueIdentity(GetUniqueIdentityRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetByUniqueIdentity(request, OnGetQuery(), cancellationToken);
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
        [HttpPut]
        public virtual Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.Update(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual Task<MessageContract> HardDeleteById(DeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.HardDeleteById(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual Task<MessageContract> SoftDeleteById(SoftDeleteRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.SoftDeleteById(request, cancellationToken);
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
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract<List<TResponseContract>>> GetAllByUniqueIdentity(GetUniqueIdentityRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetAllByUniqueIdentity(request, OnGetAllQuery(), cancellationToken);
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