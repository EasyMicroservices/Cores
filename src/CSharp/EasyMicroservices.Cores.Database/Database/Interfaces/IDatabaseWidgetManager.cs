using EasyMicroservices.Cores.Interfaces;
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
    /// <param name="contract"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task Add<T, TEntity>(IBaseUnitOfWork baseUnitOfWork, T contract, TEntity entity)
        where TEntity : class;
}
