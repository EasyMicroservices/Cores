using EasyMicroservices.ContentsMicroservice.Clients.Helpers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System.Linq;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
internal class InternalContentResolver : IContentResolver
{
    readonly ContentLanguageHelper _contentLanguageHelper;
    public InternalContentResolver(ContentLanguageHelper contentLanguageHelper)
    {
        _contentLanguageHelper = contentLanguageHelper;
    }

    public async Task AddToContentLanguage(params object[] items)
    {
        await Task.WhenAll(_contentLanguageHelper.AddToContentLanguage(items));
            //.Select(x => x.AsCheckedResult())); ; ;
    }

    public Task ResolveContentAllLanguage(object contract)
    {
        return _contentLanguageHelper.ResolveContentAllLanguage(contract);
    }

    public Task ResolveContentLanguage(object contract, string language)
    {
        return _contentLanguageHelper.ResolveContentLanguage(contract, language);
    }

    public Task UpdateToContentLanguage(params object[] item)
    {
        return _contentLanguageHelper.UpdateToContentLanguage(item);
    }
}
