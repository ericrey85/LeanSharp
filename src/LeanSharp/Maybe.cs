using System;
using System.Collections;
using System.Threading.Tasks;

namespace LeanSharp
{
    public abstract class Maybe<TValue> : IEquatable<Maybe<TValue>>, IStructuralEquatable
    {
        public static explicit operator Maybe<TValue>(TValue value) => Some(value);

        public static Maybe<TValue> Some(TValue value) => value == null ? (Maybe<TValue>)new Choices.None() : new Choices.Some(value);
        public static Maybe<TValue> None { get; } = new Choices.None();

        public abstract TDestination Match<TDestination>(Func<TValue, TDestination> someFunc, Func<TDestination> noneFunc);
        public abstract void Tee(Action<TValue> someAction, Action noneAction);
        public abstract Task TeeAsync(Func<TValue, Task> someAction, Func<Task> noneAction);

        public Maybe<TDestination> Map<TDestination>(Func<TValue, TDestination> map) => Match(v => Maybe<TDestination>.Some(map(v)), () => Maybe<TDestination>.None);

        public TDestination Fold<TDestination>(Func<TDestination, TValue, TDestination> foldFunc, TDestination seed) => Match(v => foldFunc(seed, v), () => seed);
        public TDestination GetOrElse<TDestination>(Func<TValue, TDestination> foldFunc, TDestination seed) => Fold((_, v) => foldFunc(v), seed);

        public Maybe<TDestination> Bind<TDestination>(Func<TValue, Maybe<TDestination>> map) => Match(v => map(v).Match(d => Maybe<TDestination>.Some(d), () => Maybe<TDestination>.None), () => Maybe<TDestination>.None);

        public static bool operator ==(Maybe<TValue> x, Maybe<TValue> y) => x.Equals(y);
        public static bool operator !=(Maybe<TValue> x, Maybe<TValue> y) => !(x == y);

        bool IEquatable<Maybe<TValue>>.Equals(Maybe<TValue> other) => Equals(other);
        public abstract bool Equals(object other, IEqualityComparer comparer);
        public abstract int GetHashCode(IEqualityComparer comparer);

        private Maybe() { }

        private static class Choices
        {
            public class Some : Maybe<TValue>
            {
                private TValue Value { get; }
                public Some(TValue value) => Value = value;

                public override TDestination Match<TDestination>(Func<TValue, TDestination> someFunc, Func<TDestination> noneFunc) => someFunc(Value);
                public override void Tee(Action<TValue> someAction, Action noneAction) => someAction(Value);
                public override async Task TeeAsync(Func<TValue, Task> someAction, Func<Task> noneAction) => await someAction(Value);
                public override string ToString() => $"Some ({Value})";

                public override bool Equals(object obj)
                {
                    if (obj is Some s)
                    {
                        return Value.Equals(s.Value);
                    }

                    return false;
                }

                public override bool Equals(object other, IEqualityComparer comparer)
                {
                    if (other is Some s)
                    {
                        return comparer.Equals(Value, s.Value);
                    }

                    return false;
                }

                public override int GetHashCode() => "Some ".GetHashCode() ^ Value.GetHashCode();
                public override int GetHashCode(IEqualityComparer comparer) => "Some ".GetHashCode() ^ comparer.GetHashCode(Value);
            }

            public class None : Maybe<TValue>
            {
                public override TDestination Match<TDestination>(Func<TValue, TDestination> someFunc, Func<TDestination> noneFunc) => noneFunc();
                public override void Tee(Action<TValue> someAction, Action noneAction) => noneAction();
                public override async Task TeeAsync(Func<TValue, Task> someAction, Func<Task> noneAction) => await noneAction();
                public override string ToString() => "None";

                public override bool Equals(object obj) => obj is None;
                public override int GetHashCode() => "None".GetHashCode();
                public override bool Equals(object other, IEqualityComparer comparer) => Equals(other);
                public override int GetHashCode(IEqualityComparer comparer) => GetHashCode();
            }
        }
    }
}
