using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IActorRepository : IDisposable
    {
        #region Public Methods
        void DeleteActor(int id);
        
        IEnumerable<Movies>? GetActorMovies(int actorId);
        Actor? GetActorById(int id);
        Actor? GetActorByName(string actorName);
        Actor? GetActorByTMID(int tmdbId);
        void InsertActor(Actor actor);
        bool Save();

        Actor? GetOrCreateActor(string actorName);
        void UpdateActor(Actor actor);
        void AddActor(Actor actor);
        void Save(Actor actor);
        void SetDetailsFromCastMember(Actor actor, CastMember person);

        #endregion Public Methods
    }
}
