using LeanSharp.Extensions;
using System.Collections.Generic;
using Xunit;

namespace LeanSharp.Tests
{
    public class SafeEnumerableExtensionsTests
    {
        [Fact]
        public void SafeEnumerableExtensionMethodsReturnFalseIfCollectionIsNull()
        {
            List<int> numbers = null;

            Assert.False(numbers.SafeAny());
            Assert.False(numbers.SafeAny(number => number > 1));

            Assert.False(numbers.SafeCount());
            Assert.False(numbers.SafeCount(number => number > 1));
        }

        [Fact]
        public void SafeEnumerableExtensionMethodsReturnFalseIfCollectionIsEmpty()
        {
            var numbers = new List<int>();

            Assert.False(numbers.SafeAny());
            Assert.False(numbers.SafeAny(number => number > 1));

            Assert.False(numbers.SafeCount());
            Assert.False(numbers.SafeCount(number => number > 1));
        }

        [Fact]
        public void SafeEnumerableExtensionMethodsReturnTrueIfCollectionIsNotNullOrEmpty()
        {
            var numbers = new List<int> { 1, 2, 3 };

            Assert.True(numbers.SafeAny());
            Assert.True(numbers.SafeAny(number => number > 1));

            Assert.True(numbers.SafeCount());
            Assert.True(numbers.SafeCount(number => number > 1));
        }
    }
}
