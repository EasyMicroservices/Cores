using EasyMicroservices.Cores.DataTypes;

namespace EasyMicroservices.Cores.Database.Schemas;
/// <summary>
/// 
/// </summary>
public class ActivityChangeLogSchema : FullAbilitySchema
{
    /// <summary>
    /// 
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string RecordIdentity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string JsonData { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string RequesterUniqueIdentity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ActivityChangeLogType Type { get; set; }
}
