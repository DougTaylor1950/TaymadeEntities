
using Avalonia.Controls;

using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.ViewModels;
using System.Linq;

namespace TaymadeEntities.Dialogs
{
    public partial class TrimMovieDialog : Window
    {

        #region Fields

        //private DownloadViewModel? download = null;

       // private MovieViewModelBase? mvvm = null;

        #endregion  

        public TrimMovieDialog()
        {
            InitializeComponent();

            //mvvm = Support.Support.GetMainWindowViewModel();

            //if (mvvm != null)
            //{
            //    if (mvvm.CurrentMovie == null)
            //    {
            //        int? id = DataController.MovieProperties.LastMoveID;
            //        mvvm.CurrentMovie = DataController.SandboxEntities.Movies.Find(id);
            //    }
            //    MovieConversionViewModel movieConversionViewModel = new MovieConversionViewModel() { CurrentMovie = mvvm.CurrentMovie };
            //    //movieConversionViewModel.Caller = this;
            //    movieConversionViewModel.CallingViewModel = mvvm;
            //    this.DataContext = movieConversionViewModel;
            //SetupControls();
        }




        public TrimMovieDialog(MovieViewModelBase mVVM, Movies? movie)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
            MovieConversionViewModel movieConversionViewModel = new MovieConversionViewModel() { CurrentMovie = movie };
            // movieConversionViewModel.Caller = this;
            movieConversionViewModel.CallingViewModel = mVVM;



            this.DataContext = movieConversionViewModel;

            if (movie != null)
            {
                if (movie.Bookmarks.Count > 0)
                {
                    movieConversionViewModel.Bookmark = movie.Bookmarks.FirstOrDefault();
                    movieConversionViewModel.BaseBitmap = movieConversionViewModel.Bookmark.ImageBMP;
                }

                else
                {
                    // has no bookmarks 

                    Bookmark bookmark = new Bookmark()
                    {
                        Name = "example",
                        Time = 10
                    };
                    GetDetails(null, movie, movieConversionViewModel, bookmark);
                }
                //    movie.CreateFirstBookmark();
                //    movieConversionViewModel.Bookmark = movie.Bookmarks.FirstOrDefault();
                //    movieConversionViewModel.BaseBitmap = movieConversionViewModel.Bookmark.ImageBMP;

                SetupControls();
            }

            //mvvm = mVVM;
        }

        private void SetupControls()
        {
            NumericUpDown udLeft = this.udLeft;

            if (udLeft != null)
            {
                udLeft.ValueChanged += UdLeft_ValueChanged1;
            }

            NumericUpDown udTop = this.udTop;
            if (udTop != null)
            {
                udTop.ValueChanged += UdTop_ValueChanged1;
            }

            NumericUpDown udScale = this.udScale;
            if (udScale != null)
            {
                udScale.ValueChanged += UdScale_ValueChanged;
            }

            ReScale();
        }

