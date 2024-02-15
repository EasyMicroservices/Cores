using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Widgets;
/// <summary>
/// 
/// </summary>
public abstract class ReportingBuilderWidget : IWidget
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Type GetObjectType()
    {
        return typeof(ReportingBuilderWidget);
    }
}
