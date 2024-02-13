using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Database.Interfaces;
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
    /// <param name="baseUnitOfWork"></param>
    /// <param name="contract"></param>
    /// <returns></returns>
    Task Add<T>(IBaseUnitOfWork baseUnitOfWork, T contract);
}
