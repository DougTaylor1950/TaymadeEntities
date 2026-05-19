using Avalonia.Controls;
using Avalonia.Media.Imaging;
using TaymadeEntities.Dialogs;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using ReactiveUI;
using System;
using System.Reactive;

namespace TaymadeEntities.ViewModels
{
    public class MovieConversionViewModel : ViewModelBase
    {
        #region Fields

        private Movies? currentMovie;
        private GammaCorrections gammaCorrections;
        private Bookmark bookmark;



        #endregion

        #region Constructors
        public MovieConversionViewModel()
        {
            Play = ReactiveCommand.Create(DoPlay);
            //Accept = ReactiveCommand.Create(DoAccept);
            //Cancel = ReactiveCommand.Create(DoCancel);
            Correction = ReactiveCommand.Create(DoCorrectionsAsync);
            Redraw = ReactiveCommand.Create(DoRedraw);

            ResultTask = new DialogResultButton() { Result = DialogResultButton.ResultType.Cancel };

            GammaCorrections = new GammaCorrections();


        }

        public void Left_ValueChanged (object sender, NumericUpDownValueChangedEventArgs e)
        {

        }

        public void Top_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {

        }

        private void DoRedraw()
        {
            Bookmark.ImageBMP = Support.Support.AddRectangle(BaseBitmap, GammaCorrections.ClipRectangle);
        }


        #endregion

        #region Methods

        public void SaveGamma()
        {
            // Save settings convert gammacorrection to json file save as movie name +config.json
            if (CurrentMovie != null)
            {                
                string mpath = Support.Support.FixImagePath(CurrentMovie.MoviePath);
                string configPath = System.IO.Path.ChangeExtension(mpath, ".json");
                GammaCorrections.Save(configPath);
            }
        }
        private async void DoCorrectionsAsync()
        {
            GammaCorrections.ShowOrProcess = false;
            if (GammaCorrections.clip)
            {
                gammaCorrections.Start = TimeSpan.Parse(CurrentMovie.StartBookmark.FormattedTime);
                gammaCorrections.Until = TimeSpan.Parse(CurrentMovie.EndBookmark.FormattedTime).Subtract(gammaCorrections.Start);
            }

            string mpath = Support.Support.FixImagePath(CurrentMovie.MoviePath);
            string param = GammaCorrections.GammaCorrectionString() + " " + '"' + mpath + '"';

            FFMpegSupport mpegSupport = new FFMpegSupport();
            int error = await mpegSupport.DoCliWrapPlay(param);

            GammaCorrections.ShowOrProcess = true;

        }
        private async void DoPlay()
        {
            //throw new NotImplementedException();
            GammaCorrections.ShowOrProcess = false;
            string param = FFMpegSupport.TrimMovieParameter(CurrentMovie, "", false);

            FFMpegSupport mpegSupport = new FFMpegSupport();
            int error = await mpegSupport.DoCliWrapPlay(param);

            GammaCorrections.ShowOrProcess = true;

        }

        private new void DoCancel()
        {
            ResultTask = new DialogResultButton() { Result = DialogResultButton.ResultType.Cancel };

            if (CallingViewModel != null)
            {
                if (CallingViewModel is MovieViewModelBase)
                {
                    MovieViewModelBase? download = CallingViewModel as MovieViewModelBase;
                    if (download != null) download.ResultTask = ResultTask;
                    download?.Caller.Close(ResultTask);
                }

                //if (CallingViewModel is MainWindowViewModel)
                //{
                //    MainWindowViewModel? download = CallingViewModel as MainWindowViewModel;
                //    if (download != null) download.ResultTask = ResultTask;
                //}
                
            }

            

        }

