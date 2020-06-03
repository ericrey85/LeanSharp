using System;

namespace LeanSharp
{
    public class Result<TSuccess, TFailure>
    {
        public static Result<TSuccess, TFailure> Succeeded(TSuccess success)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            return new Result<TSuccess, TFailure>
            {
                IsSuccessful = true,
                Success = success
            };
        }

        public static Result<TSuccess, TFailure> Failed(TFailure failure)
        {
            if (failure == null) throw new ArgumentNullException(nameof(failure));

            return new Result<TSuccess, TFailure>
            {
                IsSuccessful = false,
                Failure = failure
            };
        }

        private Result()
        {
        }

        public bool IsSuccess => IsSuccessful;
        public bool IsFailure => !IsSuccessful;

        public TSuccess Success { get; private set; }
        public TFailure Failure { get; private set; }
        private bool IsSuccessful { get; set; }
    }
}
