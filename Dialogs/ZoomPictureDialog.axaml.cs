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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using TaymadeEntities.Models;
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

    private Image? pictureImage { get; set; }

    private PointerPoint? start { get; set; }

    #endregion Private Properties

    #region Private Methods

    private void Build_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        BuildImagesInternal();
    }

    private void BuildImagesInternal(string? imagePathOveride = "",
        Support.Support support = null)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null
                    && end != null && vm.ZoomFrames > 0)
        {
            // get new width and height

            double newWidth = endX - startX;
            double newHeight = endY - startY;

            double scalingX = vm.ImageWidth / newWidth;
            double scalingY = vm.ImageHeight / newHeight;

            double widthStep = (vm.ImageWidth - newWidth) / vm.ZoomFrames;
            double heightStep = (vm.ImageHeight - newHeight) / vm.ZoomFrames;
            double stepX = (startX / vm.ZoomFrames);
            double stepY = (startY / vm.ZoomFrames);

            double xWidth = (vm.ImageWidth - widthStep);

            double xStart = stepX;
            double yStart = stepY;

            double yHeight = (vm.ImageHeight - heightStep);

            string orginalFilename = vm.ImagePath;

            string imagePath = "";
            if (!string.IsNullOrEmpty(imagePathOveride))
            {
                imagePath = Support.Support.FixImagePath(imagePathOveride);
            }
            else
            {
                imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
            }
            string fileNameStub = System.IO.Path.GetFileNameWithoutExtension(orginalFilename);


            Clear_Click(null, null);
            vm.ImageBMPConverted?.Dispose();

            for (int i = 0; i < vm.ZoomFrames; i++)
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
                    // convert image back to avalonia and display
                    var fileBytes = File.ReadAllBytes(filename);
                    using (var ms2 = new MemoryStream(fileBytes, writable: false))
                    {
                        vm.ImageBMPConverted = new Avalonia.Media.Imaging.Bitmap(ms2);
                    }
                    int progress = (i * 100 / vm.ZoomFrames);
                    //this.ConvertedImage.Source = vm.ImageBMPConverted;
                    


                    Support.MovieProgressEventargs movieProgressEventargs =
                        new Support.MovieProgressEventargs(progress, null)
                        {
                            Bitmap = vm.ImageBMPConverted,
                            BitmapPath = filename,
                            ProgressPercentage = progress
                        };

                    OnProgress(movieProgressEventargs);
                    System.Threading.Thread.Sleep(250);

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

    private async void Zoom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        // similar to build click, but only create a single zoomed image based on the rectangle
        if (DataContext is ZoomPictureViewModel vm && start != null
                    && end != null && vm.ZoomFrames > 0)
        {
            string orginalFilename = vm.ImagePath;
            string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
            imagePath = System.IO.Path.Combine(imagePath, "Zoomed");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            Support.MovieProgressEventargs progressChangedEventArgs = null;
            Support.Support support = new Support.Support();
            support.ProgressInformation += Support_ProgressInformation;
            ProgressInformation += Support_ProgressInformation;
            //this.Clear_Click(null, null);

            vm.ImageBMPConverted.Dispose();

            // clear out existing images in the zoomed folder
            var files = Directory.GetFiles(imagePath, "*.jpg").ToList();
            foreach (var file in files)
            {
                File.Delete(file);
            }

            // now rebuild images
            vm.IsConvertedImageVisible = true;
            BuildImagesInternal(imagePath, support);

            vm.CurrentSubFolder.FrameSetHeader = DataController.MovieController.GetFrameSetHeaderByMovieImageId(vm.CurrentSubFolder.Id);
            vm.IsConvertedImageVisible = false;

            int maxWidth = vm.CurrentSubFolder.FrameSetHeader.MaxXSize;
            int maxHeight = vm.CurrentSubFolder.FrameSetHeader.MaxYSize;
            if (vm.CurrentSubFolder.FrameSetHeader.MaxXSize == 0 || vm.CurrentSubFolder.FrameSetHeader.MaxYSize == 0)
            {
                ImageItemsCollection? images = vm.CurrentSubFolder.ImageItems;


                //(absMaxWidth, absMaxHeight, progressChangedEventArgs, indx,
                (maxWidth, maxHeight) =
                    await support.GetMaxSizes(progressChangedEventArgs, images);
                vm.CurrentSubFolder.FrameSetHeader.MaxXSize = maxWidth;
                vm.CurrentSubFolder.FrameSetHeader.MaxYSize = maxHeight;
                //currentSubFolder.ToJson();
                vm.CurrentSubFolder.Save();
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
                System.Threading.Thread.Sleep(150);
                vm.Progress = (indx * 100) / files.Count;
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
            bool success = await support.BuildImages(imageItems, imageFileStub, absMaxWidth, absMaxHeight, progressChangedEventArgs, null, maxWidth, maxHeight
                    , count);

            if (success)
            {
                // check to see if the Movies directory exists, if not create it
                string imageFileDir = System.IO.Path.Combine(imageFileStub, "Movies");

                if (!Directory.Exists(imageFileDir))
                {
                    Directory.CreateDirectory(imageFileDir);
                }

                string outputFileName = imageFileDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(vm.CurrentSubFolder.Path) + ".mp4";

                int duration = 5; // default duration (to calculate frame rate)
                if (vm.CurrentSubFolder.CurrentFrameSet != null)
                {
                    outputFileName = imageFileDir + "\\FrameSet" + vm.CurrentSubFolder.CurrentFrameSet.Index.ToString("000") + ".mp4";
                    if (vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration != null)
                    {
                        duration = vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration.Value;
                        vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration = 5;
                        vm.CurrentSubFolder.CurrentFrameSet.Save();
                    }
                    duration = (duration > 0) ? duration : 5;
                }
                double framerate = imageItems.Count / duration;  // should produce a sub movie lasting 5 seconds

                //FFMpegSupport fFMpeg = new FFMpegSupport();
                string ffMpegCommand = " -framerate " + framerate.ToString("0.00") + " -i " + '"' + imageFileStub + "\\" + "%04d.jpg" + '"' + " -c:v libx264 -pix_fmt yuv420p -r 20 " + '"' + outputFileName + '"' + " -y";

                //Views.MainWindow? main = GetMainWindow();
                Support.FFMpegSupport fFMpeg = new Support.FFMpegSupport();
                fFMpeg.action = "CreateMovie";
                fFMpeg.FrameCount = imageItems.Count;

                int result = await fFMpeg.DoCliWrapCreateMovie(ffMpegCommand);

                if (result == 0 && vm.CurrentSubFolder.CurrentFrameSet != null)
                {
                    vm.CurrentSubFolder.CurrentFrameSet.MoviePath = outputFileName;
                    vm.CurrentSubFolder.CurrentFrameSet.HasMovie = true;
                    vm.CurrentSubFolder.Save();

                }
                vm.ImageBMPConverted?.Dispose();
                vm.ImageBMP?.Dispose();
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
                this.OkButton_Click(null, null);
                // really need to reduce frameseet to just the start item and rejig all the following ones
            }
        }
    }

    private void Support_ProgressInformation1(object sender, Support.MovieProgressEventargs e)
    {
        throw new NotImplementedException();
    }

    public event ProgressEventHandler ProgressInformation;

    public delegate void ProgressEventHandler(object sender, Support.MovieProgressEventargs e);

    protected virtual void OnProgress(Support.MovieProgressEventargs e)
    {
        ProgressEventHandler handler = ProgressInformation;
        handler?.Invoke(this, e);
    }

    private void Support_ProgressInformation(object sender, Support.MovieProgressEventargs e)
    {
        if (this.DataContext != null && this.DataContext is ZoomPictureViewModel zoomPictureView)
        {
            if (e != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    zoomPictureView.Progress = e.ProgressPercentage;
                    if (e.Bitmap != null)
                    {
                        zoomPictureView.ImageBMPConverted = e.Bitmap;

                    }
                    zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.Progress));
                }); zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ImageBMPConverted));
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

    private void DrawRectangle(bool cloneRectangle = false)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // create a system drawing bitmap

            double width = 0;
            double height = 0;
            double imageWidth = pictureImage.Width;
            double imageHeight = pictureImage.Height;

            var rect = new Rectangle(0, 0, vm.SystemBitmap.Width, vm.SystemBitmap.Height);
            using (var newBitmap = vm.SystemBitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                if (start != null && end != null)
                {
                    width = endX - startX;
                    height = endY - startY;

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

                if (cloneRectangle)
                {
                    rect = new Rectangle((int)startX, (int)startY, (int)width, (int)height);
                    using (var newClone = vm.SystemBitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        var reSizedImage = Support.Support.ResizeImage(newClone, (int)imageWidth, (int)imageHeight);
                        using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                        {
                            reSizedImage?.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                            memory.Position = 0;

                            vm.ImageBMPConverted = new Avalonia.Media.Imaging.Bitmap(memory);
                        }
                    }
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
        DrawRectangle(true);
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
            DrawRectangle(true);
        }
    }

    private void Shrink_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            // decrease the rectangle size by the step amount in both directions
            endX -= vm.Step;
            endY -= vm.Step;
            DrawRectangle(true);
        }
    }

    private void MoveDown_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startY += step;
            endY += step;
            DrawRectangle(true);
        }
    }

    private void MoveLeft_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startX -= step;
            endX -= step;
            DrawRectangle(true);
        }
    }

    private void MoveRight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startX += step;
            endX += step;
            DrawRectangle(true);
        }
    }

    private void MoveUp_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm && start != null && end != null)
        {
            int step = vm.Step;
            startY -= step;
            endY -= step;
            DrawRectangle(true);
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

    private void LoadConfig_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.LoadConfig();
        }
    }

    private void ResetToDefault_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.GammaCorrections?.ResetToDefaults();
        }
    }

    private void ConvertedImage_LayoutUpdated(object? sender, System.EventArgs e)
    {
    }

    #endregion Private Methods
}