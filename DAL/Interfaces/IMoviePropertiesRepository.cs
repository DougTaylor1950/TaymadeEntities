using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMoviePropertiesRepository : IDisposable
    {
        #region Public Methods
        
        
        MovieProperties? GetById(int id);
        
        bool Save();

        void Update(MovieProperties MovieProperties);
        
        void Save(MovieProperties MovieProperties);
       

        #endregion Public Methods
    }
}
