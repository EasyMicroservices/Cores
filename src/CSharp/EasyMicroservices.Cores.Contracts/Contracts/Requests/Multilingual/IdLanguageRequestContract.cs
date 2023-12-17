namespace EasyMicroservices.Cores.Contracts.Requests.Multilingual;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class IdLanguageRequestContract<T> : IdRequestContract<T>
{
    /// <summary>
    /// 
    /// </summary>
    public string LanguageShortName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public static implicit operator IdLanguageRequestContract<T>(T id)
    {
        return new IdLanguageRequestContract<T>()
        {
            Id = id
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public static implicit operator IdLanguageRequestContract<T>((T Id, string LanguageShortName) input)
    {
        return new IdLanguageRequestContract<T>()
        {
            Id = input.Id,
            LanguageShortName = input.LanguageShortName
        };
    }
}