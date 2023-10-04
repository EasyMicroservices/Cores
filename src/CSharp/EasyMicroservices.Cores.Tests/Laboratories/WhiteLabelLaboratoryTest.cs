using EasyMicroservices.Cores.Tests.Database;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.Laboratory.Engine;
using EasyMicroservices.Laboratory.Engine.Net.Http;
using EasyMicroservices.Payments.VirtualServerForTests.TestResources;
using EasyMicroservices.WhiteLabelsMicroservice.VirtualServerForTests;
using System.Text;

namespace EasyMicroservices.Cores.Tests.Laboratories
{
    public abstract class WhiteLabelLaboratoryTest
    {
        int Port = 6041;
        string _routeAddress = "";
        public static HttpClient HttpClient { get; set; } = new HttpClient();
        public WhiteLabelLaboratoryTest(int portNumber)
        {
            Port = portNumber;
            _routeAddress = $"http://localhost:{Port}";
        }

        protected static WhiteLabelVirtualTestManager WhiteLabelVirtualTestManager { get; set; } = new WhiteLabelVirtualTestManager();

        static bool _isInitialized = false;
        static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected async Task OnInitialize()
        {
            if (_isInitialized)
                return;
            try
            {
                await Semaphore.WaitAsync();
                if (_isInitialized)
                    return;
                _isInitialized = true;
                if (await WhiteLabelVirtualTestManager.OnInitialize(Port))
                {
                    foreach (var item in WhiteLabelResource.GetResources(new MyTestContext(new DatabaseBuilder()), "TextExample"))
                    {
                        WhiteLabelVirtualTestManager.AppendService(Port, item.Key, item.Value);
                    }
                }
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
