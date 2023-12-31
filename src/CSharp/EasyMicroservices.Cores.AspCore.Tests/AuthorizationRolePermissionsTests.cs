using EasyMicroservices.AuthenticationsMicroservice.VirtualServerForTests;
using EasyMicroservices.AuthenticationsMicroservice.VirtualServerForTests.TestResources;
using EasyMicroservices.Cores.AspCore.Tests.Fixtures;
using EasyMicroservices.ServiceContracts;
using System.Net.Http.Headers;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class AuthorizationRolePermissionsTests : BasicTests, IClassFixture<AuthorizationRolePermissionsFixture>
    {
        public override int AppPort => 4566;
        public AuthorizationRolePermissionsTests() : base()
        { }

        protected override void AssertTrue(MessageContract messageContract)
        {
            Assert.False(messageContract.IsSuccess);
            Assert.True(messageContract.Error.FailedReasonType == FailedReasonType.AccessDenied, messageContract.ToString());
        }

        protected override void AssertFalse(MessageContract messageContract)
        {
            Assert.False(messageContract);
            AssertTrue(messageContract);
        }

        protected override void AuthorizeAssert(MessageContract messageContract)
        {
            AssertTrue(messageContract);
        }

        static AuthenticationVirtualTestManager AuthenticationVirtualTestManager = new();
        [Theory]
        [InlineData("TestExampleFailed", "EndUser", "NoAccess", "NoAccess", false)]
        [InlineData("TestExample", "EndUser", "User", "CheckHasAccess", true)]
        [InlineData("TestExample", "EndUser", null, "CheckHasAccess", true)]
        [InlineData("TestExample", "EndUser", null, null, true)]
        [InlineData("TestExample", "EndUser", "User", null, true)]
        public async Task WriterRoleTest(string microserviceName, string roleName, string serviceName, string methodName, bool result)
        {
            int portNumber = 1045;
            await AuthenticationVirtualTestManager.OnInitialize(portNumber);
            var resources = AuthenticationResource.GetResources(microserviceName, new Dictionary<string, List<TestServicePermissionContract>>()
            {
                {
                    roleName ,
                    new List<TestServicePermissionContract>()
                    {
                        new TestServicePermissionContract()
                        {
                            MicroserviceName = microserviceName,
                            MethodName = methodName,
                            ServiceName = serviceName
                        }
                    }
                }
            });

            foreach (var resource in resources)
            {
                AuthenticationVirtualTestManager.AppendService(portNumber, resource.Key, resource.Value);
            }

            HttpClient CurrentHttpClient = new HttpClient();
            if (result)
            {
                var loginResult = await CurrentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/login");
                Assert.True(loginResult);
                CurrentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
            }
            var data = await CurrentHttpClient.GetFromJsonAsync<MessageContract>($"{GetBaseUrl()}/api/user/CheckHasAccess");
            if (!result)
                AssertTrue(data);
            else
                Assert.True(data, data.ToString());
        }

        [Theory]
        [InlineData("TestExampleFailed", "EndUser", "NoAccess")]
        [InlineData("TestExample", "EndUser", "User")]
        [InlineData("TestExample", "EndUser", null)]
        public async Task ReaderRoleTest(string microserviceName, string roleName, string serviceName)
        {
            int portNumber = 1045;
            await AuthenticationVirtualTestManager.OnInitialize(portNumber);
            var resources = AuthenticationResource.GetResources(microserviceName, new Dictionary<string, List<TestServicePermissionContract>>()
            {
                {
                    roleName ,
                    new List<TestServicePermissionContract>()
                    {
                        new TestServicePermissionContract()
                        {
                            MicroserviceName = microserviceName,
                            MethodName = "CheckHasAccess",
                            ServiceName = serviceName
                        }
                    }
                }
            });

            foreach (var resource in resources)
            {
                AuthenticationVirtualTestManager.AppendService(portNumber, resource.Key, resource.Value);
            }
            HttpClient CurrentHttpClient = new HttpClient();
            var loginResult = await CurrentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/login");
            Assert.True(loginResult);
            CurrentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
            var data = await CurrentHttpClient.GetFromJsonAsync<MessageContract>($"{GetBaseUrl()}/api/user/CheckHasNoAccess");
            AssertTrue(data);
        }
    }
}
