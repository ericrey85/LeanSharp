using System;
using System.Threading.Tasks;

namespace LeanSharp
{
    public static class Dispose
    {
        public static async Task UsingAsync<TDisposable>(
            Func<TDisposable> factory,
            Func<TDisposable, Task> map)
            where TDisposable : IDisposable
        {
            using (var disposable = factory())
            {
                await map(disposable);
            }
        }

        public static async Task<TResult> UsingAsync<TDisposable, TResult>(
            Func<TDisposable> factory,
            Func<TDisposable, Task<TResult>> map)
            where TDisposable : IDisposable
        {
            using (var disposable = factory())
            {
                return await map(disposable);
            }
        }
    }
}
