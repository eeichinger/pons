using System;
using Pons.Util;

namespace Pons
{
    public class DisposableAdapter : IDisposable
    {
        private readonly Action _onDisposeHandler;

        public DisposableAdapter(Action onDisposeHandler)
        {
            Ensure.NotNull(onDisposeHandler, "onDisposeHandler");
            _onDisposeHandler = onDisposeHandler;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _onDisposeHandler();
        }
    }
}
