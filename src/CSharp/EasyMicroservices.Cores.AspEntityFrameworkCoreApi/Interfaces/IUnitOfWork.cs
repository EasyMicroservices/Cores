using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.Interfaces;
using Microsoft.Extensions.Configuration;

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
        IConfiguration GetConfiguration();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IAuthorization GetAuthorization();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase<TContext>()
                where TContext : RelationalCoreContext;
    }
}
