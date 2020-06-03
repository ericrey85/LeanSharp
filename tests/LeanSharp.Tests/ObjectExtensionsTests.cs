using LeanSharp.Extensions;
using LeanSharp.Tests.Extensions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LeanSharp.Tests
{
    public class ObjectExtensionsTests
    {
        private ITestOutputHelper Output { get; }

        public ObjectExtensionsTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void MapTo_MapsToNewObject()
        {
            var greeting = "hello";

            var message = greeting.MapTo(g => $"{g} world");

            Assert.Equal("hello world", message);
        }

        [Fact]
        public void MapToOrDefaultTo_MapsToNewObjectIfAppliedOnANonNullObject()
        {
            var greeting = "hello";

            var message = greeting.MapToOrDefaultTo(g => $"{g} world", "no greetings");

            Assert.Equal("hello world", message);
        }

        [Fact]
        public void MapToOrDefaultTo_ReturnsPassedInDefaultValueIfAppliedOnANullObject()
        {
            string greeting = null;

            var message = greeting.MapToOrDefaultTo(g => $"{g} world", "no greetings");

            Assert.Equal("no greetings", message);
        }

        [Fact]
        public void IfElseMapTo_UsesFirstMappingDelegateIfPredicateIsTrue()
        {
            var greeting = "hello";

            var message = greeting.IfElseMapTo(g => g.Length > 3, g => $"{g} world", _ => "I am a short greeting word");

            Assert.Equal("hello world", message);
        }

        [Fact]
        public void IfElseMapTo_UsesSecondMappingDelegateIfPredicateIsFalse()
        {
            var greeting = "hello";

            var message = greeting.IfElseMapTo(g => g.Length > 10, g => $"{g} world", _ => "I am a not very long greeting word");

            Assert.Equal("I am a not very long greeting word", message);
        }

        [Fact]
        public void DoIfNotNull_ExecutesActionIfAppliedOnANonNullObject()
        {
            var greeting = "hello";

            greeting.DoIfNotNull(Output.WriteLine);

            Assert.Equal("hello", Output.GetOutputAsString());
        }

        [Fact]
        public void DoIfNotNull_DoNotExecuteActionIfAppliedOnANullObject()
        {
            string greeting = null;

            greeting.DoIfNotNull(Output.WriteLine);

            Assert.Equal("", Output.GetOutputAsString());
        }

        [Fact]
        public async Task DoIfNotNullAsync_ExecutesActionIfAppliedOnANonNullObject()
        {
            var greeting = "hello";

            await greeting.DoIfNotNullAsync(async g =>
            {
                Output.WriteLine(g);
                await Task.CompletedTask;
            });

            Assert.Equal("hello", Output.GetOutputAsString());
        }

        [Fact]
        public async Task DoIfNotNullAsync_DoNotExecuteActionIfAppliedOnANullObject()
        {
            string greeting = null;

            await greeting.DoIfNotNullAsync(async g =>
            {
                Output.WriteLine(g);
                await Task.CompletedTask;
            });

            Assert.Equal("", Output.GetOutputAsString());
        }

        [Fact]
        public void IfElseDo_ExecutesFirstActionIfPredicateIsTrue()
        {
            var greeting = "hello";

            greeting.IfElseDo(
                g => g.Length > 3,
                g => Output.WriteLine($"{g} world"), 
                _ => Output.WriteLine("I am a short greeting word")
            );

            Assert.Equal("hello world", Output.GetOutputAsString());
        }

        [Fact]
        public void IfElseDo_ExecutesSecondActionIfPredicateIsTrue()
        {
            var greeting = "hello";

            greeting.IfElseDo(
                g => g.Length > 10,
                g => Output.WriteLine($"{g} world"),
                _ => Output.WriteLine("I am a not very long greeting word")
            );

            Assert.Equal("I am a not very long greeting word", Output.GetOutputAsString());
        }

        [Fact]
        public async Task IfElseDoAsync_ExecutesFirstActionIfPredicateIsTrue()
        {
            var greeting = "hello";

            await greeting.IfElseDoAsync(
                g => g.Length > 3,
                async g =>
                {
                    Output.WriteLine($"{g} world");
                    await Task.CompletedTask;
                },
                async _ =>
                {
                    Output.WriteLine("I am a short greeting word");
                    await Task.CompletedTask;
                });

            Assert.Equal("hello world", Output.GetOutputAsString());
        }

        [Fact]
        public async Task IfElseDoAsync_ExecutesSecondActionIfPredicateIsTrue()
        {
            var greeting = "hello";

            await greeting.IfElseDoAsync(
                g => g.Length > 10,
                async g =>
                {
                    Output.WriteLine($"{g} world");
                    await Task.CompletedTask;
                },
                async _ =>
                {
                    Output.WriteLine("I am a not very long greeting word");
                    await Task.CompletedTask;
                });

            Assert.Equal("I am a not very long greeting word", Output.GetOutputAsString());
        }
    }
}
