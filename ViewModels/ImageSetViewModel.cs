using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Office.Interop.Word;

//using Microsoft.AspNetCore.Razor.TagHelpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TaymadeEntities.Controls;
using TaymadeEntities.Dialogs;
using TaymadeEntities.Models;
//using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.ViewModels;
using TaymadeEntities.Views;
using static TaymadeEntities.Support.FFMpegSupport;
using Task = System.Threading.Tasks.Task;
using Window = Avalonia.Controls.Window;

namespace TaymadeEntities.ViewModels
{
    public class ImageSetViewModel : TaymadeEntities.ViewModels.MovieViewModelBase
    {
        #region Private Fields

        private ImageItem? lastImageItem = null;
        private RootFolder rootFolder;
        private ObservableCollection<MovieImage> subDirectoryList;
        private bool usePictures;
        private Avalonia.Media.Imaging.Bitmap currentIcon = null;
        private string playPauseLabel = "Play";
        private int? progressPercent;
        private string? missingInfo;
        private int? progressPercentShow;
        private FileMonitor _monitor;
        private Avalonia.Media.Imaging.Bitmap? currentImage;


        internal Avalonia.Media.IBrush backgroundColor { get; set; }

        #endregion Private Fields


        #region Public Constructors

        public ImageSetViewModel()
        {
            CreateSubFolder = ReactiveCommand.Create(DoCreateSubFolder);
            EditPicture = ReactiveCommand.Create(DoEditPicture);
            ReloadPictures = ReactiveCommand.Create(DoReloadPictures);
            ReloadPicture = ReactiveCommand.Create(DoReloadPicture);
            DeletePicture = ReactiveCommand.Create(DoDeletePicture);
            //ZoomToFeature = ReactiveCommand.Create(DoZoomToFeature);
            UsePicturesAsItems = true;
            CurrentIcon = PlayIcon;
            if (RootFolder.CurrentImageFolder == null)
            {
                if (RootFolder.SubDirectoryList == null || RootFolder.SubDirectoryList.Count == 0)
                {
                    List<MovieImage>? movieImages = DataController.movieController.GetMovieImagesById(RootFolder.Id);
                    RootFolder.SubDirectoryList = new ObservableCollection<MovieImage>(movieImages);
                }
                MovieImage? lastIdMovieImage = DataController.MovieController.GetMovieImageById(RootFolder.LastId);
                if (lastIdMovieImage != null && lastIdMovieImage.FolderType == "FolderList")
                    RootFolder.CurrentImageFolder = lastIdMovieImage;
                else
                    RootFolder.CurrentImageFolder = DataController.MovieController.GetMovieImageById(lastIdMovieImage.ParentId);
            }
            if (RootFolder != null && RootFolder.CurrentImageFolder != null
                && RootFolder.CurrentImageFolder.SubDirectoryList != null)

                if (RootFolder.CurrentSubFolder == null ||
                    RootFolder.CurrentSubFolder.Id != RootFolder.CurrentImageFolder.LastId)
                {
                    if (RootFolder.CurrentImageFolder.LastId > 0)
                        RootFolder.CurrentSubFolder = RootFolder.CurrentImageFolder.SubDirectoryList.Where(f => f.Id == RootFolder.CurrentImageFolder.LastId).FirstOrDefault();
                    else
                        RootFolder.CurrentSubFolder = RootFolder.CurrentImageFolder.SubDirectoryList.First();
                }

            if (RootFolder.CurrentSubFolder != null && RootFolder.CurrentSubFolder.ImageItems.Count == 0)
            {
                RootFolder.CurrentSubFolder.ImageItems.ReloadImageItems(
                    RootFolder.CurrentSubFolder.Path
                    );
            }

            if (!string.IsNullOrEmpty(RootFolder.CurrentSubFolder.LastImageName))
            {
                RootFolder.CurrentImageItem = RootFolder.CurrentSubFolder.ImageItems.Where(
                    s => s.ImageName == RootFolder.CurrentSubFolder.LastImageName).FirstOrDefault();
            }
            else
            {
                RootFolder.CurrentImageItem = RootFolder.CurrentImageFolder.ImageItems.FirstOrDefault();
            }
            CurrentImage = RootFolder?.CurrentImageItem?.ImageBMP;
            if (this.ImageSetControl != null && this.ImageSetControl.dgItemImages != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ImageSetControl.dgItemImages.ScrollIntoView(RootFolder.CurrentImageItem, null);
                });
                //this.ImageSetControl.SetCurrentRow(RootFolder.CurrentImageItem);
            }

