using EasyMicroservices.Cores.AspCore.Tests.Fixtures;
using EasyMicroservices.ServiceContracts;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public class AuthorizationTests : BasicTests, IClassFixture<AuthorizationFixture>
    {
        public override int AppPort { get; } = 4565;

        protected override void AssertTrue(MessageContract messageContract)
        {
            Assert.True(messageContract.Error.FailedReasonType == FailedReasonType.SessionAccessDenied);
        }

        protected override void AssertFalse(MessageContract messageContract)
        {
            Assert.False(messageContract);
            AssertTrue(messageContract);
        }
    }
}
