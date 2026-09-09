using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.ViewModels
{
    public class MovieSearchViewModel : ReactiveObject, IDisposable
    {
        #region Private Fields

        private Movies? currentMovie;

        /// <summary>
        /// Defines the FoundMovies.
        /// </summary>
        private IEnumerable<MovieBase>? foundMovies;
        private string? movieTitle;
        private MovieBase? selectedTMDBItem;
        private string? year;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public MovieSearchViewModel()
        {
        }

        public MovieSearchViewModel(Movies searchMovie)
        {
            this.CurrentMovie = searchMovie;
            SearchTMDP = ReactiveCommand.Create(SearchDatabase);
        }

        #endregion Public Constructors

        #region Public Properties

        public ReactiveCommand<Unit, Unit>? SearchTMDP { get; set; }
        public ReactiveCommand<Unit, Unit>? SearchTMDPOnly { get; set; }

        public Movies? CurrentMovie
        {
            get => currentMovie;
            set
            {
                this.RaiseAndSetIfChanged(ref currentMovie, value);
                // if the value is not null set the movie title field
                if (value != null)
                {
                    MovieTitle = value.MovieName;
                    year = value.Year?.ToString();
                }
            }
        }

        public IEnumerable<MovieBase>? FoundMovies
        {
            get => foundMovies;
            set => this.RaiseAndSetIfChanged(ref foundMovies, value);
        }

        public string? MovieTitle
        {
            get => movieTitle;
            set => this.RaiseAndSetIfChanged(ref movieTitle, value);
        }

        public MovieBase? CurrentItem
        {
            get => selectedTMDBItem;
            set => this.RaiseAndSetIfChanged(ref selectedTMDBItem, value);
        }

        public string? Year
        {
            get => year;
            set => this.RaiseAndSetIfChanged(ref year, value);
        }

        #endregion Public Properties

        private void SearchDatabase()
        {
            if (!string.IsNullOrEmpty(MovieTitle))
            {
                string? searchText = this.MovieTitle;

                FoundMovies = TmdbSupport.SearchMovieDatabaseList(searchText);
            }
        }

        private void SearchDatabaseOnly()
        {
            
            if (!string.IsNullOrEmpty(MovieTitle))
            {
                string searchText = MovieTitle;

                FoundMovies = TmdbSupport.SearchMovieDatabaseList(searchText);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //// TODO: dispose managed state (managed objects)
                    //this.CurrentMovie?.Dispose();
                    //this.CurrentMovie = null;
                    this.CurrentItem = null;
                    this.FoundMovies = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieSearchViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        //ValueTask IAsyncDisposable.DisposeAsync()
        //{
        //    Dispose();
        //    return ValueTask.CompletedTask;
        //}
    }
}