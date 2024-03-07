using EasyMicroservices.ServiceContracts;
using System.Collections;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Interfaces;
/// <summary>
/// 
/// </summary>
public interface IContentResolver
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    Task ResolveContentLanguage(object contract, string language);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task AddToContentLanguage(params object[] item);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task UpdateToContentLanguage(params object[] item);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="contract"></param>
    /// <returns></returns>
    Task ResolveContentAllLanguage(object contract);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    Task ResolveContentAllLanguage(IEnumerable items);
}
