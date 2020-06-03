using LeanSharp.Extensions;
using LeanSharp.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LeanSharp.Tests
{
    public class RopExtensionsTests
    {
        private ITestOutputHelper Output { get; }
        private bool IsEven(int number) => number % 2 == 0;
        private bool IsOdd(int number) => !IsEven(number);

        public RopExtensionsTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Handle_ExecutesSuccessDelegateIfResultIsSuccess()
        {
            var result = Result<string, string>.Succeeded("success");

            void SuccessDelegate(string r) => Output.WriteLine(r);

            result.Handle(SuccessDelegate, null);

            Assert.Equal("success", Output.GetOutputAsString());
        }

        [Fact]
        public void Handle_ExecutesFailureDelegateIfResultIsFailure()
        {
            var result = Result<string, string>.Failed("failure");

            void FailureDelegate(string e) => Output.WriteLine(e);

            result.Handle(null, FailureDelegate);

            Assert.Equal("failure", Output.GetOutputAsString());
        }

        [Fact]
        public async Task HandleAsync_ExecutesSuccessDelegateIfResultIsSuccess()
        {
            var result = Result<string, string>.Succeeded("success");

            async Task SuccessDelegate(string r)
            {
                Output.WriteLine(r);
                await Task.CompletedTask;
            }

            await result.HandleAsync(SuccessDelegate, null);

            Assert.Equal("success", result.Success);
        }

        [Fact]
        public async Task HandleAsync_ExecutesFailureDelegateIfResultIsFailure()
        {
            var result = Result<string, string>.Failed("failure");

            async Task FailureDelegate(string e)
            {
                Output.WriteLine(e);
                await Task.CompletedTask;
            }

            await result.HandleAsync(null, FailureDelegate);

            Assert.Equal("failure", result.Failure);
        }

        [Fact]
        public void Either_ReturnsLeftExpressionIfResultIsSuccess()
        {
            var result = Result<string, string>.Succeeded("result");
            var success = Result<string, string>.Succeeded("success");
            var failure = Result<string, string>.Failed("failure");

            Assert.Equal(success, result.Either(r => success, e => failure));
        }

        [Fact]
        public void Either_ReturnsRightExpressionIfResultIsSuccess()
        {
            var result = Result<string, string>.Failed("result");
            var success = Result<string, string>.Succeeded("success");
            var failure = Result<string, string>.Failed("failure");

            Assert.Equal(failure, result.Either(r => success, e => failure));
        }

        [Fact]
        public void EitherFold_ReturnsLeftExpressionIfResultIsSuccess()
        {
            var result = Result<string, string>.Succeeded("result");
            var success = Result<string, string>.Succeeded("success");
            var failure = Result<string, string>.Failed("failure");

            Assert.Equal(success.Success, result.EitherFold(r => success.Success, e => failure.Failure));
        }

        [Fact]
        public void EitherFold_ReturnsRightExpressionIfResultIsSuccess()
        {
            var result = Result<string, string>.Failed("result");
            var success = Result<string, string>.Succeeded("success");
            var failure = Result<string, string>.Failed("failure");

            Assert.Equal(failure.Failure, result.EitherFold(r => success.Success, e => failure.Failure));
        }

        [Fact]
        public void ToFailure_ConvertSuccessToEmptyFailureArray()
        {
            var success = Result<string, string[]>.Succeeded("success");
            
            Assert.IsAssignableFrom<string[]>(success.ToFailure().Failure);
        }

        [Fact]
        public void ToFailure_ReturnsFailureIfResultIsAlreadyFailure()
        {
            var failure = Result<string, string[]>.Failed(new[] {"failure"});

            Assert.Equal("failure", failure.ToFailure().Failure.First());
        }

        [Fact]
        public void Merge_CreatesSuccessfulResultIfAccumulatorAndNextResultsAreSuccess()
        {
            var aggregatedSuccess = Result<string[], string[]>.Succeeded(new []{ "success1" });
            var nextResult = Result<string, string[]>.Succeeded("success2");

            var mergedResult = aggregatedSuccess.Merge(nextResult);

            Assert.Contains("success1", mergedResult.Success);
            Assert.Contains("success2", mergedResult.Success);
        }

        [Fact]
        public void Merge_CreatesFailedResultIfAccumulatorOrNextResultsAreFailure()
        {
            var aggregatedSuccess = Result<string[], string[]>.Succeeded(new[] { "success" });
            var nextResult = Result<string, string[]>.Failed(new []{ "failure2" });

            var mergedResult = aggregatedSuccess.Merge(nextResult);

            Assert.Single(mergedResult.Failure);
            Assert.Contains("failure2", mergedResult.Failure);
        }

        [Fact]
        public void Aggregate_CreatesSuccessfulResultIfAllResultsAreSuccess()
        {
            var aggregatedSuccess = new List<Result<string, string[]>>
            {
                Result<string, string[]>.Succeeded("success1"),
                Result<string, string[]>.Succeeded("success2")
            };

            var aggregatedResult = aggregatedSuccess.Aggregate();

            Assert.Contains("success1", aggregatedResult.Success);
            Assert.Contains("success2", aggregatedResult.Success);
        }

        [Fact]
        public void Aggregate_CreatesFailureResultIfAnyResultsIsFailure()
        {
            var aggregatedSuccess = new List<Result<string, string[]>>
            {
                Result<string, string[]>.Succeeded("success1"),
                Result<string, string[]>.Failed(new []{ "failure" })
            };

            var aggregatedResult = aggregatedSuccess.Aggregate();

            Assert.Single(aggregatedResult.Failure);
            Assert.Contains("failure", aggregatedResult.Failure);
        }

        [Fact]
        public void Map_AppliesMappingDelegateIfItIsSuccessfulResult()
        {
            var success = Result<int, string[]>.Succeeded(2);
            var newResult = success.Map(two => two + 2);

            Assert.Equal(4, newResult.Success);
        }

        [Fact]
        public void Map_DoesNotApplyMappingDelegateIfItIsFailureResult()
        {
            var failure = Result<int, string>.Failed("failed");
            var newResult = failure.Map(r => r + "not added");

            Assert.Null(newResult.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public void Bind_AppliesMappingResultReturningDelegateIfItIsSuccessfulResult()
        {
            var success = Result<int, string[]>.Succeeded(2);
            var newResult = success.Bind(r => Result<int, string[]>.Succeeded(r + 3));

            Assert.Equal(5, newResult.Success);
        }

        [Fact]
        public void Bind_DoesNotApplyMappingResultReturningDelegateIfItIsFailureResult()
        {
            var failure = Result<int, string>.Failed("failed");
            var newResult = failure.Bind(r => Result<string, string>.Succeeded(r + "not added"));

            Assert.Null(newResult.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public async Task MapAsync_AppliesMappingDelegateIfItIsSuccessfulResult()
        {
            var success = Result<int, string[]>.Succeeded(2);
            var newResult = await success.MapAsync(async r => await Task.FromResult(r + 3));

            Assert.Equal(5, newResult.Success);
        }

        [Fact]
        public async Task MapAsync_DoesNotApplyMappingDelegateIfItIsFailureResult()
        {
            var failure = Result<int, string>.Failed("failed");
            var newResult = await failure.MapAsync(async r => await Task.FromResult("not added"));

            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public async Task BindAsync_AppliesMappingResultReturningDelegateIfItIsSuccessfulResult()
        {
            var success = Result<int, string[]>.Succeeded(2);
            var newResult = await success.BindAsync(async r => 
                await Task.FromResult(Result<int, string[]>.Succeeded(r + 3)));

            Assert.Equal(5, newResult.Success);
        }

        [Fact]
        public async Task BindAsync_DoesNotApplyMappingResultReturningDelegateIfItIsFailureResult()
        {
            var failure = Result<int, string>.Failed("failed");
            var newResult = await failure.BindAsync(async r => 
                await Task.FromResult(Result<string, string>.Succeeded(r + "not added")));

            Assert.Null(newResult.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public void IfElseMap_PassesAlongFailureIfAppliedOnAFailureResult()
        {
            var failure = Result<string, string>.Failed("failed");
            var newResult = failure.IfElseMap(_ => true, r => r + 1, r => r + 2);

            Assert.Null(failure.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public void IfElseMap_ExecutesFirstDelegateIfPredicateReturnsTrue()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = success.IfElseMap(IsEven, r => r * 10, r => r);

            Assert.Equal(20, newResult.Success);
        }

        [Fact]
        public void IfElseMap_ExecutesSecondDelegateIfPredicateReturnsFalse()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = success.IfElseMap(IsOdd, r => r * 10, r => r);

            Assert.Equal(2, newResult.Success);
        }

        [Fact]
        public async Task IfElseMapAsync_PassesAlongFailureIfAppliedOnAFailureResult()
        {
            var failure = Result<string, string>.Failed("failed");
            var newResult = await failure.IfElseMapAsync(_ => true, async r => await Task.FromResult(r + 1),
                                                                    async r => await Task.FromResult(r + 2));

            Assert.Null(failure.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public async Task IfElseMapAsync_ExecutesFirstDelegateIfPredicateReturnsTrue()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = await success.IfElseMapAsync(IsEven, async r => await Task.FromResult(r * 10),
                                                                 async r => await Task.FromResult(r));

            Assert.Equal(20, newResult.Success);
        }

        [Fact]
        public async Task IfElseMapAsync_ExecutesSecondDelegateIfPredicateReturnsFalse()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = await success.IfElseMapAsync(IsOdd,  async r => await Task.FromResult(r * 10),
                                                                 async r => await Task.FromResult(r));

            Assert.Equal(2, newResult.Success);
        }

        [Fact]
        public void IfElseBind_PassesAlongFailureIfAppliedOnAFailureResult()
        {
            var failure = Result<string, string>.Failed("failed");
            var newResult = failure.IfElseBind(_ => true, r => Result<string, string>.Succeeded(r + 1),
                                                          r => Result<string, string>.Succeeded(r + 2));

            Assert.Null(failure.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public void IfElseBind_ExecutesResultReturningFirstDelegateIfPredicateReturnsTrue()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = success.IfElseBind(IsEven, r => Result<int, string[]>.Succeeded(r * 10),
                                                       Result<int, string[]>.Succeeded);
            Assert.Equal(20, newResult.Success);
        }

        [Fact]
        public void IfElseBind_ExecutesResultReturningSecondDelegateIfPredicateReturnsTrue()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = success.IfElseBind(IsOdd, r => Result<int, string[]>.Succeeded(r * 10),
                                                           Result<int, string[]>.Succeeded);
            Assert.Equal(2, newResult.Success);
        }

        [Fact]
        public async Task IfElseBindAsync_PassesAlongFailureIfAppliedOnAFailureResult()
        {
            var failure = Result<string, string>.Failed("failed");

            var newResult = await failure.IfElseBindAsync(
                _ => true, 
                async r => await Task.FromResult(Result<string, string>.Succeeded(r + 1)),
                async r => await Task.FromResult(Result<string, string>.Succeeded(r + 2))
            );

            Assert.Null(failure.Success);
            Assert.Equal("failed", newResult.Failure);
        }

        [Fact]
        public async Task IfElseBindAsync_ExecutesFirstDelegateIfPredicateReturnsTrue()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = await success.IfElseBindAsync(
                IsEven, 
                async r => await Task.FromResult(Result<int, string[]>.Succeeded(r * 10)),
                async r => await Task.FromResult(Result<int, string[]>.Succeeded(r))
            );

            Assert.Equal(20, newResult.Success);
        }

        [Fact]
        public async Task IfElseBindAsync_ExecutesSecondDelegateIfPredicateReturnsFalse()
        {
            var success = Result<int, string[]>.Succeeded(2);

            var newResult = await success.IfElseBindAsync(
                IsOdd,
                async r => await Task.FromResult(Result<int, string[]>.Succeeded(r * 10)),
                async r => await Task.FromResult(Result<int, string[]>.Succeeded(r))
            );

            Assert.Equal(2, newResult.Success);
        }

        [Fact]
        public void Tee_ExecutesActionIfResultIsSuccess()
        {
            var success = Result<string, string[]>.Succeeded("success");

            success.Tee(Output.WriteLine);

            Assert.Equal("success", Output.GetOutputAsString());
        }

        [Fact]
        public void Tee_DoesNotExecuteActionIfResultIsFailure()
        {
            var failure = Result<string, string>.Failed("failure");

            failure.Tee(Output.WriteLine);

            Assert.NotEqual("failure", Output.GetOutputAsString());
            Assert.Equal("", Output.GetOutputAsString());
        }

        [Fact]
        public void TeeIfFailure_ExecutesActionIfResultIsFailure()
        {
            var failure = Result<string, string>.Failed("failure");

            failure.TeeIfFailure(Output.WriteLine);

            Assert.Equal("failure", Output.GetOutputAsString());
        }

        [Fact]
        public void TeeIfFailure_DoesNotExecuteActionIfResultIsSuccess()
        {
            var success = Result<string, string>.Succeeded("success");

            success.TeeIfFailure(Output.WriteLine);

            Assert.NotEqual("success", Output.GetOutputAsString());
        }

        [Fact]
        public void SelectMany_AllowsLinqSyntacticSugarForMonadicComposition()
        {
            var success1 = Result<int, string>.Succeeded(1);
            var success2 = Result<int, string>.Succeeded(2);
            var success3 = Result<int, string>.Succeeded(3);

            var result = from s1 in success1
                         from s2 in success2
                         from s3 in success3
                         select s1 + s2 + s3;

            Assert.Equal(6, result.Success);
        }

        [Fact]
        public void SelectMany_MonadicCompositionReturnsFirstFailureIfThereIsAny()
        {
            var success1 = Result<int, string[]>.Succeeded(1);
            var failure1 = Result<int, string[]>.Failed(new []{ "failed1" });
            var success3 = Result<int, string[]>.Succeeded(3);
            var failure2 = Result<int, string[]>.Failed(new[] { "failed2" });

            var result = from s1 in success1
                         from s2 in failure1
                         from s3 in success3
                         from s4 in failure2
                         select s1 + s2 + s3 + s4;

            Assert.Single(result.Failure);
            Assert.Equal("failed1", result.Failure.First());
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void TryingToCreateSuccessWithNullValueThrowsException()
            => Assert.Throws<ArgumentNullException>(() => Result<string, string>.Succeeded(null));

        [Fact]
        public void TryingToCreateFailureWithNullValueThrowsException()
            => Assert.Throws<ArgumentNullException>(() => Result<string, string>.Failed(null));

        [Fact]
        public void IfNullOrEmpty_PassesResultAlongIfStringIsNotNullOrEmpty()
        {
            var word = "word";

            var result = word.IfNullOrEmpty("The word was empty");

            Assert.Equal("word", result.Success);
        }

        [Fact]
        public void IfNullOrEmpty_ReturnsFailedResultWithCustomErrorMessageIfStringIsNullOrEmpty()
        {
            var word = "";

            var result = word.IfNullOrEmpty("The word was empty");

            Assert.Null(result.Success);
            Assert.Equal("The word was empty", result.Failure.First());
        }

        [Fact]
        public void IfNull_PassesResultAlongIfValueTypeIsNotNull()
        {
            DateTime? date = new DateTime(2020, 1, 31);

            var result = date.IfNull("The date was null.");

            Assert.Equal(date, result.Success);
        }

        [Fact]
        public void IfNull_ReturnsFailedResultWithCustomErrorMessageIfValueTypeIsNull()
        {
            DateTime? date = null;

            var result = date.IfNull("The date was null.");

            Assert.Equal("The date was null.", result.Failure.First());
        }

        [Fact]
        public void IfNull_PassesResultAlongIfReferenceTypeIsNotNull()
        {
            object patient = new { name = "John Doe" };

            var result = patient.IfNull("The patient was null.");

            Assert.Equal(patient, result.Success);
        }

        [Fact]
        public void IfNull_ReturnsFailedResultWithCustomErrorMessageIfReferenceTypeIsNull()
        {
            object patient = null;

            var result = patient.IfNull("The patient was null.");

            Assert.Equal("The patient was null.", result.Failure.First());
        }
    }
}
