using System.Text;

namespace EasyMicroservices.Cores.Models;

/// <summary>
/// 
/// </summary>
public class DatabaseConfig
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ProviderName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsSqlServer()
    {
        if (!ProviderName.HasValue())
            return false;
        return ProviderName.Equals("SqlServer", System.StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsInMemory()
    {
        if (!ProviderName.HasValue())
            return false;
        return ProviderName.Equals("InMemory", System.StringComparison.OrdinalIgnoreCase);
    }
}
