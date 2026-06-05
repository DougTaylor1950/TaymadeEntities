using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IBookmarkRepository : IDisposable
    {
        #region Public Methods
        void Delete(int id);

        IEnumerable<Bookmark>? GetBookmarksByMovieId(int id);
        Bookmark? GetById(int id);
        
      
        void Insert(Bookmark actor);
        bool Save();

        
        void Update(Bookmark bookmark);
        void Add(Bookmark bookmark);
        bool Save(Bookmark actor);
        Task<bool> UpdateAsync(Bookmark bookmark);


        #endregion Public Methods
    }
}
