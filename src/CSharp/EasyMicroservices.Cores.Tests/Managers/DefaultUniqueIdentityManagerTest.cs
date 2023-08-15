using EasyMicroservices.Cores.Database.Managers;

namespace EasyMicroservices.Cores.Tests.Managers
{
    public class DefaultUniqueIdentityManagerTest
    {
        [Theory]
        [InlineData("1-a", "g-b")]
        [InlineData("1-2", "1-1")]
        [InlineData("Ab-xD1", "qe1-Efd")]
        public void TestMerge(string u1, string u2)
        {
            var result = DefaultUniqueIdentityManager.MergeUniqueIdentities(u1, u2);
            Assert.Equal(result, u1 + "-" + u2);
        }
    }
}
