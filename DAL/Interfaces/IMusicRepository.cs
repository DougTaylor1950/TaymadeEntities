using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMusicRepository : IDisposable
    {
        #region Public Methods
        void Delete(int id);
        
        
        bool Save();

       

        #endregion Public Methods
    }
}
