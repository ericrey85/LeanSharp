using LeanSharp.Extensions;
using System;
using System.Threading.Tasks;

namespace LeanSharp
{
    public class SafePipeline<TSuccess, TFailure>
    {
        private Func<Task<Result<TSuccess, TFailure>>> Expression { get; }

        public SafePipeline(Func<Task<Result<TSuccess, TFailure>>> expression)
        {
            Expression = expression;
        }

        public Task<Result<TSuccess, TFailure>> Flatten() => Expression();

        public SafePipeline<TNewSuccess, TFailure> Select<TNewSuccess>(
            Func<TSuccess, Task<Result<TNewSuccess, TFailure>>> fn)
        {
            async Task<Result<TNewSuccess, TFailure>> CombineExpressions()
            {
                var result = await Expression();
                return await result.BindAsync(async value => await fn(value));
            }

            return CreateSafePipeline.With(CombineExpressions);
        }

        public SafePipeline<TNewSuccess, TFailure> Select<TNewSuccess>(
            Func<TSuccess, Task<TNewSuccess>> fn)
        {
            async Task<Result<TNewSuccess, TFailure>> CombineExpressions()
            {
                var result = await Expression();
                return await result.MapAsync(async value => await fn(value));
            }

            return CreateSafePipeline.With(CombineExpressions);
        }

        public SafePipeline<TNewSuccess, TFailure> Select<TNewSuccess>(
            Func<TSuccess, TNewSuccess> fn)
        {
            async Task<Result<TNewSuccess, TFailure>> CombineExpressions()
            {
                var result = await Expression();
                return result.Map(fn);
            }

            return CreateSafePipeline.With(CombineExpressions);
        }

        public SafePipeline<TNewSuccess, TFailure> SelectMany<TNewSuccess>(
            Func<TSuccess, SafePipeline<TNewSuccess, TFailure>> fn)
        {
            async Task<Result<TNewSuccess, TFailure>> CombineExpressions()
            {
                var result = await Expression();

                return (await result.Map(value => fn(value))
                    .MapAsync(async safePipeline => await safePipeline.Flatten())).Bind(innerResult => innerResult);
            }

            return CreateSafePipeline.With(CombineExpressions);
        }

        public SafePipeline<TNewSuccess, TFailure> SelectMany<TIntermediateSuccess, TNewSuccess>(
            Func<TSuccess, SafePipeline<TIntermediateSuccess, TFailure>> fn, Func<TSuccess, TIntermediateSuccess, TNewSuccess> select)
        => SelectMany(a => fn(a).Select(b => select(a, b)));
    }

    public static class CreateSafePipeline
    {
        public static SafePipeline<TNewSuccess, TFailure> With<TNewSuccess, TFailure>(Func<Task<Result<TNewSuccess, TFailure>>> fn)
        => new SafePipeline<TNewSuccess, TFailure>(fn);

        public static SafePipeline<TNewSuccess, TFailure> With<TNewSuccess, TFailure>(Func<Result<TNewSuccess, TFailure>> fn)
        => new SafePipeline<TNewSuccess, TFailure>(() => fn().AsTask());

        public static SafePipeline<TNewSuccess, Exception> TryWith<TNewSuccess>(Func<TNewSuccess> fn)
        => new SafePipeline<TNewSuccess, Exception>(() => Try.Expression(() => fn()).AsTask());

        public static SafePipeline<TNewSuccess, Exception> TryWith<TNewSuccess>(Func<Task<TNewSuccess>> fn)
        => new SafePipeline<TNewSuccess, Exception>(async () => await Try.ExpressionAsync(async () => await fn()));
    }
}
