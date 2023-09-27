using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
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
    public class SimpleQueryServiceController<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> : ReadableQueryServiceController<TEntity, FilterRequestContract, TResponseContract, TId>
            where TResponseContract : class
            where TEntity : class, IIdSchema<TId>
    {
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="contractLogic"></param>
        public SimpleQueryServiceController(IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> contractLogic) : base(contractLogic)
        {
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public SimpleQueryServiceController(IBaseUnitOfWork unitOfWork) : base(unitOfWork.GetContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> WritableContractLogic
        {
            get
            {
                return ContractLogic as IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId>;
            }
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
            return WritableContractLogic.Add(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<MessageContract> AddBulk(CreateBulkRequestContract<TCreateRequestContract> request, CancellationToken cancellationToken = default)
        {
            return WritableContractLogic.AddBulk(request, cancellationToken);
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
            return WritableContractLogic.Update(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        public virtual Task<MessageContract> UpdateBulk(UpdateBulkRequestContract<TUpdateRequestContract> request, CancellationToken cancellationToken = default)
        {
            return WritableContractLogic.UpdateBulk(request, cancellationToken);
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
            return WritableContractLogic.HardDeleteById(request, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual Task<MessageContract> HardDeleteBulkByIds(DeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return WritableContractLogic.HardDeleteBulkByIds(request, cancellationToken);
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
            return WritableContractLogic.SoftDeleteById(request, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual Task<MessageContract> SoftDeleteBulkByIds(SoftDeleteBulkRequestContract<TId> request, CancellationToken cancellationToken = default)
        {
            return WritableContractLogic.SoftDeleteBulkByIds(request, cancellationToken);
        }
    }
}