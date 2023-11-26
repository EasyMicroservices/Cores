using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.Database.Interfaces;
using System;
using System.Threading.Tasks;

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
        /// <param name="baseUnitOfWork"></param>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns>is need update database</returns>
        Task<bool> UpdateUniqueIdentity<TEntity>(IBaseUnitOfWork baseUnitOfWork, IContext context, TEntity entity);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        bool IsUniqueIdentityForThisTable<TEntity>(IContext context, string uniqueIdentity);
        /// <summary>
        /// get table first segment item of unique identity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetTableUniqueIdentity<TEntity>(IContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        string GetLastTableUniqueIdentity(string uniqueIdentity);
    }
}
