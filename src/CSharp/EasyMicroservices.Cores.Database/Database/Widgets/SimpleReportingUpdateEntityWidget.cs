using EasyMicroservices.Cores.Database.Helpers;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Widgets;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TReportEntity"></typeparam>
public class SimpleReportingUpdateEntityWidget<TEntity, TReportEntity> : SimpleReportingBaseEntityWidget, IDatabaseWidget<TEntity>
    where TReportEntity : class
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Type GetObjectType()
    {
        return typeof(TEntity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Initialize(params TEntity[] parameters)
    {
        return TaskHelper.GetCompletedTask();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task UpdateBulkProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, List<TEntity> items, CancellationToken cancellationToken = default)
    {
        var logic = baseUnitOfWork.GetLogic<TReportEntity>(new Models.LogicOptions()
        {
            DoStopReporting = true
        });
        var context = logic.GetReadableContext();
        var getRecordIds = items.Select(x => DatabaseExtensions.GetId(context, x).First()).ToArray();
        var reportEntities = await baseUnitOfWork
           .GetMapper()
           .MapToDictionaryAsync<TEntity, TReportEntity>(items);

        var allItems = await logic
             .GetAll(q => q.Where(x => getRecordIds.Contains(((IRecordIdSchema<long>)x).RecordId)), cancellationToken)
             .AsCheckedResult();

        await logic
            .UpdateBulk(reportEntities.Values.ToList(), cancellationToken)
            .AsCheckedResult();
    }
}
