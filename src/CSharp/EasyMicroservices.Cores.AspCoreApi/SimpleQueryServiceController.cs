using EasyMicroservices.Cores.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TRequestContract"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SimpleQueryServiceController<TEntity, TId, TRequestContract, TResponseContract> : ControllerBase
    {
        private readonly IContractLogic<TEntity, TRequestContract, TResponseContract, TId> _contractLogic;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractReadable"></param>
        public SimpleQueryServiceController(IContractLogic<TEntity, TRequestContract, TResponseContract, TId> contractReadable)
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
        public async Task<MessageContract<TId>> Add(TRequestContract request, CancellationToken cancellationToken = default)
        {
            var result = await  _contractLogic.Add(request, cancellationToken);
            await _contractLogic.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}