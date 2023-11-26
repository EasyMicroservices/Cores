using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.ServiceContracts;

namespace EasyMicroservices.Cores.AspCore.Tests
{
    public abstract class AuthorizationTests : BasicTests
    {
        public override int AppPort => 4565;
        public AuthorizationTests() : base()
        { }

        protected override void InitializeTestHost(bool isUseAuthorization, Action<IServiceCollection> serviceCollection)
        {
            serviceCollection = (services) =>
            {
                services.AddScoped<IAuthorization, AppAuthorization>();
            };
            base.InitializeTestHost(true, serviceCollection);
        }

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
