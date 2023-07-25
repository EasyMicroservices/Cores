using EasyMicroservices.Database.Interfaces;

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
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        bool IsUniqueIdentityForThisTable<TEntity>(IContext context, string uniqueIdentity);
    }
}
