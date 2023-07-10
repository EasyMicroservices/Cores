using EasyMicroservices.Database.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUniqueIdentityManager
    {
        /// <summary>
        /// update unique identity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns>is need update database</returns>
        bool UpdateUniqueIdentity<TEntity>(IContext context, TEntity entity);
    }
}
