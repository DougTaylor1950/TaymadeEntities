using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MusicController : IDisposable
    {

        private bool disposedValue;
        private IMusicRepository musicRepository;

        public MusicController()
        {
            this.musicRepository = new MusicRepository(new DBContext.MusicEntitiesContext());
        }

        public MusicController(IMusicRepository repository)
        {
            this.musicRepository = repository;
        }

        public bool Save()
        {
            return musicRepository.Save();
        }

        

        //public void Delete(int id)
        //{
        //    templateRepository.DeleteMovie(id);
        //}
                

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
    }
}
