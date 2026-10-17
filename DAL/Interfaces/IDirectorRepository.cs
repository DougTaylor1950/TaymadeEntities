using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IDirectorRepository : IDisposable
    {
        #region Public Methods
        

        List<Director>? GetDirectors();

        Director? GetById(int id);
        Director? GetByName(string directorName);
      
        bool Insert(Director director);
        bool Save();

        Director? GetOrCreate(string directorName);
        bool Update(Director director);

        
        
        bool Save(Director director);
        
        bool DeleteDirector(Director director);
        bool AddDirector(Director director);
        Director? GetDirectorById(int id);
        bool UpdateDirector(Director director);
        
        Task<bool> UpdateDirectorAsync(Director director);
        List<Director>? GetDirectorsByName(string findDirectorText);

        #endregion Public Methods
    }
}
