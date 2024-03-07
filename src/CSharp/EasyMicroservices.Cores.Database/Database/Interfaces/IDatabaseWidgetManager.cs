using EasyMicroservices.Cores.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Interfaces;
/// <summary>
/// 
/// </summary>
public interface IDatabaseWidgetManager : IWidgetManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="entity"></param>
    /// <param name="contract"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Add<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, TEntity entity, T contract, CancellationToken cancellationToken = default)
        where TEntity : class;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddBulk<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, Dictionary<T, TEntity> items, CancellationToken cancellationToken = default)
        where TEntity : class;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateBulk<TEntity>(IBaseUnitOfWork baseUnitOfWork, List<TEntity> items, CancellationToken cancellationToken = default)
        where TEntity : class;
}
