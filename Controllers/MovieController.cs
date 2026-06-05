using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MovieController : IDisposable
    {

        private bool disposedValue;
        private IMovieRepository movieRepository;

        public MovieController()
        {
            this.movieRepository = new MovieRepository(new DBContext.SandboxEntities());
        }

        public MovieController(IMovieRepository movieRepository)
        {
            this.movieRepository = movieRepository;
        }

        public bool Save()
        {
            return movieRepository.Save();
        }

        public bool Save(Models.Movies movie)
        {
            return movieRepository.Save(movie);
        }

        public bool UpdateMovie(Models.Movies movie)
        {
            return movieRepository.UpdateMovie(movie);
        }

        public void DeleteMovie(int id)
        {
            movieRepository.DeleteMovie(id);
        }

        public Movies? GetMoviesById(int id)
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
