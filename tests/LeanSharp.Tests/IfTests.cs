using LeanSharp.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace LeanSharp.Tests
{
    public class IfTests
    {
        private ITestOutputHelper Output { get; }

        private void ThenAction() => Output.WriteLine("1");
        private void ElseAction() => Output.WriteLine("2");
        private bool TruePredicate() => 3 == 3;
        private bool FalsePredicate() => 3 == 2;

        public IfTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void IfTrueThen_ExecutesActionIfConditionalIsTrue()
        {
            If.True(TruePredicate)
                .Then(ThenAction)
                .Then(ThenAction)
                .Else(ElseAction);

            Assert.Equal("11", Output.GetOutputAsString());
        }

        [Fact]
        public void IfTrueThen_DoesNotExecuteActionIfConditionalIsFalse()
        {
            If.True(FalsePredicate)
                .Then(ThenAction)
                .Then(ThenAction)
                .Else(ElseAction);

            Assert.Equal("2", Output.GetOutputAsString());
        }

        [Fact]
        public void IfFalseThen_ExecutesActionIfConditionalIsFalse()
        {
            If.False(FalsePredicate)
                .Then(ThenAction)
                .Then(ThenAction)
                .Else(ElseAction);

            Assert.Equal("11", Output.GetOutputAsString());
        }

        [Fact]
        public void IfFalseThen_DoesNotExecuteActionIfConditionalIsTrue()
        {
            If.False(TruePredicate)
                .Then(ThenAction)
                .Else(ElseAction);

            Assert.NotEqual("1", Output.GetOutputAsString());
        }
    }
}

