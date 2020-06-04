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
            var pipeline = CreatePipeLine.With(() => 5);
            var task = pipeline.Flatten();

            Assert.Equal(5, await task);
        }

        [Fact]
        public async Task Select_ComposesNonTaskReturningMethod()
        {
            int AddFour(int number) => number + 4;

            var pipeline = CreatePipeLine.With(() => 5).Select(AddFour);
            var task = pipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task Select_ComposesTaskReturningMethod()
        {
            async Task<int> AddFour(int number) => await (number + 4).AsTask();

            var pipeline = CreatePipeLine.With(() => 5).Select(AddFour);
            var task = pipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task SelectMany_ComposesPipeReturningMethod()
        {
            var initialPipeline = CreatePipeLine.With(() => 5);

            var resultingPipeline = initialPipeline.SelectMany(five => CreatePipeLine.With(() => five + 4));

            var task = resultingPipeline.Flatten();

            Assert.Equal(9, await task);
        }

        [Fact]
        public async Task SelectMany_AllowsLinqSyntacticSugarForMonadicComposition()
        {
            var firstPipeline = CreatePipeLine.With(() => 5);
            var secondPipeline = CreatePipeLine.With(() => 6);
            var thirdPipeline = CreatePipeLine.With(() => 9);

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
            var pipeline = CreatePipeLine.Return(5);
            var task = pipeline.Flatten();

            Assert.Equal(5, await task);
        }             
    }
}
