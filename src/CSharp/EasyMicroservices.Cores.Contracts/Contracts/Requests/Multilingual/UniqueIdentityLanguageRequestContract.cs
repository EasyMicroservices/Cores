namespace EasyMicroservices.Cores.Contracts.Requests.Multilingual;
/// <summary>
/// 
/// </summary>
public class UniqueIdentityLanguageRequestContract : UniqueIdentityRequestContract
{
    /// <summary>
    /// 
    /// </summary>
    public string LanguageShortName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueIdentity"></param>
    public static implicit operator UniqueIdentityLanguageRequestContract(string uniqueIdentity)
    {
        return new UniqueIdentityLanguageRequestContract()
        {
            UniqueIdentity = uniqueIdentity
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public static implicit operator UniqueIdentityLanguageRequestContract((string UniqueIdentity, string LanguageShortName) input)
    {
        return new UniqueIdentityLanguageRequestContract()
        {
            UniqueIdentity = input.UniqueIdentity,
            LanguageShortName = input.LanguageShortName
        };
    }
}
