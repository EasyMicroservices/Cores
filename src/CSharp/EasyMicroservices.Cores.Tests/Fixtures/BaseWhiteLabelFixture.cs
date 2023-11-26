using EasyMicroservices.Cores.Tests.Database;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts;
using EasyMicroservices.WhiteLabelsMicroservice.VirtualServerForTests;
using EasyMicroservices.WhiteLabelsMicroservice.VirtualServerForTests.TestResources;

namespace EasyMicroservices.Cores.Tests.Fixtures;
public class BaseWhiteLabelFixture
{
    public static async Task Run(int port)
    {
        var whiteLabelVirtualTestManager = new WhiteLabelVirtualTestManager();

        if (await whiteLabelVirtualTestManager.OnInitialize(port))
        {
            Console.WriteLine($"WhiteLabelVirtualTestManager Initialized! {port}");
            foreach (var item in WhiteLabelResource.GetResources(new MyTestContext(new DatabaseBuilder()), "TextExample"))
            {
                whiteLabelVirtualTestManager.AppendService(port, item.Key, item.Value);
            }
        }
    }
}
