using LeanSharp.Extensions;
using System.Text;
using Xunit;

namespace LeanSharp.Tests
{
    public class StringBuilderExtensionsTests
    {
        [Fact]
        public void AppendIf_AppendsTextIfPredicateIsTrue()
        {
            var stringBuilder = new StringBuilder("initial text.");

            stringBuilder.AppendIf(sb => sb.Length > 5, " appended text");

            Assert.Equal("initial text. appended text", stringBuilder.ToString());
        }

        [Fact]
        public void AppendIf_DoesNotAppendTextIfPredicateIsFalse()
        {
            var stringBuilder = new StringBuilder("initial text.");

            stringBuilder.AppendIf(sb => sb.Length > 20, " appended text");

            Assert.Equal("initial text.", stringBuilder.ToString());
        }
    }
}
