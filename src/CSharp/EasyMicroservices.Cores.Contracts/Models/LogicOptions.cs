using EasyMicroservices.Cores.DataTypes;
using System;

namespace EasyMicroservices.Cores.Models;
/// <summary>
/// 
/// </summary>
public struct LogicOptions
{
    /// <summary>
    /// 
    /// </summary>
    public LogicOptions()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public UniqueIdentityStrategy UniqueIdentityStrategy { get; set; } = UniqueIdentityStrategy.Default;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueIdentityStrategy"></param>

    public static implicit operator LogicOptions(UniqueIdentityStrategy uniqueIdentityStrategy)
    {
        return new LogicOptions()
        {
            UniqueIdentityStrategy = uniqueIdentityStrategy
        };
    }
}
