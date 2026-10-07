//-----------------------------------------------------------------------
// <copyright file="SeriesDialogWindow.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>09/05/2023 12:30:37 09/05/2023 12:30:37 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Dialogs
{
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    //using Avalonia.ReactiveUI;
    using TaymadeEntities.ViewModels;
    using ReactiveUI;
    using System;

    /// <summary>
    /// Defines the <see cref="SeriesDialogWindow" />.
    /// </summary>
    public partial class SeriesDialogWindow : Window,IDisposable
    {
        private bool disposedValue;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesDialogWindow"/> class.
        /// </summary>
        public SeriesDialogWindow()
        {
            InitializeComponent();
            //this.WhenActivated(d => d(ViewModel!.AddSeasonCommand.Subscribe(Close)));
        }

        public SeriesDialogWindow(SeriesViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            //this.WhenActivated(d => d(ViewModel!.AddSeasonCommand.Subscribe(Close)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The DoInitialise.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void DoInitialise(object? sender, RoutedEventArgs e)
        {
            MovieEditViewModel? viewModel = DataContext as MovieEditViewModel;
            if (viewModel != null)
            {
                if (viewModel.CurrentSeries != null)
                {
                    viewModel.NewSeason = new Models.Season(viewModel.CurrentSeries);
                }
            }
        }

        private void OKButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                this.Close(true);
            });
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                this.Close(false);
            });
        }

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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SeriesDialogWindow()
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
