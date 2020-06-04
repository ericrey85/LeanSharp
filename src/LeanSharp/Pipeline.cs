using LeanSharp.Extensions;
using System;
using System.Threading.Tasks;

namespace LeanSharp
{
    public class Pipeline<TSource>
    {
        private Func<Task<TSource>> Expression { get; }

        public Pipeline(Func<Task<TSource>> expression)
        {
            Expression = expression;
        }

        public Task<TSource> Flatten() => Expression();

        public Pipeline<TDestination> Select<TDestination>(Func<TSource, Task<TDestination>> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return await fn(result);
            }

            return CreatePipeline.With(CombineExpressions);
        }

        public Pipeline<TDestination> Select<TDestination>(Func<TSource, TDestination> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return fn(result);
            }

            return CreatePipeline.With(CombineExpressions);
        }

        public Pipeline<TDestination> SelectMany<TDestination>(Func<TSource, Pipeline<TDestination>> fn)
        {
            async Task<TDestination> CombineExpressions()
            {
                var result = await Expression();
                return await fn(result).Flatten();
            }

            return CreatePipeline.With(CombineExpressions);
        }

        public Pipeline<TDestination> SelectMany<TIntermediate, TDestination>(
                Func<TSource, Pipeline<TIntermediate>> fn, Func<TSource, TIntermediate, TDestination> select)
        => SelectMany(a => fn(a).Select(b => select(a, b)));
    }

    public static class CreatePipeline
    {
        public static Pipeline<TDestination> With<TDestination>(Func<Task<TDestination>> fn)
        => new Pipeline<TDestination>(fn);

        public static Pipeline<TDestination> With<TDestination>(Func<TDestination> fn)
        => new Pipeline<TDestination>(() => fn().AsTask());

        public static Pipeline<TDestination> Return<TDestination>(TDestination value)
        => new Pipeline<TDestination>(() => value.AsTask());
    }
}