        private void UdScale_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            ReScale();
        }

        private void UdTop_ValueChanged1(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            MovieConversionViewModel? movieConversionViewModel = this.DataContext as MovieConversionViewModel;

            double newValue = (double)e.NewValue;

            if (movieConversionViewModel != null && movieConversionViewModel.Bookmark != null
                && movieConversionViewModel.Bookmark.imageBMP != null)
            {
                movieConversionViewModel.GammaCorrections.Height = movieConversionViewModel.Bookmark.imageBMP.Size.Height - newValue * 2;
                ReScale(movieConversionViewModel.GammaCorrections);
            }
        }

        private void UdLeft_ValueChanged1(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            MovieConversionViewModel? movieConversionViewModel = this.DataContext as MovieConversionViewModel;

            double newValue = (double)e.NewValue;


            if (movieConversionViewModel != null && movieConversionViewModel.Bookmark != null)
            {
                movieConversionViewModel.GammaCorrections.Width = movieConversionViewModel.Bookmark.imageBMP.Size.Width - newValue * 2;
                ReScale(movieConversionViewModel.GammaCorrections);
            }
        }

        private void ReScale()
        {
            MovieConversionViewModel? movieConversionViewModel = this.DataContext as MovieConversionViewModel;
            if (movieConversionViewModel != null)
            {
                ReScale(movieConversionViewModel.GammaCorrections);
            }
        }

        private void ReScale(GammaCorrections? gammaCorrections)
        {
            if (gammaCorrections != null)
            {
                gammaCorrections.ScaleHeight = (int)(gammaCorrections.Height * gammaCorrections.ScaleFactor);
                gammaCorrections.ScaleWidth = (int)(gammaCorrections.Width * gammaCorrections.ScaleFactor);
            }
        }

        //public TrimMovieDialog(DownloadViewModel mvvm, Movies? movie)
        //{
        //    InitializeComponent();

        //    MovieConversionViewModel movieConversionViewModel = new MovieConversionViewModel() { CurrentMovie = movie };
        //    //movieConversionViewModel.Caller = this;
        //    movieConversionViewModel.CallingViewModel = mvvm;

        //    // will have no bookmarks 

        //    Bookmark bookmark = new Bookmark()
        //    {
        //        Name = "example",
        //        Time = 10
        //    };
        //    GetDetails(mvvm, movie, movieConversionViewModel, bookmark);

        //    SetupControls();

        //}

        private MovieConversionViewModel? GetModel()
        {
            MovieConversionViewModel? retValue = this.DataContext as MovieConversionViewModel;

            return retValue;
        }

        private void UdHeight_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            MovieConversionViewModel? model = GetModel();

            double newValue = (double)e.NewValue;


            if (model != null)
            {
                model.GammaCorrections.Height = newValue;
                model.Bookmark.ImageBMP = Support.Support.AddRectangle(model.Bookmark.ImageBMP, model.GammaCorrections.ClipRectangle);

            }
        }

        private void UdWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            MovieConversionViewModel? model = GetModel();

            if (model != null)
            {
                model.GammaCorrections.Width = (double)e.NewValue;
                model.Bookmark.ImageBMP = Support.Support.AddRectangle(model.Bookmark.ImageBMP, model.GammaCorrections.ClipRectangle);
            }
        }

        private void UdTop_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            MovieConversionViewModel? model = GetModel();

            if (model != null)
            {
                model.GammaCorrections.Top = (double)e.NewValue;
                model.Bookmark.ImageBMP = Support.Support.AddRectangle(model.Bookmark.ImageBMP, model.GammaCorrections.ClipRectangle);

            }
        }

        private void UdLeft_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {



        }

        private async void GetDetails(MovieViewModelBase? mvvm, Movies? movie, MovieConversionViewModel movieConversionViewModel, Bookmark bookmark)
        {
          //  string ipath = await VideoSupport.GrabBookmarkImage(movie, bookmark);

            bookmark.SetImageBMP();



            movieConversionViewModel.Bookmark = bookmark;

            if (bookmark.ImageBMP != null) movieConversionViewModel.BaseBitmap = bookmark.ImageBMP;

            this.DataContext = movieConversionViewModel;

          
        }

        private void Accept_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton dialogResultButton = new DialogResultButton() { Result = DialogResultButton.ResultType.Ok, Paramater = "TrimMovie" };
            if (this.DataContext is MovieConversionViewModel)
            {
                MovieConversionViewModel movieConversionViewModel = this.DataContext as MovieConversionViewModel;
                if (movieConversionViewModel != null)
                {
                    dialogResultButton.Paramater = movieConversionViewModel.GammaCorrections.GammaCorrectionString();
                }
            }
            this.Close(dialogResultButton);
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton dialogResultButton = new DialogResultButton() { Result = DialogResultButton.ResultType.Cancel, Paramater = "TrimMovie" };

            this.Close(dialogResultButton);
        }
    }
}
