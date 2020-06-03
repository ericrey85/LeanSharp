using LeanSharp.Extensions;
using System;
using System.Threading.Tasks;

namespace LeanSharp
{
    public class AsyncLazyPipeline<TSource>
    {
        private Func<Task<TSource>> Expression { get; }

        public AsyncLazyPipeline(Func<Task<TSource>> expression)
        {
            Expression = expression;
        }

        public Task<TSource> Flatten() => Expression();

        public AsyncLazyPipeline<TDestination> Select<TDestination>(Func<TSource, Task<TDestination>> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return await fn(result);
            }

            return CreatePipeLine.With(CombineExpressions);
        }

        public AsyncLazyPipeline<TDestination> Select<TDestination>(Func<TSource, TDestination> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return fn(result);
            }

            return CreatePipeLine.With(CombineExpressions);
        }

        public AsyncLazyPipeline<TDestination> SelectMany<TDestination>(Func<TSource, AsyncLazyPipeline<TDestination>> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return await fn(result).Flatten();
            }

            return CreatePipeLine.With(CombineExpressions);
        }

        public AsyncLazyPipeline<TDestination> SelectMany<TIntermediate, TDestination>(
                Func<TSource, AsyncLazyPipeline<TIntermediate>> fn, Func<TSource, TIntermediate, TDestination> select)
        => SelectMany(a => fn(a).Select(b => select(a, b)));
    }

    public static class CreatePipeLine
    {
        public static AsyncLazyPipeline<TDestination> With<TDestination>(Func<Task<TDestination>> fn)
        => new AsyncLazyPipeline<TDestination>(fn);

        public static AsyncLazyPipeline<TDestination> With<TDestination>(Func<TDestination> fn)
        => new AsyncLazyPipeline<TDestination>(() => fn().AsTask());

        public static AsyncLazyPipeline<TDestination> Return<TDestination>(TDestination value)
        => new AsyncLazyPipeline<TDestination>(() => value.AsTask());
    }
}
