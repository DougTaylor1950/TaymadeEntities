using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MaintenaceController : IDisposable
    {

        private bool disposedValue;
        private IMaintenaceRepository maintenaceRepository;

        public MaintenaceController()
        {
            this.maintenaceRepository = new MaintenanceRepository(new DBContext.SandboxEntities());
        }

        public MaintenaceController(IMaintenaceRepository repository)
        {
            this.maintenaceRepository = repository;
        }

        public bool Save()
        {
            return maintenaceRepository.Save();
        }

       

        public bool UpdateLog(MVMLogs log)
        {
            return maintenaceRepository.UpdateLog(log);
        }

        public void DeleteLog(int id)
        {
            maintenaceRepository.DeleteLog(id);
        }

        public MVMLogs? GetById(int id)
        {
            return maintenaceRepository.GetById(id);
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

        public IEnumerable<MVMLogs> GetLogs()
        {
            return maintenaceRepository.GetLogs();
        }

        internal void InsertLog(MVMLogs mVMLogs)
        {
            maintenaceRepository.InsertLog(mVMLogs);
        }
    }
}
