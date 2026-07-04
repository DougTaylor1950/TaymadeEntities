using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TaymadeEntities.Dialogs;
//using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.Views;
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
using Avalonia;
using TaymadeEntities.ViewModels;
using TaymadeEntities.Models;


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

            UsePicturesAsItems = true;
            CurrentIcon = PlayIcon;

            //MVVM = Support.Support.GetMainWindowViewModel();
        }

       

        #endregion Public Constructors

        #region Public Properties

        public MovieViewModelBase MVVM { get; set; }

        public TaymadeControls.Buttons.ImagedButton PlayButton
        { get; set; }

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
           DialogResultButton resultButton =     await entryDialog.ShowDialog<DialogResultButton>(main as Window);
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
            if (RootFolder.Movies != null)
            {
                // entity exists just add new file
                File.Delete(RootFolder.Movies.MoviePath);
                File.Move(destFile, RootFolder.Movies.MoviePath);
                success = true;
                RootFolder.HasMovieEntity = true;
            }
            else
            {
                RootFolder.Movies = null;
                RootFolder.HasMovieEntity = false;
                success = await CreateActualMovieFromPath(destFile,null,null);
            }
            if (success)
            {
                RootFolder.HasMovieEntity = true;

                RootFolder.Movies = CurrentMovie;

                if (File.Exists(destFile)) File.Delete(destFile);
                //File.Move(imageSetViewModel.RootFolder.Movies.MoviePath, destFile);
                RootFolder.HasMP4 = false;
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
        public void PlayMP4_Click()
        {
            if (RootFolder.CurrentSubFolder != null && RootFolder.CurrentSubFolder.Movies != null)
                PlayMP4File(RootFolder.CurrentSubFolder.Movies.MoviePath);
        }

        /// <summary>
        /// Plays the movie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public void PlayMovie_Click()
        {
            PlayMP4File(RootFolder.OutputMoviePath());
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

        }

        private void FFMpeg_CliWrapError(object sender, CliWrapErrorEventArgs e)
        {

        }

        private void FFMpeg_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        {

            MissingInfo = "Completed";
            RootFolder.HasMP4 = true; // indicate temporary file 

            // need to change button visibility
        }

        internal void FFMpeg_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        {
            //con.WriteLine(e.Progress);
            MissingInfo = e.Progress;

            if (e.ProgressPercentage > 0) ProgressPercent = e.ProgressPercentage;
        }

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
            support.ProgressInformation += Support_ProgressInformation;

            MakeMovieFromImages(this);
        }

        public async Task<int> MakeMovieFromImages(ImageSetViewModel? mainWindowViewModel)
        {
            int error = -1;
            if (mainWindowViewModel != null
               && mainWindowViewModel.RootFolder != null
               && mainWindowViewModel.RootFolder.CurrentSubFolder.ImageItems != null
               && mainWindowViewModel.RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {

                // go through all the images and find maxsizes
                double absMaxWidth = 0;
                double absMaxHeight = 0;



                mainWindowViewModel.MissingInfo = "Building List";
                foreach (ImageItem item in mainWindowViewModel.RootFolder.CurrentSubFolder.ImageItems)
                {
                    if (item.ImageBMP != null && item.ImageBMP.Size.Height > absMaxHeight) absMaxHeight = item.ImageBMP.Size.Height;
                    if (item.ImageBMP != null && item.ImageBMP.Size.Width > absMaxWidth) absMaxWidth = item.ImageBMP.Size.Width;
                }

                // convert to integer
                int maxWidth = (int)absMaxWidth;
                int maxHeight = (int)absMaxHeight;

                // we will have a maximum size of 1024  x 1024

                double aspectRatio = absMaxWidth / absMaxHeight;
                if (maxWidth > 1200 || maxHeight > 1024)
                {
                    if (maxWidth - 1200 > maxHeight - 1024)
                    {
                        maxWidth = 1200;
                        maxHeight = (int)(absMaxHeight / aspectRatio);
                    }
                }

                MovieProgressEventargs progressChangedEventArgs = new MovieProgressEventargs(0, null);
                progressChangedEventArgs.Info = "Creating Images";
                Support_ProgressInformation(null, progressChangedEventArgs);

                int index = 1;
                // need to ensure the values are even 
                if (maxHeight % 2 != 0) maxHeight += 1;
                if (maxWidth % 2 != 0) maxWidth += 1;

                // then we go through all images and save them to a created temp directory 
                // resizing the images to fit 

                string outputDirectory = mainWindowViewModel.RootFolder.TempDirectory();
                string imageFileStub = outputDirectory + @"\temp";
                string outputFileName = outputDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(mainWindowViewModel.RootFolder.CurrentSubFolder.Path) + ".mp4";

                Directory.CreateDirectory(outputDirectory);


                SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.WhiteSmoke);

                int count = mainWindowViewModel.RootFolder.CurrentSubFolder.ImageItems.Count * 2;

                foreach (ImageItem item in mainWindowViewModel.RootFolder.CurrentSubFolder.ImageItems)
                {
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(item.ImagePath);
                    System.Drawing.Image reSizedImage = image;

                    Color averageColour = Support.Support.GetAverageColorFast(image);

                    solidBrush = new SolidBrush(averageColour);


                    aspectRatio = (double)image.Width / (double)image.Height;

                    int newHeight = image.Height;
                    int newWidth = image.Width;
                    // check size
                    if (absMaxWidth - image.Width >= absMaxHeight - image.Height)
                    {

                        newHeight = maxHeight;
                        newWidth = (int)(maxHeight * aspectRatio);
                    }
                    else
                    {
                        newWidth = maxWidth;
                        newHeight = (int)((double)maxWidth / aspectRatio);
                    }

                    reSizedImage = Support.Support.ResizeImage(image, newWidth, newHeight);

                    // find which dimension is furthest away

                    int xdif = (maxWidth - reSizedImage.Width) / 2;
                    int ydif = (maxHeight - reSizedImage.Height) / 2;
                    // create new bitmap of max sizes
                    System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(maxWidth, maxHeight);


                    using (Graphics g = Graphics.FromImage(newBitmap))
                    {
                        g.FillRectangle(solidBrush, 0, 0, maxWidth, maxHeight);
                        g.DrawImage(reSizedImage, xdif, ydif, reSizedImage.Width, reSizedImage.Height);
                    }

                    newBitmap.Save(imageFileStub + index.ToString("0000") + ".jpg", ImageFormat.Jpeg);
                    index += 1;
                    newBitmap.Save(imageFileStub + index.ToString("0000") + ".jpg", ImageFormat.Jpeg);
                    index += 1;

                    newBitmap.Dispose();

                    progressChangedEventArgs.ProgressPercentage = (index * 100) / count;
                    progressChangedEventArgs.Info = "building bitmaps";
                    Support_ProgressInformation(null, progressChangedEventArgs);

                }
                solidBrush.Dispose();

                string ffMpegCommand = " -framerate 1 -i " + '"' + imageFileStub + "%04d.jpg" + '"' + " -c:v libx264 -r 25 " + '"' + outputFileName + '"';

                FFMpegSupport fFMpeg = new FFMpegSupport();
                progressChangedEventArgs.Info = "Creating temp MP4";
                Support_ProgressInformation(null, progressChangedEventArgs);

                mainWindowViewModel.MissingInfo = "Creating temp MP4";

                //Views.MainWindow? main = GetMainWindow();

                fFMpeg.action = "CreateMovie";
                fFMpeg.FrameCount = index * 25;

                fFMpeg.CliWrapCompleted += FFMpeg_CliWrapCompleted;
                fFMpeg.CliWrapError += FFMpeg_CliWrapError;
                fFMpeg.CliWrapProgress += FFMpeg_CliWrapProgress;

                error = await fFMpeg.DoCliWrap(ffMpegCommand);

            }

            return error;
        }



        /// <summary>
        /// Copies to selected folder.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public void CopyToSelectedFolder()
        {
            if (RootFolder != null && RootFolder.CurrentImageItem != null)
            {
                MovieImage? selectedFolder = SelectedFolder();

                if (selectedFolder != null && RootFolder.CurrentImageItem != null)
                {
                    string oldPath = RootFolder.CurrentImageItem.ImagePath;
                    string newPath = selectedFolder.Path + @"\" + System.IO.Path.GetFileName(RootFolder.CurrentImageItem.ImagePath);
                    int idx = 1;

                    // if file exists add a number to it;
                    while (File.Exists(newPath))
                    {
                        string extn = Path.GetExtension(newPath);
                        string stub = System.IO.Path.GetFileNameWithoutExtension(newPath) + "-" + idx.ToString("00");
                        newPath = selectedFolder.Path + @"\" + stub + extn;
                    }
                    File.Copy(oldPath, newPath);

                    selectedFolder.ImageItems.ReloadImageItems(selectedFolder.Path);
                }
            }
        }

        /// <summary>
        /// Moves to selected folder.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        public void MoveToSelectedFolder()
        {
            if (RootFolder != null && RootFolder.CurrentImageItem != null)
            {
                MovieImage? selectedFolder = SelectedFolder();

                if (selectedFolder != null && RootFolder.CurrentImageItem != null)
                {
                    string oldPath = RootFolder.CurrentImageItem.ImagePath;
                    string newPath = selectedFolder.Path + @"\" + System.IO.Path.GetFileName(RootFolder.CurrentImageItem.ImagePath);

                    // check new folder exists
                    if (!Directory.Exists(selectedFolder.Path))
                    {
                        Directory.CreateDirectory(selectedFolder.Path);
                    }

                    int idx = 1;
                    while (File.Exists(newPath))
                    {
                        string extn = Path.GetExtension(newPath);
                        string stub = System.IO.Path.GetFileNameWithoutExtension(newPath) + "-" + idx.ToString("00");
                        newPath = selectedFolder.Path + @"\" + stub + extn;
                    }
                    if (!File.Exists(newPath) && File.Exists(oldPath))
                    {
                        File.Move(oldPath, newPath);
                        if (RootFolder.CurrentSubFolder != null)
                        {
                            RootFolder.CurrentSubFolder.ImageItems.Remove(RootFolder.CurrentImageItem);
                        }
                        RootFolder.CurrentImageItem.ImagePath = newPath;
                        selectedFolder.ImageItems.ReloadImageItems(selectedFolder.Path);
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
                DialogResultButton resultButton =  await entryDialog.ShowDialog<DialogResultButton>(main as Window);

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
        private void DoReloadPictures()
        {
            RootFolder.ReloadPictures();
        }

        #endregion Private Methods
    }
}
