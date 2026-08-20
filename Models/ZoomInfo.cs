using Avalonia.Input;
using Avalonia.Media.Imaging;
using TaymadeEntities.Dialogs;

namespace TaymadeEntities.Models
{
    public class ZoomInfo
    {
        #region Public Properties

        public PointerPoint? End { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }

        public bool ImageFixed { get; set; } = false;
        public Bitmap? ImageBMP { get; set; }
        public double ImageHeight { get; set; }
        public string? ImagePath { get; set; }
        public double ImageWidth { get; set; }
        public PointerPoint? Start { get; set; }
        public double StartX { get; set; }

        public double StartY { get; set; }

        public int ZoomFrames { get; set; }
        public ZoomPictureDialog? ZoomPictureDialog { get; set; }

        internal ZoomInfo Clone()
        {
            ZoomInfo returnValue = new ZoomInfo()
            {
                End = this.End,
                EndX = this.EndX,
                EndY = this.EndY,
                ImageFixed = this.ImageFixed,
                ImageHeight = this.ImageHeight,
                ImageWidth = this.ImageWidth,
                //ImageBMP = this.ImageBMP,
                Start = this.Start,
                StartY = this.StartY,
                StartX  = this.StartX,
                ZoomFrames = this.ZoomFrames
            };
            return returnValue;
        }

        #endregion Public Properties
    }
}