using System;

namespace LeanSharp
{
    public abstract class If
    {
        public static If True(Func<bool> predicate)
        {
            if (predicate())
            {
                return new ExpressionExecuter();
            }
            return new ExpressionDisregarder();
        }

        public static If False(Func<bool> predicate)
        {
            if (!predicate())
            {
                return new ExpressionExecuter();
            }
            return new ExpressionDisregarder();
        }

        public abstract If Then(Action action);

        private class ExpressionExecuter : If
        {
            public override If Then(Action action)
            {
                action();
                return this;
            }
        }

        private class ExpressionDisregarder : If
        {
            public override If Then(Action action)
            {
                return this;
            }
        }
    }
}

