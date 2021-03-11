using FluentAssertions;
using Xunit;

namespace Notifo.SDK.UnitTests
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
