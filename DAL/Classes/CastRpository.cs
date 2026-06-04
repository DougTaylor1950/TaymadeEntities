using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;
using Microsoft.EntityFrameworkCore;

namespace TaymadeEntities.DAL
{
    public class CastRepository : ICastRepository, IDisposable
    {
        #region Private Fields

        private readonly DBContext.SandboxEntities _context;


        private bool disposed = false;

        #endregion Private Fields

        #region Public Constructors

        public CastRepository(DBContext.SandboxEntities context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="actorId">The actor identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026  </created>
        /// </remarks>
        public Cast GetOrCreateCast(int movieId, int actorId)
        {
            // Check if the cast already exists
            Cast existingCast = _context.Casts.SingleOrDefault(c => c.MovieID == movieId && c.ActorId == actorId);
            if (existingCast != null)
            {
                return existingCast; // Return the existing cast if it already exists
            }

            // Create a new cast
            Cast newCast = new Cast()
            {
                MovieID = movieId,
                ActorId = actorId
            };
            //using var ctx = DataController.SandboxEntities;
            Cast? temp = _context.CreateCast(newCast);
            //_context.Casts.Add(newCast);
            newCast.Id = temp.Id; // Assign the generated ID to the new cast
            //Save(); // Save changes to the database to generate the ID for the new cast
            //_context.Casts.Add(newCast);
            return newCast;
        }

        public void DeleteCast(int id)
        {
            Cast cast = _context.Casts.Find(id);
            if (cast != null)
            {
                _context.Casts.Remove(cast);
            }

            using var ctx = _context;
            ctx.DeleteCastMember(id);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Cast> GetActorCasts(int actorId)
        {
            return _context.Casts.Where(c => c.ActorId == actorId).ToList();
        }

        public Cast GetCastById(int id)
        {
            return _context.Casts.Find(id);
        }

        public Cast GetCastByRelationships(int movieId, int actorId)
        {
            return _context.Casts.FirstOrDefault(c => c.MovieID == movieId && c.ActorId == actorId);
        }

        public IEnumerable<Cast> GetMovieCasts(int movieId)
        {
            return _context.Casts.Where(c => c.MovieID == movieId).ToList();
        }

        public void InsertCast(Cast cast)
        {
            _context.Casts.Add(cast);
        }

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        public void UpdateCast(Cast cast)
        {
            _context.Entry(cast).State = EntityState.Modified;
            
        }

       

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        #endregion Protected Methods
    }
}

