//-----------------------------------------------------------------------
// <copyright file="SeriesUserControl.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>28/04/2022 15:06:03 28/04/2022 15:06:03 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Controls
{
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;
    using TaymadeEntities.Models;
    using TaymadeEntities.ViewModels;
    //using TaymadeControls;

    /// <summary>
    /// Defines the <see cref="SeriesUserControl" />.
    /// </summary>
    public partial class SeriesUserControl : UserControl
    {
        private MovieViewModelBase? movieViewModelBase;
        //private MovieViewModel? movieViewModel;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesUserControl"/> class.
        /// </summary>
        public SeriesUserControl()
        {
            InitializeComponent();

            this.Initialized += this.SeriesUserControl_Initialized;
            this.DataContextChanged += this.SeriesUserControl_DataContextChanged;
        }

        private void SeriesUserControl_DataContextChanged(object? sender, System.EventArgs e)
        {
            this.SetDataContext();
        }

        private void SetDataContext()
        {
            if (this.DataContext != null)
            {
                if (this.DataContext is MovieViewModelBase)
                {
                    movieViewModelBase = this.DataContext as MovieViewModelBase;

                    if (movieViewModelBase != null)
                    {
                        this.SeriesHeaderPanel.BindTitle("SeriesHeaderInfo");
                    }
                }

                //if (this.DataContext is MovieViewModel)
                //{
                //    movieViewModel = this.DataContext as MovieViewModel;

                //    if (movieViewModel != null)
                //    {
                //        this.SeriesHeaderPanel.BindTitle("CurrentMovie.MovieName");
                //        //this.SeriesHeaderPanel.Title = movieViewModel.CurrentMovie?.MovieName;
                //    }
                //}
            }
        }

        private void SeriesUserControl_Initialized(object? sender, System.EventArgs e)
        {
            this.SetDataContext();
        }

        private void SeriesList_LayoutUpdated(object? sender, System.EventArgs e)
        {
        }

        private void SeasonList_LayoutUpdated(object? sender, System.EventArgs e)
        {
            if (this.DataContext != null && DataContext is MovieViewModelBase viewModel)
            {

            }
        }

        private void EpisodeList_LayoutUpdated(object? sender, System.EventArgs e)
        {
            if (this.DataContext != null && DataContext is MovieViewModelBase viewModel)
            {
                //if (sender is ComboBox comboBox && 
                //    viewModel.CurrentMovie.SeasonEntity.TVEpisodes != null)
                //{
                //    TVEpisode? episode = viewModel.CurrentMovie.SeasonEntity.TVEpisodes.
                //        Where(e => e.Id == viewModel.CurrentMovie?.Episode).FirstOrDefault();
                    
                //    //    comboBox.SelectedItem = episode;
                    
                //}
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}

        #endregion
    }
}
