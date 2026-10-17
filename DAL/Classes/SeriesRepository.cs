using Microsoft.EntityFrameworkCore;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Classes
{
    public class SeriesRepository : ISeriesRepository, IDisposable
    {
        #region Private Fields

        private bool disposedValue;

        #endregion Private Fields

        #region Public Methods

        public Season? AddSeason(Season season)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            _context.Seasons.Add(season);
            if (_context.SaveChanges() > 0)
                return season;
            else
                return null;
        }

        public TVEpisode? AddTVEpisode(TVEpisode tVEpisode)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            if (tVEpisode == null)
                return null;
            if (tVEpisode.Id > 2)
            {
                UpdateTVEpisode(tVEpisode);
                return null;
            }
            _context.TVEpisodes.Add(tVEpisode);
            _context.Entry(tVEpisode).State = EntityState.Added;
            if (_context.SaveChanges() > 0)
                return tVEpisode;
            else
                return null;
        }

        public Series? CreateSeries(string newSeriesName)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.CreateSeries(newSeriesName);
        }

        public bool DeleteSeason(Season season)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            _context.Seasons.Remove(season);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteTVEpisode(TVEpisode tVEpisode)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            _context.TVEpisodes.Remove(tVEpisode);
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Season? GetSeasonById(int? season)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.Seasons.Find(season);
        }

        public List<Season>? GetSeasonsBySeriesID(int id)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.Seasons.Where(s => s.Id == id).OrderBy(s => s.SeasonNo).ToList();
        }

        public Series? GetSeriesById(int? id)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();

            if (id == null) return null;
            return _context.Series.Find(id);
        }

        public List<Series> GetSeriesList()
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.Series.OrderBy(s => s.Name).AsNoTracking().ToList();
        }

        public TVEpisode? GetTVEpisodeById(int? episode)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.TVEpisodes.Find(episode);
        }

        public EntityState? GetTVEpisodeEntityState(TVEpisode episode)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.Entry(episode).State;
        }
        public List<TVEpisode>? GetTVEpisodesBySeasonID(int id)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            return _context.TVEpisodes.Where(t => t.SeasonID == id).OrderBy(t => t.EpisodeNumber).ToList();
        }
        public bool UpdateSeason(Season season)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            _context.Seasons.Update(season);
            return _context.SaveChanges() > 0;
        }

        //    return Save();
        //}
        public bool UpdateSeries(Series series)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();

            try
            {
                _context.Series.Update(series);
            }
            catch (Exception)
            {
                return false;
            }
            return _context.SaveChanges() > 0;
        }

        public bool UpdateTVEpisode(TVEpisode tVEpisode)
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            _context.Entry(tVEpisode).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        #endregion Public Methods

        #region Protected Methods

        //public bool UpdateSeries(Series series)
        //{
        //    try
        //    {
        //        using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
        //        _context.Series.Update(series);
        //        return _context.SaveChanges() > 0;
        //    }
        //    catch (Exception)
        //    {
        //    }
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SeriesRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}