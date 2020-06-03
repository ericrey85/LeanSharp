using System;
using System.Threading.Tasks;

namespace LeanSharp
{
    public static class Try
    {
        public static Result<TSuccess, Exception> Expression<TSuccess>(
            Func<TSuccess> tryFn)
        {
            try
            {
                return Result<TSuccess, Exception>.Succeeded(tryFn());
            }
            catch (Exception ex)
            {
                return Result<TSuccess, Exception>.Failed(ex);
            }
        }

        public static async Task<Result<TSuccess, Exception>> ExpressionAsync<TSuccess>(
            Func<Task<TSuccess>> tryFn)
        {
            try
            {
                return Result<TSuccess, Exception>.Succeeded(await tryFn());
            }
            catch (Exception ex)
            {
                return Result<TSuccess, Exception>.Failed(ex);
            }
        }
    }
}
