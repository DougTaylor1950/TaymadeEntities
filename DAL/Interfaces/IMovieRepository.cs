using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMovieRepository:IDisposable
    {
        bool Save();

        bool Save(Movies movie);

        bool UpdateMovie(Movies movie);
        void DeleteMovie(int id);

        Movies? GetMoviesById(int id);


    }
}
