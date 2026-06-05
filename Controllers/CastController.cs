using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    /// <summary>
    /// </summary>
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 04/06/2026 12:53 </created>
    /// </remarks>
    public class CastController : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The cast repository
        /// </summary>
        private ICastRepository castRepository;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CastController"/> class.
        /// </summary>
        public CastController()
        {
            this.castRepository = new CastRepository(new DBContext.SandboxEntities());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CastController"/> class.
        /// </summary>
        /// <param name="castRepository">The cast repository.</param>
        public CastController(ICastRepository castRepository)
        {
            this.castRepository = castRepository;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="castId">The cast identifier.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public void DeleteCast(int castId)
        {
            castRepository.DeleteCast(castId);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (castRepository is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="actorId">The actor identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public IEnumerable<Models.Cast> GetActorCasts(int actorId)
        {
            return castRepository.GetActorCasts(actorId);
        }

        /// <summary>
        /// </summary>
        /// <param name="castId">The cast identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public Models.Cast GetCastById(int castId)
        {
            return castRepository.GetCastById(castId);
        }

        /// <summary>
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="actorId">The actor identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public Models.Cast GetCastByRelationships(int movieId, int actorId)
        {
            return castRepository.GetCastByRelationships(movieId, actorId);
        }

        /// <summary>
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public IEnumerable<Models.Cast> GetCastsByMovieId(int movieId)
        {
            return castRepository.GetMovieCasts(movieId);
        }

        /// <summary>
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="actorId">The actor identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public Models.Cast GetOrCreateCast(int movieId, int actorId)
        {
            return castRepository.GetOrCreateCast(movieId, actorId);
        }
        /// <summary>
        /// </summary>
        /// <param name="cast">The cast.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/06/2026 04/06/2026 </created>
        /// </remarks>
        public void UpdateCast(Models.Cast cast)
        {
            castRepository.UpdateCast(cast);
        }

        internal Cast? GetCastByCreditId(string creditId)
        {
            return castRepository.GetCastByCreditId(creditId);
        }

        #endregion Public Methods
    }
}