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
    using DocumentFormat.OpenXml.Bibliography;
    using TaymadeEntities.ViewModels;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Disposables;
    using TaymadeEntities.Support;

    /// <summary>
    /// Defines the <see cref="TMDBSearchDialog" />.
    /// </summary>
    public partial class TMDBSearchDialog : Window, IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the FoundMovies.
        /// </summary>
        private IEnumerable<MovieBase>? FoundMovies;
        private bool disposedValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TMDBSearchDialog"/> class.
        /// </summary>
        public TMDBSearchDialog()
        {
            InitializeComponent();
        }

        private readonly CompositeDisposable _disposables = new();

        protected override void OnClosed(EventArgs e)
        {
            _disposables.Dispose();
            
            Content = null;

            base.OnClosed(e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TMDBSearchDialog"/> class.
        /// </summary>
        /// <param name="model">The model<see cref="ViewModels.MovieViewModel"/>.</param>
        public TMDBSearchDialog(ViewModels.MovieSearchViewModel model)
        {
            InitializeComponent();

            DataContext = model;

            //TextBox searchFor = this.Find<TextBox>("SearchFor");

            Opened += TMDBSearchDialog_Opened;
        }

        private void TMDBSearchDialog_Opened(object? sender, System.EventArgs e)
        {

            this.WindowState = WindowState.Maximized;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SearchTMDP.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? SearchTMDP { get; set; }
        public ReactiveCommand<Unit, Unit>? SearchTMDPOnly { get; set; }
        public MovieBase? FoundItem { get; private set; }

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


        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MovieBase? returnValue = new MovieBase();
            if (this.DataContext is MovieSearchViewModel viewModel)
            { 
                if (viewModel != null && viewModel.CurrentItem != null)
                {
                    returnValue.ID = viewModel.CurrentItem.ID;
                    returnValue.Overview = viewModel.CurrentItem.Overview;
                    returnValue.Year = viewModel.CurrentItem.Year;
                    returnValue.Title = viewModel.CurrentItem.Title;

                }
            }

            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
            this.Close(returnValue));
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            Avalonia.Threading.Dispatcher.UIThread.Post(() => this.Close(null));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FoundItem = null;
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TMDBSearchDialog()
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
        #endregion
    }
}
