using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface ICastRepository: IDisposable
    {
        #region Public Methods
        void DeleteCast(int id);

        IEnumerable<Cast> GetMovieCasts(int movieId);
        IEnumerable<Cast> GetActorCasts(int actorId);
        Cast GetCastById(int id);
        Cast GetCastByRelationships(int movieId, int actorId);

        void InsertCast(Cast cast);
        bool Save();

        Cast GetOrCreateCast(int movieId, int actorId);
        void UpdateCast(Cast cast);

        #endregion Public Methods
    }
}
