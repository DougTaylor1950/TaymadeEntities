using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class Controller : IDisposable
    {

        private bool disposedValue;
        private IRepository Repository;

        public Controller()
        {
            this.Repository = new Repository(new DBContext.SandboxEntities());
        }

        public Controller(IRepository Repository)
        {
            this.Repository = Repository;
        }

        public bool Save()
        {
            return Repository.Save();
        }

        public bool Save(Models.Movies movie)
        {
            return Repository.Save(movie);
        }

        public bool Update(Models.Movies movie)
        {
            return movieRepository.UpdateMovie(movie);
        }

        public void Delete(int id)
        {
            movieRepository.DeleteMovie(id);
        }

        public Movies? GetById(int id)
        {
            return movieRepository.GetMoviesById(id);
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
    }
}
