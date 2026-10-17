using Microsoft.EntityFrameworkCore;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface ISeriesRepository : IDisposable
    {
        List<Series> GetSeriesList();
        //bool UpdateSeries(Series series);

        Series? GetSeriesById(int? id);

        Series? CreateSeries(string newSeriesName);
        bool DeleteSeason(Season season);
        bool DeleteTVEpisode(TVEpisode tVEpisode);
        Season? AddSeason(Season season);
        TVEpisode? AddTVEpisode(TVEpisode tVEpisode);
        List<TVEpisode>? GetTVEpisodesBySeasonID(int id);

        EntityState? GetTVEpisodeEntityState(TVEpisode episode);
        bool UpdateSeason(Season season);
        bool UpdateSeries(Series series);
        bool UpdateTVEpisode(TVEpisode tVEpisode);

        TVEpisode? GetTVEpisodeById(int? episode);
       

        List<Season>? GetSeasonsBySeriesID(int id);
        
        Season? GetSeasonById(int? season);
    }
}
