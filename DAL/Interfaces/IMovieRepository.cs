using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Controllers;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMovieRepository : IDisposable
    {

        #region Public Methods

        bool Add(Movies movie);

        bool AddFrameSet(FrameSet frameSet);

        bool AddMovieImage(MovieImage movieImage);
        Season? AddSeason(Season season);
        TVEpisode? AddTVEpisode(TVEpisode tVEpisode);
        Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "");

        EntityState? GetTVEpisodeEntityState(TVEpisode episode);

        MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey);

        Series? CreateSeries(string newSeriesName);

        bool DeleteFrameSet(FrameSet frameSet);

        bool DeleteMovie(int id);

        bool DeleteMovieImage(MovieImage movieImage);
        bool DeleteSeason(Season season);
        bool DeleteTVEpisode(TVEpisode tVEpisode);
        List<MovieIntResult> GetActorMovieIds(string actorName);

        FrameSet? GetFrameSetById(int Id);

        FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId);

        IEnumerable<FrameSet>? GetFrameSetsByHeaderId(int frameSetHeaderId);
        MovieImage? GetMovieImageById(int? lastId);

        List<MovieImage>? GetMovieImagesByFolder(string v);

        IEnumerable<MovieImage>? GetMovieImagesById(int id);

        IEnumerable<Movies>? GetMoviesByActor(int id);

        IEnumerable<Movies>? GetMoviesbyBookmarkName(string bookmarkText);

        IEnumerable<Movies>? GetMoviesByDirector(int id);

        IEnumerable<Movies>? GetMoviesByGenre(string? genre,
            string? subGenre = "");

        Movies? GetMoviesById(int id);

        IEnumerable<Movies>? GetMoviesByInfo(string stub);

        IEnumerable<Movies>? GetMoviesByTitle(string title);

        Task<List<Movies>> GetMoviesByTitleAsync(string title);

        List<Series> GetSeriesList();
        List<TVEpisode>? GetTVEpisodesBySeasonID(int id);

        List<Season>? GetSeasonsBySeriesID(int id);

        bool InsertFrameSetHeader(FrameSetHeader frameSetHeader);

        bool Save();

        bool Save(Movies movie);

        bool SaveMovieImage(MovieImage movieImage);

        bool UpdateFrameSet(FrameSet frameSet);

        bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader);

        bool UpdateMovie(Movies movie);

        Task<bool> UpdateMovieAsync(Movies movie);
        bool UpdateSeason(Season season);
        bool UpdateSeries(Series series);
        bool UpdateTVEpisode(TVEpisode tVEpisode);
        Series? GetSeriesById(int? id);
        Season GetSeasonById(int? season);
        TVEpisode GetTVEpisodeById(int? episode);
        IEnumerable<Movies> GetMoviesBySeason(int id);

        #endregion Public Methods

    }
}