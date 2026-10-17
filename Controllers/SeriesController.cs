using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.ObjectModel;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class SeriesController : IDisposable
    {
        #region Private Fields

        private bool disposedValue;
        private ISeriesRepository seriesRepository;

        #endregion Private Fields

        #region Public Constructors

        public SeriesController()
        {
            this.seriesRepository = new SeriesRepository();
        }

        public SeriesController(SeriesRepository seriesRepository)
        {
            this.seriesRepository = seriesRepository;
        }

        #endregion Public Constructors

        #region Public Methods

        public Season? AddSeason(Season season)
        {
            return seriesRepository.AddSeason(season);
        }

        public TVEpisode? AddTVEpisode(TVEpisode tVEpisode)
        {
            return seriesRepository.AddTVEpisode(tVEpisode);
        }

        public bool DeleteSeason(Season season)
        {
            return seriesRepository.DeleteSeason(season);
        }

        public bool DeleteTVEpisode(TVEpisode tVEpisode)
        {
            return seriesRepository.DeleteTVEpisode(tVEpisode);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Season? GetSeasonById(int? season)
        {
            return seriesRepository.GetSeasonById(season);
        }

        public ObservableCollection<Season>? GetSeasonsBySeriesID(int id)
        {
            List<Season>? temp = seriesRepository.GetSeasonsBySeriesID(id)?.ToList();
            return new ObservableCollection<Season>(temp ?? new List<Season>());
        }

        public TVEpisode? GetTVEpisodeById(int? episode)
        {
            return seriesRepository.GetTVEpisodeById(episode);
        }

        public EntityState? GetTVEpisodeEntityState(TVEpisode episode)
        {
            return seriesRepository.GetTVEpisodeEntityState(episode);
        }

        public ObservableCollection<TVEpisode>? GetTVEpisodesBySeasonID(int id)
        {
            List<TVEpisode>? temp = seriesRepository.GetTVEpisodesBySeasonID(id)?.ToList();
            return new ObservableCollection<TVEpisode>(temp ?? new List<TVEpisode>());
        }

        public bool UpdateSeason(Season season)
        {
            return seriesRepository.UpdateSeason(season);
        }

        public bool UpdateTVEpisode(TVEpisode tVEpisode)
        {
            return seriesRepository.UpdateTVEpisode(tVEpisode);
        }

        #endregion Public Methods

        #region Internal Methods

        public Series? CreateSeries(string newSeriesName)
        {
            return this.seriesRepository.CreateSeries(newSeriesName);
        }

        public Series? GetSeriesById(int? value)
        {
            return this.seriesRepository.GetSeriesById(value);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SeriesController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public List<Series> GetSeriesList()
        {
            return this.seriesRepository.GetSeriesList().ToList();
        }

        #endregion Internal Methods

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