using System;

namespace LeanSharp.Extensions
{
    public static class SafePipelineExtensions
    {
        public static SafePipeline<TSuccess, TNewFailure> ToFailure<TSuccess, TFailure, TNewFailure>(
            this SafePipeline<TSuccess, TFailure> @this, Func<TFailure, TNewFailure> transformFailure)
        => CreateSafePipeline.With(async () => await @this.Flatten().MapToAsync(async task =>
               (await task).EitherFold(
                    result => Result<TSuccess, TNewFailure>.Succeeded(result.Success),
                    result => Result<TSuccess, TNewFailure>.Failed(transformFailure(result.Failure)))
           ));

        public static SafePipeline<TSuccess, string> ToStringFailure<TSuccess>(
            this SafePipeline<TSuccess, Exception> @this)
        => CreateSafePipeline.With(async () => await @this.Flatten().MapToAsync(async task =>
               (await task).EitherFold(
                    result => Result<TSuccess, string>.Succeeded(result.Success),
                    result => Result<TSuccess, string>.Failed(result.Failure.ToString()))
           ));
    }
}