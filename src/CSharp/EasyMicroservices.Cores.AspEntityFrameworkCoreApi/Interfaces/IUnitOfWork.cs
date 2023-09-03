using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.Interfaces;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase<TContext>()
                where TContext : RelationalCoreContext;
    }
}
