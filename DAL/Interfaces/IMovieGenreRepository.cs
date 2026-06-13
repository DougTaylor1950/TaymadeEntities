using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMovieGenreRepository : IDisposable
    {
        #region Public Methods
        void Delete(int id);

        void DeleteMovieGenre(MovieGenre genre);

        MovieGenre CreateMovieGenre(int movieId, string genre, string? subGenre);
        MovieGenre? GetById(int id);
        
      
        void Insert(MovieGenre genre);
        bool Save(MovieGenre genre);

        

        bool Update(MovieGenre genre);
        void Add(MovieGenre genre);
        bool Save();
       

        #endregion Public Methods
    }
}
