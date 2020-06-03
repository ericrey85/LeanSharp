using System;

namespace LeanSharp.Tests
{
    public class DisposableStub : IDisposable
    {
        public bool IsItDisposed { get; private set; }

        public DisposableStub(bool isItDisposed)
        {
            IsItDisposed = IsItDisposed;
        }

        public void Dispose()
        {
            IsItDisposed = true;
        }
    }
}
