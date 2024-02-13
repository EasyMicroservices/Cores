using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Widgets;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Managers;

/// <summary>
/// 
/// </summary>
public class DatabaseWidgetManager : WidgetManager, IDatabaseWidgetManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="contract"></param>
    /// <returns></returns>
    public async Task Add<T>(IBaseUnitOfWork baseUnitOfWork, T contract)
    {
        var widgets = GetWidgetsByType(typeof(T));
        foreach (var widget in widgets)
        {
            if (widget is IDatabaseWidget<T> databaseWidget && databaseWidget.CanProcess(baseUnitOfWork))
            {
                await databaseWidget.Process(this, baseUnitOfWork, contract);
            }
        }
    }
}