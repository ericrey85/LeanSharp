using LeanSharp.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LeanSharp.Tests
{
    public class SafePipelineTests
    {
        [Fact]
        public async Task SafePipeComposesWithFunctionsAndOtherSafePipesCorrectly()
        {
            async Task<Result<int, string>> GetFirstValue(int number) => await Result<int, string>.Succeeded(number + 4).AsTask();
            async Task<Result<int, string>> GetSecondValue(int number) => await Result<int, string>.Succeeded(number + 5).AsTask();

            async Task<int> GetThirdValue(int number) => await (number + 4).AsTask();

            int GetFourthValue(int number) => number + 5;

            SafePipeline<int, Exception> GetFithValue(int number) => CreateSafePipeline.TryWith(() => number + 6);

            var firstPipe = CreateSafePipeline.With(() => GetFirstValue(5));
            var secondPipe = firstPipe.Select(GetSecondValue);
            var thirdPipe = secondPipe.Select(GetThirdValue);
            var fourthPipe = secondPipe.Select(GetFourthValue);
            var fithPipe = fourthPipe.SelectMany(value => GetFithValue(value).ToStringFailure());

            var finalPipe = from firstValue in secondPipe
                            from secondValue in thirdPipe
                            from thirdValue in fithPipe
                            select firstValue + secondValue + thirdValue;

            var result = await finalPipe.Flatten();

            Assert.True(result.IsSuccess);
            Assert.Equal(57, result.Success);
        }

        [Fact]
        public async Task Select_SafePipeResturnsFailedResultIfThereIsAnException()
        {
            int InvalidDivideByZeroOperation(int number) => number / 0;
            async Task<int> GetValue(int number) => await (number + 4).AsTask();

            var firstPipe = CreateSafePipeline.TryWith(() => InvalidDivideByZeroOperation(5));
            var finalPipe = firstPipe.Select(GetValue);

            var result = await finalPipe.Flatten();

            Assert.True(result.IsFailure);
            Assert.True(result.Failure is Exception);
        }

        [Fact]
        public async Task SelectMany_SafePipeResturnsFailedResultIfThereIsAnException()
        {
            int InvalidDivideByZeroOperation(int number) => number / 0;
            async Task<int> GetValue(int number) => await (number + 4).AsTask();

            var firstPipe = CreateSafePipeline.TryWith(() => GetValue(5));
            var secondPipe = firstPipe.SelectMany(value => CreateSafePipeline.TryWith(() => InvalidDivideByZeroOperation(value)));

            var finalPipe = from firstValue in firstPipe
                            from secondValue in secondPipe
                            select firstValue + secondValue;

            var result = await secondPipe.Flatten();

            Assert.True(result.IsFailure);
            Assert.True(result.Failure is Exception);
        }
    }
}
