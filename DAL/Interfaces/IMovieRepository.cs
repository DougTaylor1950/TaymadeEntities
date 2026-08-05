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

        bool AddFrameSet(FrameSet frameSet);

        Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "");

        MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey);

        bool DeleteMovie(int id);

        bool DeleteMovieImage(MovieImage movieImage);

        List<MovieIntResult> GetActorMovieIds(string actorName);

        IEnumerable<FrameSet>? GetFrameSetsByHeaderId(int frameSetHeaderId);

        FrameSet? GetFrameSetById(int Id);

        FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId);

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

        bool InsertFrameSetHeader(FrameSetHeader frameSetHeader);

        bool Save();

        bool Save(Movies movie);

        bool SaveMovieImage(MovieImage movieImage);

        bool UpdateFrameSet(FrameSet frameSet);

        bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader);

        bool UpdateMovie(Movies movie);

        Task<bool> UpdateMovieAsync(Movies movie);
        bool DeleteFrameSet(FrameSet frameSet);

        #endregion Public Methods
    }
}