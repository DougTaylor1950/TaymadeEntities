using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using ReactiveUI;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Drawing;
using System.Drawing.Imaging;
using TaymadeEntities.ViewModels;
using Image = Avalonia.Controls.Image;

namespace TaymadeEntities.Dialogs;

public partial class ZoomPictureDialog : WindowBase
{
    #region Private Fields

    private bool released = false;

    #endregion Private Fields

    #region Public Constructors

    public ZoomPictureDialog()
    {
        InitializeComponent();
        this.DataContext = new ZoomPictureViewModel();
    }

    public ZoomPictureDialog(ZoomPictureViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }

    #endregion Public Constructors

    #region Public Properties

    public double endX { get; private set; }

    public double endY { get; private set; }

    public double startX { get; private set; }

    public double startY { get; private set; }

    #endregion Public Properties

    #region Private Properties

    private PointerPoint? end { get; set; }

    private Image pictureImage { get; set; }

    private PointerPoint? start { get; set; }

    #endregion Private Properties

    #region Private Methods

    private void Build_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null
            && end != null && vm.Frames > 0)
        {
            // get new width and height

            double newWidth = endX - startX;
            double newHeight = endY - startY;

            double scalingX = vm.ImageWidth / newWidth;
            double scalingY = vm.ImageHeight / newHeight;

            double widthStep = (vm.ImageWidth - newWidth) / vm.Frames;
            double heightStep = (vm.ImageHeight - newHeight) / vm.Frames;
            double stepX = (startX / vm.Frames);
            double stepY = (startY / vm.Frames);

            //stepX = stepX / vm.Frames;
            //stepY = stepY / vm.Frames;

            double xWidth = (vm.ImageWidth - widthStep);

            double xStart = stepX;
            double yStart = stepY;

            double yHeight = (vm.ImageHeight - heightStep);

            string orginalFilename = vm.ImagePath;
            string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
            string fileNameStub = System.IO.Path.GetFileNameWithoutExtension(orginalFilename);

            //for (int i = 0; i < vm.Frames; i++)
            //{
            //Rectangle rectangle = new Rectangle(xStart, yStart, xWidth, yHeight);
            //System.Drawing.Size size = new System.Drawing.Size(xWidth, yHeight);
            //System.Drawing.Bitmap temp = null;
            Clear_Click(null, null);
            for (int i = 0; i < vm.Frames; i++)
            {
                var temp = Support.Support.ConvertAvaloniaBMPToSystem(vm.ImageBMP);
                var rect = new Rectangle((int)xStart, (int)yStart, (int)xWidth, (int)yHeight);

                // Clip to bitmap bounds
                if (rect.X < 0) { rect.Width += rect.X; rect.X = 0; }
                if (rect.Y < 0) { rect.Height += rect.Y; rect.Y = 0; }
                if (rect.X + rect.Width > temp.Width) rect.Width = temp.Width - rect.X;
                if (rect.Y + rect.Height > temp.Height) rect.Height = temp.Height - rect.Y;

                if (rect.Width <= 0 || rect.Height <= 0)
                    continue; // skip invalid crop

                using (var newBitmap = temp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (var reSizedImage = Support.Support.ResizeImage(newBitmap, (int)temp.Width, (int)temp.Height))
                {
                    string filename = System.IO.Path.Combine(imagePath, $"{fileNameStub}-{(i + 1):000}.jpg");
                    reSizedImage.Save(filename, ImageFormat.Jpeg);
                }
                xStart += stepX;
                yStart += stepY;
                yHeight -= heightStep;
                xWidth -= widthStep;
                // update xStart, yStart, xWidth, yHeight as before
                temp?.Dispose();
            }
               
            
        }
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            this.Close(false);
        }
        );
    }

    private void Clear_Click(object? sender, RoutedEventArgs e)
    {

        start = null;
        end = null;
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.RaisePropertyChanged(nameof(vm.ImageBMP));
        }
        DrawRectangle();
    }

    private void Clip_Click(object? sender, RoutedEventArgs e)
    {
        // clone image from the clipped rectangle 
        if (DataContext is ZoomPictureViewModel vm)
        {
            var rect = new Rectangle(0, 0, vm.SystemBitmap.Width, vm.SystemBitmap.Height);
            using (var newBitmap = vm.SystemBitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                if (start != null && end != null)
                {
                    double width = endX - startX;
                    double height = endY - startY;

                    width = height * vm.AspectRatio;
                    // must correct the end positions
                    endY = startY + height;

                    Rectangle rectClone = new Rectangle((int)startX, (int)startY,
                            (int)width, (int)height);
                    // now clone new bitmap
                    using (var clonedBitmap = newBitmap.Clone(rectClone, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (var reSizedImage = Support.Support.ResizeImage(clonedBitmap, (int)vm.ImageWidth, (int)vm.ImageHeight))
                        {


                            string orginalFilename = vm.ImagePath;
                            string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
                            string fileNameStub = System.IO.Path.GetFileNameWithoutExtension(orginalFilename);

                            // save image to a temporary file first, then replace the original.
                            // This avoids GDI+ "generic error" when the original file is locked.
                            string filename = System.IO.Path.Combine(imagePath, $"{fileNameStub}.jpg");
                            string tempFilename = System.IO.Path.Combine(imagePath, $"{fileNameStub}.tmp.jpg");

                            // Save to temporary file


                            try
                            {
                                reSizedImage.Save(tempFilename, ImageFormat.Jpeg);
                                // try to release the original image handle before replacing
                                ReplaceFile(vm, filename, tempFilename);

                                // rebild bitmaps
                                vm.CreateInMemoryBitmaps();

                            }
                            catch
                            {
                                // fallback: attempt delete + move, otherwise leave temp file for inspection
                                try
                                {
                                    if (System.IO.File.Exists(filename))
                                        System.IO.File.Delete(filename);
                                    System.IO.File.Move(tempFilename, filename);
                                }
                                catch
                                {
                                    vm.SaveImageAfterClose = true;
                                    // swallow to avoid losing the new image; caller can inspect temp file
                                    this.OkButton_Click(null, null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ReplaceFile(ZoomPictureViewModel vm, string filename, string tempFilename)
    {
        try
        {
            // clear Avalonia UI references so controls don't hold native handles
            vm.ImageBMP = null;
            vm.ImageBMPConverted = null;

            // dispose and clear the System.Drawing bitmap
            vm.SystemBitmap?.Dispose();
            vm.SystemBitmap = null;

            // force finalizers to free native resources
            GC.Collect();
            GC.WaitForPendingFinalizers();
            System.Threading.Thread.Sleep(50);
        }
        catch { }

        if (System.IO.File.Exists(filename))
        {
            // atomically replace the existing file
            System.IO.File.Replace(tempFilename, filename, null);
        }
        else
        {
            System.IO.File.Move(tempFilename, filename);
        }
    }

    private void DrawRectangle()
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // create a system drawing bitmap


            double imageWidth = pictureImage.Width;
            double imageHeight = pictureImage.Height;

            var rect = new Rectangle(0, 0, vm.SystemBitmap.Width, vm.SystemBitmap.Height);
            using (var newBitmap = vm.SystemBitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                if (start != null && end != null)
                {
                    double width = endX - startX;
                    double height = endY - startY;

                    width = height * vm.AspectRatio;
                    // must correct the end positions
                    endY = startY + height;
                    endX = startX + width;

                    System.Drawing.Pen solidBrush =
                        new System.Drawing.Pen(System.Drawing.Color.Yellow);
                    using (Graphics g = Graphics.FromImage(newBitmap))
                    {
                        g.DrawRectangle(solidBrush, (int)startX, (int)startY,
                            (int)width, (int)height);
                    }
                }
                // recreate av bitmap
                using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                {
                    newBitmap?.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;

                    vm.ImageBMP = new Avalonia.Media.Imaging.Bitmap(memory);
                }
                //vm.SystemBitmap?.Dispose();
            }
        }
    }

    private void SaveImage(object? sender, RoutedEventArgs e)
    {
        if (DataContext != null && DataContext is ZoomPictureViewModel vm)
        {

            string orginalFilename = vm.ImagePath;
            string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
            string fileNameStub = System.IO.Path.GetFileNameWithoutExtension(orginalFilename);

            // save image to a temporary file first, then replace the original.
            // This avoids GDI+ "generic error" when the original file is locked.
            string filename = System.IO.Path.Combine(imagePath, $"{fileNameStub}.jpg");
            //string tempFilename = System.IO.Path.Combine(imagePath, $"{fileNameStub}.tmp.jpg");

            ReplaceFile(vm, orginalFilename, vm.outputImagePath);
            vm.SaveGamma();
            vm.CreateInMemoryBitmaps();
        }

    }

    private void Image_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!released && sender != null)
        {
            pictureImage = sender as Image;
            end = e.GetCurrentPoint(pictureImage);

            //DrawRectangle();
        }
    }

    private void Image_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        released = false;
        Image pictureImage = sender as Image;
        start = e.GetCurrentPoint(pictureImage);
        startX = start.Value.Position.X;
        startY = start.Value.Position.Y;
        Dispatcher.UIThread.Post(() => { DrawRectangle(); });
        System.Threading.Thread.Sleep(50);
    }

    private void Image_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        pictureImage = sender as Image;
        end = e.GetCurrentPoint(pictureImage);
        endX = end.Value.Position.X;
        endY = end.Value.Position.Y;
        DrawRectangle();
        released = true;
    }

    private bool downright = true;
    private void Stretch_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
        if (DataContext is ZoomPictureViewModel vm)
        {
            if (downright)
            {
                // increase the rectangle size by the step amount in both directions
                endX += vm.Step;
                endY += vm.Step;
            }
            else
            {
                // increase the rectangle size by the step amount in both directions
                startX -= vm.Step;
                startY -= vm.Step;
            }
            downright = !downright;
            DrawRectangle();
        }
    }

    private void Shrink_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // decrease the rectangle size by the step amount in both directions
            endX -= vm.Step;
            endY -= vm.Step;
            DrawRectangle();
        }
    }

    private void MoveDown_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startY += step;
            endY += step;
            DrawRectangle();
        }
    }

    private void MoveLeft_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startX -= step;
            endX -= step;
            DrawRectangle();
        }
    }

    private void MoveRight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startX += step;
            endX += step;
            DrawRectangle();
        }
    }

    private void MoveUp_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startY -= step;
            endY -= step;
            DrawRectangle();
        }
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            this.Close(true);
        }
        );
    }

    private void ApplyConfig_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.UpdateImage();
        }
    }

    private void ResetToDefault_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.GammaCorrections?.ResetToDefaults();
        }
    }

    #endregion Private Methods
}