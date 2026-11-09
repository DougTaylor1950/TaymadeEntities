using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.ViewModels
{
    public class MovieListViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<Movies>? movieList;
        private Movies? localMovie;
        private PhraseEntry? currentPhrase;
        private PhraseEntry? currentSubPhrase;
        private bool disposedValue;

        public  ObservableCollection<Movies>? MovieList
        {
            get => movieList;
            set => this.RaiseAndSetIfChanged(ref movieList, value);
        }


        public  Movies? CurrentMovie
        {
            get => localMovie;
            set
            {
                //if (!Initialise)
                //{
                this.RaiseAndSetIfChanged(ref localMovie, value);
                this.RaisePropertyChanged(nameof(CurrentMovie));

                //}
            }
        }

        public bool EditVisible { get; set; }

        public List<PhraseEntry>? Phrases { get; set; }
        public List<PhraseEntry>? SubPhrases { get; set; }
        public PhraseEntry? CurrentPhrase { get; set; }
        public PhraseEntry? NewPhrase { get; set; }
        public PhraseEntry? CurrentSubPhrase { get; set; }
        public PhraseEntry? NewSubPhrase { get; set; }
        public Bookmark? CurrentBookmark { get; internal set; }
        public bool FirstMovieSelected { get; internal set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var movie in MovieList)
                    {
                        movie.Dispose();
                    }
                    movieList?.Clear();
                    movieList = null;
                    // TODO: dispose managed state (managed objects)
                }
                Phrases?.Clear();
                SubPhrases?.Clear();
                CurrentMovie?.Dispose();
               // CurrentMovie = null;

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieListViewModel()
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

        internal void DoFindMoviesByBookmark()
        {
            throw new NotImplementedException();
        }

        internal void FindMovie(bool v)
        {
            throw new NotImplementedException();
        }
    }
}