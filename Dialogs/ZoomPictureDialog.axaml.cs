using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Drawing;
using TaymadeEntities.ViewModels;
using Image = Avalonia.Controls.Image;

namespace TaymadeEntities.Dialogs;

public partial class ZoomPictureDialog : WindowBase
{
    public ZoomPictureDialog()
    {
        InitializeComponent();
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

    private PointerPoint start { get; set; }

    private PointerPoint end { get; set; }

    private Image pictureImage { get; set; }

    private void Image_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Image pictureImage = sender as Image;
        start = e.GetCurrentPoint(pictureImage);
    }

    private void Image_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        pictureImage = sender as Image;
        end = e.GetCurrentPoint(pictureImage);

        DrawRectangle();
    }

    private void DrawRectangle()
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // create a system drawing bitmap 
            System.Drawing.Bitmap temp = new System.Drawing.Bitmap(vm.ImagePath);

            double imageWidth = pictureImage.Width;
            double imageHeight = pictureImage.Height;

            System.Drawing.Pen solidBrush = 
                new System.Drawing.Pen(System.Drawing.Color.White);
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawRectangle(solidBrush, (int)start.Position.X, (int)start.Position.Y,
                    (int)(end.Position.X-start.Position.X), (int)(end.Position.Y -start.Position.Y));
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
}