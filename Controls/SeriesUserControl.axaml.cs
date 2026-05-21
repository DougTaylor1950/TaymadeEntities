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
    using TaymadeEntities.ViewModels;
    //using TaymadeControls;

    /// <summary>
    /// Defines the <see cref="SeriesUserControl" />.
    /// </summary>
    public partial class SeriesUserControl : UserControl
    {
        private MovieEditViewModel? movieViewModelBase;
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
                if (this.DataContext is MovieEditViewModel)
                {
                    movieViewModelBase = this.DataContext as MovieEditViewModel;

                    if (movieViewModelBase != null)
                    {
                        this.SeriesHeaderPanel.Title = movieViewModelBase.CurrentMovie?.MovieName;
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
