using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface ITemplateRepository : IDisposable
    {
        #region Public Methods
        void Delete(int id);
        
        
        Actor? GetById(int id);
        Actor? GetByName(string actorName);
      
        void Insert(Actor actor);
        bool Save();

        Actor? GetOrCreate(string actorName);
        void Update(Actor actor);
        void Add(Actor actor);
        void Save(Actor actor);
       

        #endregion Public Methods
    }
}
