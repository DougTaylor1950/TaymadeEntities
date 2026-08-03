using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 08/06/2026 10:51 </created>
    /// </remarks>
    public class MovieController : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// The movie repository
        /// </summary>
        private IMovieRepository movieRepository;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        public MovieController()
        {
            this.movieRepository = new MovieRepository(new DBContext.SandboxEntities());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        /// <param name="movieRepository">The movie repository.</param>
        public MovieController(IMovieRepository movieRepository)
        {
            this.movieRepository = movieRepository;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public bool DeleteMovie(int id)
        {
            return movieRepository.DeleteMovie(id);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public List<Movies>? GetMoviesByActor(int id)
        {
            return movieRepository.GetMoviesByActor(id)?.ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public List<Movies>? GetMoviesByDirector(int id)
        {
            return movieRepository.GetMoviesByDirector(id)?.ToList();
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        /// <summary>
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="subGenre">The sub genre.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public List<Movies>? GetMoviesByGenre(string? genre,
            string? subGenre = "")

        {
            return movieRepository.GetMoviesByGenre(genre, subGenre)?.ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public Movies? GetMoviesById(int id)
        {
            return movieRepository.GetMoviesById(id);
        }

        /// <summary>
        /// </summary>
        /// <param name="stub">The stub.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public List<Movies>? GetMoviesByInfo(string? stub)
        {
            if (!string.IsNullOrEmpty(stub))
                return movieRepository.GetMoviesByInfo(stub)?.ToList();
            else return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public List<Movies>? GetMoviesByTitle(string? title)
        {
            if (!string.IsNullOrEmpty(title))
                return movieRepository.GetMoviesByTitle(title)?.ToList();
            else return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public async Task<List<Movies>>? GetMoviesByTitleAsync(string? title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                IEnumerable<Movies>? tempList = await movieRepository.GetMoviesByTitleAsync(title);
                return tempList.ToList();
            }
            else return null;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return movieRepository.Save();
        }

        /// <summary>
        /// Saves the specified movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        public bool Save(Models.Movies movie)
        {
            return movieRepository.Save(movie);
        }

        /// <summary>
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public bool UpdateMovie(Models.Movies movie)
        {
            return movieRepository.UpdateMovie(movie);
        }

        public FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId)
        {
            return movieRepository.GetFrameSetHeaderByMovieImageId(movieImageId);
        }

        public bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader)
        {
            return movieRepository.UpdateFrameSetHeader(frameSetHeader);
        }

        public bool InsertFrameSetHeader(FrameSetHeader frameSetHeader)
        {
            return movieRepository.InsertFrameSetHeader(frameSetHeader);
        }

        /// <summary>
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 08/06/2026 08/06/2026 </created>
        /// </remarks>
        public async Task<bool> UpdateMovieAsync(Models.Movies movie)
        {
            return await movieRepository.UpdateMovieAsync(movie);
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Adds the specified movies.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <returns></returns>
        internal bool Add(Movies movies)
        {
            return movieRepository.Add(movies);
        }

        #endregion Internal Methods

        #region Protected Methods

        public List<MovieIntResult> GetActorMovieIds(string actorName)
        {
            List<MovieIntResult> intResults = new List<MovieIntResult>();
            if (!string.IsNullOrEmpty(actorName))
            {
                intResults = movieRepository.GetActorMovieIds(actorName);
            }
            return intResults;
        }

        public List<Movies>? GetMoviesbyBookmarkName(string bookmarkText)
        {
            if (!string.IsNullOrEmpty(bookmarkText))
            {
                IEnumerable<Movies>? tempList = movieRepository.GetMoviesbyBookmarkName(bookmarkText);
                return tempList.ToList();
            }
            else return null;
        }

        internal bool AddMovieImage(MovieImage movieImage)
        {
            return movieRepository.AddMovieImage(movieImage);
        }

        internal bool DeleteMovieImage(MovieImage movieImage)
        {
            return movieRepository.DeleteMovieImage(movieImage);
        }

        internal List<MovieImage>? GetMovieImagesByFolder(string v)
        {
            return movieRepository.GetMovieImagesByFolder(v);
        }

        internal List<MovieImage>? GetMovieImagesById(int id)
        {
            return movieRepository.GetMovieImagesById(id).ToList();
        }

        internal bool SaveMovieImage(MovieImage movieImage)
        {
            return movieRepository.SaveMovieImage(movieImage);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        internal MovieImage? GetMovieImageById(int? lastId)
        {
            return movieRepository.GetMovieImageById(lastId);
        }

        internal Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "")
        {
            return movieRepository.CreateMovie(filmName, year, path, filmGroup);
        }

        public MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey)
        {
            return movieRepository.CreateMovieGenre(movieId, genreCompKey, subGenreCompKey);
        }
        #endregion Protected Methods
    }
}