using System;

namespace LeanSharp.Extensions
{
    public static class MaybeExtensions
    {
        public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f, Func<TA, TB, TC> select)
        => ma.Bind(a => f(a).Map(b => select(a, b)));
    }
}