            //MVVM = Support.Support.GetMainWindowViewModel();
        }

        public void SetImageRow()
        {

            if (this.ImageSetControl != null && this.ImageSetControl.dgItemImages != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ImageSetControl.dgItemImages.ScrollIntoView(RootFolder.CurrentImageItem, null);
                });
                //this.ImageSetControl.SetCurrentRow(RootFolder.CurrentImageItem);
            }
        }


        #endregion Public Constructors

        #region Public Properties

        public MovieViewModelBase MVVM { get; set; }

        public TaymadeControls.Buttons.ImagedButton PlayButton
        { get; set; }


        public Avalonia.Media.Imaging.Bitmap? CurrentImage
        {
            get => currentImage;
            set
            {
                //currentImage?.Dispose();
                this.RaiseAndSetIfChanged(ref currentImage, value);
                if (value != null && RootFolder != null)
                {
                    RootFolder?.LastId = RootFolder?.CurrentSubFolder.Id;
                    RootFolder?.Save();
                }
            }

        }
        public Avalonia.Media.Imaging.Bitmap? CurrentIcon
        {
            get
            {
                if (currentIcon == null)
                {
                    currentIcon = PlayIcon;
                }
                return currentIcon;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref currentIcon, value);

            }
        }

        internal Avalonia.Media.Imaging.Bitmap? PlayIcon { get; } = TaymadeControls.ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/play.png"));
        internal Avalonia.Media.Imaging.Bitmap? StopIcon { get; } = TaymadeControls.ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/stop.png"));

        internal Avalonia.Media.Imaging.Bitmap? PauseIcon { get; } = TaymadeControls.ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/Pause.png"));

        public MovieImage CurrentSubFolder
        {
            get
            {
                if (RootFolder != null)
                {
                    return RootFolder.CurrentSubFolder;
                }
                else
                {
                    return null;
                }
            }



        }

        public string? CurrentSubFolderName
        {
            get
            {
                if (RootFolder != null && RootFolder.CurrentSubFolder != null && !string.IsNullOrEmpty
                   (RootFolder.CurrentSubFolder.Name))
                {
                    return RootFolder.CurrentSubFolder.Name;

                }
                else
                {
                    return null;
                }
            }
        }

        public ReactiveCommand<Unit, Unit> CreateSubFolder { get; private set; }
        public ReactiveCommand<Unit, Unit> DeletePicture { get; private set; }
        public ReactiveCommand<Unit, Unit> EditPicture { get; private set; }
        public ReactiveCommand<Unit, Unit> ReloadPictures { get; private set; }
        public ReactiveCommand<Unit, Unit> ReloadPicture { get; private set; }

        public ReactiveCommand<Unit, Unit> ZoomToFeature { get; private set; }
        public RootFolder? RootFolder
        {
            get
            {
                if (rootFolder == null) rootFolder = new RootFolder();
                return rootFolder;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref rootFolder, value);
                if (value != null)
                {
                    value.CurrentSubFolder.ImageItems.ImageChanged += ImageItems_ImageChanged;

                }
            }
        }

        internal void ImageItems_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            this.RaisePropertyChanged("ImageItems");

            if (e != null)
            {
                if (e.Action != "Stopped")
                {
                    if (RootFolder.CurrentImageItem != null) RootFolder.CurrentImageItem.Selected = false;
                    RootFolder.CurrentImageFolder.PlayButtonContent = e.Action;
                    this.RaisePropertyChanged("RootFolder.CurrentImageFolder.PlayButtonContent");
                    //if (e.ImageItem != null) _ = e.ImageItem.ImageBMP;
                    RootFolder.CurrentImageItem = e.ImageItem;
                    RootFolder.CurrentImageItem.Selected = true;

                    if (RootFolder.CurrentSubFolder != null)
                    {
                        RootFolder.CurrentImageFolder.CommandIsVisible = true;

                        //CurrentSubFolder.CommandIsVisible = true;
                        RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentSubFolder.ImageItems.Count;
                    }
                    else
                    {
                        RootFolder.CurrentImageFolder.CommandIsVisible = true;
                        RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentImageFolder.ImageItems.Count;
                    }
                }
                else
                {
                    RootFolder.CurrentImageFolder.CommandIsVisible = false;
                    RootFolder.CurrentImageFolder.PlayButtonContent = "Play";
                }

                // calculate perentage using position and count
                if (RootFolder.CurrentSubFolder != null && RootFolder.CurrentSubFolder.ImageItems != null && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
                {
                    int percent = (int)(((double)(e.ImagePosition + 1) / (double)RootFolder.CurrentSubFolder.ImageItems.Count) * 100);
                    // update ISVM property to show percentage
                    ProgressPercentShow = percent;

                    // RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentSubFolder.ImageItems.Count + " : " + percent.ToString() + "%";
                }

            }
        }

        public ObservableCollection<MovieImage> SubDirectoryList
        {
            get
            {
                if (subDirectoryList == null) subDirectoryList = RootFolder.SubDirectoryList;
                return subDirectoryList;
            }

            set => this.RaiseAndSetIfChanged(ref subDirectoryList, value);
        }
        public bool UsePicturesAsItems { get => usePictures; set => this.RaiseAndSetIfChanged(ref usePictures, value); }

        public string? PlayButtonText
        {
            get
            {
                if (RootFolder != null && RootFolder.CurrentSubFolder != null)
                {
                    return RootFolder.CurrentSubFolder.PlayButtonContent;
                }
                else
                {
                    return "Play";
                }
            }
        }

        public string? PlayPauseLabel { get => playPauseLabel; set => this.RaiseAndSetIfChanged(ref playPauseLabel, value); }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Nexts the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void NextButton_Click()
        {
            if (RootFolder != null
                   && RootFolder.CurrentSubFolder.ImageItems != null
                   && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {
                RootFolder.CurrentSubFolder.ImageItems.MoveNext();
            }
        }

        /// <summary>
        /// Plays the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void PlayButton_Click()
        {
            CurrentIcon = PauseIcon;

            if (RootFolder != null
                && RootFolder.CurrentSubFolder.ImageItems != null
                && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {
                RootFolder.CurrentSubFolder.ImageItems.ISVM = this;
                Task.Run(RootFolder.CurrentSubFolder.ImageItems.Play);

            }
        }

        public void PlayMP4File(string moviePath)
        {
            TaymadeEntities.Support.Support.PlayMovie(moviePath, null);
        }

        public void RefreshSubDir()
        {
            if (RootFolder != null)
            {
                RootFolder.BuildSubFolders();
            }
        }

        /// <summary>
        /// Refreshes the sub folder.
        /// </summary>
        /// <autogeneratedoc />
        public void RefreshSubFolder()
        {
            if (RootFolder != null) RootFolder.ReloadPictures();
        }

        /// <summary>
        /// Renames the pic file.
        /// </summary>
        /// <autogeneratedoc />
        public async void RenamePicFile()
        {
            if (RootFolder != null && RootFolder.CurrentImageItem != null)
            {
                // get an entry dialog
                EntryDialogModel dialogModel = new EntryDialogModel()
                {
                    EntryTypeValue = EntryDialogModel.EntryType.Text,
                    EntryText = RootFolder.CurrentImageItem.ImagePath,
                    MaxStringLength = 150
                };

                // create dialog
                EntryDialog entryDialog = new EntryDialog(dialogModel);

                // dialogModel.Caller = entryDialog;

                Visual? main = TaymadeEntities.Support.Support.GetMainWindow();

                // show dialog and process if ok
                DialogResultButton resultButton = await entryDialog.ShowDialog<DialogResultButton>(main as Window);
                if (resultButton != null && resultButton.Result == DialogResultButton.ResultType.Ok)
                {
                    string? oldname = RootFolder.CurrentImageItem.ImagePath;
                    string? newName = resultButton.Paramater;
                    if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(oldname) && !File.Exists(newName))
                    {
                        File.Move(oldname, newName);
                        RootFolder.CurrentImageItem.ImagePath = newName;
                        //RootFolder.CurrentImageItem.Save();
                        RootFolder.ReloadPictures();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the m p4 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void CreateMP4_Click()
        {
            bool success = false;
            string destFile = RootFolder.OutputMoviePath();
            RootFolder.HasMovieEntity = false;
            if (RootFolder.CurrentSubFolder.Movies != null)
            {
                // entity exists just add new file
                File.Delete(RootFolder.CurrentSubFolder.Movies.MoviePath);
                File.Move(destFile, RootFolder.CurrentSubFolder.Movies.MoviePath);
                success = true;
                RootFolder.HasMovieEntity = true;
            }
            else
            {
                RootFolder.CurrentSubFolder.Movies = null;
                RootFolder.HasMovieEntity = false;
                PhraseEntry? phrase = DataController.PhraseEntries.Where(p => p.Id == "IMAGES" && p.PhraseID == 1).FirstOrDefault();
                success = await CreateActualMovieFromPath(destFile, phrase, null);
            }
            if (success)
            {
                RootFolder.HasMovieEntity = true;
                CurrentMovie = Support.Support.CreatedMovie;
                if (CurrentMovie != null)
                {
                    RootFolder.CurrentSubFolder.Movies = CurrentMovie;
                    RootFolder.CurrentSubFolder.MovieId = CurrentMovie.Id;
                    if (File.Exists(destFile)) File.Delete(destFile);
                    //File.Move(imageSetViewModel.RootFolder.Movies.MoviePath, destFile);
                    RootFolder.HasTempMP4 = false;
                    RootFolder.CurrentSubFolder.Save();
                }
            }

            // delete image files to tidy
            string[] files = Directory.GetFiles(RootFolder.TempDirectory(), "*.jpg");
            foreach (var item in files)
            {
                File.Delete(item);
            }

            // perhaps should delete temp directory

        }

        /// <summary>
        /// Plays the m p4 click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void PlayMP4_Click()
        {
            if (RootFolder.CurrentSubFolder != null && RootFolder.CurrentSubFolder.Movies != null)
            {
                using PlayerViewModel playerViewModel = new PlayerViewModel(RootFolder.CurrentSubFolder.Movies, true, false);
                using PlayerDialog playerDialog = new PlayerDialog(playerViewModel);

                Avalonia.Controls.Window? main = Support.Support.GetWindow();

                if (main != null)
                    await playerDialog.ShowDialog(main);

                //PlayMP4File(RootFolder.CurrentSubFolder.Movies.MoviePath);
            }
        }

        /// <summary>
        /// Plays the movie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void PlayMovie_Click()
        {
            string tempFileName = RootFolder.OutputMoviePath();
            await PlayFromFile(tempFileName);
        }

        internal async Task PlayFromFile(string tempFileName)
        {
            if (!string.IsNullOrEmpty(tempFileName))
            {
                if (File.Exists(tempFileName))
                {
                    using PlayerViewModel playerViewModel = new PlayerViewModel(tempFileName, true);
                    using TaymadeEntities.PlayerDialog playerDialog = new PlayerDialog(playerViewModel);

                    Window? main = Support.Support.GetWindow();
                    if (main != null)
                    {
                        await playerDialog.ShowDialog(main);
                    }

                    else PlayMP4File(RootFolder.OutputMoviePath());
                }
            }
        }

        public new string? MissingInfo
        {
            get => missingInfo;
            set => this.RaiseAndSetIfChanged(ref missingInfo, value);
        }

        public new int? ProgressPercentShow
        {
            get => progressPercentShow;
            set => this.RaiseAndSetIfChanged(ref progressPercentShow, value);
        }

        public new int? ProgressPercent
        {
            get => progressPercent;
            set => this.RaiseAndSetIfChanged(ref progressPercent, value);
        }
        public Controls.ImageSetControl ImageSetControl { get; set; }
        public Avalonia.Controls.Image CurrentImageControl { get; set; }
        public string OutputVideoPath { get; set; }
        public double Framerate { get; internal set; } = 1.0;



        internal new void Support_ActionCompleted(object sender, MovieCompletedEventArgs e)
        {
            Exception? ex = e.Error;

            if (ex != null)
            {
                MissingInfo = "Not created " + ex.ToString();
            }
            else
            {
                MissingInfo = "created ";
                if (e.Movie != null)
                {
                    MissingInfo += e.Movie.MovieName + " : " + e.MovieId.ToString();
                }
            }
        }

        internal new void Support_ProgressInformation(object sender, MovieProgressEventargs e)
        {
            MissingInfo = e.Info;
            ProgressPercent = e.ProgressPercentage;
            CurrentImage = e.Bitmap;
            if (!string.IsNullOrEmpty(e.BitmapPath))
            {
                DisplayImage(e.BitmapPath);


            }
            Dispatcher.UIThread.Post(() =>
            {

                this.RaisePropertyChanged(nameof(MissingInfo));
                this.RaisePropertyChanged(nameof(CurrentImage));
                this.RaisePropertyChanged(nameof(ProgressPercent));
            }
                );

        }

        private void FFMpeg_CliWrapError(object sender, CliWrapErrorEventArgs e)
        {

        }

        private void FFMpeg_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        {

            MissingInfo = "Completed";
            RootFolder.HasTempMP4 = true; // indicate temporary file 
            PlayFromFile(OutputVideoPath);
            // need to change button visibility
        }

        internal void FFMpeg_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        {
            //con.WriteLine(e.Progress);
            MissingInfo = e.Progress;

            if (e.ProgressPercentage > 0) ProgressPercent = e.ProgressPercentage;
        }

        public delegate void ProgressEventHandler(object sender, MovieProgressEventargs e);

        /// <summary>
        /// Called when [progress].
        /// </summary>
        /// <param name="e">The e.</param>
        /// <autogeneratedoc />
        //protected virtual void OnProgress(MovieProgressEventargs e)
        //{
        //    ProgressEventHandler handler = ProgressInformation;
        //    handler?.Invoke(this, e);
        //}

        /// <summary>
        /// Makes the movie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void MakeMovie_Click()
        {
            TaymadeEntities.Support.Support support = new TaymadeEntities.Support.Support();
            support.ActionCompleted += Support_ActionCompleted;
            //support.ProgressInformation += Support_ProgressInformation;
            MovieProgress += ImageSetViewModel_MovieProgress;
            support.ProgressInformation += ImageSetViewModel_MovieProgress;

            int success = await support.MakeMovieFromImages(this);
            if (success == 0)
            {
                string outputDirectory = RootFolder.TempDirectory();
                string imageFileStub = outputDirectory + @"\temp";
                string outputFileName = outputDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(RootFolder.CurrentSubFolder.Path) + ".mp4";
            }
            else
            {
            }
        }

        private void ImageSetViewModel_MovieProgress(object sender, MovieProgressEventargs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                MissingInfo = e.Info;
                ProgressPercent = e.ProgressPercentage;
                CurrentImage?.Dispose();
                if (e.Bitmap != null)
                    CurrentImage = e.Bitmap;
                else if (!string.IsNullOrEmpty(e.BitmapPath))
                {
                    DisplayImage(e.BitmapPath);
                }


                //this.RaisePropertyChanged(nameof(MissingInfo));
                //this.RaisePropertyChanged(nameof(CurrentImage));
                //this.RaisePropertyChanged(nameof(ProgressPercent));
            }
                );

        }




        public new event MovieProgressEventHandler MovieProgress;
        public delegate void MovieProgressEventHandler(object sender, MovieProgressEventargs e);


        protected virtual void OnMovieProgress(MovieProgressEventargs e)
        {
            MovieProgressEventHandler handler = MovieProgress;
            handler?.Invoke(this, e);
        }

        private void DisplayImage(string tempImageFileName)
        {

            Avalonia.Media.Imaging.Bitmap? tempAVImage;
            Support.Support.SetImageBMP(tempImageFileName, out tempAVImage);
            CurrentImage = tempAVImage;
            this.RaisePropertyChanged(nameof(CurrentImage));
            if (this.CurrentImageControl != null)
                this.CurrentImageControl.Source = tempAVImage;

        }





        /// <summary>
        /// Copies to selected folder.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void CopyToSelectedFolder()
        {
            if (RootFolder != null && RootFolder.CurrentImageItem != null)
            {
                var window = Support.Support.GetMainWindow();
                if (window != null)
                {
                    Uri uri = new Uri(RootFolder.CurrentSubFolder.Path);
                    Avalonia.Platform.Storage.IStorageFolder? currentImageFolder = await window.StorageProvider.TryGetFolderFromPathAsync(uri);
                    // Use StorageProvider API to pick a folder
                    var folder = await window.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
                    {
                        Title = " folder location",
                        SuggestedStartLocation = currentImageFolder
                    });


                    if (folder != null && RootFolder.CurrentImageItem != null)
                    {
                        string oldPath = RootFolder.CurrentImageItem.ImagePath;
                        string newPath = folder[0].Path.LocalPath + System.IO.Path.GetFileName(RootFolder.CurrentImageItem.ImagePath);
                        int idx = 1;

                        // if file exists add a number to it;
                        while (File.Exists(newPath))
                        {
                            string extn = Path.GetExtension(newPath);
                            string stub = System.IO.Path.GetFileNameWithoutExtension(newPath) + "-" + idx.ToString("00");
                            newPath = folder[0].Path.LocalPath + stub + extn;
                        }
                        File.Copy(oldPath, newPath);

                        //selectedFolder.ImageItems.ReloadImageItems(selectedFolder.Path);
                    }
                }
            }
        }

        /// <summary>
        /// Moves to selected folder.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void MoveToSelectedFolder()
        {
            if (RootFolder != null && RootFolder.CurrentImageItem != null)
            {
                var window = Support.Support.GetMainWindow();
                if (window != null)
                {
                    Uri uri = new Uri(RootFolder.CurrentSubFolder.Path);
                    Avalonia.Platform.Storage.IStorageFolder? currentImageFolder = await window.StorageProvider.TryGetFolderFromPathAsync(uri);
                    // Use StorageProvider API to pick a folder
                    var folder = await window.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
                    {
                        Title = " folder location",
                        SuggestedStartLocation = currentImageFolder
                    });



                    if (folder != null && RootFolder.CurrentImageItem != null)
                    {
                        string oldPath = RootFolder.CurrentImageItem.ImagePath;
                        string newPath = folder[0].Path.LocalPath + System.IO.Path.GetFileName(RootFolder.CurrentImageItem.ImagePath);

                        // check new folder exists
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }

                        int idx = 1;
                        while (File.Exists(newPath))
                        {
                            string extn = Path.GetExtension(newPath);
                            string stub = System.IO.Path.GetFileNameWithoutExtension(newPath) + "-" + idx.ToString("00");
                            newPath = folder[0].Path.LocalPath + stub + extn;
                        }
                        if (!File.Exists(newPath) && File.Exists(oldPath))
                        {
                            File.Move(oldPath, newPath);
                            if (RootFolder.CurrentSubFolder != null)
                            {
                                RootFolder.CurrentSubFolder.ImageItems.Remove(RootFolder.CurrentImageItem);
                            }
                            RootFolder.CurrentImageItem.ImagePath = newPath;
                            // selectedFolder.ImageItems.ReloadImageItems(selectedFolder.Path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Previouses the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void PrevButton_Click()
        {
            if (RootFolder != null
                   && RootFolder.CurrentSubFolder.ImageItems != null
                   && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {
                RootFolder.CurrentSubFolder.ImageItems.MovePrev();
            }
        }

        /// <summary>
        /// Selecteds the folder.
        /// </summary>
        /// <returns></returns>
        /// <autogeneratedoc />
        private MovieImage SelectedFolder()
        {
            MovieImage? selectedFolder = null;

            foreach (var imFolder in RootFolder.SubDirectoryList)
            {
                foreach (var subFolder in imFolder.SubDirectoryList)
                {
                    if (subFolder.Selected != null && subFolder.Selected.Value)
                    {
                        selectedFolder = subFolder;
                        break;
                    }
                }
            }

            return selectedFolder;
        }
        /// <summary>
        /// Stops the button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public async void StopButton_Click()
        {
            if (RootFolder != null
                   && RootFolder.CurrentSubFolder.ImageItems != null
                   && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {
                RootFolder.CurrentSubFolder.ImageItems.StopPlaying();
                //CurrentIcon = StopIcon;
            }
        }
        #endregion Public Methods

        #region Internal Methods

        internal async void DoEditPicture()
        {
            string PSP = @"C:\Program Files (x86)\Corel\Corel Paint Shop Pro X\Paint Shop Pro X.exe";



            //clear old image from last image item if it exists
            if (lastImageItem != null && lastImageItem != RootFolder.CurrentImageItem)
            {
                lastImageItem.ImageBMP.Dispose();
                lastImageItem.ImageBMP = null;
            }

            Visual? main = TaymadeEntities.Support.Support.GetMainWindow();
            Window? window = main as Window;
            //update last item
            lastImageItem = RootFolder.CurrentImageItem;
            window.WindowState = WindowState.Minimized;

            // quate image path
            string imagepath = '"' + RootFolder.CurrentImageItem.ImagePath + '"';

            // fire up Monitor to watch for changes to the file and reload it when it is saved

            if (_monitor != null)
            {
                _monitor.Cancel();
                _monitor.Dispose();
            }

            _monitor = new FileMonitor();

            _monitor.FileFound += (_, _) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    RootFolder.CurrentImageItem.ReloadImage();
                });

            };

            _ = _monitor.RunAsync(RootFolder.CurrentImageItem.ImagePath);

            int ec = await FFMpegSupport.DoCliWrap(PSP, imagepath);
            if (ec == 0 && lastImageItem != null)
            {
                lastImageItem.ImageBMP.Dispose();
                lastImageItem.ImageBMP = null;

                Avalonia.Media.Imaging.Bitmap? bmp = lastImageItem.ImageBMP;

                lastImageItem = null;
            }
            window.WindowState = WindowState.Maximized;
        }

        //internal void ImageItems_ImageChanged(object sender, ImageChangedEventArgs e)
        //{
        //    this.RaisePropertyChanged("ImageItems");

        //    if (e != null)
        //    {
        //        if (e.Action != "Stopped")
        //        {
        //            if (RootFolder.CurrentImageItem != null) RootFolder.CurrentImageItem.Selected = false;
        //            RootFolder.CurrentImageFolder.PlayButtonContent = e.Action;
        //            this.RaisePropertyChanged("RootFolder.CurrentImageFolder.PlayButtonContent");
        //            //if (e.ImageItem != null) _ = e.ImageItem.ImageBMP;
        //            RootFolder.CurrentImageItem = e.ImageItem;
        //            RootFolder.CurrentImageItem.Selected = true;

        //            if (RootFolder.CurrentSubFolder != null)
        //            {
        //                RootFolder.CurrentImageFolder.CommandIsVisible = true;

        //                //CurrentSubFolder.CommandIsVisible = true;
        //                RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentSubFolder.ImageItems.Count;
        //            }
        //            else
        //            {
        //                RootFolder.CurrentImageFolder.CommandIsVisible = true;
        //                RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentImageFolder.ImageItems.Count;
        //            }
        //        }
        //        else
        //        {
        //            RootFolder.CurrentImageFolder.CommandIsVisible = false;
        //            RootFolder.CurrentImageFolder.PlayButtonContent = "Play";
        //        }

        //        // calculate perentage using position and count
        //        if (RootFolder.CurrentSubFolder != null && RootFolder.CurrentSubFolder.ImageItems != null && RootFolder.CurrentSubFolder.ImageItems.Count > 0)
        //        {
        //            int percent = (int)(((double)(e.ImagePosition + 1) / (double)RootFolder.CurrentSubFolder.ImageItems.Count) * 100);
        //            // update ISVM property to show percentage
        //            ProgressPercentShow = percent;

        //            // RootFolder.CurrentImageFolder.Info = (e.ImagePosition + 1).ToString() + " of " + RootFolder.CurrentSubFolder.ImageItems.Count + " : " + percent.ToString() + "%";
        //        }

        //    }
        //}

        #endregion Internal Methods

        #region Private Methods

        public void AddPictures()
        {

        }

        public void DeleteSubFolder()
        {
            if (rootFolder != null && rootFolder.CurrentSubFolder != null)
            {
                string? name = rootFolder.CurrentSubFolder.Name;
                // delete CurrentImageFolder from database
                rootFolder.CurrentSubFolder.Delete();
                // remove from SubDirectoryList 
                rootFolder.SubDirectoryList.Remove(rootFolder.CurrentSubFolder);
                // set CurrentImageFolder to null
                rootFolder.CurrentSubFolder = null;
            }
        }

        private async void DoCreateSubFolder()
        {
            if (rootFolder != null && rootFolder.CurrentImageFolder != null)
            {
                // get new name
                EntryDialogModel dialogModel = new EntryDialogModel()
                {
                    EntryTypeValue = EntryDialogModel.EntryType.Text,
                    EntryText = RootFolder.CurrentImageFolder.Path,
                    MaxStringLength = 150
                };


                // create dialog
                EntryDialog entryDialog = new EntryDialog(dialogModel);

                // dialogModel.Caller = entryDialog;

                Visual? main = Support.Support.GetMainWindow();
                DialogResultButton resultButton = await entryDialog.ShowDialog<DialogResultButton>(main as Window);

                if (resultButton != null && resultButton.Result == DialogResultButton.ResultType.Ok)
                {


                    MovieImage newSubFolder = new MovieImage()
                    {
                        Path = resultButton.Paramater,
                        ParentId = RootFolder.CurrentImageFolder.Id
                    };
                    RootFolder.CurrentImageFolder.SubDirectoryList.Add(newSubFolder);
                    newSubFolder.Insert();
                    newSubFolder.Save();
                    RootFolder.CurrentSubFolder = newSubFolder;
                }
            }
        }

        private void DoDeletePicture()
        {
            RootFolder.DoDeletePicture();
            //if (CurrentImageItem != null)
            //{
            //    File.Delete(CurrentImageItem.ImagePath);
            //    int idx = RootFolder.CurrentSubFolder.ImageItems.IndexOf(CurrentImageItem);
            //    RootFolder.CurrentSubFolder.ImageItems.Remove(CurrentImageItem);

            //    if (idx > -1 && idx < RootFolder.CurrentSubFolder.ImageItems.Count)
            //    {
            //        CurrentImageItem = RootFolder.CurrentSubFolder.ImageItems[idx];
            //        CurrentImageItem.Selected = true;
            //    }
            //    else
            //    {
            //        CurrentImageItem = RootFolder.CurrentSubFolder.ImageItems[idx - 1];
            //        CurrentImageItem.Selected = true;
            //    }
            //}
        }

        private void DoReloadPicture()
        {
            RootFolder.CurrentImageItem.ReloadImage();
        }

        public async void DoZoomToFeature(ImageSetControl imageSetControl)
        {
            string filename = string.Empty;
            bool moveFile = false;
            using ViewModels.ZoomPictureViewModel viewModel = new ZoomPictureViewModel(RootFolder.CurrentImageItem.ImagePath);
            {
                using Dialogs.ZoomPictureDialog pictureDialog = new Dialogs.ZoomPictureDialog(viewModel);
                {
                    Window? main = Support.Support.GetMainWindow() as Window;

                    if (main != null)
                    {
                        bool ok = await pictureDialog.ShowDialog<bool>(main);
                        if (ok)
                        {
                            if (viewModel.SaveImageAfterClose)
                            {
                                // need to move viewmodel.output file to current file

                            }
                            DoReloadPictures();
                            DoReloadPicture();
                            ImageSetControl = imageSetControl;
                            SetImageRow();

                        }
                    }
                }
            }
            // check wheter to move file
            if (moveFile)
            {
                //File.Delete(RootFolder.CurrentImageItem.ImagePath);
                //File.Move(filename, RootFolder.CurrentImageItem.ImagePath);

            }
        }

        public void DoReloadPictures()
        {

            RootFolder.ReloadPictures();
            //RootFolder.CurrentImageItem = temp;

        }

        #endregion Private Methods
    }
}
