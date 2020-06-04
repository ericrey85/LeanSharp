using System;

namespace LeanSharp.Extensions
{
    public static class TraversableExtensions
    {
        public static Result<T, string> MapToStringFailure<T>(this Result<T, Exception> result) 
        => result.Either(
                r => Result<T, string>.Succeeded(r.Success),
                e => Result<T, string>.Failed(e.Failure.Message));
        public static Pipeline<Result<TSuccess, TFailure>> ToPipeline<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result)
        => result.IsSuccess
            ? CreatePipeLine.Return(Result<TSuccess, TFailure>.Succeeded(result.Success))
            : CreatePipeLine.Return(Result<TSuccess, TFailure>.Failed(result.Failure));

        public static Pipeline<Result<TSuccess, TFailure>> FlipMerge<TSuccess, TFailure>(
            this Result<Pipeline<Result<TSuccess, TFailure>>, TFailure> result)
        => result.EitherFold(r => r.Success, e => CreatePipeLine.Return(Result<TSuccess, TFailure>.Failed(e.Failure)));

        public static Pipeline<Result<TSuccess, TFailure>> FlipMerge<TSuccess, TFailure>(
            this Pipeline<Result<Pipeline<TSuccess>, TFailure>> pipeLine)
        => pipeLine
            .SelectMany(result =>
                result.EitherFold(
                    r => r.Success.Select(Result<TSuccess, TFailure>.Succeeded), 
                    error => CreatePipeLine.Return(Result<TSuccess, TFailure>.Failed(error.Failure))));
    }
}
