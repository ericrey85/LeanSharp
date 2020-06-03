using System;
using System.Text;

namespace LeanSharp.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIf(this StringBuilder @this, Func<StringBuilder, bool> predicate, string text)
        {
            if (predicate(@this))
            {
                @this.Append(text);
            }

            return @this;
        }
    }
}
