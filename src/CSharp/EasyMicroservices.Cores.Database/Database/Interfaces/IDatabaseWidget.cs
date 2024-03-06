using EasyMicroservices.Cores.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Interfaces;
/// <summary>
/// 
/// </summary>
public interface IDatabaseWidget<TEntity, T> : IWidget<T>
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    bool CanProcess(IBaseUnitOfWork baseUnitOfWork);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="entity"></param>
    /// <param name="contract"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, TEntity entity, T contract, CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddBulkProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, Dictionary<T, TEntity> items, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
public interface IDatabaseWidget<TEntity> : IWidget<TEntity>
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    bool CanProcess(IBaseUnitOfWork baseUnitOfWork);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateBulkProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, List<TEntity> items, CancellationToken cancellationToken = default);
}