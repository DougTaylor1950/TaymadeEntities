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
    public partial class SeriesDialogWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesDialogWindow"/> class.
        /// </summary>
        public SeriesDialogWindow()
        {
            InitializeComponent();
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
            this.Close(true);
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close(false);
        }

        #endregion
    }
}
