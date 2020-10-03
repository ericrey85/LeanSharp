namespace LeanSharp.Extensions
{
    public static class GeneralResultExtensions
    {
        public static Result<string, string[]> IfNullOrEmpty(
            this string @this,
            string failMessage)
            => string.IsNullOrEmpty(@this)
                ? Result<string, string[]>.Failed(new[] { failMessage })
                : Result<string, string[]>.Succeeded(@this);

        public static Result<TResult, TFailure> IfNull<TResult, TFailure>(
            this TResult? @this,
            TFailure failure) where TResult : struct
            => @this.HasValue
                ? Result<TResult, TFailure>.Succeeded(@this.Value)
                : Result<TResult, TFailure>.Failed(failure);

        public static Result<TResult, TFailure> IfNull<TResult, TFailure>(
            this TResult @this,
            TFailure failure) where TResult : class
            => @this != null
                ? Result<TResult, TFailure>.Succeeded(@this)
                : Result<TResult, TFailure>.Failed(failure);
    }
}
