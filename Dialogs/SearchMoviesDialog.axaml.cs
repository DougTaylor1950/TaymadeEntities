using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using System.Threading.Tasks;

namespace TaymadeEntities.Dialogs;

public partial class SearchMoviesDialog : Window, IDisposable
{
    private bool disposedValue;

    public ViewModels.MainWindowViewModel ViewModel { get; internal set; }

    //private MainWindowViewModel viewModel;

    // public MainWindowViewModel ViewModel { get => viewModel; set => viewModel = value; }

    public SearchMoviesDialog()
    {
        InitializeComponent();

        DataContextChanged += this.SearchMoviesDialog_DataContextChanged;

        this.Closed += SearchMoviesDialog_Closed;
    }

    public SearchMoviesDialog(SearchViewModel searchViewModel)
    {
        InitializeComponent();

        DataContext = searchViewModel;

        this.Closed += SearchMoviesDialog_Closed;
    }

    private void SearchMoviesDialog_Closed(object? sender, EventArgs e)
    {
        //if (DataContext is IDisposable disposable)
        //{
        //    disposable.Dispose();
        //}
        //DataContext = null;
    }

    private void SearchMoviesDialog_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext != null)
        {

        }
    }

    private async void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SearchViewModel? searchViewModel = DataContext as SearchViewModel;
        Models.SearchModelInfo info = new Models.SearchModelInfo()
        {
            Title = searchViewModel.MovieTitle,
            ActorName = searchViewModel.ActorName,
            BookmarkText = searchViewModel?.BookmarkText,
            MovieId = searchViewModel?.MovieId,
            InfoText = searchViewModel?.InfoText
        };

        info.MovieList = await GetList(info);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
         {
             this.Close(info);
         });

        //this.Dispose();
    }
    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(null);
        this.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.DataContext = null;
                //this.ViewModel = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SearchMoviesDialog()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    private async Task<List<Movies>> GetList(Models.SearchModelInfo info)
    {
        List<Movies>? tempList = new List<Movies>();
        if (info.MovieId != null)
        {
            Movies? temp = DataController.MovieController.GetMoviesById(info.MovieId.Value);
            if (temp != null)
            {
                tempList.Add(temp);
            }
        }
        else
            if (!string.IsNullOrEmpty(info.Title))
            {
                // see if we can find some movies
                tempList = await DataController.MovieController.GetMoviesByTitleAsync(info.Title);
            }

        // look for actors
        if (!string.IsNullOrEmpty(info.ActorName))
        {
            // two possible situations we have a list already or we don't
            List<MovieIntResult> ActorMovies = DataController.MovieController.GetActorMovieIds(info.ActorName);
            // this should have returned a list of names

            // Extract the Ids from ActorMovies for comparison
            var actorMovieIds = MovieIntResult.GetMovieIds(ActorMovies);

            if (tempList == null || tempList.Count == 0)
            {
                var result = from x in DataController.SandboxEntities.Movies
                             where actorMovieIds.Contains(x.Id)
                             select x;
                // should be a list of movies
                tempList = result.ToList();
                info.MovieList = tempList;
                result = null;
            }
            else
            {
                var result = from x in tempList
                             where actorMovieIds.Contains(x.Id)
                             select x;
                // should be a list of movies
                tempList = result.ToList();
                result = null;
            }
        }

        // need to add bookmark text and with info
        // yet to decide whether should be cumulative or standalone
        // leaning towards standalone

        if (!string.IsNullOrEmpty(info.BookmarkText))
        {
            tempList = DataController.MovieController.GetMoviesbyBookmarkName(info.BookmarkText);
        }

        if (!string.IsNullOrEmpty(info.InfoText))
        {
            tempList = DataController.MovieController.GetMoviesByInfo(info.InfoText);
        }

        //if (info.CurrentSeries != null)
        //{
        //    tempList = DataController.SandboxEntities.Movies.Where(m => m.Series == info.CurrentSeries.Id).ToList();
        //}
        if (tempList != null)
        {
            if (DataContext is SearchViewModel ViewModel)
                ViewModel.MovieList = new ObservableCollection<Movies>(tempList);
            //ViewModel = null;
        }

        return tempList;
    }



    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}