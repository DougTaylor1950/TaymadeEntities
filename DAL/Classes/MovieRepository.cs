using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Classes
{
    public class MovieRepository : IMovieRepository, IDisposable
    {
        #region Private Fields

        private readonly DBContext.SandboxEntities _context;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public MovieRepository(DBContext.SandboxEntities context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool Add(Movies movie)
        {
            _context.Add(movie);
            return _context.SaveChanges() >= 1;
        }

        public bool AddFrameSet(FrameSet frameSet)
        {
            _context.FrameSet.Add(frameSet);
            return Save();
        }

        public bool AddMovieImage(MovieImage movieImage)
        {
            _context.MovieImage.Add(movieImage);
            return Save();
        }

        public Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "")
        {
            return _context.CreateMovie(filmName, year, path, filmGroup);
        }

        public MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey)
        {
            return _context.CreateMovieGenre(movieId, genreCompKey, subGenreCompKey);
        }

        public Series? CreateSeries(string newSeriesName)
        {
            return _context.CreateSeries(newSeriesName);
        }

        public bool DeleteFrameSet(FrameSet frameSet)
        {
            _context.FrameSet.Remove(frameSet);
            return Save();
        }
        
        

        public bool DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            bool success = false;
            if (movie != null)
            {
                _context.Entry(movie).State = EntityState.Detached;
                success = _context.DeleteMovie(id);
            }

            return success;
        }

        public bool DeleteMovieImage(MovieImage movieImage)
        {
            _context.MovieImage.Remove(movieImage);
            return Save();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public List<MovieIntResult> GetActorMovieIds(string actorName)
        {
            return _context.GetActorMovieIds(actorName);
        }

        public FrameSet? GetFrameSetById(int Id)
        {
            return _context.FrameSet.Find(Id);
        }

        public FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId)
        {
            return _context.FrameSetHeader.FirstOrDefault(f => f.MovieImageId == movieImageId);
        }

        public List<FrameSet>? GetFrameSetsByHeaderId(int frameSetHeaderId)
        {
            return _context.FrameSet.
                Where(f => f.FrameSetHeaderId == frameSetHeaderId).
                OrderBy(f => f.StartImage).
                ToList();
        }

        public MovieImage? GetMovieImageById(int? lastId)
        {
            if (lastId != null)
                return _context.MovieImage.Find(lastId);
            else return null;
        }

        public List<MovieImage>? GetMovieImagesByFolder(string v)
        {
            return _context.MovieImage.Where(i => i.FolderType == v).ToList();
        }

        public List<MovieImage>? GetMovieImagesById(int id)
        {
            return _context.MovieImage.Where(i => i.ParentId == id).OrderBy(x => x.Name).ToList();
        }

        public List<Movies>? GetMoviesByActor(int id)
        {
            List<Movies> tempList = new List<Movies>();

            List<Cast> actorCasts = _context.Casts.Where(c => c.ActorId == id).ToList();

            List<int?> actorMovieIds = actorCasts.Select(c => c.MovieID).ToList();

            var result = from x in _context.Movies
                         where actorMovieIds.Contains(x.Id)
                         select x;
            // should be a list of movies
            tempList = result.ToList();
            foreach (Movies movie in tempList)
            {
                movie.Loaded = true;
            }
            result = null;
            return tempList;
        }

        public List<Movies>? GetMoviesbyBookmarkName(string bookmarkText)
        {
            List<Movies>? tempList = _context.GetMoviesbyBookmarkName(bookmarkText);
            return tempList;
        }

        public List<Movies>? GetMoviesByDirector(int id)
        {
            return _context.Movies.Where(d => d.DirectorID == id)

                .ToList();
        }

        public List<Movies>? GetMoviesByGenre(string? genre, string? subGenre = "")
        {
            List<Movies>? returnValue = null;

            if (string.IsNullOrEmpty(genre)) return returnValue;

            if (!string.IsNullOrEmpty(subGenre))
            {
                returnValue = _context.Movies.FromSql($"Execute dbo.GetMoviesBySubGenre {subGenre}").ToList();
            }
            else
                returnValue = _context.Movies.FromSql($"Execute dbo.GetMoviesByGenre {genre}").ToList();

            return returnValue;
        }

        public async Task<List<Movies>>? GetMoviesByGenreAsync(string? genre, string? subGenre = "")
        {
            await using (var context = new SandboxEntities())
            {
                List<Movies>? returnValue = null;

                if (string.IsNullOrEmpty(genre)) return returnValue;

                if (!string.IsNullOrEmpty(subGenre))
                {
                    returnValue = await _context.Movies.FromSql($"Execute dbo.GetMoviesBySubGenre {subGenre}").ToListAsync();
                }
                else
                    returnValue = await _context.Movies.FromSql($"Execute dbo.GetMoviesByGenre {genre}").ToListAsync();

                return returnValue;
            }
        }

        public Movies? GetMoviesById(int id)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.Movies.Find(id)
                                ;
        }

        public List<Movies>? GetMoviesByInfo(string stub)
        {
            return _context.GetMoviesbyInfo(stub)
                                ;
        }

        public List<Movies>? GetMoviesBySeason(int id)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            List<Movies>? tempList = _context.Movies.Where(m => m.Season == id)
                .Include(x => x.Director)
                .Include(x => x.MovieGenres)
                .Include(x => x.Bookmarks)
                .Include(x => x.Casts).ToList();

            return tempList;
        }

        public List<Movies>? GetMoviesByTitle(string title)
        {
            return _context.GetMoviesbyTitle(title);
        }

        public async Task<List<Movies>>? GetMoviesByTitleAsync(string title)
        {
            List<Movies>? tempList = await _context.GetMoviesByTitleAsync(title);
            return tempList;
        }

        public bool InsertFrameSetHeader(FrameSetHeader frameSetHeader)
        {
            _context.FrameSetHeader.Add(frameSetHeader);
            return _context.SaveChanges() > 0;
        }

        public bool Save()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
                //throw;
            }
        }

        public bool Save(Movies movie)
        {
            return UpdateMovie(movie);
            //Movies movieTemp = _context.Movies.Find(movie.Id);
            //if (movieTemp != null)
            //{
            //    _context.Movies.Remove(movieTemp);
            //}
            //_context.Movies.Add(movie);
            //return _context.SaveChanges() > 0;
        }

        public bool SaveMovieImage(MovieImage movieImage)
        {
            MovieImage image = _context.MovieImage.Find(movieImage.Id);
            if (image != null)
            {
                image.LastId = movieImage.LastId;
                image.Name = movieImage.Name;
            }
            _context.MovieImage.Update(image);
            return Save();
        }

        public bool UpdateFrameSet(FrameSet frameSet)
        {
            _context.FrameSet.Update(frameSet);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader)
        {
            _context.FrameSetHeader.Update(frameSetHeader);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateMovie(Movies movie)
        {
            //_context.Movies.Update(movie);
            _context.Entry(movie).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateMovieAsync(Movies movie)
        {
            // _context.Movies.Update(movie);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion Public Methods

        #region Protected Methods

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

        #endregion Protected Methods
    }
}