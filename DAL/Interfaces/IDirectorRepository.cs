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
        void Delete(int id);

        IEnumerable<Director>? GetDirectors();

        Director? GetById(int id);
        Director? GetByName(string directorName);
      
        bool Insert(Director director);
        bool Save();

        Director? GetOrCreate(string directorName);
        bool Update(Director director);

        Task<bool> UpdateAsync(Director director);
        void Add(Director director);
        bool Save(Director director);
        Task<bool> SaveAsync();

        #endregion Public Methods
    }
}
