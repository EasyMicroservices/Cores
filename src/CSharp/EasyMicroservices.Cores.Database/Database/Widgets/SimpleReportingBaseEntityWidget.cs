using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Widgets;
/// <summary>
/// 
/// </summary>
public abstract class SimpleReportingBaseEntityWidget
{
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
    public abstract Type GetObjectType();
}
