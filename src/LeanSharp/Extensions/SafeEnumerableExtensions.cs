using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanSharp.Extensions
{
    public static class SafeEnumerableExtensions
    {
        public static bool SafeAny<T>(this IEnumerable<T> @this)
        => @this?.Any() == true;

        public static bool SafeAny<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        => @this.SafeAny() && @this.Any(predicate);

        public static bool SafeCount<T>(this IEnumerable<T> @this)
        => @this?.Count() > 0;

        public static bool SafeCount<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        => @this?.Count(predicate) > 0;
    }
}
