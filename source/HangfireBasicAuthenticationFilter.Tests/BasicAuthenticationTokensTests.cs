using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace HangfireBasicAuthenticationFilter.Tests
{
    [TestFixture]
    public class BasicAuthenticationTokensTests
    {
        [Test]
        public void Are_Invalid_WhenWhenValid_ShouldReturnFalse()
        {
            //---------------Arrange-------------------
            var tokens = new[] {"user", "pass"};
            var sut = new BasicAuthenticationTokens(tokens);
            //---------------Act----------------------
            var actual = sut.Are_Invalid();
            //---------------Assert-----------------------
            actual.Should().BeFalse();
        }

        [TestCase("","")]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase("  ", " ")]
        public void Are_Invalid_WhenWhenInvalid_ShouldReturnFalse(string token1, string token2)
        {
            //---------------Arrange-------------------
            var tokens = new[] { token1, token2 };

            var sut = new BasicAuthenticationTokens(tokens);
            //---------------Act----------------------
            var actual = sut.Are_Invalid();
            //---------------Assert-----------------------
            actual.Should().BeTrue();
        }
    }
}
