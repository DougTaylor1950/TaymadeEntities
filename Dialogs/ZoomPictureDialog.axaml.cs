using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DocumentFormat.OpenXml.Drawing.Charts;
using ReactiveUI;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Drawing;
using System.Drawing.Imaging;
using TaymadeEntities.ViewModels;
using Image = Avalonia.Controls.Image;

namespace TaymadeEntities.Dialogs;

public partial class ZoomPictureDialog : WindowBase
{
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

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            this.Close(true);
        }
        );
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            this.Close(false);
        }
        );
    }

    private PointerPoint? start { get; set; }
    public double startX { get; private set; }
    public double startY { get; private set; }
    private PointerPoint? end { get; set; }
    public double endX { get; private set; }
    public double endY { get; private set; }
    private Image pictureImage { get; set; }

    private bool released = false;

    private void Image_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        released = false;
        Image pictureImage = sender as Image;
        start = e.GetCurrentPoint(pictureImage);
        startX = start.Value.Position.X;
        startY = start.Value.Position.Y;
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


    private void DrawRectangle()
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // create a system drawing bitmap 
            System.Drawing.Bitmap temp = new System.Drawing.Bitmap(vm.ImagePath);

            double imageWidth = pictureImage.Width;
            double imageHeight = pictureImage.Height;



            if (start != null && end != null)
            {
                double width = endX - startX;
                double height = endY - startY;

                height = width / vm.AspectRatio;
                // must correct the end positions
                endY = startY + height;
                System.Drawing.Pen solidBrush =
                    new System.Drawing.Pen(System.Drawing.Color.Black);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.DrawRectangle(solidBrush, (int)startX, (int)startY,
                        (int)width, (int)height);
                }
            }
            // recreate av bitmap
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                temp?.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                vm.ImageBMP = new Avalonia.Media.Imaging.Bitmap(memory);
            }
            temp?.Dispose();
        }
    }


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
            string imagePath = Support.Support.FixImagePath(Path.GetDirectoryName(orginalFilename));
            string fileNameStub = Path.GetFileNameWithoutExtension(orginalFilename);

            //for (int i = 0; i < vm.Frames; i++)
            //{
            //Rectangle rectangle = new Rectangle(xStart, yStart, xWidth, yHeight);
            //System.Drawing.Size size = new System.Drawing.Size(xWidth, yHeight);
            //System.Drawing.Bitmap temp = new System.Drawing.Bitmap(vm.ImagePath);

            using (var temp = new Bitmap(vm.ImagePath))
            {
                for (int i = 0; i < vm.Frames; i++)
                {
                    var rect = new Rectangle((int)xStart, (int)yStart, (int)xWidth, (int)yHeight);

                    // Clip to bitmap bounds
                    if (rect.X < 0) { rect.Width += rect.X; rect.X = 0; }
                    if (rect.Y < 0) { rect.Height += rect.Y; rect.Y = 0; }
                    if (rect.X + rect.Width > temp.Width) rect.Width = temp.Width - rect.X;
                    if (rect.Y + rect.Height > temp.Height) rect.Height = temp.Height - rect.Y;

                    if (rect.Width <= 0 || rect.Height <= 0)
                        continue; // skip invalid crop

                    using (var newBitmap = temp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    using (var reSizedImage = Support.Support.ResizeImage(newBitmap, (int)vm.ImageWidth, (int)vm.ImageHeight))
                    {
                        string filename = Path.Combine(imagePath, $"{fileNameStub}-{(i + 1):000}.jpg");
                        reSizedImage.Save(filename, ImageFormat.Jpeg);
                    }
                    xStart += stepX;
                    yStart += stepY;
                    yHeight -= heightStep;
                    xWidth -= widthStep;
                    // update xStart, yStart, xWidth, yHeight as before
                }
                Clear_Click(null, null);
            }
        }
    }

    private void Clear_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        start = null;
        end = null;
        DrawRectangle();
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

    private void Image_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!released)
        {
            pictureImage = sender as Image;
            end = e.GetCurrentPoint(pictureImage);

            DrawRectangle();
        }
    }
}
