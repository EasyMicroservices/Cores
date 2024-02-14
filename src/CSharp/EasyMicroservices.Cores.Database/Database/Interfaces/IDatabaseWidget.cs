using EasyMicroservices.Cores.Interfaces;
using System.Collections.Generic;
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
    /// <returns></returns>
    Task AddProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, TEntity entity, T contract);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    Task AddBulkProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, Dictionary<T, TEntity> items);
}
