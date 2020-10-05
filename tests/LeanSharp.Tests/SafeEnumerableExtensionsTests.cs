using LeanSharp.Extensions;
using System.Collections.Generic;
using System.Linq;
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

            Assert.Equal(0, numbers.SafeCount());
            Assert.Equal(0, numbers.SafeCount(number => number > 1));
        }

        [Fact]
        public void SafeEnumerableExtensionMethodsReturnFalseIfCollectionIsEmpty()
        {
            var numbers = new List<int>();

            Assert.False(numbers.SafeAny());
            Assert.False(numbers.SafeAny(number => number > 1));

            Assert.Equal(0, numbers.SafeCount());
            Assert.Equal(0, numbers.SafeCount(number => number > 1));
        }

        [Fact]
        public void SafeEnumerableExtensionMethodsReturnTrueIfCollectionIsNotNullOrEmpty()
        {
            var numbers = new List<int> { 1, 2, 3 };

            Assert.True(numbers.SafeAny());
            Assert.True(numbers.SafeAny(number => number > 1));

            Assert.Equal(3, numbers.SafeCount());
            Assert.Equal(2, numbers.SafeCount(number => number > 1));
        }

        [Fact]
        public void IfSafeAnyMap_ExecutesMappingFunctionIfCollectionIsNotNull()
        {
            var numbers = new List<int> { 1, 2, 3 };

            var sum1 = numbers.IfSafeAnyMap(nums => nums.Aggregate(Add), -1);
            var sum2 = numbers.IfSafeAnyMap(n => n > 1, nums => nums.Aggregate(Add), -1);

            Assert.Equal(6, sum1);
            Assert.Equal(5, sum2);
        }

        [Fact]
        public void IfSafeAnyMap_ReturnsDefaultValueIfCollectionIsNull()
        {
            List<int> numbers = null;

            var sum1 = numbers.IfSafeAnyMap(nums => nums.Aggregate(Add), -1);
            var sum2 = numbers.IfSafeAnyMap(n => n > 1, nums => nums.Aggregate(Add), -1);

            Assert.Equal(-1, sum1);
            Assert.Equal(-1, sum2);
        }

        [Fact]
        public void IfSafeAnyDo_ExecutesDelegateIfCollectionIsNotNull()
        {
            var numbers = new List<int> { 1, 2, 3 };
            var sum1 = 0;
            var sum2 = 0;

            numbers.IfSafeAnyDo(nums => { sum1 = nums.Aggregate(Add); });
            numbers.IfSafeAnyDo(n => n > 1, nums => { sum2 = nums.Aggregate(Add); });

            Assert.Equal(6, sum1);
            Assert.Equal(5, sum2);
        }

        [Fact]
        public void IfSafeAnyDo_DoNotExecuteDelegateIfCollectionIsNull()
        {
            List<int> numbers = null;
            var sum1 = 0;
            var sum2 = 0;

            numbers.IfSafeAnyDo(nums => { sum1 = nums.Aggregate(Add); });
            numbers.IfSafeAnyDo(n => n > 1, nums => { sum2 = nums.Aggregate(Add); });

            Assert.Equal(0, sum1);
            Assert.Equal(0, sum2);
        }

        private int Add(int number1, int number2) => number1 + number2;
    }
}
