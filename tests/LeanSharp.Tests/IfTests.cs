using System;
using LeanSharp.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace LeanSharp.Tests
{
    public class IfTests
    {
        private ITestOutputHelper Output { get; }

        public IfTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void IfTrueThen_ExecutesActionIfConditionalIsTrue()
        {
            void Action() => Output.WriteLine("1");

            If.True(() => 3 == 3)
                .Then(Action)
                .Then(Action);

            Assert.Equal("11", Output.GetOutputAsString());
        }

        [Fact]
        public void IfTrueThen_DoesNotExecuteActionIfConditionalIsFalse()
        {
            void Action() => Output.WriteLine("1");

            If.True(() => 2 == 3).Then(Action);

            Assert.NotEqual("1", Output.GetOutputAsString());
        }

        [Fact]
        public void IfFalseThen_ExecutesActionIfConditionalIsFalse()
        {
            void Action() => Output.WriteLine("1");

            If.False(() => 2 == 3)
                .Then(Action)
                .Then(Action);

            Assert.Equal("11", Output.GetOutputAsString());
        }

        [Fact]
        public void IfFalseThen_DoesNotExecuteActionIfConditionalIsTrue()
        {
            void Action() => Output.WriteLine("1");

            If.False(() => 3 == 3).Then(Action);

            Assert.NotEqual("1", Output.GetOutputAsString());
        }
    }
}

