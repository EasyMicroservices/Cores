using EasyMicroservices.Cores.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Collections.Generic;
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
    public class SimpleQueryServiceController<TEntity, TId, TCreateRequestContract, TUpdateRequestContract, TResponseContract> : ControllerBase
    {
        private readonly IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> _contractLogic;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractReadable"></param>
        public SimpleQueryServiceController(IContractLogic<TEntity, TCreateRequestContract, TUpdateRequestContract, TResponseContract, TId> contractReadable)
        {
            _contractLogic = contractReadable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<MessageContract<TResponseContract>> GetById(TId id, CancellationToken cancellationToken = default)
        {
            return _contractLogic.GetById(id, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageContract<TId>> Add(TCreateRequestContract request, CancellationToken cancellationToken = default)
        {
            var result = await _contractLogic.Add(request, cancellationToken);
            await _contractLogic.SaveChangesAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageContract<TResponseContract>> Update(TUpdateRequestContract request, CancellationToken cancellationToken = default)
        {
            var result = await _contractLogic.Update(request, cancellationToken);
            await _contractLogic.SaveChangesAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageContract<TResponseContract>> HardDeleteById(TId id, CancellationToken cancellationToken = default)
        {
            var result = await _contractLogic.HardDeleteById(id, cancellationToken);
            await _contractLogic.SaveChangesAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<MessageContract<List<TResponseContract>>> GetAll(CancellationToken cancellationToken = default)
        {
            return _contractLogic.GetAll(cancellationToken);
        }
    }
}