using EasyMicroservices.Cores.Database.Interfaces;
using ServiceContracts;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TResponseContract"></typeparam>
    public class SimpleQueryServiceCore<TEntity, TId, TResponseContract>
        where TEntity : class, IIdSchema<TId>
    {
        IContractReadableLogic<TEntity, TResponseContract, TId> _databaseLogicBase;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseLogicBase"></param>
        public SimpleQueryServiceCore(IContractReadableLogic<TEntity, TResponseContract, TId> databaseLogicBase)
        {
            _databaseLogicBase = databaseLogicBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MessageContract<TResponseContract>> GetById(TId id, CancellationToken cancellationToken = default)
        {
            return _databaseLogicBase.GetById(id, cancellationToken);
        }


    }
}