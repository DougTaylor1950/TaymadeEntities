using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using TaymadeControls.Buttons;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using Path = System.IO.Path;

namespace TaymadeEntities.ViewModels
{
    public class ZoomPictureViewModel : ViewModelBase
    {
        #region Internal Fields

        internal string? outputImagePath;

        #endregion Internal Fields

        #region Private Fields

        private string? fixedImagePath;
        private int frames = 10;
        private int zoomFrames = 50;
        private GammaCorrections? gammaCorrections;
        private Bitmap? imageBMP;
        private Bitmap? imageBMPConverted;
        private double imageHeight = 800;
        private string? imagePath;
        private double imageWidth = 1600;
        private int progress;
        private string startingImagePath = "K:\\DriveF\\Teen\\Girls\\img\\IMG_01817_71.jpg";
        private int step = 5;
        private HorizontalAlignment? mainImageAlignment = HorizontalAlignment.Center;
        private bool isConvertedImageVisible = false;

        #endregion Private Fields

        public delegate void ProgressEventHandler(object sender, MovieProgressEventargs e);

        public event ProgressEventHandler ProgressInformation;

        protected virtual void OnProgress(MovieProgressEventargs e)
        {
           
                ProgressEventHandler handler = ProgressInformation;
                handler?.Invoke(this, e);
            
        }

        #region Public Constructors

        public ZoomPictureViewModel()
        {
            ImagePath = startingImagePath;
            SetupModel();
        }

        public ZoomPictureViewModel(string? imagePath)
        {
            ImagePath = imagePath;
            SetupModel();
            this.RaisePropertyChanged(nameof(ImageBMP));
        }

        #endregion Public Constructors

        #region Public Properties

        public double AspectRatio { get; set; }

        public bool IsConvertedImageVisible
        {
            get => isConvertedImageVisible;
            set => this.RaiseAndSetIfChanged(ref isConvertedImageVisible, value);
        }

        public Avalonia.Layout.HorizontalAlignment? MainImageAlignment
        {
            get => mainImageAlignment;
            set => this.RaiseAndSetIfChanged(ref mainImageAlignment, value);
        }

        public Models.MovieImage CurrentSubFolder { get; internal set; }

        public int Frames
        {
            get => frames;
            set => this.RaiseAndSetIfChanged(ref frames, value);
        }

        public int ZoomFrames
        {
            get => zoomFrames;
            set => this.RaiseAndSetIfChanged(ref zoomFrames, value);
        }

        public ZoomInfo ZoomInfo { get; set; }

        public GammaCorrections? GammaCorrections
        {
            get => gammaCorrections;
            set
            {
                this.RaiseAndSetIfChanged(ref gammaCorrections, value);
            }
        }

        public Bitmap? ImageBMP
        {
            get
            {
                return imageBMP;
            }

            set => this.RaiseAndSetIfChanged(ref imageBMP, value);
        }

        public Bitmap? ImageBMPConverted
        {
            get
            {
                return imageBMPConverted;
            }

            set => this.RaiseAndSetIfChanged(ref imageBMPConverted, value);
        }

        public double ImageBorderHeight
        {
            get => imageHeight + 8;
        }

        public double ImageBorderWidth
        {
            get => imageWidth + 8;
        }

        public double ImageHeight
        {
            get => imageHeight;
            set => this.RaiseAndSetIfChanged(ref imageHeight, value);
        }

        public string? ImagePath
        {
            get => imagePath;
            private set => this.RaiseAndSetIfChanged(ref imagePath, value);
        }

        public double ImageWidth
        {
            get => imageWidth;
            set => this.RaiseAndSetIfChanged(ref imageWidth, value);
        }

        public int Progress
        {
            get => progress;
            set => this.RaiseAndSetIfChanged(ref progress, value);
        }

        public bool SaveImageAfterClose { get; set; } = false;

        public int Step
        {
            get => step;
            set => this.RaiseAndSetIfChanged(ref step, value);
        }

