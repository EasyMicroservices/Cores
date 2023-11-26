using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Tests.Contracts.Common;
using EasyMicroservices.Cores.Tests.Fixtures;
using EasyMicroservices.ServiceContracts;
using Newtonsoft.Json;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public abstract class BasicTests : WhiteLabelLaboratoryFixture
    {
        public BasicTests()
        {
            InitializeTestHost(false, null);
        }
        public virtual int AppPort { get; } = 4564;
        protected virtual void InitializeTestHost(bool isUseAuthorization, Action<IServiceCollection> serviceCollection)
        {
            Exception exception = default;
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
            Thread thread = new Thread(async () =>
            {
                try
                {
                    await Startup.Run(AppPort, serviceCollection, null);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    taskCompletionSource.SetResult();
                }
            });
            thread.Start();
            taskCompletionSource.Task.GetAwaiter().GetResult();
            if (exception != default)
                throw new Exception("see inner", exception);
        }

        string RouteAddress
        {
            get
            {
                return $"http://{localhost}:{AppPort}";
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
            var result = JsonConvert.DeserializeObject<MessageContract>(await data.Content.ReadAsStringAsync());
            AssertTrue(result);
            var getAllRespone = await Get_EndpointsReturnSuccessAndCorrectContentType();
            var users = JsonConvert.DeserializeObject<ListMessageContract<UpdateUserContract>>(getAllRespone);
            AssertTrue(users);
            if (users.IsSuccess)
                AssertTrue(users.Result.All(x => DefaultUniqueIdentityManager.DecodeUniqueIdentity(x.UniqueIdentity).Length > 2));
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
