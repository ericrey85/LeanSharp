using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeanSharp.Extensions
{
    public static class ResultExtensions
    {
        public static void Handle<TSuccess, TFailure>(this Result<TSuccess, TFailure> result,
            Action<TSuccess> onSuccess,
            Action<TFailure> onFailure)
        {
            if (result.IsSuccess)
                onSuccess(result.Success);
            else
                onFailure(result.Failure);
        }

        public static async Task HandleAsync<TSuccess, TFailure>(this Result<TSuccess, TFailure> result,
            Func<TSuccess, Task> onSuccess,
            Func<TFailure, Task> onFailure)
        {
            if (result.IsSuccess)
                await onSuccess(result.Success);
            else
                await onFailure(result.Failure);
        }

        public static Result<TSuccess2, TFailure2> Either<TSuccess, TFailure, TSuccess2, TFailure2>(
            this Result<TSuccess, TFailure> result,
            Func<Result<TSuccess, TFailure>, Result<TSuccess2, TFailure2>> onSuccess,
            Func<Result<TSuccess, TFailure>, Result<TSuccess2, TFailure2>> onFailure)
                => result.IsSuccess ? onSuccess(result) : onFailure(result);

        public static TSuccess2 EitherFold<TSuccess, TFailure, TSuccess2>(
            this Result<TSuccess, TFailure> result,
            Func<Result<TSuccess, TFailure>, TSuccess2> onSuccess,
            Func<Result<TSuccess, TFailure>, TSuccess2> onFailure)
                => result.IsSuccess ? onSuccess(result) : onFailure(result);

        // Whatever result is, make it a failure.
        // The trick is that failure is an array type, it can be made an empty array failure.
        public static Result<TSuccess, TFailure[]> ToFailure<TSuccess, TFailure>(
            this Result<TSuccess, TFailure[]> result)
                => result.Either(
                    a => Result<TSuccess, TFailure[]>.Failed(new TFailure[0]),
                    b => b
                );

        // Put accumulator and next together.
        // If they are both successes, then put them together as a success.
        // If either/both are failures, then put them together as a failure.
        // Because success and failure is an array, they can be put together
        public static Result<TSuccess[], TFailure[]> Merge<TSuccess, TFailure>(
            this Result<TSuccess[], TFailure[]> accumulator,
            Result<TSuccess, TFailure[]> next)
        {
            if (accumulator.IsSuccess && next.IsSuccess)
            {
                return Result<TSuccess[], TFailure[]>
                    .Succeeded(accumulator.Success.Concat(new List<TSuccess> { next.Success })
                        .ToArray());
            }

            return Result<TSuccess[], TFailure[]>
                .Failed(accumulator.ToFailure().Failure.Concat(next.ToFailure().Failure).ToArray());
        }

        // Aggregate an array of results together.
        // If any of the results fail, return combined failures
        // Will only return success if all results succeed
        public static Result<TSuccess[], TFailure[]> Aggregate<TSuccess, TFailure>(
            this IEnumerable<Result<TSuccess, TFailure[]>> accumulator)
        {
            var emptySuccess = Result<TSuccess[], TFailure[]>.Succeeded(new TSuccess[0]);

            return accumulator.Aggregate(emptySuccess, (acc, o) => acc.Merge(o));
        }

        // Map: functional map
        // if result is a a success call f, otherwise pass it through as a failure
        public static Result<TSuccessNew, TFailure> Map<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> result, Func<TSuccess, TSuccessNew> f)
                => result.IsSuccess ? Result<TSuccessNew, TFailure>.Succeeded(f(result.Success)) :     Result<TSuccessNew, TFailure>.Failed(result.Failure);
                    
        public static async Task<Result<TSuccessNew, TFailure>> MapAsync<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> result, Func<TSuccess, Task<TSuccessNew>> f)
                => result.IsSuccess ? Result<TSuccessNew, TFailure>.Succeeded(await f(result.Success)) : Result<TSuccessNew, TFailure>.Failed(result.Failure);

        public static Result<TSuccessNew, TFailure> IfElseMap<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, bool> predicate,
            Func<TSuccess, TSuccessNew> @if,
            Func<TSuccess, TSuccessNew> @else)
        {
            if (result.IsFailure)
            {
                return Result<TSuccessNew, TFailure>.Failed(result.Failure);
            }

            return predicate(result.Success)
                ? Result<TSuccessNew, TFailure>.Succeeded(@if(result.Success))
                : Result<TSuccessNew, TFailure>.Succeeded(@else(result.Success));
        }

        public static async Task<Result<TSuccessNew, TFailure>> IfElseMapAsync<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, bool> predicate,
            Func<TSuccess, Task<TSuccessNew>> @if,
            Func<TSuccess, Task<TSuccessNew>> @else)
        {
            if (result.IsFailure)
            {
                return Result<TSuccessNew, TFailure>.Failed(result.Failure);
            }

            return predicate(result.Success)
                ? Result<TSuccessNew, TFailure>.Succeeded(await @if(result.Success))
                : Result<TSuccessNew, TFailure>.Succeeded(await @else(result.Success));
        }

        // Bind: functional bind
        // Monadize it!
        public static Result<TSuccessNew, TFailure> Bind<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> x, Func<TSuccess, Result<TSuccessNew, TFailure>> f)
                => x.IsSuccess ? f(x.Success) : Result<TSuccessNew, TFailure>.Failed(x.Failure);

        public static async Task<Result<TSuccessNew, TFailure>> BindAsync<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> x, Func<TSuccess, Task<Result<TSuccessNew, TFailure>>> f)
                => x.IsSuccess ? await f(x.Success) : Result<TSuccessNew, TFailure>.Failed(x.Failure);

        public static Result<TSuccessNew, TFailure> IfElseBind<TSuccess, TFailure, TSuccessNew>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, bool> predicate,
        Func<TSuccess, Result<TSuccessNew, TFailure>> @if,
        Func<TSuccess, Result<TSuccessNew, TFailure>> @else)
        {
            if (result.IsFailure)
            {
                return Result<TSuccessNew, TFailure>.Failed(result.Failure);
            }

            return predicate(result.Success)
                ? @if(result.Success)
                : @else(result.Success);
        }

        public static async Task<Result<TSuccessNew, TFailure>> IfElseBindAsync<TSuccess, TFailure, TSuccessNew>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, bool> predicate,
            Func<TSuccess, Task<Result<TSuccessNew, TFailure>>> @if,
            Func<TSuccess, Task<Result<TSuccessNew, TFailure>>> @else)
        {
            if (result.IsFailure)
            {
                return Result<TSuccessNew, TFailure>.Failed(result.Failure);
            }

            return predicate(result.Success)
                ? await @if(result.Success)
                : await @else(result.Success);
        }

        public static Result<TSuccess, TFailure> Tee<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result,
            Action<TSuccess> f)
        {
            if (result.IsSuccess)
            {
                f(result.Success);
            }

            return result;
        }

        public static Result<TSuccess, TFailure> TeeIfFailure<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result,
            Action<TFailure> f)
        {
            if (result.IsFailure)
            {
                f(result.Failure);
            }

            return result;
        }

        public static Result<TSuccess3, TFailure> SelectMany<TSuccess, TFailure, TSuccess2, TSuccess3>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, Result<TSuccess2, TFailure>> fn,
            Func<TSuccess, TSuccess2, TSuccess3> select)
                => result.Bind(a => fn(a).Map(b => select(a, b)));

        public static Result<TSuccessNew, Exception> Try<TSuccess, TSuccessNew, TFailure>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, Result<TSuccessNew, Exception>> tryFn)
        {
            try
            {
                return tryFn(result.Success);
            }
            catch (Exception ex)
            {
                return Result<TSuccessNew, Exception>.Failed(ex);
            }
        }

        public static Result<TSuccessNew, Exception> Try<TSuccess, TSuccessNew, TFailure>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, TSuccessNew> tryFn)
        {
            try
            {
                return Result<TSuccessNew, Exception>.Succeeded(tryFn(result.Success));
            }
            catch (Exception ex)
            {
                return Result<TSuccessNew, Exception>.Failed(ex);
            }
        }

        public static Result<bool, Exception> Try<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result,
            Action<TSuccess> tryFn)
        {
            try
            {
                tryFn(result.Success);

                return Result<bool, Exception>.Succeeded(true);
            }
            catch (Exception ex)
            {
                return Result<bool, Exception>.Failed(ex);
            }
        }

        public static async Task<Result<TSuccessNew, Exception>> TryAsync<TSuccess, TSuccessNew, TFailure>(
            this Result<TSuccess, TFailure> result,
            Func<TSuccess, Task<Result<TSuccessNew, Exception>>> tryFn)
        {
            try
            {
                return await tryFn(result.Success);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Result<TSuccessNew, Exception>.Failed(ex));
            }
        }
    }
}
