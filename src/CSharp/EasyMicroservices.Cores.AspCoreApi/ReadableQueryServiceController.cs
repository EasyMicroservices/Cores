using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFilterContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    /// <typeparam name="TId"></typeparam>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ReadableQueryServiceController<TEntity, TFilterContract, TResponseContract, TId> : ControllerBase
        where TResponseContract : class
        where TEntity : class
        where TFilterContract : FilterRequestContract
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual IContractReadableLogic<TEntity, TResponseContract, TId> ContractLogic { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractReadable"></param>
        public ReadableQueryServiceController(IContractReadableLogic<TEntity, TResponseContract, TId> contractReadable)
        {
            ContractLogic = contractReadable;
        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ReadableQueryServiceController(IBaseUnitOfWork unitOfWork)
        {
            ContractLogic = unitOfWork.GetReadableContractLogic<TEntity, TResponseContract, TId>();
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
            return ContractLogic.GetByUniqueIdentity(request, request.Type, OnGetQuery(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<ListMessageContract<TResponseContract>> Filter(TFilterContract filterRequest, CancellationToken cancellationToken = default)
        {
            return ContractLogic.Filter(filterRequest, OnGetAllQuery(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual Task<ListMessageContract<TResponseContract>> GetAll(CancellationToken cancellationToken = default)
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
        public virtual Task<ListMessageContract<TResponseContract>> GetAllByUniqueIdentity(GetUniqueIdentityRequestContract request, CancellationToken cancellationToken = default)
        {
            return ContractLogic.GetAllByUniqueIdentity(request, request.Type, OnGetAllQuery(), cancellationToken);
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