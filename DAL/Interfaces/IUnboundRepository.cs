using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IUnboundRepository : IDisposable
    {
        #region Public Methods
        void Delete(int id);
        
        
        UnboundGridData? GetById(int id);
       
      
        bool Insert(UnboundGridData UnboundGridData);
        bool Save();

        
        bool Update(UnboundGridData UnboundGridData);
        void Add(UnboundGridData UnboundGridData);
        bool Save(UnboundGridData UnboundGridData);
        IEnumerable<UnboundGridData> GetData();
        bool UpdateDownloadProperties(DownloadProperties downloadProperties);


        #endregion Public Methods
    }
}
