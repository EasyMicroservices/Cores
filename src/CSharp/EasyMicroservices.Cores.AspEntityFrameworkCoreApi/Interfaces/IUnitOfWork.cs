using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Database.Interfaces;
using EasyMicroservices.Mapper.Interfaces;
using System;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase<TContext>()
                where TContext : RelationalCoreContext;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMapperProvider GetMapper();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IUniqueIdentityManager GetUniqueIdentityManager();
    }
}
