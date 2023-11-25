using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Clients
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CoreSwaggerClientBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string BearerToken { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public void SetBearerToken(string token)
        {
            BearerToken = token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var requestMesasge = new HttpRequestMessage();
            if (BearerToken != null)
                requestMesasge.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
            return Task.FromResult(requestMesasge);
        }
    }
}
