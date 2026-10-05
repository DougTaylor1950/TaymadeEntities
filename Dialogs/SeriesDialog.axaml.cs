//-----------------------------------------------------------------------
// <copyright file="SeriesDialog.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>28/04/2022 14:56:11 28/04/2022 14:56:11 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Dialogs
{
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Models;
    using OpenXmlPowerTools;
    using ReactiveUI.Avalonia;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using TaymadeEntities.Support;
    using TaymadeEntities.ViewModels;

    /// <summary>
    /// Defines the <see cref="SeriesDialog" />.
    /// </summary>
    public partial class SeriesDialog : ReactiveWindow<SeriesViewModel>, IDisposable
    {

        #region Private Fields

        private bool disposedValue;

        /// <summary>
        /// Defines the initialise.
        /// </summary>
        private bool initialise = true;

        /// <summary>
        /// Defines the model.
        /// </summary>
        private ViewModels.MainWindowViewModel? model;

        /// <summary>
        /// Defines the sd.
        /// </summary>
        private UserControl? sd;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesDialog"/> class.
        /// </summary>
        public SeriesDialog()
        {
            InitializeComponent();

            Opened += SeriesDialog_Opened;
        }

        public SeriesDialog(SeriesViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            SeriesViewModel = viewModel;

            Opened += SeriesDialog_Opened;
        }

        #endregion Public Constructors

        #region Public Properties

        public SeriesViewModel? SeriesViewModel { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// The CreateSeason.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void CreateSeason(object sender, RoutedEventArgs e)
        {
        }

        private async void EditMovie_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            using MovieEditViewModel? vm = new MovieEditViewModel(ViewModel.CurrentSeasonMovie);
            using TaymadeEntities.Dialogs.EditMovie editor = new TaymadeEntities.Dialogs.EditMovie(vm);
            //Window main = Support.GetWindow();
            //Dialogs.DialogResultButton result;

            bool result = await editor.ShowDialog<bool>(this);

            if (result)
            {
                // save movie
                Movies? movie = vm.CurrentMovie;
                if (movie != null)
                {
                    if (movie.SeriesEntity != null)
                    {
                        if (movie.Series == null)
                        {
                            movie.Series = movie.SeriesEntity.Id;
                        }
                    }

                    if (movie.Director != null)
                    {
                        if (movie.DirectorID == null) movie.DirectorID = movie.Director.Id;
                    }

                    movie.MoviePath = Support.FixPathBack(movie.MoviePath);

                    movie.Save();

                    if (ViewModel.CurrentSeries != null)
                    {
                        ViewModel.CurrentSeries.Update();
                    }
                }
            }
        }

        /// <summary>
        /// The EpisodeRowChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        private void EpisodeRowChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                DataGrid? dg = sender as DataGrid;

                //MainWindowViewModel? mvm = Support.Support.GetMainWindowViewModel();

                if (!initialise && dg != null && dg.SelectedItem != null)
                {
                    Models.TVEpisode? episode = dg.SelectedItem as Models.TVEpisode;

                    SeriesViewModel.EpisodeEntity = episode;

                    if (episode.MovieId == null)
                    {
                        Movies? movie = DataController.MovieList.Where(e => e.Episode == episode.EpisodeNumber && e.Season == episode.SeasonNumber).FirstOrDefault();

                        if (movie != null)
                        {
                            episode.MovieId = movie.Id;
                            episode.Save();
                        }
                    }

                    //EpisodeEntity = episode;
                    if (episode != null && episode.MovieId != null && episode.MovieId.Value > 1)
                    {
                        if (episode.Movie == null)
                        {
                            episode.Movie = DataController.SandboxEntities.Movies.Find(episode.MovieId.Value);
                        }
                        SeriesViewModel.ShowVisible = true;
                    }
                    else
                        SeriesViewModel.ShowVisible = false;

                    // setup play
                    if (episode != null && episode.Movie != null)
                    {
                        //MainWindowViewModel mvm = Support.Support.GetMainWindowViewModel();
                        if (SeriesViewModel != null)
                        {
                            SeriesViewModel.CurrentMissingMovie = new MissingFile() { Path = episode.Movie.MoviePath };
                        }
                    }
                }
            }
        }

        private void GetEpisodes(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void GetSeasons(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void PlayEpisodeMovie(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            if (ViewModel.CurrentSeasonMovie == null) return;
            ViewModel?.DoPlay(ViewModel?.CurrentSeasonMovie, null, false, "", false, true);
        }

        /// <summary>
        /// The RowChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        private void RowChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                DataGrid? dg = sender as DataGrid;

                //SeriesViewModel = DataContext as ViewModels.MainWindowViewModel;

                if (!initialise && dg != null && dg.SelectedItem != null && SeriesViewModel != null)
                {
                    Models.Series? series = dg.SelectedItem as Models.Series;

                    SeriesViewModel.CurrentSeries = series;

                    if (series != null)

                        if (series.Seasons == null)
                        {
                            List<Models.Season> seasonList = Models.DataController.SandboxEntities.Seasons.ToList();
                            series.Seasons = new ObservableCollection<Models.Season>(seasonList.Where(s => s.Series == series.Id).OrderBy(s => s.SeasonNo).ToList());
                            //Model.SeasonList = seasonList.Where(s => s.Series == series.Id).ToList();
                        }
                }
                initialise = false;
            }
        }

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
        /// <summary>
        /// The SeasonRowChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        private void SeasonRowChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                DataGrid? dg = sender as DataGrid;

                if (!initialise && dg != null && dg.SelectedItem != null && SeriesViewModel != null)
                {
                    Models.Season? season = dg.SelectedItem as Models.Season;

                    SeriesViewModel.CurrentSeason = season;

                    try
                    {
                        if (season != null)
                        {
                            if (season.TVEpisodes == null)
                            {
                                season.TVEpisodes = new ObservableCollection<TVEpisode>(Models.DataController.SandboxEntities.TVEpisodes.Where(s => s.SeasonID == season.Id).OrderBy(s => s.EpisodeNumber).ToList());
                            }

                            if (season.TVEpisodes != null && season.TVEpisodes.Count == 0)
                            {
                                season.TVEpisodes = new ObservableCollection<TVEpisode>(Models.DataController.SandboxEntities.TVEpisodes.Where(s => s.SeasonID == season.Id).OrderBy(s => s.EpisodeNumber).ToList());
                            }

                            if (season.TVEpisodes != null)
                                SeriesViewModel.EpisodeList = new System.Collections.ObjectModel.ObservableCollection<TVEpisode>(season.TVEpisodes.ToList());
                        }
                    }
                    catch (System.Exception)
                    {
                        //throw;
                    }
                }
            }
        }

        /// <summary>
        /// The SeriesDialog_Opened.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="System.EventArgs"/>.</param>
        private void SeriesDialog_Opened(object? sender, System.EventArgs e)
        {
            DataGrid dg = this.Find<DataGrid>("dgSeries");
            if (dg != null)
            {
                // DataContext = Support.Support.GetMainWindowViewModel();

                if (this.DataContext != null)
                {
                    SeriesViewModel = this.DataContext as SeriesViewModel;
                }

                if (SeriesViewModel != null)
                {
                    dg.ItemsSource = SeriesViewModel.SeriesList;
                    SeriesViewModel.ByMovie = false;
                }
            }
        }

        private void SeriesDialog_Opened1(object? sender, System.EventArgs e)
        {
            //if (Screens.ScreenCount > 1 && DataController.ShowOnAlternateScreen())
            //{
            //    int screenWidth = (int)this.Width;
            //    this.Position = new PixelPoint(-screenWidth, 50);
            //}
            this.WindowState = WindowState.Maximized;
            //Series.DataContext = this.DataContext;
        }

        private void Accept_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                this.Close(true);
            });
            
        }

        #endregion Private Methods

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SeriesDialog()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}