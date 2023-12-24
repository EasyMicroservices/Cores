namespace EasyMicroservices.Cores.Contracts.Requests.Multilingual;
/// <summary>
/// 
/// </summary>
public class GetByLanguageRequestContract
{
    /// <summary>
    /// 
    /// </summary>
    public string LanguageShortName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="languageShortName"></param>
    public static implicit operator GetByLanguageRequestContract(string languageShortName)
    {
        return new GetByLanguageRequestContract()
        {
            LanguageShortName = languageShortName
        };
    }
}
