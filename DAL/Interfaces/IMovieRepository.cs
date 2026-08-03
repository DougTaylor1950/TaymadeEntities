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

        bool AddMovieImage(MovieImage movieImage);

        Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "");

        MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey);

        bool DeleteMovie(int id);

        bool DeleteMovieImage(MovieImage movieImage);

        List<MovieIntResult> GetActorMovieIds(string actorName);

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

        Task<IEnumerable<Movies>> GetMoviesByTitleAsync(string title);

        bool Save();

        bool Save(Movies movie);

        bool SaveMovieImage(MovieImage movieImage);

        bool UpdateMovie(Movies movie);

        Task<bool> UpdateMovieAsync(Movies movie);

        FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId);
        bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader);

        bool InsertFrameSetHeader(FrameSetHeader frameSetHeader);
        #endregion Public Methods
    }
}