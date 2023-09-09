using EasyMicroservices.Laboratory.Engine;
using EasyMicroservices.Laboratory.Engine.Net.Http;
using System.Text;

namespace EasyMicroservices.Cores.Tests.Laboratories
{
    public class WhiteLabelLaboratoryTest
    {
        const int Port = 6041;
        string _routeAddress = "";
        public static HttpClient HttpClient { get; set; } = new HttpClient();
        public WhiteLabelLaboratoryTest()
        {
            _routeAddress = $"http://localhost:{Port}";
        }

        static bool _isInitialized = false;
        static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected async Task OnInitialize()
        {
            if (_isInitialized)
                return;
            try
            {
                await Semaphore.WaitAsync();
                _isInitialized = true;

                ResourceManager resourceManager = new ResourceManager();
                HttpHandler httpHandler = new HttpHandler(resourceManager);
                await httpHandler.Start(Port);
                resourceManager.Append(@$"GET /api/WhiteLabel/GetAll HTTP/1.1
Host: localhost:{Port}
Accept: text/plain*RequestSkipBody*"
,
@"HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 0

{""isSuccess"":true,""result"":[{""id"":1,""name"":""ProjectName""},{""id"":2,""name"":""TenantName"",""parentId"":1}]}");


                resourceManager.Append(@$"GET /api/Microservice/GetAll HTTP/1.1
Host: localhost:{Port}
Accept: text/plain*RequestSkipBody*"
,
@"HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 0

{""isSuccess"":true,""result"":[{""id"":1,""name"":""TextExample"",""description"":""Automatically added""}]}");


                resourceManager.Append(@$"POST /api/Microservice/Add HTTP/1.1
Host: localhost:{Port}
Accept: text/plain*RequestSkipBody*"
,
@"HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 0

{""isSuccess"":true,""result"": 1}");

                resourceManager.Append(@$"GET /api/MicroserviceContextTable/GetAll HTTP/1.1
Host: localhost:{Port}
Accept: text/plain*RequestSkipBody*"
,
@"HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 0

{""isSuccess"":true,""result"":[{""microserviceName"":""TextExample"",""microserviceId"":1,""contextName"":""MyTestContext"",""tableName"":""AddressEntity"",""contextTableId"":1},{""microserviceName"":""TextExample"",""microserviceId"":1,""contextName"":""MyTestContext"",""tableName"":""CompanyEntity"",""contextTableId"":2},{""microserviceName"":""TextExample"",""microserviceId"":1,""contextName"":""MyTestContext"",""tableName"":""ProfileEntity"",""contextTableId"":3},{""microserviceName"":""TextExample"",""microserviceId"":1,""contextName"":""MyTestContext"",""tableName"":""UserCompanyEntity"",""contextTableId"":4},{""microserviceName"":""TextExample"",""microserviceId"":1,""contextName"":""MyTestContext"",""tableName"":""UserEntity"",""contextTableId"":5}]}");

                resourceManager.Append(@$"GET /api/ContextTable/GetAll HTTP/1.1
Host: localhost:{Port}
Accept: text/plain*RequestSkipBody*"
                ,
                @"HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 0

{""isSuccess"":true,""result"":[{""id"":1,""tableName"":""AddressEntity"",""contextName"":""MyTestContext""},{""id"":2,""tableName"":""CompanyEntity"",""contextName"":""MyTestContext""},{""id"":3,""tableName"":""ProfileEntity"",""contextName"":""MyTestContext""},{""id"":4,""tableName"":""UserCompanyEntity"",""contextName"":""MyTestContext""},{""id"":5,""tableName"":""UserEntity"",""contextName"":""MyTestContext""}]}");

            }
            finally
            {
                Semaphore.Release();
            }
        }

        [Fact]
        public async Task WhiteLabelGetAllTest()
        {
            await OnInitialize();
            var whiteLabelClient = new WhiteLables.GeneratedServices.WhiteLabelClient(_routeAddress, HttpClient);
            var all = await whiteLabelClient.GetAllAsync();
            Assert.True(all.IsSuccess);
            Assert.True(all.Result.Count == 2);
            Assert.True(all.Result.All(x => x.Name.HasValue()));
        }

        [Fact]
        public async Task MicroserviceGetAllTest()
        {
            await OnInitialize();
            var client = new WhiteLables.GeneratedServices.MicroserviceClient(_routeAddress, HttpClient);
            var all = await client.GetAllAsync();
            Assert.True(all.IsSuccess);
            Assert.True(all.Result.Count == 1);
            Assert.True(all.Result.All(x => x.Name == "TextExample"));
        }

        [Fact]
        public async Task MicroserviceAddTest()
        {
            await OnInitialize();
            var client = new WhiteLables.GeneratedServices.MicroserviceClient(_routeAddress, HttpClient);
            var added = await client.AddAsync(new WhiteLables.GeneratedServices.MicroserviceContract()
            {
                Name = "TextExample",
                Description = "Automatically added"
            });
            Assert.True(added.IsSuccess);
            Assert.True(added.Result == 1);
        }


        [Fact]
        public async Task MicroserviceContextTableGetAllTest()
        {
            await OnInitialize();
            var client = new WhiteLables.GeneratedServices.MicroserviceContextTableClient(_routeAddress, HttpClient);
            var all = await client.GetAllAsync();
            Assert.True(all.IsSuccess);
            Assert.True(all.Result.Count >= 5);
            Assert.True(all.Result.All(x => x.MicroserviceName == "TextExample"));
        }

        [Fact]
        public async Task ContextTableGetAllTest()
        {
            await OnInitialize();
            var client = new WhiteLables.GeneratedServices.ContextTableClient(_routeAddress, HttpClient);
            var all = await client.GetAllAsync();
            Assert.True(all.IsSuccess);
            Assert.True(all.Result.Count >= 5);
        }
    }
}
