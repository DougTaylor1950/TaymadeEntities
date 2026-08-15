using Avalonia.Input;
using Avalonia.Media.Imaging;
using TaymadeEntities.Dialogs;

namespace TaymadeEntities.Models
{
    public class ZoomInfo
    {
        public PointerPoint? End { get; set; }
        public PointerPoint? Start { get; set; }

        public double endX { get;  set; }

        public double endY { get;  set; }

        public double startX { get;  set; }

        public double startY { get;  set; }

        public ZoomPictureDialog? ZoomPictureDialog { get; set; }
        public int ZoomFrames { get;  set; }
        public double ImageWidth { get;  set; }
        public double ImageHeight { get;  set; }

        public string? ImagePath { get; set; }
        public Bitmap? ImageBMP { get; set; }
    }
}