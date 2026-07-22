using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Controllers;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMovieRepository:IDisposable
    {
        bool Save();

        bool Save(Movies movie);

        bool UpdateMovie(Movies movie);

        Task<bool> UpdateMovieAsync(Movies movie);

        bool DeleteMovie(int id);

        Movies? GetMoviesById(int id);
        IEnumerable<Movies>? GetMoviesByGenre(string? genre,
            string? subGenre = "");
        bool Add(Movies movie);
        IEnumerable<Movies>? GetMoviesByDirector( int id);
        IEnumerable<Movies>? GetMoviesByActor(int id);
        IEnumerable<Movies>? GetMoviesByInfo(string stub);
        IEnumerable<Movies>? GetMoviesByTitle(string title);
        Task<IEnumerable<Movies>> GetMoviesByTitleAsync(string title);
        IEnumerable<Movies>? GetMoviesbyBookmarkName(string bookmarkText);
        List<MovieIntResult> GetActorMovieIds(string actorName);
        bool SaveMovieImage(MovieImage movieImage);
        IEnumerable<MovieImage>? GetMovieImagesById(int id);
        bool AddMovieImage(MovieImage movieImage);
        bool DeleteMovieImage(MovieImage movieImage);
        List<MovieImage>? GetMovieImagesByFolder(string v);
        MovieImage? GetMovieImageById(int? lastId);
    }
}
