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
        /// <typeparam name="TContext"></typeparam>
        /// <param name="entity"></param>
        /// <returns>is need update database</returns>
        bool UpdateUniqueIdentity<TContext, TEntity>(TEntity entity);
    }
}
