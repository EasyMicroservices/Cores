using Contents.GeneratedServices;
using EasyMicroservices.ContentsMicroservice.Clients.Attributes;

namespace EasyMicroservices.Cores.Tests.Contracts.Common;
public class MultiLanguageUserContract
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string UniqueIdentity { get; set; }
    [ContentLanguage(nameof(Names))]
    public List<LanguageDataContract> Names { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDateTime { get; set; }
    public DateTime CreationDateTime { get; set; }
    public DateTime? ModificationDateTime { get; set; }
}
