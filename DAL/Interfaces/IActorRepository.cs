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
        void AddActor(Actor actor);

        void DeleteActor(int id);

        Actor? GetActorById(int id);

        Actor? GetActorByName(string actorName);

        Actor? GetActorByTMID(int tmdbId);

        IEnumerable<Movies>? GetActorMovies(int actorId);
        IEnumerable<Actor> GetActors();

        IEnumerable<Actor> GetActorsByName(string findText);

        Actor? GetOrCreateActor(string actorName);

        void InsertActor(Actor actor);
        bool Save();
        void Save(Actor actor);

        void SetDetailsFromCastMember(Actor actor, CastMember person);

        void UpdateActor(Actor actor);
        #endregion Public Methods
    }
}
