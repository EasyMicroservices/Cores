using EasyMicroservices.ContentsMicroservice.Clients.Helpers;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
internal class InternalContentResolver : IContentResolver
{
    readonly ContentLanguageHelper _contentLanguageHelper;
    public InternalContentResolver(ContentLanguageHelper contentLanguageHelper)
    {
        _contentLanguageHelper = contentLanguageHelper;
    }

    public Task AddToContentLanguage(params object[] item)
    {
        return _contentLanguageHelper.AddToContentLanguage(item)
            .AsCheckedResult();
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
