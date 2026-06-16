using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMaintenaceRepository : IDisposable
    {
        #region Public Methods
        void DeleteLog(int id);
        
        
        MVMLogs? GetById(int id);
        IEnumerable<MVMLogs> GetLogs();

        void InsertLog(MVMLogs MVMLogs);
        bool Save();
        bool SaveLog();

        bool UpdateLog(MVMLogs log);
        
       

        #endregion Public Methods
    }
}
