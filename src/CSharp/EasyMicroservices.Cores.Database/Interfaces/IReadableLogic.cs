using ServiceContracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSchema"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IReadableLogic<TSchema, TId>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<TSchema>> GetById(TId id, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MessageContract<List<TSchema>>> GetAll(CancellationToken cancellationToken = default);
    }
}
