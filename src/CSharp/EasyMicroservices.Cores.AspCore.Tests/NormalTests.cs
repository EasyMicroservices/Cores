using EasyMicroservices.Cores.AspCore.Tests.Fixtures;

namespace EasyMicroservices.Cores.AspCore.Tests;

public class NormalTests : BasicTests, IClassFixture<NormalFixture>
{
    public override int AppPort { get; } = 4564;
}
