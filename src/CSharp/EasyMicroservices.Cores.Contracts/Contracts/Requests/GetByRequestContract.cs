using EasyMicroservices.Cores.DataTypes;
using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Contracts.Requests;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TId"></typeparam>
public class GetByRequestContract<TId> : IUniqueIdentitySchema, IIdSchema<TId>
{
    /// <summary>
    /// 
    /// </summary>
    public TId Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UniqueIdentity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public GetUniqueIdentityType? UniqueIdentityType { get; set; }
}
