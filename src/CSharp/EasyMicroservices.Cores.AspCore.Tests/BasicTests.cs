using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

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

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var client = _testServer.CreateClient();
            var data = await client.GetStringAsync($"api/user/getall");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            Assert.True(result);
        }

        [Fact]
        public async Task AuthorizeTest()
        {
            var client = _testServer.CreateClient();
            var data = await client.GetStringAsync($"api/user/AuthorizeError");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            Assert.False(result);
        }

        [Fact]
        public async Task InternalErrorTest()
        {
            var client = _testServer.CreateClient();
            var data = await client.GetStringAsync($"api/user/InternalError");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            Assert.False(result);
            Assert.Contains(result.Error.StackTrace, x => x.Contains("UserController.cs"));
        }
    }
}
