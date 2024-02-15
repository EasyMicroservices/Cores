using EasyMicroservices.Cores.Interfaces;
using System;
using System.Reflection.Emit;

namespace EasyMicroservices.Cores.Database.Widgets;
/// <summary>
/// 
/// </summary>
public abstract class DatabaseBuilderWidget<TModelBuilder> : IWidget
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="suffix"></param>
    /// <param name="prefix"></param>
    public abstract void OnModelCreating(TModelBuilder modelBuilder, string suffix = "", string prefix = "");
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Type GetObjectType()
    {
        return typeof(DatabaseBuilderWidget<TModelBuilder>);
    }
}
