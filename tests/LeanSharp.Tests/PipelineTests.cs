using LeanSharp.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LeanSharp.Tests
{
    public class PipelineTests
    {
        [Fact]
        public async Task Flatten_ReturnsATask()
        {
            var pipeline = CreatePipeline.With(() => 5);
            var task = pipeline.Flatten();

            Assert.Equal(5, await task);
        }

        [Fact]
        public async Task Select_ComposesNonTaskReturningMethod()
        {
            int AddFour(int number) => number + 4;

            var pipeline = CreatePipeline.With(() => 5).Select(AddFour);
            var task = pipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task Select_ComposesTaskReturningMethod()
        {
            async Task<int> AddFour(int number) => await (number + 4).AsTask();

            var pipeline = CreatePipeline.With(() => 5).Select(AddFour);
            var task = pipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task SelectMany_ComposesPipeReturningMethod()
        {
            var initialPipeline = CreatePipeline.With(() => 5);

            var resultingPipeline = initialPipeline.SelectMany(five => CreatePipeline.With(() => five + 4));

            var task = resultingPipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task SelectMany_AllowsLinqSyntacticSugarForMonadicComposition()
        {
            var firstPipeline = CreatePipeline.With(() => 5);
            var secondPipeline = CreatePipeline.With(() => 6);
            var thirdPipeline = CreatePipeline.With(() => 9);

            var resultingPipeline = from firstValue in firstPipeline
                                    from secondValue in secondPipeline
                                    from thirdValue in thirdPipeline
                                    select firstValue + secondValue + thirdValue;

            var task = resultingPipeline.Flatten();

            Assert.Equal(20, await task);
        }

        [Fact]
        public async Task Return_LiftsConstantAndSideEffectFreeValueIntoAPipeline()
        {
            var pipeline = CreatePipeline.Return(5);
            var task = pipeline.Flatten();

            Assert.Equal(5, await task);
        }             
    }
}
