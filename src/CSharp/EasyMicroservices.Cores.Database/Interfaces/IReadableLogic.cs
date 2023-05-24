using ServiceContracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Interfaces
{
    public interface IReadableLogic<TSchema, TId>
    {
        Task<MessageContract<TSchema>> GetById(TId id, CancellationToken cancellationToken = default);
        Task<MessageContract<List<TSchema>>> GetAll(CancellationToken cancellationToken = default);
    }
}
