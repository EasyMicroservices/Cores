using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.Cores.Widgets;
using System.Collections.Generic;
using System.Threading;
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
    /// <param name="widgetBuilder"></param>
    public DatabaseWidgetManager(IWidgetBuilder widgetBuilder) : base(widgetBuilder)
    {
    }

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
    public async Task Add<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, TEntity entity, T contract, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var widgets = GetWidgetsByType(typeof(T));
        foreach (var widget in widgets)
        {
            if (widget is IDatabaseWidget<TEntity, T> databaseWidget && databaseWidget.CanProcess(baseUnitOfWork))
            {
                await databaseWidget.AddProcess(this, baseUnitOfWork, entity, contract);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddBulk<TEntity, T>(IBaseUnitOfWork baseUnitOfWork, Dictionary<T, TEntity> items, CancellationToken cancellationToken = default) where TEntity : class
    {
        var widgets = GetWidgetsByType(typeof(T));
        foreach (var widget in widgets)
        {
            if (widget is IDatabaseWidget<TEntity, T> databaseWidget && databaseWidget.CanProcess(baseUnitOfWork))
            {
                await databaseWidget.AddBulkProcess(this, baseUnitOfWork, items);
            }
        }
    }
}