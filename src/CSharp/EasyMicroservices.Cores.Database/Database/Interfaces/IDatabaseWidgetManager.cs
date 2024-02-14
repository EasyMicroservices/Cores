using EasyMicroservices.Cores.Interfaces;
using System.Collections.Generic;
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
    /// <returns></returns>
    Task Add<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, TEntity entity, T contract)
        where TEntity : class;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    Task AddBulk<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, Dictionary<T, TEntity> items)
        where TEntity : class;
}
