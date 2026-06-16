using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Classes
{
    public class MaintenanceRepository : IMaintenaceRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.SandboxEntities _context;

        public MaintenanceRepository(SandboxEntities context)
        {
            _context = context;
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
        // ~ActorRepository()
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

        public void DeleteLog(int id)
        {
            MVMLogs? logToDelete = _context.MVMLogs.Find(id);
            if (logToDelete != null)
            {
                _context.MVMLogs.Remove(logToDelete);
                Save();
            }
        }




        

        public bool UpdateLog(MVMLogs log)
        {
            _context.MVMLogs.Update(log);
            return Save();
        }

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        MVMLogs? IMaintenaceRepository.GetById(int id)
        {
            return _context.MVMLogs.Find(id);
        }

        public void InsertLog(MVMLogs log)
        {
            _context.MVMLogs.Add(log);
            SaveLog();
        }

        public bool SaveLog()
        {
            bool success = false;
            int result = _context.SaveChanges();
            return result > 0;
        }

        public IEnumerable<MVMLogs> GetLogs()
        {
            return _context.MVMLogs.OrderByDescending(m => m.CreatedOn);
        }
    }
}