        private new void DoAccept()
        {
            ResultTask = new DialogResultButton() { Result = DialogResultButton.ResultType.Ok };

            if (CallingViewModel != null)
            {
                if (CallingViewModel is MovieViewModelBase)
                {
                    MovieViewModelBase? download = CallingViewModel as MovieViewModelBase;
                    if (download != null)
                    {
                        gammaCorrections.ShowOrProcess = true;
                        gammaCorrections.Start = TimeSpan.Parse(CurrentMovie.StartBookmark.FormattedTime);
                        gammaCorrections.Until = TimeSpan.Parse(CurrentMovie.EndBookmark.FormattedTime);
                        download.ResultTask = ResultTask;

                        string mpath = Support.Support.FixImagePath(CurrentMovie.MoviePath);

                        ResultTask.Paramater = " -y -i " + '"' + mpath + '"' + " " + gammaCorrections.GammaCorrectionString() + " -c:v libx264 -preset slow -crf 22 ";

                        if (gammaCorrections.Crop)
                        {
                            ResultTask.Seconds = (int)gammaCorrections.Until.Subtract(gammaCorrections.Start).TotalSeconds;
                        }
                        download?.Caller.Close();
                    }
                }

                if (CallingViewModel is MainWindowViewModel)
                {
                    MainWindowViewModel? download = CallingViewModel as MainWindowViewModel;
                    if (download != null)
                    {
                        gammaCorrections.ShowOrProcess = true;
                        gammaCorrections.Start = TimeSpan.Parse(CurrentMovie.StartBookmark.FormattedTime);
                        gammaCorrections.Until = TimeSpan.Parse(CurrentMovie.EndBookmark.FormattedTime);
                       // ResultTask;

                        string mpath = Support.Support.FixImagePath(CurrentMovie.MoviePath);

                        ResultTask.Paramater = " -y -i " + '"' + mpath + '"' + " " + gammaCorrections.GammaCorrectionString() + " -c:v libx264 -preset slow -crf 22 ";

                        if (gammaCorrections.Crop)
                        {
                            ResultTask.Seconds = (int)gammaCorrections.Until.Subtract(gammaCorrections.Start).TotalSeconds;
                        }

                        
                    }
                }
                }
            }


           
        #endregion

        #region Properties

        public Bitmap BaseBitmap { get; set; }

        public Bookmark Bookmark
        {
            get => bookmark;

            set
            {
                this.RaiseAndSetIfChanged(ref bookmark, value);

                if (value != null && GammaCorrections != null && value.ImageBMP != null)
                {
                    gammaCorrections.Width = value.ImageBMP.Size.Width;
                    gammaCorrections.Height = value.ImageBMP.Size.Height;
                }
            }
        }

        public GammaCorrections GammaCorrections { get => gammaCorrections; set => this.RaiseAndSetIfChanged(ref gammaCorrections, value); }

        public ViewModelBase? CallingViewModel { get; set; }
        public Movies? CurrentMovie
        {
            get => currentMovie;
            set
            {
                this.RaiseAndSetIfChanged(ref currentMovie, value);

                // load the config file if it exists
                if (value != null)
                {
                    string mpath = Support.Support.FixImagePath(value.MoviePath);
                    string configPath = System.IO.Path.ChangeExtension(mpath, ".json");
                    GammaCorrections.Load(configPath);
                    Bookmark = value.StartBookmark;
                    if (Bookmark != null && Bookmark.ImageBMP != null)
                    {
                        BaseBitmap = Bookmark.ImageBMP;
                        GammaCorrections.Width = Bookmark.ImageBMP.Size.Width;
                        GammaCorrections.Height = Bookmark.ImageBMP.Size.Height;
                        //GammaCorrections.ClipRectangle = Support.Support.GetRectFromBookmark(value.StartBookmark, value.EndBookmark, Bookmark.ImageBMP);
                        //Bookmark.ImageBMP = Support.Support.AddRectangle(Bookmark.ImageBMP, GammaCorrections.ClipRectangle);
                    }
                }
            }
        }

        public DialogResultButton ResultTask { get; private set; }


        public ReactiveCommand<Unit, Unit> Play { get; set; }

        public ReactiveCommand<Unit, Unit> Correction { get; set; }

        public ReactiveCommand<Unit, Unit> Redraw { get; set; }
        #endregion

    }
}
