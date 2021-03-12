using FluentAssertions;
using Xunit;

namespace NotifoIO.SDK.UnitTests
{
    public class TargetTests
    {
        [Fact]
        public void TestProject_ShouldUseNetStandardTarget()
        {
			Notifo.PlatformName.Should().BeEquivalentTo(".NET Standard");
        }
    }
}
