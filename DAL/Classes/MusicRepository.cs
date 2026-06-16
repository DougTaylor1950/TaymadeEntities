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
    public class MusicRepository : IMusicRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.MusicEntitiesContext _context;

        public MusicRepository(MusicEntitiesContext context)
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

        public void Delete(int id)
        {
            //Actor actorToDelete = _context.Actors.Find(id);
            //if (actorToDelete != null)
            //{
            //    _context.Actors.Remove(actorToDelete);
            //    Save();
            //}
        }



        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }


    }
}
