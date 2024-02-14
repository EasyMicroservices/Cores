using EasyMicroservices.Cores.Database.Helpers;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Widgets;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TReportEntity"></typeparam>
/// <typeparam name="TObjectContract"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class SimpleReportingEntityWidget<TEntity, TReportEntity, TObjectContract> : IDatabaseWidget<TEntity, TObjectContract>
    where TReportEntity : class
    where TEntity : class
    where TObjectContract : class
{
    /// <summary>
    /// 
    /// </summary>
    public void Build()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public bool CanProcess(IBaseUnitOfWork baseUnitOfWork)
    {
        if (baseUnitOfWork.LogicOptions.HasValue)
            return !baseUnitOfWork.LogicOptions.Value.DoStopReporting;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Type GetObjectType()
    {
        return typeof(TObjectContract);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Initialize(params TObjectContract[] parameters)
    {
        return TaskHelper.GetCompletedTask();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="contract"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Process(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, TObjectContract contract, TEntity entity)
    {
        var reportEntity = await baseUnitOfWork
            .GetMapper()
            .MapAsync<TReportEntity>(contract);
        var logic = baseUnitOfWork.GetLogic<TReportEntity>(new Models.LogicOptions()
        {
            DoStopReporting = true
        });
        DatabaseExtensions.SetIdToRecordId(logic.GetReadableContext(), entity, reportEntity);
        await logic
            .Add(reportEntity)
            .AsCheckedResult();
    }
}
