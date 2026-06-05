using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Classes
{
    public class MovieRepository : IMovieRepository, IDisposable
    {
        private bool disposedValue;
        private readonly DBContext.SandboxEntities _context;

        public MovieRepository(DBContext.SandboxEntities context)
        {
            _context = context;
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool Save(Movies movie)
        {
            return UpdateMovie(movie);
            //Movies movieTemp = _context.Movies.Find(movie.Id);
            //if (movieTemp != null)
            //{
            //    _context.Movies.Remove(movieTemp);
            //}
            //_context.Movies.Add(movie);
            //return _context.SaveChanges() > 0;
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
        // ~MovieRepository()
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

        public bool UpdateMovie(Movies movie)
        {
            _context.Movies.Update(movie);
            return _context.SaveChanges() > 0;
        }

        public void DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }
        }

        public Movies? GetMoviesById(int id)
        {
            return _context.Movies.Find(id);
        }

        public bool Add(Movies movie)
        {
             _context.Add(movie);
            return _context.SaveChanges() >= 1;
        }
    }
}
