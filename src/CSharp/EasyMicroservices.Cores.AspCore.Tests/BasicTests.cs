using Microsoft.AspNetCore.TestHost;
namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class BasicTests
    {
        protected TestServer _testServer;
        public BasicTests()
        {
            var webBuilder = new WebHostBuilder();
            webBuilder.UseStartup<Startup>();

            _testServer = new TestServer(webBuilder);
        }

        [Theory]
        [InlineData("/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var client = _testServer.CreateClient();
            var data = await client.GetStringAsync($"api/user/getall");
        }
    }
}
