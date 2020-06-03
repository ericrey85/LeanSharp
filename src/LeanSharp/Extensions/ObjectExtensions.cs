using System;
using System.Threading.Tasks;

namespace LeanSharp.Extensions
{
    public static class ObjectExtensions
    {
        public static Task<T> AsTask<T>(this T that) => Task.FromResult(that);

        public static TDestination MapTo<TSource, TDestination>(this TSource @this, Func<TSource, TDestination> map)
        => map(@this);

        public static TDestination MapToOrDefaultTo<TSource, TDestination>(
            this TSource @this, Func<TSource, TDestination> map, TDestination defaultValue)
        => @this != null ? map(@this) : defaultValue;

        public static TDestination IfElseMapTo<TSource, TDestination>(
            this TSource @this, Func<TSource, bool> predicate, Func<TSource, TDestination> mapIf, Func<TSource, TDestination> mapElse)
        => predicate(@this) ? mapIf(@this) : mapElse(@this);

        public static void DoIfNotNull<TSource>(this TSource @this, Action<TSource> action)
        {
            if (@this != null)
            {
                action(@this);
            }
        }

        public static async Task DoIfNotNullAsync<TSource>(this TSource @this, Func<TSource, Task> func)
        {
            if (@this != null)
            {
                await func(@this);
            }
        }

        public static void IfElseDo<TSource>(
            this TSource @this, Func<TSource, bool> predicate, Action<TSource> doIf, Action<TSource> doElse)
        {
            if (predicate(@this))
            {
                doIf(@this);
            }
            else
            {
                doElse(@this);
            }
        }

        public static async Task IfElseDoAsync<TSource>(
            this TSource @this, Func<TSource, bool> predicate, Func<TSource, Task> doIf, Func<TSource, Task> doElse)
        {
            if (predicate(@this))
            {
                await doIf(@this);
            }
            else
            {
                await doElse(@this);
            }
        }
    }
}
