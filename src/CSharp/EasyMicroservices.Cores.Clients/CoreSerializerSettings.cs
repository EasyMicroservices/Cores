using Newtonsoft.Json;

namespace EasyMicroservices.Cores.Clients
{
    /// <summary>
    /// 
    /// </summary>
    public class CoreSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public CoreSerializerSettings(JsonSerializerSettings settings)
        {
            this.ContractResolver = new CoreContractResolver();
        }
    }
}
