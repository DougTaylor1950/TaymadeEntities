using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MoviePropertiesController : IDisposable
    {

        private bool disposedValue;
        private IMoviePropertiesRepository moviePropertiesRepository;

        public MoviePropertiesController()
        {
            this.moviePropertiesRepository = new MoviePropertiesRepository(new DBContext.SandboxEntities());
        }

        public MoviePropertiesController(IMoviePropertiesRepository repository)
        {
            this.moviePropertiesRepository = repository;
        }

        public bool Save()
        {
            return moviePropertiesRepository.Save();
        }

        public void Save(Models.MovieProperties movie)
        {
            moviePropertiesRepository.Save(movie);
        }

        public void Update(Models.MovieProperties movie)
        {
            moviePropertiesRepository.Update(movie);
        }

        

        public MovieProperties? GetById(int id)
        {
            return moviePropertiesRepository.GetById(id);
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
