using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Tests.Contracts.Common;
using EasyMicroservices.Cores.Tests.Laboratories;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class BasicTests : WhiteLabelLaboratoryTest
    {
        protected TestServer _testServer;
        public BasicTests() : base(6041)
        {
            var webBuilder = new WebHostBuilder();
            webBuilder.UseStartup<Startup>();
            base.OnInitialize().Wait();
            _testServer = new TestServer(webBuilder);
        }

        [Fact]
        public async Task<string> Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var client = _testServer.CreateClient();
            var data = await client.GetStringAsync($"api/user/getall");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            Assert.True(result);
            return data;
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

        [Fact]
        public async Task AddUser()
        {
            var client = _testServer.CreateClient();
            var data = await client.PostAsJsonAsync($"api/user/Add", new UpdateUserContract()
            {
                UserName = "Ali",
                UniqueIdentity = "1-2"
            });
            var result = JsonConvert.DeserializeObject<MessageContract>(await data.Content.ReadAsStringAsync());
            Assert.True(result);
            var getAllRespone = await Get_EndpointsReturnSuccessAndCorrectContentType();
            var users = JsonConvert.DeserializeObject<ListMessageContract<UpdateUserContract>>(getAllRespone);
            Assert.True(users);
            Assert.True(users.Result.All(x => DefaultUniqueIdentityManager.DecodeUniqueIdentity(x.UniqueIdentity).Length > 2));
        }
    }
}
