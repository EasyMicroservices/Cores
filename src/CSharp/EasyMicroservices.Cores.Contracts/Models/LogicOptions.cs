using EasyMicroservices.Cores.DataTypes;

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
    UniqueIdentityStrategy _UniqueIdentityStrategy = UniqueIdentityStrategy.Default;
    /// <summary>
    /// 
    /// </summary>
    public UniqueIdentityStrategy UniqueIdentityStrategy
    {
        get
        {
            if (_UniqueIdentityStrategy == UniqueIdentityStrategy.None)
                _UniqueIdentityStrategy = UniqueIdentityStrategy.Default;
            return _UniqueIdentityStrategy;
        }
        set
        {
            _UniqueIdentityStrategy = value;
        }
    }
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
