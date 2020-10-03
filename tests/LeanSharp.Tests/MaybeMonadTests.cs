using LeanSharp.Extensions;
using LeanSharp.Tests.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LeanSharp.Tests
{
    public class MaybeMonadTests
    {
        private ITestOutputHelper Output { get; }

        public MaybeMonadTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void ToMaybe_LiftsValueIntoSomeIfDifferentThanNull()
        {
            var testObject = new object();
            var maybeOfObject = testObject.ToMaybe();

            Assert.NotEqual(Maybe<object>.None, maybeOfObject);
        }

        [Fact]
        public void ToMaybe_LiftsValueIntoNoneIfNull()
        {
            object testObject = null;
            var maybeOfObject = testObject.ToMaybe();

            Assert.Equal(Maybe<object>.None, maybeOfObject);
        }

        [Fact]
        public void Match_ExecutesFirstFuncIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            string Delegate(string word) => word + 1;

            var newWord = maybeOfWord.Match(Delegate, () => "none");

            Assert.Equal("word1", newWord);
        }

        [Fact]
        public void Match_ExecutesSecondFuncIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            string Delegate() => "none";

            var newWord = maybeOfWord.Match(_ => "some", Delegate);

            Assert.Equal("none", newWord);
        }

        [Fact]
        public void Tee_ExecutesFirstActionIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            void Action(string word) => Output.WriteLine(word + 1);

            maybeOfWord.Tee(Action, () => { });

            Assert.Equal("word1", Output.GetOutputAsString());
        }

        [Fact]
        public void Tee_ExecutesSecondActionIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            void Action() => Output.WriteLine("Maybe was none");

            maybeOfWord.Tee(_ => { }, Action);

            Assert.Equal("Maybe was none", Output.GetOutputAsString());
        }

        [Fact]
        public void TeeAsync_ExecutesFirstActionIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            async Task Action(string word)
            {
                Output.WriteLine(word + 1);
                await Task.CompletedTask;
            }

            maybeOfWord.TeeAsync(Action, async () => await Task.CompletedTask);

            Assert.Equal("word1", Output.GetOutputAsString());
        }

        [Fact]
        public void TeeAsync_ExecutesSecondActionIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            async Task Action()
            {
                Output.WriteLine("Maybe was none");
                await Task.CompletedTask;
            }

            maybeOfWord.TeeAsync(async m => await Task.CompletedTask, Action);

            Assert.Equal("Maybe was none", Output.GetOutputAsString());
        }

        [Fact]
        public void Map_AppliesMappingFunctionIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            var maybeOfNewWord = maybeOfWord.Map(word => word + 1);

            Assert.Equal(Maybe<string>.Some("word1"), maybeOfNewWord);
        }

        [Fact]
        public void Map_ReturnsMaybeOfNoneIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            var maybeOfNewWord = maybeOfWord.Map(word => word + 1);

            Assert.Equal(Maybe<string>.None, maybeOfNewWord);
        }

        [Fact]
        public void Fold_ExecutesFirstFuncIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            var newWord = maybeOfWord.Fold((number, word) => word + number, "1");

            Assert.Equal("word1", newWord);
        }

        [Fact]
        public void Fold_ReturnsSeedValueIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            var newWord = maybeOfWord.Fold((number, word) => word + number, "1");

            Assert.Equal("1", newWord);
        }

        [Fact]
        public void GetOrElse_ExecutesFirstFuncIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            string Delegate(string word) => word + 1;

            var newWord = maybeOfWord.GetOrElse(Delegate, "none");

            Assert.Equal("word1", newWord);
        }

        [Fact]
        public void GetOrElse_ReturnsSeedValueIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            string Delegate(string word) => word + 1;

            var newWord = maybeOfWord.GetOrElse(Delegate, "1");

            Assert.Equal("1", newWord);
        }

        [Fact]
        public void Bind_AppliesMappingMaybeReturningFunctionIfMaybeIsSome()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            var maybeOfNewWord = maybeOfWord.Bind(word => Maybe<string>.Some(word + 1));

            Assert.Equal(Maybe<string>.Some("word1"), maybeOfNewWord);
        }

        [Fact]
        public void Bind_DoesNotApplyMappingFunctionIfMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some(null);

            var maybeOfNewWord = maybeOfWord.Bind(word => Maybe<string>.Some(word + 1));

            Assert.Equal(Maybe<string>.None, maybeOfNewWord);
        }

        [Fact]
        public void Bind_DoesNotApplyMappingFunctionIfMonadicMaybeIsNone()
        {
            var maybeOfWord = Maybe<string>.Some("word");

            var maybeOfNewWord = maybeOfWord.Bind(word => Maybe<string>.Some(null));

            Assert.Equal(Maybe<string>.None, maybeOfNewWord);
        }

        [Fact]
        public void SelectMany_AllowsLinqSyntacticSugarForMonadicComposition()
        {
            var maybe1 = Maybe<int>.Some(1);
            var maybe2 = Maybe<int>.Some(2);
            var maybe3 = Maybe<int>.Some(3);

            var result = from m1 in maybe1
                         from m2 in maybe2
                         from m3 in maybe3
                         select m1 + m2 + m3;

            Assert.Equal(Maybe<int>.Some(6), result);
        }

        [Fact]
        public void SelectMany_MonadicCompositionReturnsNoneIfThereIsAny()
        {
            var maybe1 = Maybe<int>.Some(1);
            var maybe2 = Maybe<string>.None;
            var maybe3 = Maybe<int>.Some(3);

            var result = from m1 in maybe1
                         from m2 in maybe2
                         from m3 in maybe3
                         select m1 + m2 + m3;

            Assert.Equal(Maybe<string>.None, result);
        }

        [Fact]
        public void MaybesAreEqualIfMonadicValuesAreTheSameAndEqualityComparerIsUsed()
        {
            var maybe1 = Maybe<int>.Some(1);
            var maybe2 = Maybe<int>.Some(1);

            Assert.True(maybe1.Equals(maybe2, new EqualityComparerStub()));
        }

        [Fact]
        public void MaybesAreEqualIfBothAreNoneAndEqualityComparerIsUsed()
        {
            var maybe1 = Maybe<string>.Some(null);
            var maybe2 = Maybe<string>.Some(null);

            Assert.True(maybe1.Equals(maybe2, new EqualityComparerStub()));
        }

        [Fact]
        public void NoneMaybesAreNotEqualIfTheyContainDifferentTypes()
        {
            var maybe1 = Maybe<int>.Some(1);
            var maybe2 = Maybe<string>.Some(null);

            Assert.NotEqual<object>(maybe1, maybe2);
        }

        [Fact]
        public void MaybesAreNotEqualIfMonadicValuesAreDifferent()
        {
            var maybe1 = Maybe<int>.Some(1);
            var maybe2 = Maybe<int>.Some(2);

            Assert.True(maybe1 != maybe2);
        }

        [Fact]
        public void ExplicitConversionToMaybeReturnsSomeOfValue()
        {
            var maybe = (Maybe<int>)1;

            Assert.IsAssignableFrom<Maybe<int>>(maybe);
        }

        [Fact]
        public void ToStringReturnsCorrectValueIfMaybeIsSome()
        {
            var maybe = Maybe<int>.Some(1);

            Assert.Equal("Some (1)", maybe.ToString());
        }

        [Fact]
        public void ToStringReturnsCorrectValueIfMaybeIsNone()
        {
            var maybe = Maybe<string>.Some(null);

            Assert.Equal("None", maybe.ToString());
        }
    }
}
