using EasyMicroservices.Cores.Database.Schemas;
using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Database.Entities;
/// <summary>
/// 
/// </summary>
public class ActivityChangeLogEntity : ActivityChangeLogSchema, IIdSchema<long>
{
    /// <summary>
    /// 
    /// </summary>
    public long Id { get; set; }
}
