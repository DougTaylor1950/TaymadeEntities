using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MovieGenreController : IDisposable
    {

        private bool disposedValue;
        private IMovieGenreRepository movieGenreRepository;

        public MovieGenreController()
        {
            this.movieGenreRepository = new MovieGenreRepository(new DBContext.SandboxEntities());
        }

        public MovieGenreController(IMovieGenreRepository repository)
        {
            this.movieGenreRepository = repository;
        }

        public bool Save()
        {
            return movieGenreRepository.Save();
        }

        public bool Save(Models.MovieGenre movie)
        {
            return movieGenreRepository.Save(movie);
        }

        public bool Update(Models.MovieGenre movie)
        {
            return movieGenreRepository.Update(movie);
        }

        public void Delete(int id)
        {
            movieGenreRepository.Delete(id);
        }

        public MovieGenre? CreateMovieGenre(int movieId, string genre, string? subGenre)
        {
            return movieGenreRepository.CreateMovieGenre(movieId, genre, subGenre);
        }

        public MovieGenre? GetById(int id)
        {
            return movieGenreRepository.GetById(id);
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
