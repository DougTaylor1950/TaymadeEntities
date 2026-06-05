using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class BookmarkController : IDisposable
    {
        #region Private Fields

        private IBookmarkRepository bookmarkRepository;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public BookmarkController()
        {
            this.bookmarkRepository = new BookmarkRepository(new DBContext.SandboxEntities());
        }

        public BookmarkController(IBookmarkRepository Repository)
        {
            this.bookmarkRepository = Repository;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Delete(int id)
        {
            bookmarkRepository.Delete(id);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public List<Bookmark>? GetBookmarksByMovieId(int id)
        {
            List<Bookmark> tempList = bookmarkRepository.GetBookmarksByMovieId(id).ToList();

            return tempList;
        }

        public Bookmark? GetById(int id)
        {
            return bookmarkRepository.GetById(id);
        }

        public bool Save()
        {
            return bookmarkRepository.Save();
        }

        public bool Save(Models.Bookmark bookmark)
        {
            return bookmarkRepository.Save(bookmark);
        }

        public bool Update(Models.Bookmark bookmark)
        {
            bookmarkRepository.Update(bookmark);
            return true;
        }

        #endregion Public Methods

        #region Internal Methods

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        internal void Add(Bookmark bookmark)
        {
            bookmarkRepository.Add(bookmark);
        }

        internal Task<bool> UpdateAsync(Bookmark bookmark)
        {
            return bookmarkRepository.UpdateAsync(bookmark);
        }

        #endregion Internal Methods

        #region Protected Methods

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

        #endregion Protected Methods
    }
}