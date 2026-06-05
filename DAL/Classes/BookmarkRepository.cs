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
    public class BookmarkRepository : IBookmarkRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.SandboxEntities _context;

        public BookmarkRepository(SandboxEntities context)
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

        

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }


        

        public bool Save(Bookmark bookmark)
        {
            _context.Bookmarks.Update(bookmark);
            return Save();
        }

        public void Delete(int id)
        {   Bookmark? bookmark = _context.Bookmarks.Find(id);
            if (bookmark != null)
            _context.Bookmarks.Remove(bookmark);
            _context.SaveChanges();
        }

        public Bookmark? GetById(int id)
        {
            return _context.Bookmarks.Find(id);
        }

        public void Insert(Bookmark bookmark)
        {
            _context.Bookmarks.Add(bookmark);
            _context.SaveChanges(); 
        }

        public void Update(Bookmark bookmark)
        {
            _context.Bookmarks.Update(bookmark);
            _context.SaveChanges();
        }

        public void Add(Bookmark bookmark)
        {
            _context.Bookmarks.Add(bookmark);
            _context.SaveChanges();
        }

        public IEnumerable<Bookmark>? GetBookmarksByMovieId(int id)
        {
            return _context.Bookmarks.Where(b => b.MovieID == id);
        }

        public async Task<bool> UpdateAsync(Bookmark bookmark)
        {
             _context.Bookmarks.Update(bookmark);
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
