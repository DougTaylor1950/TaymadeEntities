using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using ReactiveUI;
using TaymadeControls.Buttons;
using TaymadeEntities.Support;
using System.Reactive.Linq;


namespace TaymadeEntities.ViewModels
{
    public class ZoomPictureViewModel : ViewModelBase
    {
        private string? imagePath;
        private string? fixedImagePath;
        private Bitmap? imageBMP;
        private Bitmap? imageBMPConverted;
        private double imageWidth = 1600;
        private double imageHeight = 800;
        private int frames = 10;

        private string startingImagePath = "K:\\DriveF\\Teen\\Girls\\img\\IMG_01817_71.jpg";
        private int step = 5;
        private GammaCorrections? gammaCorrections;
        internal string? outputImagePath;
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

        public void SaveImage()
        {
            // save corrected image 
            
        }

        public string? ImagePath
        {
            get => imagePath;
            private set => this.RaiseAndSetIfChanged(ref imagePath, value);
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

        public double ImageWidth
        {
            get => imageWidth;
            set => this.RaiseAndSetIfChanged(ref imageWidth, value);
        }

        public double ImageHeight
        {
            get => imageHeight;
            set => this.RaiseAndSetIfChanged(ref imageHeight, value);
        }

        public double AspectRatio { get; set; }

        public GammaCorrections? GammaCorrections
        {
            get => gammaCorrections;
            set
            {
                this.RaiseAndSetIfChanged(ref gammaCorrections, value);

            }
        }



        public System.Drawing.Bitmap? SystemBitmap { get; set; }
        public double ImageBorderWidth
        {
            get => imageWidth + 8;

        }

        public double ImageBorderHeight
        {
            get => imageHeight + 8;
        }

        public int Frames
        {
            get => frames;
            set => this.RaiseAndSetIfChanged(ref frames, value);
        }

        public int Step
        {
            get => step;
            set => this.RaiseAndSetIfChanged(ref step, value);
        }
        public bool SaveImageAfterClose { get; set; } = false;
        public Models.MovieImage CurrentSubFolder { get; internal set; }

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

        internal async void UpdateImage()
        {
            if (GammaCorrections == null) return;

            string? corrections = GammaCorrections?.GammaCorrectionString();
            string param = " -y -i " + '"' + fixedImagePath + '"' + " " + corrections + " -c:a copy " + outputImagePath;

           using  FFMpegSupport mpegSupport = new FFMpegSupport();
            {
                int error = await mpegSupport.DoCliWrap(param);

                if (File.Exists(outputImagePath))
                {
                    ImageBMPConverted = Support.Support.GetBMP(outputImagePath);
                }

            }
            
        }
    }
}