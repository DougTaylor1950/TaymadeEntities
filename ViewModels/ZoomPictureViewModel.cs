using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using ReactiveUI;
using TaymadeControls.Buttons;


namespace TaymadeEntities.ViewModels
{
    public class ZoomPictureViewModel : ViewModelBase
    {
        private string? imagePath;
        private Bitmap? imageBMP;
        private double imageWidth = 1600;
        private double imageHeight = 800;
        private int frames = 10;

        private string startingImagePath = @"K:\DriveF\Teen\Girls\GIN\gin080.jpg";
        private int step = 5;

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

        public double ImageWidth
        {
            get => imageWidth;
            set => this.RaiseAndSetIfChanged(ref imageWidth, value); 
        }

        public double ImageHeight 
        { 
            get => imageHeight;
            set => this.RaiseAndSetIfChanged(ref imageHeight , value); 
        }

        public double AspectRatio { get; set; }

        public double ImageBorderWidth
        {
            get => imageWidth+8;
            
        }

        public double ImageBorderHeight
        {
            get => imageHeight +8;
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

        internal void SetupModel()
        {
            if (imageBMP == null && !string.IsNullOrEmpty(ImagePath))
            {
                Support.Support.SetImageBMP(ImagePath, out imageBMP);

                if (imageBMP != null)
                {
                    ImageWidth = imageBMP.Size.Width;
                    ImageHeight = ImageBMP.Size.Height;
                }

                AspectRatio = ImageWidth / ImageHeight;
            }
            this.RaisePropertyChanged(nameof(ImageWidth));
            this.RaisePropertyChanged(nameof(ImageHeight));
        }
    }
}