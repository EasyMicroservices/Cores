namespace EasyMicroservices.Cores.Contracts.Requests.Multilingual;
/// <summary>
/// 
/// </summary>
public class GetByUniqueIdentityLanguageRequestContract : GetByUniqueIdentityRequestContract
{
    /// <summary>
    /// 
    /// </summary>
    public string LanguageShortName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueIdentity"></param>
    public static implicit operator GetByUniqueIdentityLanguageRequestContract(string uniqueIdentity)
    {
        return new GetByUniqueIdentityLanguageRequestContract()
        {
            UniqueIdentity = uniqueIdentity
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public static implicit operator GetByUniqueIdentityLanguageRequestContract((string UniqueIdentity, string LanguageShortName) input)
    {
        return new GetByUniqueIdentityLanguageRequestContract()
        {
            UniqueIdentity = input.UniqueIdentity,
            LanguageShortName = input.LanguageShortName
        };
    }
}
