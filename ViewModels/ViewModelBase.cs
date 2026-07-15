using ReactiveUI;
using System;

namespace TaymadeEntities.ViewModels
{

    
    public class ViewModelBase : ReactiveObject, IDisposable
    {
        private bool _disposed = false;
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                // ...
            }

            // Free unmanaged resources.
            // ...

            _disposed = true;
        }
    }
}
