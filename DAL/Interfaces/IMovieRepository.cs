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
        Movies? CreateMovie(string filmName, int year = 0, string path = "", string filmGroup = "");


        MovieGenre? CreateMovieGenre(int movieId, string? genreCompKey, string? subGenreCompKey);


        bool DeleteFrameSet(FrameSet frameSet);

        

        bool DeleteMovie(int id);

        bool DeleteMovieImage(MovieImage movieImage);
       
        List<MovieIntResult> GetActorMovieIds(string actorName);

        

        FrameSet? GetFrameSetById(int Id);

        FrameSetHeader? GetFrameSetHeaderByMovieImageId(int movieImageId);

        List<FrameSet>? GetFrameSetsByHeaderId(int frameSetHeaderId);
        MovieImage? GetMovieImageById(int? lastId);

        List<MovieImage>? GetMovieImagesByFolder(string v);

        List<MovieImage>? GetMovieImagesById(int id);

        List<Movies>? GetMoviesByActor(int id);

        List<Movies>? GetMoviesbyBookmarkName(string bookmarkText);

        List<Movies>? GetMoviesByDirector(int id);

        List<Movies>? GetMoviesByGenre(string? genre,
            string? subGenre = "");

        Task<List<Movies>>? GetMoviesByGenreAsync(string? genre, string? subGenre = "");

        Movies? GetMoviesById(int id);

        List<Movies>? GetMoviesByInfo(string stub);

        List<Movies>? GetMoviesByTitle(string title);

        Task<List<Movies>> GetMoviesByTitleAsync(string title);


        bool InsertFrameSetHeader(FrameSetHeader frameSetHeader);

        bool Save();

        bool Save(Movies movie);

        bool SaveMovieImage(MovieImage movieImage);

       

        bool UpdateFrameSet(FrameSet frameSet);

        bool UpdateFrameSetHeader(FrameSetHeader frameSetHeader);

        bool UpdateMovie(Movies movie);

        Task<bool> UpdateMovieAsync(Movies movie);

        List<Movies>? GetMoviesBySeason(int id);
        

        #endregion Public Methods

    }
}