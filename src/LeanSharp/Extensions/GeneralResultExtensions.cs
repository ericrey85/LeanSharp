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

        public static Result<TResult, string[]> IfNull<TResult>(
            this TResult? @this,
            string failMessage) where TResult : struct
            => @this.HasValue
                ? Result<TResult, string[]>.Succeeded(@this.Value)
                : Result<TResult, string[]>.Failed(new[] { failMessage });

        public static Result<TResult, string[]> IfNull<TResult>(
            this TResult @this,
            string failMessage) where TResult : class
            => @this != null
                ? Result<TResult, string[]>.Succeeded(@this)
                : Result<TResult, string[]>.Failed(new[] { failMessage });
    }
}