        public System.Drawing.Bitmap? SystemBitmap { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void CreateInMemoryBitmaps()
        {
            // load the image bytes into memory so the on-disk file is not locked
            var fileBytes = File.ReadAllBytes(ImagePath);

            // create Avalonia Bitmaps from in-memory stream
            using (var ms = new MemoryStream(fileBytes, writable: false))
            {
                imageBMP = new Avalonia.Media.Imaging.Bitmap(ms);
            }

            using (var ms2 = new MemoryStream(fileBytes, writable: false))
            {
                ImageBMPConverted = new Avalonia.Media.Imaging.Bitmap(ms2);
            }

            // create an in-memory System.Drawing.Bitmap copy so it does not lock the file
            using (var ms3 = new MemoryStream(fileBytes, writable: false))
            using (var img = System.Drawing.Image.FromStream(ms3))
            {
                SystemBitmap = new System.Drawing.Bitmap(img);
            }

            this.RaisePropertyChanged(nameof(ImageBMP));
            this.RaisePropertyChanged(nameof(ImageBMPConverted));
        }

        public new void Dispose()
        {
            //this.GammaCorrections?.Dispose();
            this.ImageBMP?.Dispose();
            this.SystemBitmap?.Dispose();
            this.ImageBMPConverted?.Dispose();
            base.Dispose();
        }

        public void SaveGamma()
        {
            // Save settings convert gammacorrection to json file save as movie name +config.json
            if (GammaCorrections != null)
            {
                string folder = Path.GetDirectoryName(fixedImagePath);
                folder = Path.Combine(folder, "temp");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string configPath = Path.Combine(folder, "config.json");
                GammaCorrections.Save(configPath);
            }
        }

        public void SaveImage()
        {
            // save corrected image
        }

        #endregion Public Methods

        #region Internal Methods

        internal void LoadConfig()
        {
            if (GammaCorrections != null)
            {
                string folder = Path.GetDirectoryName(fixedImagePath);
                folder = Path.Combine(folder, "temp");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string configPath = Path.Combine(folder, "config.json");
                if (File.Exists(configPath))
                    GammaCorrections.Load(configPath);
            }
        }
        internal void SetupModel()
        {
            if (imageBMP == null && !string.IsNullOrEmpty(ImagePath))
            {
                CreateInMemoryBitmaps();

                if (imageBMP != null)
                {
                    ImageWidth = imageBMP.Size.Width;
                    ImageHeight = imageBMP.Size.Height;
                }

                AspectRatio = ImageWidth / ImageHeight;

                if (ImageHeight > 800)
                {
                    // we need to scale the image down
                    // new width will be 800 * aspect ratio
                    //using (var newBitmap = new System.Drawing.Bitmap(ImagePath))
                    using (var reSizedImage = Support.Support.ResizeImage(SystemBitmap, (int)(800 * AspectRatio), 800))
                    {
                        //SystemBitmap = reSizedImage;
                        imageBMP = Support.Support.ConvertFileToAvaloniaBitmap(reSizedImage);
                        // convert to Avalonia Image
                    }

                    if (imageBMP != null)
                    {
                        ImageWidth = imageBMP.Size.Width;
                        ImageHeight = imageBMP.Size.Height;
                    }

                    SystemBitmap = Support.Support.ResizeImage(SystemBitmap, (int)(800 * AspectRatio), 800);
                }

                this.RaisePropertyChanged(nameof(ImageWidth));
                this.RaisePropertyChanged(nameof(ImageHeight));
                this.RaisePropertyChanged(nameof(ImageBorderWidth));
                this.RaisePropertyChanged(nameof(ImageBorderHeight));

                this.GammaCorrections = new GammaCorrections();
                GammaCorrections.IsVideo = false;
                GammaCorrections.Correct = true;

                fixedImagePath = Support.Support.FixImagePath(ImagePath);
                string folder = Path.GetDirectoryName(fixedImagePath);
                folder = Path.Combine(folder, "temp");

                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string filename = Path.GetFileName(fixedImagePath);
                outputImagePath = Path.Combine(folder, filename);

                LoadConfig();
                UpdateImage();

                this.GammaCorrections.PropertyChanged += (_, e) =>
                {
                    UpdateImage();
                };
            }
        }
        internal async void UpdateImage()
        {
            if (GammaCorrections == null) return;

            string? corrections = GammaCorrections?.GammaCorrectionString();
            string param = " -y -i " + '"' + fixedImagePath + '"' + " " + corrections + " -c:a copy " + outputImagePath;

            using FFMpegSupport mpegSupport = new FFMpegSupport();
            {
                int error = await mpegSupport.DoCliWrap(param);

                if (File.Exists(outputImagePath))
                {
                    ImageBMPConverted = Support.Support.GetBMP(outputImagePath);
                }
            }
        }

        public async void ZoomClick()
        {
            if (ZoomInfo != null && ZoomInfo.Start != null
                 && ZoomInfo.End != null && ZoomFrames > 0)
            {
                string orginalFilename = ImagePath;
                string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
                imagePath = System.IO.Path.Combine(imagePath, "Zoomed");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                //ImageBMP?.Dispose();
                //ImageBMP = null;
                //var fileBytes = File.ReadAllBytes(imagePath);

                //// create Avalonia Bitmaps from in-memory stream
                //using (var ms = new MemoryStream(fileBytes, writable: false))
                //{
                //    ImageBMP = new Avalonia.Media.Imaging.Bitmap(ms);
                //}

                //if (ZoomInfo != null && ZoomInfo.ZoomPictureDialog != null)
                //    this.ProgressInformation += ZoomInfo.ZoomPictureDialog.Support_ProgressInformation;
                //else
                this.ProgressInformation += Support_ProgressInformation;

                Support.MovieProgressEventargs progressChangedEventArgs = null;
                Support.Support support = new Support.Support();
                support.ZoomPictureViewModel = this;
                support.ProgressInformation += Support_ProgressInformation;
                // ZoomInfo?.ZoomPictureDialog?.Clear_Click(null, null);

                Support.FFMpegSupport fFMpeg = new Support.FFMpegSupport();

                ImageBMPConverted.Dispose();

                // clear out existing images in the zoomed folder
                var files = Directory.GetFiles(imagePath, "*.jpg").ToList();
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                // now rebuild images
                IsConvertedImageVisible = true;
                support.ProgressInformation += Support_ProgressInformation;




                ZoomInfo.ImagePath = imagePath;
                ZoomInfo.ImageWidth = imageWidth;
                ZoomInfo.ImageHeight = imageHeight;

                ZoomInfo.ImageBMP = ImageBMP;
                ZoomInfo.ZoomFrames = ZoomFrames;
                ZoomInfo.ZoomPictureDialog.Clear_Click(this, null);
                ZoomInfo.ImageBMP = ImageBMP;

                bool done = await support.BuildImagesInternal(ZoomInfo, this, imagePath);


                //ZoomInfo.ImageBMP = ImageBMP;

                CurrentSubFolder.FrameSetHeader = DataController.MovieController.GetFrameSetHeaderByMovieImageId(CurrentSubFolder.Id);
                IsConvertedImageVisible = false;

                int maxWidth = CurrentSubFolder.FrameSetHeader.MaxXSize;
                int maxHeight = CurrentSubFolder.FrameSetHeader.MaxYSize;
                if (CurrentSubFolder.FrameSetHeader.MaxXSize == 0 || CurrentSubFolder.FrameSetHeader.MaxYSize == 0)
                {
                    ImageItemsCollection? images = CurrentSubFolder.ImageItems;


                    //(absMaxWidth, absMaxHeight, progressChangedEventArgs, indx,
                    (maxWidth, maxHeight) =
                        await support.GetMaxSizes(progressChangedEventArgs, images);
                    CurrentSubFolder.FrameSetHeader.MaxXSize = maxWidth;
                    CurrentSubFolder.FrameSetHeader.MaxYSize = maxHeight;
                    //currentSubFolder.ToJson();
                    CurrentSubFolder.Save();
                }

                // need to covert these images to a video, 
                ImageItemsCollection? imageItems = new ImageItemsCollection();

                files = Directory.GetFiles(imagePath, "*.jpg").ToList();
                int indx = 1;
                foreach (var file in files)
                {
                    var imageItem = new ImageItem()
                    {
                        ImagePath = file,
                        ImageName = System.IO.Path.GetFileName(file),
                        FrameSetIndex = 0,
                        Selected = false
                    };
                    imageItems.Add(imageItem);
                    progressChangedEventArgs = new Support.MovieProgressEventargs(0, null);
                    progressChangedEventArgs.ProgressPercentage = (indx * 100) / files.Count;
                    progressChangedEventArgs.Info = "building bitmaps";
                    progressChangedEventArgs.Bitmap = imageItem.ImageBMP;
                    System.Threading.Thread.Sleep(50);
                    Progress = (indx * 100) / files.Count;
                    indx += 1;
                }

                if (maxHeight % 2 != 0) maxHeight += 1;
                if (maxWidth % 2 != 0) maxWidth += 1;

                // then we go through all images and save them to a created temp directory 
                // resizing the images to fit 
                System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.WhiteSmoke);

                int count = imageItems.Count;
                double absMaxWidth = 0;
                double absMaxHeight = 0;


                string imageFileStub = imagePath;
                bool success = await support.BuildImages(imageItems, imageFileStub, absMaxWidth,
                    absMaxHeight, null, maxWidth, maxHeight
                        , count);

                if (success)
                {
                    // check to see if the Movies directory exists, if not create it
                    string imageFileDir = System.IO.Path.Combine(imageFileStub, "Movies");

                    if (!Directory.Exists(imageFileDir))
                    {
                        Directory.CreateDirectory(imageFileDir);
                    }

                    string outputFileName = imageFileDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(CurrentSubFolder.Path) + ".mp4";

                    int duration = 5; // default duration (to calculate frame rate)
                    duration = (duration > 0) ? duration : 5;

                    if (CurrentSubFolder.CurrentFrameSet != null)
                    {
                        outputFileName = imageFileDir + "\\FrameSet" + CurrentSubFolder.CurrentFrameSet.Index.ToString("000") + ".mp4";
                        if (CurrentSubFolder.CurrentFrameSet.ZoomDuration != null)
                        {
                            duration = CurrentSubFolder.CurrentFrameSet.ZoomDuration.Value;
                            duration = (duration > 0) ? duration : 5;
                            CurrentSubFolder.CurrentFrameSet.ZoomDuration = duration;
                            CurrentSubFolder.CurrentFrameSet.Save();
                        }

                    }
                    double framerate = imageItems.Count / duration;  // should produce a sub movie lasting 5 seconds

                    //FFMpegSupport fFMpeg = new FFMpegSupport();
                    string ffMpegCommand = " -framerate " + framerate.ToString("0.00") + " -i " + '"' + imageFileStub + "\\" + "%04d.jpg" + '"' + " -c:v libx264 -pix_fmt yuv420p -r 20 " + '"' + outputFileName + '"' + " -y";

                    //Views.MainWindow? main = GetMainWindow();

                    fFMpeg.action = "CreateMovie";
                    fFMpeg.FrameCount = imageItems.Count;

                    int result = await fFMpeg.DoCliWrapCreateMovie(ffMpegCommand);

                    if (result == 0 && CurrentSubFolder.CurrentFrameSet != null)
                    {
                        CurrentSubFolder.CurrentFrameSet.MoviePath = outputFileName;
                        CurrentSubFolder.CurrentFrameSet.HasMovie = true;
                        CurrentSubFolder.Save();

                    }
                    ImageBMPConverted?.Dispose();
                    ImageBMP?.Dispose();
                    // clear out existing images in the zoomed folder
                    files = Directory.GetFiles(imagePath, "*.jpg").ToList();
                    System.Threading.Thread.Sleep(100);
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }

                    // should close this now
                    ZoomInfo?.ZoomPictureDialog?.OkButton_Click(null, null);
                    // really need to reduce frameseet to just the start item and rejig all the following ones
                }
            }
        }





        private void Support_ProgressInformation(object sender, MovieProgressEventargs e)
        {

            if (e != null)
            {
                Progress = e.ProgressPercentage;
                if (e.Bitmap != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        ImageBMPConverted = e.Bitmap;
                        this.RaisePropertyChanged(nameof(Progress));
                        this.RaisePropertyChanged(nameof(ImageBMPConverted));
                    });
                }
                else if (!string.IsNullOrEmpty(e.BitmapPath))
                {
                    ImageBMPConverted = Support.Support.GetBMP(e.BitmapPath);
                    Dispatcher.UIThread.Post(() =>
                    {

                        this.RaisePropertyChanged(nameof(Progress));
                        this.RaisePropertyChanged(nameof(ImageBMPConverted));
                    });

                    System.Threading.Thread.Sleep(50);
                }




            }

        }



        #endregion Internal Methods
    }
}