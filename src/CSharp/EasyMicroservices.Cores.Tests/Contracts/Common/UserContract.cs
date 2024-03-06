namespace EasyMicroservices.Cores.Tests.Contracts.Common;
public class UserContract
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string UniqueIdentity { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDateTime { get; set; }
    public DateTime CreationDateTime { get; set; }
    public DateTime? ModificationDateTime { get; set; }
}
