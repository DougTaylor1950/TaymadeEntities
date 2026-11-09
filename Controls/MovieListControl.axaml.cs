//-----------------------------------------------------------------------
// <copyright file="MovieListControl.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>25/04/2022 11:57:13 25/04/2022 11:57:13 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Controls
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Avalonia.Media.TextFormatting;
    using TaymadeEntities.Models;
    //using MovieDBViewer.Support;
    using TaymadeEntities.ViewModels;
    using DocumentFormat.OpenXml.Wordprocessing;
    using DynamicData;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using TaymadeControls;
    using TaymadeControls.Buttons;
    
    
    using TaymadeEntities.Support;
    
    using Avalonia.VisualTree;
    using Avalonia.Media.Imaging;

    /// <summary>
    /// Defines the <see cref="MovieListControl" />.
    /// </summary>
    public partial class MovieListControl : UserControl, INotifyPropertyChanged
    {

        #region Public Fields

        /// <summary>
        /// Defines the rowId.
        /// </summary>
        public int? rowId;

        #endregion Public Fields

        #region Internal Fields

        private ComboBox _SeasonsCombo;

        private ComboBox _SeriesCombo;

        internal ComboBox _subGroups;

        #endregion Internal Fields

        #region Private Fields

        private ComboBox _genresComboBox;

        private ContextMenu _genresComboContextMenu;

        private MenuItem _reloadMovies;
        private MenuItem _MoveToSelected;
        private bool building = true;

        private Series currentSeries;
        private ImagedButton _searchButton;
        private ImagedButtonNoText _endFindMovie;
        private ImagedButton _createMovie;
        private MovieListViewModel? MovieListViewModel;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieListControl"/> class.
        /// </summary>
        public MovieListControl()
        {
            InitializeComponent();

            this.DataContextChanged += MovieListControl_DataContextChanged2;

            LastMovieID = DataController.MovieProperties.LastMoveID;



            MovieListViewModel? viewModel = null;

            //this.DataContext = viewModel;
            if (this.DataContext != null && this.DataContext is MovieListViewModel)
            {
                viewModel = this.DataContext as MovieListViewModel;
                if (dgMovies != null)
                {
                    dgMovies.ItemsSource = viewModel.MovieList;
                    dgMovies.SelectedItem = viewModel.CurrentMovie;
                }
            }

            //if (dgMovies != null)
            //{
            //    dgMovies.Sorting += DataGrid_Sorting;
            //    dgMovies.LoadingRow += DataGrid_LoadingRow;
            //    //dgMovies.SelectionChanged += this.DgMovies_SelectionChanged;

            //    //viewModel.BoundGrid = dgMovies;
            //}
            DetachedFromVisualTree += OnDetachedFromVisualTree;
        }

        private void OnDetachedFromVisualTree(object? sender,
       Avalonia.VisualTreeAttachmentEventArgs e)
        {
            DisposeButtonImages(this);
        }

        private void MovieListControl_DataContextChanged2(object? sender, EventArgs e)
        {
            if (DataContext != null)
            {
                //if (DataContext is MovieListViewModel)
                //{
                //    MovieListViewModel = DataContext as MovieListViewModel;
                //    if (dgMovies != null)
                //    {
                //        dgMovies.ItemsSource = MovieListViewModel.MovieList;
                //        dgMovies.SelectedItem = MovieListViewModel.CurrentMovie;
                //    }
                //}
            }
        }

        private double windowMaxSize = 0;

        private void MovieListControl_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            // dgMovies height defined 310; 
            double newHeight = e.NewSize.Height;
            double oldHeight = e.PreviousSize.Height;
            Width = e.NewSize.Width;
            Height = e.NewSize.Height;
            //MainWindow? main = TopLevel.GetTopLevel(this) as MainWindow;
            //if (main != null)
            //{
            //    if (main.WindowState == WindowState.Maximized && windowMaxSize == 0)
            //    {
            //        windowMaxSize = main.Height;
            //    }
            //}

            //if (oldHeight == 0 || windowMaxSize == 0 )
            //{
            //   if (this.Height != 500) this.Height = 500;
            //} 
            //else if (newHeight != 500)
            //{
            //    double possiblenewHeight = 500 + (500 - ((windowMaxSize - newHeight)));

            //    if (possiblenewHeight > 0 && possiblenewHeight < 500)
            //        this.Height = possiblenewHeight;
            //    //else this.Height = 500;
            //}

            e.Handled = true;
        }

        private void MovieListControl_DataContextChanged1(object? sender, EventArgs e)
        {

        }



        #endregion Public Constructors

        //public event PropertyChangedEventHandler PropertyChanged;


        #region Public Properties

        /// <summary>
        /// Gets the LastMovieID.
        /// </summary>
        public int? LastMovieID { get; private set; }


        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// The SetUpLastMovie.
        /// </summary>
        /// <param name="viewModel">The viewModel<see cref="MovieListViewModel?"/>.</param>
        internal void SetUpLastMovie(MovieListViewModel? viewModel)
        {
            if (viewModel != null && viewModel.MovieList != null)
            {
                //viewModel.DoRefreshMovies();

                Movies? current = viewModel.MovieList.Where(x => x.Id == LastMovieID).FirstOrDefault();

                if (current != null)
                {
                    viewModel.CurrentMovie = current;
                    //viewModel.MovieList = new ObservableCollection<Movies>( DataController.MovieList.ToList());
                }

                if (viewModel != null && viewModel.CurrentMovie != null)
                {
                    if (viewModel.CurrentMovie.Bookmarks != null)
                    {
                        viewModel.CurrentBookmark = viewModel.CurrentMovie.Bookmarks.FirstOrDefault();
                    }

                    dgMovies.SelectedItem = viewModel.CurrentMovie;
                    viewModel.FirstMovieSelected = true;
                    //dgMovies.Focus();

                    //viewModel.MoveToCurrent();

                    //int pos = viewModel.MovieList.IndexOf(viewModel.CurrentMovie);
                    //if (pos >=0) viewModel.MoveToSelectedRow(pos);

                    //dgMovies.ScrollIntoView(viewModel.CurrentMovie, null);
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// The EditMovie.
        /// </summary>
        /// <param name="main">The main<see cref="MainWindow"/>.</param>
        /// <param name="model">The model<see cref="Movies"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        //private static async Task EditMovie(MainWindow main, Movies model)
        //{
        //    ViewModels.MovieListViewModel? mvm = new MovieViewModel(model);

        //    if (mvm != null)
        //    {
        //        Dialogs.MovieEditDialog editor = new Dialogs.MovieEditDialog(mvm);

        //        // assign movie to view model
        //        mvm.CurrentMovie = model;
        //        editor.DataContext = mvm;
        //        //Window main = GetWindow();
        //        mvm.Caller = editor;
        //        // Dialogs.DialogResultButton result;

        //        await editor.ShowDialog(main);

        //        if (
        //            mvm.resultButton != null
        //            && mvm.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok
        //        )
        //        {
        //            // save movie
        //            if (mvm.CurrentMovie != null)
        //            {
        //                if (mvm.CurrentMovie.SeriesEntity != null)
        //                {
        //                    if (mvm.CurrentMovie.Series == null)
        //                    {
        //                        mvm.CurrentMovie.Series = mvm.CurrentMovie.SeriesEntity.Id;
        //                    }
        //                }

        //                if (mvm.CurrentMovie.Director != null)
        //                {
        //                    if (mvm.CurrentMovie.DirectorID == null)
        //                        mvm.CurrentMovie.DirectorID = mvm.CurrentMovie.Director.Id;
        //                }

        //                mvm.CurrentMovie.MoviePath = Support.FixPathBack(mvm.CurrentMovie.MoviePath);

        //                mvm.CurrentMovie.Save();

        //                if (mvm.CurrentSeries != null)
        //                {
        //                    mvm.CurrentSeries.Save();
        //                }
        //            }
        //        }
        //    }
        //}

        private void Binding(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
        }

        private void Binding(object? sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 18/02/2026 18/02/2026 </created>
        /// </remarks>
        private void BuildToolbarContents()
        {
            //// Series ComboBox first
            //_genresComboBox = new ComboBox()
            //{
            //    Name = "Groups",
            //    Margin = new Avalonia.Thickness(5, 5, 5, 5)
            //};
            //_genresComboBox.SelectionChanged += ComboBox_SelectionChanged;
            //MovieListToolbar.Children.Add(_genresComboBox);
            //_genresComboBox.Classes.Add("c1");

            //_genresComboContextMenu = new ContextMenu();
            //_reloadMovies = new MenuItem()
            //{
            //    Header = "Reload List",
            //    Name = "ReloadGroups"
            //    //Command="{Binding ReloadGroups}"
            //};

            //_MoveToSelected = new MenuItem()
            //{
            //    Header = "Move To Selected Row",
            //    Name = "MoveToSelected"
            //};
            //_genresComboBox.ContextMenu = _genresComboContextMenu;
            //_genresComboContextMenu.Items.Add(_reloadMovies);
            //_genresComboContextMenu.Items.Add(_MoveToSelected);

            //// SubGenres Combo
            //_subGroups = new ComboBox()
            //{
            //    Name = "SubGroups",
            //    Margin = new Avalonia.Thickness(5, 5, 5, 5)
            //};
            //_subGroups.Initialized += ComboBox_Initialized;
            //_subGroups.SelectionChanged += ComboBox_SelectionChanged;
            //_subGroups.Classes.Add("c1");

            //MovieListToolbar.Children.Add(_subGroups);

            //_SeriesCombo = new ComboBox()
            //{
            //    IsVisible = false,
            //    Margin = new Thickness(5, 5, 5, 5),

            //};
            //_SeriesCombo.SelectionChanged += SeriesComboChanged;
            //MovieListToolbar.Children.Add(_SeriesCombo);


            //_SeasonsCombo = new ComboBox()
            //{
            //    IsVisible = false,
            //    Margin = new Thickness(5, 5, 5, 5)
            //};
            //MovieListToolbar.Children.Add(_SeasonsCombo);

            //// add a searchButton to be tied to a search dialog

            //_searchButton = new ImagedButton()
            //{
            //    Margin = new Thickness(5, 5, 5, 5),
            //    LabelText = "Search",
            //    HotKey = new KeyGesture(Key.F, KeyModifiers.Control),
            //    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/search_icon.png"))
            //};
            //this.MovieListToolbar.Children.Add(_searchButton);

            //_endFindMovie = new ImagedButtonNoText() // Command="{Binding EndFindMovie, Mode=OneWay}"
            //{
            //    Margin = new Thickness(5, 5, 5, 5),
            //    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/Cross2.png"))
            //};
            //this.MovieListToolbar.Children.Add(_endFindMovie);

            //_createMovie = new ImagedButton()
            //{
            //    // Command = "{Binding CreateMovie}"
            //    LabelText = "Create Movie",
            //    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/Pharmacy.jpeg"))
            //};
            //this.MovieListToolbar.Children.Add(_createMovie);

            //TextBox currentMovieTextBox = new TextBox()
            //{
            //    Margin = new Thickness(5, 5, 5, 5),
            //    Width = 350,
            //    BorderBrush = new SolidColorBrush(Colors.Black),
            //    BorderThickness = new Thickness(1)
            //};
            //this.MovieListToolbar.Children.Add(currentMovieTextBox);
            //currentMovieTextBox.Bind(TextBox.TextProperty, new Binding("CurrentMovie.MovieName"));

        }




        private void ComboBox_Initialized(object? sender, System.EventArgs e)
        {
            building = false;
        }

        //private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        //{
        //    if (!building)
        //    {
        //        // MovieListViewModel? MovieListViewModel = this.DataContext as MovieListViewModel;
        //        _SeriesCombo.IsVisible = false;
        //        MovieListViewModel?.SubComboBox_SelectionChanged(sender, e);

        //        if (e.AddedItems != null && e.AddedItems.Count > 0)
        //        {
        //            PhraseEntry selectedGroup = e.AddedItems[0] as PhraseEntry;
        //            if (selectedGroup.Id == "SER")
        //            {
        //                // make Series button visible
        //                _SeriesCombo.IsVisible = true;
        //                //On selection change filter movielist 
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// The DataGrid_LoadingRow.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="DataGridRowEventArgs"/>.</param>
        private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            DataGridRow? row = e.Row;

            if (LastMovieID != null && row != null && row.DataContext != null)
            {
                Movies? movie = row.DataContext as Movies;

                if (movie != null && movie.Id == LastMovieID)
                {
                    movie.BackColour = Avalonia.Media.Brushes.CadetBlue;
                }
                movie.SetPercentUnmarked();
                movie.FixMovieData();
            }
        }

        /// <summary>
        /// The DataGrid_Sorting.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="DataGridColumnEventArgs"/>.</param>
        private void DataGrid_Sorting(object? sender, DataGridColumnEventArgs e)
        {
            string? columnname = e.Column.Header.ToString();

            if (DataController.MovieProperties.SortColumnString == columnname)
            {
                DataController.MovieProperties.SortColumnString = columnname;
                DataController.MovieProperties.SortDirection =
                    DataController.MovieProperties.SortDirection * -1;
            }
            else
            {
                DataController.MovieProperties.SortColumnString = columnname;
                DataController.MovieProperties.SortDirection = 1;
            }
        }

        //private void DgMovies_GotFocus(object? sender, Avalonia.Input.FocusChangedEventArgs e)
        //{
        //    //MovieListViewModel? viewModelBase = DataContext as MovieListViewModel;
        //    if (MovieListViewModel != null && MovieListViewModel.FirstMovieSelected)
        //    {
        //        dgMovies.ScrollIntoView(MovieListViewModel.CurrentMovie, dgMovies.Columns[1]);
        //        MovieListViewModel.FirstMovieSelected = false;
        //    }
        //    building = false;
        //}

        private void DgMovies_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // MovieListViewModel? MovieListViewModel = Support.GetMovieListViewModel();
            if (dgMovies.IsInitialized && DataContext != null)
            {
                if (DataContext is MovieListViewModel MovieListViewModel)
                {

                    if (MovieListViewModel != null && e.AddedItems != null && e.AddedItems.Count == 1)
                    {
                        MovieListViewModel.CurrentMovie = e.AddedItems[0] as Movies;
                        if (MovieListViewModel.CurrentMovie != null)
                        {
                            MovieListViewModel.CurrentMovie.LoadChildren();
                            MovieListViewModel.CurrentMovie.FixMovieData();
                        }

                        // MovieListViewModel.MoveToSelectedRow(null);
                    }
                }
            }
        }

        // local edit button
        /// <summary>
        /// The Edit.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private async void EditClick(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Window? main = TaymadeEntities.Support.Support.GetWindow() as Window;

            if (button != null && main != null)
            {
                Movies? model = button.DataContext as Movies;

                if (model != null)
                {
                    // MovieListViewModel? viewModel = Support.GetMovieListViewModel();
                    if (MovieListViewModel != null)
                    {
                        MovieListViewModel.CurrentMovie = model;
                        //await EditMovie(main, model);
                    }
                }
            }
        }

        //private void FindByBookmark_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        //{
        //    //  MovieListViewModel? viewModelBase = DataContext as MovieListViewModel;

        //    if (MovieListViewModel != null)
        //    {
        //        MovieListViewModel.FindWithBookmarkNoDialog();
        //    }
        //}

        /// <summary>
        /// The FindByBookmark_LostFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void FindByBookmark_LostFocus(object? sender, RoutedEventArgs e)
        {
            MovieListViewModel? viewModel = DataContext as MovieListViewModel;
            if (viewModel != null)
                viewModel.DoFindMoviesByBookmark();
        }

        //private void MovieListControl_DataContextChanged(object? sender, System.EventArgs e)
        //{
        //    // set local property for viewModel
        //    if (DataContext != null)
        //    {
        //        if (DataContext is MovieListViewModel)
        //            MovieListViewModel = DataContext as MovieListViewModel;

        //        if (DataContext is MovieListViewModel)
        //            MovieListViewModel = DataContext as MovieListViewModel;

        //        // set control properties
        //        //SetToolbarProperties();
        //    }
        //}

        /// <summary>
        /// The Play.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Play(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                MovieListViewModel? model = button.DataContext as MovieListViewModel;

                if (model != null && model.CurrentMovie != null)
                {


                    //MovieListViewModel.CurrentMovie = model;

                    TaymadeEntities.Support.Support.PlayMovie(
                        TaymadeEntities.Support.Support.FixImagePath(model.CurrentMovie.MoviePath),
                        null
                    );

                }
            }
        }

        //private void SetToolbarProperties()
        //{
        //    if (MovieListViewModel != null)
        //    {
        //        _genresComboBox.ItemsSource = MovieListViewModel.Phrases;
        //        _genresComboBox.SelectedItem = MovieListViewModel.CurrentPhrase;
        //        _reloadMovies.Command = MovieListViewModel._ReloadGroups;
        //        _MoveToSelected.Command = MovieListViewModel._MoveToSelectedRow;
        //        _subGroups.ItemsSource = MovieListViewModel.SubPhrases;
        //        _subGroups.SelectedItem = MovieListViewModel.CurrentSubPhrase;

        //        _SeriesCombo.ItemsSource = MovieListViewModel.SeriesList;
        //        _SeriesCombo.IsVisible = MovieListViewModel.SeriesVisible;
        //        _SeriesCombo.SelectedItem = MovieListViewModel.CurrentSeries;

        //        _SeasonsCombo.ItemsSource = MovieListViewModel.SeasonsList;
        //        _SeasonsCombo.IsVisible = MovieListViewModel.SeriesVisible;
        //        _SeasonsCombo.SelectedItem = MovieListViewModel.CurrentSeason;

        //        _searchButton.Command = MovieListViewModel.searchForMovie;
        //        _endFindMovie.Command = MovieListViewModel.DoEndFindMovie;

        //        if (MovieListViewModel != null)
        //        {
        //            _createMovie.Command = MovieListViewModel.CreateMovie;
        //        }

        //    }
        //}
        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
        /// <summary>
        /// The TextBox_LostFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void TextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            // MovieListViewModel? viewModel = DataContext as MovieListViewModel;
            if (MovieListViewModel != null)
                MovieListViewModel.FindMovie(true);
        }

        private void dgMovies_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if (dgMovies.IsInitialized && DataContext != null)
            {
                if (DataContext is MovieListViewModel MovieListViewModel)
                {

                    if (MovieListViewModel != null && e.AddedItems != null && e.AddedItems.Count == 1)
                    {
                        MovieListViewModel.CurrentMovie = e.AddedItems[0] as Movies;
                        if (MovieListViewModel.CurrentMovie != null)
                        {
                            //this.MovieName.Text = MovieListViewModel.CurrentMovie.MovieName;
                            MovieListViewModel.CurrentMovie.LoadChildren();
                            MovieListViewModel.CurrentMovie.FixMovieData();
                        }

                        // MovieListViewModel.MoveToSelectedRow(null);
                    }
                }
            }
        }

        private static void DisposeButtonImages(Avalonia.Controls.Control root)
        {
            foreach (var button in root.GetVisualDescendants())
                if (button is ImagedButton)
                {
                    ImagedButton temp = button as ImagedButton;
                    temp.Dispose();
                }
                else if (button is Image image &&
                    image.Source is Bitmap bitmap)
                {
                    bitmap.Dispose();

                }
        }
        #endregion Private Methods

    }
}