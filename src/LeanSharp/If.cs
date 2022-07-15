using System;

namespace LeanSharp
{
    public abstract class If
    {
        public static If True(Func<bool> predicate)
        {
            if (predicate())
            {
                return new TruthyIf();
            }
            return new FalsyIf();
        }

        public static If False(Func<bool> predicate)
        {
            if (!predicate())
            {
                return new TruthyIf();
            }
            return new FalsyIf();
        }

        public abstract If Then(Action action);
        public abstract void Else(Action action);

        private class TruthyIf : If
        {
            public override If Then(Action action)
            {
                action();
                return this;
            }

            public override void Else(Action action)
            {
            }
        }

        private class FalsyIf : If
        {
            public override If Then(Action action)
            {
                return this;
            }

            public override void Else(Action action)
            {
                action();
            }
        }
    }
}

