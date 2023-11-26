using EasyMicroservices.Cores.AspCore.Tests.Fixtures;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Tests.Contracts.Common;
using EasyMicroservices.Cores.Tests.Fixtures;
using EasyMicroservices.ServiceContracts;
using Newtonsoft.Json;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public abstract class BasicTests
    {
        protected static HttpClient HttpClient { get; set; } = new HttpClient();
        public abstract int AppPort { get; }
        string RouteAddress
        {
            get
            {
                return $"http://localhost:{AppPort}";
            }
        }

        protected string GetBaseUrl()
        {
            return RouteAddress;
        }

        [Fact]
        public async Task<string> Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var data = await HttpClient.GetStringAsync($"{GetBaseUrl()}/api/user/getall");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            AssertTrue(result);
            return data;
        }

        [Fact]
        public async Task AuthorizeTest()
        {
            var data = await HttpClient.GetStringAsync($"{GetBaseUrl()}/api/user/AuthorizeError");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            AuthorizeAssert(result);
        }

        [Fact]
        public async Task InternalErrorTest()
        {
            var data = await HttpClient.GetStringAsync($"{GetBaseUrl()}/api/user/InternalError");
            var result = JsonConvert.DeserializeObject<MessageContract>(data);
            AssertFalse(result);
            if (result.Error.FailedReasonType != FailedReasonType.SessionAccessDenied && result.Error.FailedReasonType != FailedReasonType.AccessDenied)
                AssertContains(result.Error.StackTrace, x => x.Contains("UserController.cs"));
        }

        [Fact]
        public async Task AddUser()
        {
            var data = await HttpClient.PostAsJsonAsync($"{GetBaseUrl()}/api/user/Add", new UpdateUserContract()
            {
                UserName = "Ali",
                UniqueIdentity = "1-2"
            });
            var jsondata = await data.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageContract>(jsondata);
            AssertTrue(result);
            var getAllRespone = await Get_EndpointsReturnSuccessAndCorrectContentType();
            var users = JsonConvert.DeserializeObject<ListMessageContract<UpdateUserContract>>(getAllRespone);
            AssertTrue(users);
            if (users.IsSuccess)
                Assert.True(users.Result.All(x => DefaultUniqueIdentityManager.DecodeUniqueIdentity(x.UniqueIdentity).Length > 2),
                   JsonConvert.SerializeObject(users.Result));
        }

        protected virtual void AuthorizeAssert(MessageContract messageContract)
        {
            Assert.True(messageContract.Error.FailedReasonType == FailedReasonType.SessionAccessDenied);
        }

        protected virtual void AssertTrue(MessageContract messageContract)
        {
            Assert.True(messageContract);
        }

        protected virtual void AssertFalse(MessageContract messageContract)
        {
            Assert.False(messageContract);
        }

        protected virtual void AssertContains<T>(
            IEnumerable<T> collection,
            Predicate<T> filter)
        {
            Assert.Contains(collection, filter);
        }
    }
}
