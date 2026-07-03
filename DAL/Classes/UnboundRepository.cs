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
    public class UnboundRepository : IUnboundRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.SandboxEntities _context;

        public UnboundRepository(SandboxEntities context)
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
        // ~UnboundGridDataRepository()
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

        public void Delete(int id)
        {
            UnboundGridData UnboundGridDataToDelete = _context.UnboundGridData.Find(id);
            if (UnboundGridDataToDelete != null)
            {
                _context.UnboundGridData.Remove(UnboundGridDataToDelete);
                Save();
            }
        }

        public void DeleteUnboundGridData(UnboundGridData? unbound)
        {
            
            if (unbound != null)
            {
                _context.UnboundGridData.Remove(unbound);
                Save();
            }
        }

      

        public UnboundGridData? GetUnboundGridDataById(int id)
        {
            return _context.UnboundGridData.Find(id);
        }

        public bool InsertUnboundGridData(UnboundGridData UnboundGridData)
        {
            _context.UnboundGridData.Add(UnboundGridData);
            return Save();
        }

        public bool Update(UnboundGridData UnboundGridData)
        {
            _context.UnboundGridData.Update(UnboundGridData);
            return Save();
        }

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        public void Add(UnboundGridData UnboundGridData)
        {
            _context.UnboundGridData.Add(UnboundGridData);
            Save();
        }

       

        public bool Save(UnboundGridData UnboundGridData)
        {
            _context.UnboundGridData.Update(UnboundGridData);
            return Save();
        }

        public UnboundGridData? GetById(int id)
        {
            return _context.UnboundGridData.Find(id);
        }

        public bool Insert(UnboundGridData unboundGridData)
        {
            _context.Add(unboundGridData);
            return Save();
        }

        public IEnumerable<UnboundGridData> GetData()
        {
            return _context.UnboundGridData.OrderByDescending(x => x.CreationTime).ToList();
        }

        public bool UpdateDownloadProperties(DownloadProperties downloadProperties)
        {
            _context.DownloadProperties.Update(downloadProperties);
            return Save();
        }
    }
}
