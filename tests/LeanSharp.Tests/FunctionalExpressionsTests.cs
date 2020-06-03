using System;
using System.Threading.Tasks;
using Xunit;

namespace LeanSharp.Tests
{
    public class FunctionalExpressionsTests
    {
        [Fact]
        public async Task UsingAsync_CorrectlyDisposesDisposableObjectAndReturnsValue()
        {
            var disposableStub = new DisposableStub(false);

            var result = await Dispose.UsingAsync(() => disposableStub, async stub =>
            {
                Assert.False(disposableStub.IsItDisposed);
                return await Task.FromResult(5);
            });

            Assert.Equal(5, result);
            Assert.True(disposableStub.IsItDisposed);
        }

        [Fact]
        public async Task UsingAsync_CorrectlyDisposesDisposableObject()
        {
            var disposableStub = new DisposableStub(false);

            await Dispose.UsingAsync(() => disposableStub, async stub =>
            {
                Assert.False(disposableStub.IsItDisposed);
                await Task.CompletedTask;
            });

            Assert.True(disposableStub.IsItDisposed);
        }

        [Fact]
        public void TryExpression_AppliesMappingFunctionIfNoExceptionsAreThrown()
        {
            var result = Try.Expression(() => 4 + 6);

            Assert.Equal(10, result.Success);
        }

        [Fact]
        public void TryExpression_ReturnsExceptionAsFailedResultIfExceptionIsThrown()
        {
            var result = Try.Expression(() =>
            {
                throw new Exception("Test exception");
                return 10;
            });

            Assert.True(result.IsFailure);
            Assert.Equal("Test exception", result.Failure.Message);
        }

        [Fact]
        public async Task TryExpressionAsync_AppliesMappingFunctionIfNoExceptionsAreThrown()
        {
            var result = await Try.ExpressionAsync(async() => await Task.FromResult(4 + 6));

            Assert.Equal(10, result.Success);
        }

        [Fact]
        public async Task TryExpressionAsync_ReturnsExceptionAsFailedResultIfExceptionIsThrown()
        {
            var result = await Try.ExpressionAsync(async () =>
            {
                throw new Exception("Test exception");
                return await Task.FromResult(10);
            });

            Assert.True(result.IsFailure);
            Assert.Equal("Test exception", result.Failure.Message);
        }
    }
}
