using System.Collections.Generic;

namespace EasyMicroservices.Cores.Models;
/// <summary>
/// 
/// </summary>
public class ServiceAddressInfo
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<DatabaseConfig> Databases { get; set; }
}