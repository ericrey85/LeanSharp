using Xunit.Abstractions;

namespace LeanSharp.Tests.Extensions
{
    public static class Extensions
    {
        public static string GetOutputAsString(this ITestOutputHelper @this)
            => @this
                .GetType()
                .GetProperty("Output")
                ?.GetValue(@this)
                .ToString()
                .Replace("\n", "")
                .Replace("\r", "");
    }
}
