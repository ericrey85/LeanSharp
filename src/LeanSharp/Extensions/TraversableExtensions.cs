using System;

namespace LeanSharp.Extensions
{
    public static class TraversableExtensions
    {
        public static Pipeline<Result<TSuccess, TFailure>> ToPipeline<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result)
        => result.IsSuccess
            ? CreatePipeline.Return(Result<TSuccess, TFailure>.Succeeded(result.Success))
            : CreatePipeline.Return(Result<TSuccess, TFailure>.Failed(result.Failure));

        public static Pipeline<Result<TSuccess, TFailure>> FlipMerge<TSuccess, TFailure>(
            this Result<Pipeline<Result<TSuccess, TFailure>>, TFailure> result)
        => result.EitherFold(r => r.Success, e => CreatePipeline.Return(Result<TSuccess, TFailure>.Failed(e.Failure)));

        public static Pipeline<Result<TSuccess, TFailure>> FlipMerge<TSuccess, TFailure>(
            this Pipeline<Result<Pipeline<TSuccess>, TFailure>> pipeLine)
        => pipeLine
            .SelectMany(result =>
                result.EitherFold(
                    r => r.Success.Select(Result<TSuccess, TFailure>.Succeeded), 
                    error => CreatePipeline.Return(Result<TSuccess, TFailure>.Failed(error.Failure))));
    }
}
