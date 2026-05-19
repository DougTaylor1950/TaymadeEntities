//-----------------------------------------------------------------------
// <copyright file="TMDBSearchDialog.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>03/10/2022 14:57:56 03/10/2022 14:57:56 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Dialogs
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using TaymadeEntities.Support;
    using TaymadeEntities.ViewModels;
    using DocumentFormat.OpenXml.Bibliography;
    using ReactiveUI;
    using System.Collections.Generic;
    using System.Reactive;

    /// <summary>
    /// Defines the <see cref="TMDBSearchDialog" />.
    /// </summary>
    public partial class TMDBSearchDialog : Window
    {
        #region Fields

        /// <summary>
        /// Defines the FoundMovies.
        /// </summary>
        private IEnumerable<MovieBase>? FoundMovies;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TMDBSearchDialog"/> class.
        /// </summary>
        public TMDBSearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TMDBSearchDialog"/> class.
        /// </summary>
        /// <param name="model">The model<see cref="ViewModels.MovieViewModel"/>.</param>
        public TMDBSearchDialog(ViewModels.MovieViewModel model)
        {
            InitializeComponent();

            DataContext = model;

            TextBox searchFor = this.Find<TextBox>("SearchFor");

            Button searchTMDP = this.Find<Button>("TMDBSearch");
            Button searchTMDPOnly = this.Find<Button>("TMDBSearchOnly");


            if (searchFor != null && model != null && model.CurrentMovie != null)
            {
                if (string.IsNullOrEmpty(model.MovieTitle)) model.MovieTitle = model.CurrentMovie.MovieName;
                // If there is a year put in the year property
                if (model.CurrentMovie.Year != null && model.CurrentMovie.Year > 0)
                { model.Year = model.CurrentMovie.Year.Value; }
            }


            SearchTMDP = ReactiveCommand.Create(SearchDatabase);

            if (searchTMDP != null)
            {
                searchTMDP.Command = SearchTMDP;
            }

            SearchTMDPOnly = ReactiveCommand.Create(SearchDatabaseOnly);

            if (searchTMDPOnly != null)
            {
                searchTMDPOnly.Command = SearchTMDP;
            }

            if (this.TMDBSearchOKPanel != null)
            {
                MovieViewModelBase movieViewModelBase = this.DataContext as MovieViewModelBase;
                if (movieViewModelBase != null)
                {
                    this.TMDBSearchOKPanel.OkButton.Command = movieViewModelBase.AddOKCommand();
                    this.TMDBSearchOKPanel.CancelButton.Command = movieViewModelBase.AddCancelCommand();
                }
            }
            Opened += TMDBSearchDialog_Opened;
        }

        private void TMDBSearchDialog_Opened(object? sender, System.EventArgs e)
        {
            if (Screens.ScreenCount > 1 && Models.DataController.ShowOnAlternateScreen())
            {
                int screenWidth = (int)this.Width;
                this.Position = new PixelPoint(-screenWidth, 50);
            }
            this.WindowState = WindowState.Maximized;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SearchTMDP.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? SearchTMDP { get; set; }
        public ReactiveCommand<Unit, Unit>? SearchTMDPOnly { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}

        /// <summary>
        /// The MovieSelected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        private void MovieSelected(object? sender, SelectionChangedEventArgs e)
        {
            DataGrid? movies = sender as DataGrid;

            if (movies != null)
            {
                MovieBase? selected = movies.SelectedItem as MovieBase;

                ViewModels.MovieViewModel? viewModel = DataContext as ViewModels.MovieViewModel;

                if (viewModel != null) viewModel.FoundMovie = selected;
            }
        }

        /// <summary>
        /// The SearchDatabase.
        /// </summary>
        private void SearchDatabase()
        {
            TextBox searchFor = this.Find<TextBox>("SearchFor");
            if (searchFor != null)
            {
                string searchText = searchFor.Text;

                FoundMovies = TmdbSupport.SearchMovieDatabaseList(searchText);

                DataGrid dataGrid = this.Find<DataGrid>("dgFoundMovies");
                dataGrid.SelectionChanged += MovieSelected;
                dataGrid.ItemsSource = FoundMovies;
            }
        }

        private void SearchDatabaseOnly()
        {
            TextBox searchFor = this.Find<TextBox>("SearchFor");
            if (searchFor != null)
            {
                string searchText = searchFor.Text;

                FoundMovies = TmdbSupport.SearchMovieDatabaseList(searchText);

                DataGrid dataGrid = this.Find<DataGrid>("dgFoundMovies");
                dataGrid.SelectionChanged += MovieSelected;
                dataGrid.ItemsSource = FoundMovies;
            }

        }
        #endregion
    }
}
