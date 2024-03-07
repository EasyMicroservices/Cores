﻿using EasyMicroservices.Cores.AspCore.Tests.Fixtures;
using EasyMicroservices.Cores.Contracts.Requests;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using EasyMicroservices.ServiceContracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EasyMicroservices.Cores.AspCore.Tests.Security;

public class UniqueIdentityTests : IClassFixture<UniqueIdentityAuthorizationRolePermissionsFixture>
{
	public int AppPort => 4567;
	public UniqueIdentityTests()
	{ }

	string GetBaseUrl()
	{
		return $"http://localhost:{AppPort}";
	}

	async Task Login(HttpClient currentHttpClient, string roleName, string fromUniqueIdentity)
	{
		var loginResult = await currentHttpClient.GetFromJsonAsync<MessageContract<string>>($"{GetBaseUrl()}/api/user/login2?role={roleName}&uniqueIdentity={fromUniqueIdentity}");
		Assert.True(loginResult);
		currentHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result);
	}

	[Theory]
	[InlineData("Owner", "User", "Add", "1-2", @"{""UniqueIdentity"":""""}", true)]
	[InlineData("Owner", "User", "Add", "1-2", @"{""UniqueIdentity"":""1-2-3-4""}", true)]
	[InlineData("Owner", "User", "Add", "1-2", @"{""UniqueIdentity"":""3-4""}", true)]
	[InlineData("Moderator", "User", "Add", "1-2", @"{""UniqueIdentity"":""3-4-3-4""}", false)]
	[InlineData("Moderator", "User", "Add", "1-2", @"{""UniqueIdentity"":""3-4""}", false)]
	[InlineData("Moderator", "User", "Add", "1-2", @"{""UniqueIdentity"":""1-2""}", true)]
	public async Task<long> AddAsync(string roleName, string controller, string method, string fromUniqueIdentity, string data, bool canHaveAccess)
	{
		HttpClient currentHttpClient = new HttpClient();
		await Login(currentHttpClient, roleName, fromUniqueIdentity);
		var content = new StringContent(data, Encoding.UTF8, "application/json");
		var apiResult = await currentHttpClient.PostAsync($"{GetBaseUrl()}/api/{controller}/{method}", content);
		var response = await apiResult.Content.ReadAsStringAsync();
		var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContract<long>>(response);
		if (canHaveAccess)
		{
			Assert.True(result, result.Error?.ToString());
			content = new StringContent($"{{\"Id\":{result.Result}}}", Encoding.UTF8, "application/json");
			var apiResult2 = await currentHttpClient.PostAsync($"{GetBaseUrl()}/api/{controller}/getbyid", content);
			var response2 = await apiResult2.Content.ReadAsStringAsync();
			var result2 = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContract<UserEntity>>(response2);
			var dataUid = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEntity>(data);

			Assert.True(result2.Result.UniqueIdentity.Length > dataUid.UniqueIdentity.Length);
		}
		else
		{
			Assert.False(result, "User has access, expect: no access!");
			Assert.True(!result && !result.Error.Message.Contains("There is no claim role founded"), result.Error.ToString());
			Assert.True(result.Error.FailedReasonType == FailedReasonType.AccessDenied, result.Error.ToString());
		}
		return result.Result;
	}

	[Theory]
	[InlineData("Owner", "User", "Update", "1-2", "1-2", @"{""UniqueIdentity"":""""}", @"{""UniqueIdentity"":""""}", true)]
	[InlineData("Owner", "User", "Update", "1-2", "1-2", @"{""UniqueIdentity"":""1-2-3-4""}", @"{""UniqueIdentity"":""1-2-3-4""}", true)]
	[InlineData("Owner", "User", "Update", "1-2", "1-2", @"{""UniqueIdentity"":""3-4""}", @"{""UniqueIdentity"":""3-4""}", true)]
	[InlineData("Moderator", "User", "Update", "1-2", "1-2", @"{""UniqueIdentity"":""1-2""}", @"{""UniqueIdentity"":""1-2""}", true)]
	//[InlineData("Moderator", "User", "Update", "1-2", "1-2", @"{""UniqueIdentity"":""1-2""}", @"{""UniqueIdentity"":""3-4""}", false)]
	public async Task UpdateAsync(string roleName, string controller, string method, string fromUniqueIdentity, string toUniqueIdentity, string addData, string data, bool canHaveAccess)
	{
		var model = JsonSerializer.Deserialize<DataModel>(data);
		model.Id = await AddAsync(roleName, controller, "Add", fromUniqueIdentity, addData, true);
		model.UserName = Guid.NewGuid().ToString();
		HttpClient currentHttpClient = new HttpClient();
		await Login(currentHttpClient, roleName, fromUniqueIdentity);
		var apiResult = await currentHttpClient.PutAsJsonAsync($"{GetBaseUrl()}/api/{controller}/{method}", model);
		var response = await apiResult.Content.ReadAsStringAsync();
		var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContract<UserEntity>>(response);
		if (canHaveAccess)
		{
			Assert.True(result, result.Error?.ToString());
			Assert.Equal(result.Result.UserName, model.UserName);
		}
		else
		{
			Assert.False(result, "User has access, expect: no access!");
			Assert.True(!result && !result.Error.Message.Contains("There is no claim role founded"), result.Error.ToString());
			Assert.True(result.Error.FailedReasonType == FailedReasonType.AccessDenied, result.Error.ToString());
		}

	}

	[Theory]
	[InlineData("Owner", "User", "GetByUniqueIdentity", "1-2", "1-2", "1-2", @"{""UniqueIdentity"":""""}", true)]
	[InlineData("Owner", "User", "GetByUniqueIdentity", "1-2", "1-2", "1-2", @"{""UniqueIdentity"":""1-2-3-4""}", true)]
	[InlineData("Owner", "User", "GetByUniqueIdentity", "1-2", "3-4", "1-2", @"{""UniqueIdentity"":""3-4""}", true)]
	[InlineData("Moderator", "User", "GetByUniqueIdentity", "1-2", "3-4", "1-2", @"{""UniqueIdentity"":""3-4""}", false)]
	[InlineData("Moderator", "User", "GetByUniqueIdentity", "3-4", "3-4", "3-4", @"{""UniqueIdentity"":""3-4""}", true)]
	[InlineData("Moderator", "User", "GetByUniqueIdentity", "1-2", "1-2", "1-2", @"{""UniqueIdentity"":""1-2""}", true)]
	public async Task GetByUniqueIdentityAsync(string roleName, string controller, string method, string userUniqueIdentity, string fromUniqueIdentity, string toUniqueIdentity, string data, bool canHaveAccess)
	{
		var model = JsonSerializer.Deserialize<DataModel>(data);
		model.Id = await AddAsync(roleName, controller, "Add", fromUniqueIdentity, data, true);
		HttpClient currentHttpClient = new HttpClient();
		await Login(currentHttpClient, roleName, userUniqueIdentity);
		var apiResult = await currentHttpClient.PostAsJsonAsync($"{GetBaseUrl()}/api/{controller}/{method}", new GetByUniqueIdentityRequestContract()
		{
			UniqueIdentity = fromUniqueIdentity,
			Type = DataTypes.GetUniqueIdentityType.All
		});
		var response = await apiResult.Content.ReadAsStringAsync();
		var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContract<UserEntity>>(response);
		if (canHaveAccess)
		{
			Assert.True(result, result.Error?.ToString());
			Assert.True(result.Result.UniqueIdentity.StartsWith(fromUniqueIdentity));
		}
		else
		{
			Assert.False(result, "User has access, expect: no access!");
			Assert.True(result.Error.FailedReasonType == FailedReasonType.NotFound || result.Error.FailedReasonType == FailedReasonType.AccessDenied, result.Error.ToString());
		}
	}

	class DataModel
	{
		public long Id { get; set; }
		public string UniqueIdentity { get; set; }
		public string UserName { get; set; }
	}
}

