using EasyMicroservices.Cores.Interfaces;
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
    /// <param name="contract"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task Process(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, T contract, TEntity entity);
}
