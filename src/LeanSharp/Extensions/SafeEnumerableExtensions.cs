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
        => @this?.Any(predicate) == true;

        public static int SafeCount<T>(this IEnumerable<T> @this)
        => @this?.Count() ?? 0;

        public static int SafeCount<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        => @this?.Count(predicate) ?? 0;

        public static TDestination IfSafeAnyMap<TSource, TDestination>(
            this IEnumerable<TSource> @this,
            Func<IEnumerable<TSource>, TDestination> fn,
            TDestination defaultValue)
        => @this?.Any() == true ? fn(@this) : defaultValue;

        public static TDestination IfSafeAnyMap<TSource, TDestination>(
            this IEnumerable<TSource> @this,
            Func<TSource, bool> predicate,
            Func<IEnumerable<TSource>, TDestination> mapFiltered,
            TDestination defaultValue)
        {
            var filteredElements = @this?.Where(predicate) ?? new List<TSource>();

            return filteredElements.Any() ? mapFiltered(filteredElements) : defaultValue;
        }

        public static void IfSafeAnyDo<TSource>(
            this IEnumerable<TSource> @this,
            Action<IEnumerable<TSource>> fn)
        {
            if (@this?.Any() == true)
            {
                fn(@this);
            }
        }

        public static void IfSafeAnyDo<TSource>(
            this IEnumerable<TSource> @this,
            Func<TSource, bool> predicate,
            Action<IEnumerable<TSource>> fn)
        {
            var filteredElements = @this?.Where(predicate) ?? new List<TSource>();

            if (filteredElements.Any())
            {
                fn(filteredElements);
            }
        }

        public static Maybe<T> SafeFirst<T>(this IEnumerable<T> @this)
        {
            var first = @this.FirstOrDefault();
            return Maybe<T>.Some(first);
        }

        public static Maybe<T> SafeFirst<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            var first = @this.FirstOrDefault(predicate);
            return Maybe<T>.Some(first);
        }

        public static Maybe<T> SafeSingle<T>(this IEnumerable<T> @this)
        {
            var first = @this.SingleOrDefault();
            return Maybe<T>.Some(first);
        }

        public static Maybe<T> SafeSingle<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            var first = @this.SingleOrDefault(predicate);
            return Maybe<T>.Some(first);
        }
    }
}
