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
using Support.Core.Handlers;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using Image = Avalonia.Controls.Image;
using SolidBrush = System.Drawing.SolidBrush;

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

    private bool BuildImagesInternal(string? imagePathOveride = "",
        Support.Support support = null)
    {
        bool success = false;
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

            var temp = Support.Support.ConvertAvaloniaBMPToSystem(vm.ImageBMP);
            for (int i = 0; i < vm.ZoomFrames; i++)
            {
                (bool flowControl, Rectangle rect) = CreateScalingRectangle(xWidth, xStart, yStart, yHeight, temp);
                if (!flowControl)
                {
                    continue;
                }

                string filename = support.BuildBitmap(vm, imagePath, fileNameStub, i, temp, rect);
                xStart += stepX;
                yStart += stepY;
                yHeight -= heightStep;
                xWidth -= widthStep;

                int progress = (i * 100 / vm.ZoomFrames);

                //Support.MovieProgressEventargs movieProgressEventargs =
                //    new Support.MovieProgressEventargs(progress, null)
                //    {
                //        Bitmap = vm.ImageBMPConverted,
                //        BitmapPath = filename,
                //        ProgressPercentage = progress
                //    };

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    vm.RaisePropertyChanged(nameof(vm.Progress));
                    vm.RaisePropertyChanged(nameof(vm.ImageBMPConverted));
                    //vm.RaisePropertyChanged(nameof(vm.ZoomedImage));
                });


                //OnProgress(movieProgressEventargs);
                System.Threading.Thread.Sleep(350);

                // update xStart, yStart, xWidth, yHeight as before

            }
            temp?.Dispose();
            success = true;
        }
        return success;
    }

    private static string BuildBitmap(ZoomPictureViewModel vm, string imagePath, string fileNameStub, int i, Bitmap? temp, Rectangle rect)
    {
        string filename = "";
        using (var newBitmap = temp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        using (var reSizedImage = Support.Support.ResizeImage(newBitmap, (int)temp.Width, (int)temp.Height))
        {
            filename = System.IO.Path.Combine(imagePath, $"{fileNameStub}-{(i + 1):000}.jpg");
            reSizedImage.Save(filename, ImageFormat.Jpeg);
            // convert image back to avalonia and display
            var fileBytes = File.ReadAllBytes(filename);
            using (var ms2 = new MemoryStream(fileBytes, writable: false))
            {
                vm.ImageBMPConverted = new Avalonia.Media.Imaging.Bitmap(ms2);
            }
        }

        return filename;
    }

    private static (bool flowControl, Rectangle value) CreateScalingRectangle(double xWidth, double xStart, double yStart, double yHeight, Bitmap? temp)
    {
        var rect = new Rectangle((int)xStart, (int)yStart, (int)xWidth, (int)yHeight);

        // Clip to bitmap bounds
        if (rect.X < 0) { rect.Width += rect.X; rect.X = 0; }
        if (rect.Y < 0) { rect.Height += rect.Y; rect.Y = 0; }
        if (rect.X + rect.Width > temp.Width) rect.Width = temp.Width - rect.X;
        if (rect.Y + rect.Height > temp.Height) rect.Height = temp.Height - rect.Y;

        if (rect.Width <= 0 || rect.Height <= 0)
            return (flowControl: false, value: default); // skip invalid crop
        return (flowControl: true, value: rect);
    }

    private async void Zoom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // similar to build click, but only create a single zoomed image based on the rectangle
        if (DataContext is ZoomPictureViewModel vm && start != null
                    && end != null && vm.ZoomFrames > 0)
        {
            //vm.Zooming = true;
            string orginalFilename = vm.ImagePath;
            //vm.ZoomedImage = null;
            vm.ImageBMPConverted = null;
            vm.RaisePropertyChanged(nameof(vm.ImageBMPConverted));
           // vm.RaisePropertyChanged(nameof(vm.ZoomedImage));

            string imagePath = Support.Support.FixImagePath(System.IO.Path.GetDirectoryName(orginalFilename));
            imagePath = System.IO.Path.Combine(imagePath, "Zoomed");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            //this.ProgressInformation += ZoomPictureDialog_ProgressInformation;

            Support.MovieProgressEventargs progressChangedEventArgs = null;
            Support.Support support = new Support.Support();
            support.ZoomPictureViewModel = vm;
            //this.Clear_Click(null, null);

            Support.FFMpegSupport fFMpeg = new Support.FFMpegSupport();

            vm.ImageBMPConverted?.Dispose();

            TaymadeEntities.Support.MovieProgressEventargs args = new Support.MovieProgressEventargs(10, null)
            {
                ProgressPercentage = 5,
                Info = "Building",
                BitmapPath = orginalFilename


            };

            OnProgress(args);
            // clear out existing images in the zoomed folder
            var files = Directory.GetFiles(imagePath, "*.jpg").ToList();
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {

                    //   throw;
                }

            }
            ZoomInfo returnValue = null;

            returnValue = vm.ZoomInfo.Clone();
            returnValue.ImageHeight = vm.OriginalImageHeight;
            returnValue.ImageWidth = vm.OriginalImageWidth;
            returnValue.ImageFixed = true;
            returnValue.ZoomFrames = vm.ZoomFrames;
            //Dispatcher.Yield();
            //System.Threading.Thread.Sleep(1000);
            //Dispatcher.UIThread.Post(() =>
            //{
            //    this.Close(returnValue);
            //}
            //);

            //return;

            // now rebuild images
            vm.IsConvertedImageVisible = true;

            bool success = BuildImagesInternal(imagePath, support);
            support.ProgressInformation += ZoomPictureDialog_ProgressInformation;
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

                vm.ImageBMPConverted = imageItem.ImageBMP;

                vm.Progress = (indx * 100) / files.Count;

                Dispatcher.UIThread.Post(() =>
                {
                    vm.RaisePropertyChanged(nameof(vm.Progress));
                    vm.RaisePropertyChanged(nameof(vm.ImageBMPConverted));
                });

                System.Threading.Thread.Sleep(150);
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
            success = BuildImages(imageItems, imageFileStub, absMaxWidth,
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

                string outputFileName = imageFileDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(vm.CurrentSubFolder.Path) + ".mp4";

                int duration = 5; // default duration (to calculate frame rate)
                duration = (duration > 0) ? duration : 5;

                if (vm.CurrentSubFolder.CurrentFrameSet != null)
                {
                    outputFileName = imageFileDir + "\\FrameSet" + vm.CurrentSubFolder.CurrentFrameSet.Index.ToString("000") + ".mp4";
                    if (vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration != null)
                    {
                        duration = vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration.Value;
                        duration = (duration > 0) ? duration : 5;
                        vm.CurrentSubFolder.CurrentFrameSet.ZoomDuration = duration;
                        vm.CurrentSubFolder.CurrentFrameSet.Save();
                    }

                }
                double framerate = imageItems.Count / duration;  // should produce a sub movie lasting 5 seconds

                //FFMpegSupport fFMpeg = new FFMpegSupport();
                string ffMpegCommand = " -framerate " + framerate.ToString("0.00") + " -i " + '"' + imageFileStub + "\\" + "%04d.jpg" + '"' + " -c:v libx264 -pix_fmt yuv420p -r 20 " + '"' + outputFileName + '"' + " -y";

                //Views.MainWindow? main = GetMainWindow();

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

    public bool BuildImages(ImageItemsCollection imageItemsCollection, string imageFileStub,
            double absMaxWidth, double absMaxHeight,
            List<FrameSet>? frameSets, int maxWidth, int maxHeight, int count)
    {
        ZoomPictureViewModel? vm = this.DataContext as ZoomPictureViewModel;
        bool success = true;
        double aspectRatio = 1;


        int index = 1;
        SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.WhiteSmoke);

        foreach (ImageItem item in imageItemsCollection)
        {
            // get existing image
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(item.ImagePath);
            // resize to new consistent size
            System.Drawing.Image reSizedImage = image;

            // find the average colour of image for the borders
            System.Drawing.Color averageColour = Support.Support.GetAverageColorFast(image);

            // create a brush
            solidBrush = new SolidBrush(averageColour);

            // ensure we keep aspect ratio of original image
            aspectRatio = (double)image.Width / (double)image.Height;

            int newHeight = image.Height;
            int newWidth = image.Width;
            // check size the new image will have borders added depending on
            // whether it is portrait or landscape
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
            // create the resized image
            reSizedImage = Support.Support.ResizeImage(image, newWidth, newHeight);

            // if we want to add text it needs to be on the resized image


            // find which dimension is furthest away from target
            int xdif = (maxWidth - reSizedImage.Width) / 2;
            int ydif = (maxHeight - reSizedImage.Height) / 2;
            // create new bitmap of max sizes
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(maxWidth, maxHeight);

            // create a new image with just the background colour
            // then draw the image over it.
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.FillRectangle(solidBrush, 0, 0, maxWidth, maxHeight);
                g.DrawImage(reSizedImage, xdif, ydif, reSizedImage.Width, reSizedImage.Height);
            }


            // save image twice

            // use current image item to determine the number of replicates for the image
            int saveCount = 1;
            if (frameSets != null)
            {
                FrameSet? frameSet = frameSets.Where(f => f.Index == item.FrameSetIndex).FirstOrDefault();
                if (frameSet != null) saveCount = (int)frameSet.FrameRate;
            }
            char slash = '\\';
            char endChar = imageFileStub[imageFileStub.Length - 1];
            if (endChar != '\\')
            {
                imageFileStub += @"\";
            }
            string tempImageFileName = "";
            for (int i = 0; i < saveCount; i++)
            {
                tempImageFileName = imageFileStub + index.ToString("0000") + ".jpg";
                newBitmap.Save(tempImageFileName, ImageFormat.Jpeg);
                index += 1;
            }

            // stop double saving 
            //newBitmap.Save(imageFileStub + index.ToString("0000") + ".jpg", ImageFormat.Jpeg);
            //index += 1;
            // dispose of temporary bitmap
            newBitmap.Dispose();
            newBitmap = null;
            // update progress
            //MovieProgressEventargs progressChangedEventArgs = new MovieProgressEventargs((index * 100) / count, null)
            //{
            vm?.Progress = (index * 100) / count;
            //    Info = "building bitmaps",
            vm?.ImageBMPConverted = Support.Support.ConvertFileToAvaloniaBitmap(tempImageFileName);
            //    //BitmapPath = tempImageFileName
            //};

            //OnProgress(progressChangedEventArgs);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                vm?.RaisePropertyChanged(nameof(vm.Progress));
                vm?.RaisePropertyChanged(nameof(vm.ImageBMPConverted));
            });

            System.Threading.Thread.Sleep(150);
            //await Task.Delay(50);
            solidBrush?.Dispose();
            solidBrush = null;
        }
        return success;
    }

    private void ZoomPictureDialog_ProgressInformation(object sender, Support.MovieProgressEventargs e)
    {
        if (this.DataContext != null && this.DataContext is ZoomPictureViewModel zoomPictureView)
        {
            if (e != null)
            {
                zoomPictureView.Progress = e.ProgressPercentage;
                if (e.Bitmap != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        zoomPictureView.ImageBMPConverted = e.Bitmap;
                        //zoomPictureView.ImageBMPConverted = e.Bitmap;

                        //this.ConvertedImage.UpdateLayout();
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ImageBMPConverted));
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.Progress));
                      //  zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ZoomedImage));
                    });
                    //Dispatcher.Yield();
                }
                else if (!string.IsNullOrEmpty(e.BitmapPath))
                {
                    var fileBytes = File.ReadAllBytes(e.BitmapPath);

                    // create Avalonia Bitmaps from in-memory stream
                    using (var ms = new MemoryStream(fileBytes, writable: false))
                    {
                        zoomPictureView.ImageBMPConverted = new Avalonia.Media.Imaging.Bitmap(ms);
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ImageBMPConverted));

                    }
                }
                System.Threading.Thread.Sleep(150);

            }
        }
    }



    public event ProgressEventHandler ProgressInformation;

    public delegate void ProgressEventHandler(object sender, Support.MovieProgressEventargs e);

    protected virtual void OnProgress(Support.MovieProgressEventargs e)
    {
        ProgressEventHandler handler = ProgressInformation;
        handler?.Invoke(this, e);
    }

    internal void Support_ProgressInformation(object sender, Support.MovieProgressEventargs e)
    {
        if (this.DataContext != null && this.DataContext is ZoomPictureViewModel zoomPictureView)
        {
            if (e != null)
            {
                zoomPictureView.Progress = e.ProgressPercentage;
                if (e.Bitmap != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        zoomPictureView.ImageBMPConverted = e.Bitmap;
                        this.ConvertedImage.Source = e.Bitmap;
                        //Dispatcher.Yield();
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.Progress));
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ImageBMPConverted));

                    });
                }
                else if (!string.IsNullOrEmpty(e.BitmapPath))
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        zoomPictureView.ImageBMPConverted = Support.Support.GetBMP(e.BitmapPath);
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.Progress));
                        zoomPictureView.RaisePropertyChanged(nameof(zoomPictureView.ImageBMPConverted));
                    });
                }
                //System.Threading.Thread.Sleep(150);

            }
        }
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ZoomInfo? returnValue = new ZoomInfo()
        { ImageFixed = false };
        if (DataContext is ZoomPictureViewModel vm)
        {
            returnValue = vm.ZoomInfo.Clone();
            returnValue.ImageFixed = false;
        }

        Dispatcher.UIThread.Post(() =>
        {
            this.Close(returnValue);
        }
        );
    }

    internal void Clear_Click(object? sender, RoutedEventArgs e)
    {
        PointerPoint? tempStart = start;
        PointerPoint? tempEnd = end;
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
        ZoomInfo zoomInfo = new ZoomInfo()
        {
            End = end,
            Start = start,
            EndX = endX,
            EndY = endY,
            StartX = startX,
            StartY = startY,
            ZoomPictureDialog = this
        };
        if (DataContext is ZoomPictureViewModel vm)
        {
            vm.ZoomInfo = zoomInfo;
        }
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
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
            vm.ZoomInfo.StartX = startX;
            vm.ZoomInfo.EndX = endX;
        }
    }

    public void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ZoomInfo? returnValue = new ZoomInfo()
        { ImageFixed = true };
        if (DataContext is ZoomPictureViewModel vm)
        {
            returnValue = vm.ZoomInfo.Clone();
            returnValue.ImageFixed = true;
        }

        Dispatcher.UIThread.Post(() =>
        {
            this.Close(returnValue);
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
        ConvertedImage.LayoutUpdated -= ConvertedImage_LayoutUpdated;
        ConvertedImage.UpdateLayout();
    }

    private void StackPanel_LayoutUpdated(object? sender, System.EventArgs e)
    {
    }

    #endregion Private Methods
}