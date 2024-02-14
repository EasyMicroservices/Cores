namespace EasyMicroservices.Cores.Database.Interfaces;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TId"></typeparam>
public interface IRecordIdSchema<TId>
{
    /// <summary>
    /// 
    /// </summary>
    public TId RecordId { get; set; }
}