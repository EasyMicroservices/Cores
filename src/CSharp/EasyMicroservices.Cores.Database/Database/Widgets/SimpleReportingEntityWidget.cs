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
public class SimpleReportingEntityWidget<TReportEntity, TObjectContract> : IDatabaseWidget<TObjectContract>
    where TReportEntity : class
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
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Process(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, TObjectContract contract)
    {
        var reportEntity = await baseUnitOfWork
            .GetMapper()
            .MapAsync<TReportEntity>(contract);

        await baseUnitOfWork.GetLogic<TReportEntity>(new Models.LogicOptions()
        {
            DoStopReporting = true
        })
            .Add(reportEntity)
            .AsCheckedResult();
    }
}
