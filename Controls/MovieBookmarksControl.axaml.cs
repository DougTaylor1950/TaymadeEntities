//-----------------------------------------------------------------------
// <copyright file="MovieBookmarksControl.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>25/04/2022 11:55:33 25/04/2022 11:55:33 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Controls
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using TaymadeEntities.ViewModels;
    using DynamicData;
    using System;
    using System.Threading.Channels;
    using TaymadeControls;
    using TaymadeControls.Buttons;

    /// <summary>
    /// Defines the <see cref="MovieBookmarksControl" />.
    /// </summary>
    public partial class MovieBookmarksControl : UserControl
    {
        #region Private Fields

        private ImagedButtonNoText _MissingImages;
        private ImagedButton _playFromLast;
        private ImagedButton _ReloadBookmarks;
        private ImagedButton _repeatLast;
        private ImagedButton AddBookmarks;
        private ImagedButton AddPoster;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieBookmarksControl"/> class.
        /// </summary>
        public MovieBookmarksControl()
        {
            InitializeComponent();
            Initialized += this.MovieBookmarksControl_Initialized;
            DataContextChanged += this.MovieBookmarksControl_DataContextChanged;
           
            SetupToolbar();
        }

        #endregion Public Constructors

        //public BookmarkUserControl BookmarkUserControl { get; set; }

        #region Private Methods

        private void dgBooks_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            //if (this.dgBooks.SelectedItem is Models.Bookmark bookmark)
            //{
            //    // set the datacontext for the child controls
            //    if (this.BookmarkUserControl != null)
            //    {
            //        if (this.BookmarkUserControl.DataContext is BookmarkViewModel currentBookmark)
            //        {
            //            BookmarkViewModel? bookmarkVM = this.BookmarkUserControl.DataContext as BookmarkViewModel;
            //            if (bookmarkVM != null)
            //            {
            //                bookmarkVM.CurrentBookmark = bookmark;
            //            }
            //        }
            //        else
            //        {
            //            BookmarkViewModel? bookmarkViewModel = new BookmarkViewModel();
            //            bookmarkViewModel?.CurrentBookmark = bookmark;
            //            bookmarkViewModel?.CurrentMovie = bookmark.Movies;
            //            this.BookmarkUserControl.DataContext = bookmarkViewModel;

            //        }
            //        //this.BookmarkUserControl.DataContext = bookmark;
            //    }
            //}
        }

        private void MovieBookmarksControl_DataContextChanged(object? sender, EventArgs e)
        {
            MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
            if (viewModel != null)
            {
                this.SetButtonCommands();
                // set the datacontext for the child controls

                if (this.dgBooks != null)
                {
                    // this.dgBooks.DataContext = this.DataContext;
                    // set itemssource to CurrentMovie.Bookmarks
                    this.dgBooks.ItemsSource = viewModel?.CurrentMovie?.Bookmarks;
                }
            }
        }

        private void MovieBookmarksControl_Initialized(object? sender, EventArgs e)
        {
        }

        private void SetButtonCommands()
        {
            MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
            if (viewModel != null)
            {
                AddBookmarks.Command = viewModel.NewBookmark;
                AddPoster.Command = viewModel.NewPoster;
                _playFromLast.Command = viewModel.PlayFromLast;
                _repeatLast.Command = viewModel.RepeatLast;
                _ReloadBookmarks.Command = viewModel.ReloadBookmarks;
                _MissingImages.Command = viewModel.GetMissingImages;
            }
        }

        private void SetupToolbar()
        {
            //
            // check toolbar panel exists
            if (this.ToolbarBookmarks != null)
            {
                this.ToolbarBookmarks.Height = 48;
                this.ToolbarBookmarks.Background = new SolidColorBrush(Colors.LightGray);
                // add buttons
                AddBookmarks = new ImagedButton()
                {
                    LabelText = "New Bookmark",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/NewBookmark.png")),
                    // Command = viewModel.NewBookmark
                };
                this.ToolbarBookmarks.Children.Add(AddBookmarks);

                AddPoster = new ImagedButton()
                {
                    LabelText = "New Poster",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/bookmark.png"))
                };
                this.ToolbarBookmarks.Children.Add(AddPoster);

                _playFromLast = new ImagedButton()
                {
                    LabelText = "Play Last",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/playLast.png"))
                };
                this.ToolbarBookmarks.Children.Add(_playFromLast);

                _repeatLast = new ImagedButton()
                {
                    LabelText = "Repeat Last",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/sync.png"))
                };
                this.ToolbarBookmarks.Children.Add(_repeatLast);

                _ReloadBookmarks = new ImagedButton()
                {
                    LabelText = "Reload Bookmarks",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/sync.png"))
                };
                this.ToolbarBookmarks.Children.Add(_ReloadBookmarks);

                _MissingImages = new ImagedButtonNoText()
                {
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/missing_icon.png"))
                };
                ToolTip.SetTip(_MissingImages, "Look for images not built");
                this.ToolbarBookmarks.Children.Add(_MissingImages);
            }
        }

        #endregion Private Methods

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
    }
}