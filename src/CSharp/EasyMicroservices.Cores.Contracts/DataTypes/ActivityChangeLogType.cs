namespace EasyMicroservices.Cores.DataTypes;
/// <summary>
/// 
/// </summary>
public enum ActivityChangeLogType : byte
{
    /// <summary>
    /// value is none, Never use the None to return values
    /// </summary>
    None = 0,
    /// <summary>
    /// error value is default
    /// </summary>
    Default = 1,
    /// <summary>
    /// for the filter values from web admin panel you can sent all for types
    /// </summary>
    All = 2,
    /// <summary>
    /// there is other error that is not in the types
    /// </summary>
    Other = 3,
    /// <summary>
    /// the error type is uknown to us
    /// </summary>
    Unknown = 4,
    /// <summary>
    /// there is nothing to show or validate error
    /// </summary>
    Nothing = 5,
    /// <summary>
    /// 
    /// </summary>
    Add = 6,
    /// <summary>
    /// 
    /// </summary>
    Update = 7,
    /// <summary>
    /// 
    /// </summary>
    SoftDelete = 8,
    /// <summary>
    /// 
    /// </summary>
    HardDelete = 9,
    /// <summary>
    /// 
    /// </summary>
    UpdateValuesChangedOnly = 10,
    /// <summary>
    /// 
    /// </summary>
    AddBulk = 11,
    /// <summary>
    /// 
    /// </summary>
    UpdateBulk = 12,
    /// <summary>
    /// 
    /// </summary>
    SoftDeleteBulk = 13,
    /// <summary>
    /// 
    /// </summary>
    HardDeleteBulk = 14,
}