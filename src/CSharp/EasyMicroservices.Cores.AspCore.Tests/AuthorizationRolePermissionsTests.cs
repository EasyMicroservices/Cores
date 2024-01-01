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
            Assert.False(messageContract.IsSuccess, "Access true, expect false");
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

        async Task AddPermissions(HttpClient currentHttpClient, string microserviceName, string roleName, string serviceName, string methodName, bool hasAccess)
        {
            if (!hasAccess)
                return;
            var loginResult = await currentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/login2?role=Owner&uniqueIdentity=1-2");
            Assert.True(loginResult);
            currentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
            var roleClient = new Authentications.GeneratedServices.RoleClient("http://localhost:1044", currentHttpClient);
            var addRole = await roleClient.AddAsync(new Authentications.GeneratedServices.AddRoleRequestContract()
            {
                Name = roleName,
            }).AsCheckedResult(x => x.Result);

            var servicePermissionClient = new Authentications.GeneratedServices.ServicePermissionClient("http://localhost:1044", currentHttpClient);
            var addServicePermission = await servicePermissionClient.AddAsync(new Authentications.GeneratedServices.AddServicePermissionRequestContract
            {
                AccessType = Authentications.GeneratedServices.AccessPermissionType.Granted,
                MethodName = methodName,
                ServiceName = serviceName,
                MicroserviceName = microserviceName
            }).AsCheckedResult(x => x.Result);

            var roleServicePermissionClient = new Authentications.GeneratedServices.RoleServicePermissionClient("http://localhost:1044", currentHttpClient);
            var addRolePermission = await roleServicePermissionClient.AddAsync(new Authentications.GeneratedServices.AddRoleServicePermissionRequestContract
            {
                RoleId = addRole,
                ServicePermissionId = addServicePermission
            }).AsCheckedResult(x => x.Result);
        }

        //static AuthenticationVirtualTestManager AuthenticationVirtualTestManager = new();
        //[Theory]
        //[InlineData("TestExampleFailed", "EndUser", "NoAccess", "NoAccess", false)]
        //[InlineData("TestExample", "EndUser", "User", "CheckHasAccess", true)]
        //[InlineData("TestExample", "EndUser", null, "CheckHasAccess", true)]
        //[InlineData("TestExample", "EndUser", null, null, true)]
        //[InlineData("TestExample", "EndUser", "User", null, true)]
        //public async Task WriterRoleTest(string microserviceName, string roleName, string serviceName, string methodName, bool result)
        //{
        //    HttpClient currentHttpClient = new HttpClient();
        //    await AddPermissions(currentHttpClient, microserviceName, roleName, serviceName, methodName, result);
        //    if (result)
        //    {
        //        var loginResult = await currentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/login");
        //        Assert.True(loginResult);
        //        currentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
        //    }
        //    var data = await currentHttpClient.GetFromJsonAsync<MessageContract>($"{GetBaseUrl()}/api/user/CheckHasAccess");
        //    if (!result)
        //        AssertTrue(data);
        //    else
        //        Assert.True(data, data.ToString());
        //}

        [Theory]
        [InlineData("TestExampleFailed", "TestEndUser", "NoAccess")]
        [InlineData("TestExample", "TestEndUser", "User")]
        [InlineData("TestExample", "TestEndUser", null)]
        public async Task ReaderRoleTest(string microserviceName, string roleName, string serviceName)
        {
            HttpClient currentHttpClient = new HttpClient();
            //await AddPermissions(currentHttpClient, microserviceName, roleName, serviceName, null, true);
            //var loginResult = await currentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/Login2?role={roleName}");
            //Assert.True(loginResult, loginResult.Error?.ToString());
            //currentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
            var data = await currentHttpClient.GetFromJsonAsync<MessageContract>($"{GetBaseUrl()}/api/user/CheckHasNoAccess");
            AssertTrue(data);
        }
    }
}
