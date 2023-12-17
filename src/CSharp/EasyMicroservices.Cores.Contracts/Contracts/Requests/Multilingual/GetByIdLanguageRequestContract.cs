namespace EasyMicroservices.Cores.Contracts.Requests.Multilingual;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class GetByIdLanguageRequestContract<T> : GetByIdRequestContract<T>
{
    /// <summary>
    /// 
    /// </summary>
    public string LanguageShortName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public static implicit operator GetByIdLanguageRequestContract<T>(T id)
    {
        return new GetByIdLanguageRequestContract<T>()
        {
            Id = id
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public static implicit operator GetByIdLanguageRequestContract<T>((T Id, string LanguageShortName) input)
    {
        return new GetByIdLanguageRequestContract<T>()
        {
            Id = input.Id,
            LanguageShortName = input.LanguageShortName
        };
    }
}