using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class UnboundController : IDisposable
    {

        private bool disposedValue;
        private IUnboundRepository unboundRepository;

        public UnboundController()
        {
            this.unboundRepository = new UnboundRepository(new DBContext.SandboxEntities());
        }

        public UnboundController(IUnboundRepository repository)
        {
            this.unboundRepository = repository;
        }

        public bool Save()
        {
            return unboundRepository.Save();
        }

        public bool Insert(UnboundGridData unbound)
        {
            return unboundRepository.Insert(unbound);
        }

        public bool Save(Models.UnboundGridData movie)
        {
            return unboundRepository.Save(movie);
        }

        public bool Update(Models.UnboundGridData movie)
        {
            return unboundRepository.Update(movie);
        }

        public void Delete(int id)
        {
            unboundRepository.Delete(id);
        }

        public UnboundGridData? GetById(int id)
        {
            return unboundRepository.GetById(id);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public List<UnboundGridData> GetData()
        {
            return unboundRepository.GetData().ToList();
        }

        internal bool UpdateDownloadProperties(DownloadProperties downloadProperties)
        {
            return unboundRepository.UpdateDownloadProperties(downloadProperties);
        }

        
    }
}
