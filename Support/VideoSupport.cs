//-----------------------------------------------------------------------
// <copyright file="VideoSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>22/12/2018 10:49:32 22/12/2018 10:49:32 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using TaymadeEntities.Models;
    //using MediaToolkit.Standard.Extensions;
    //using MediaToolkit.Standard.Services.Interfaces;
    //using MediaToolkit.Standard.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// A video file class.
    /// </summary>
    public class VideoFile
    {
        #region Fields

        /// <summary>
        /// My path..
        /// </summary>
        private string? myPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoFile"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public VideoFile(string path)
        {
            myPath = path;
            Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AudioFormat value..
        /// </summary>
        public string? AudioFormat { get; set; }

        /// <summary>
        /// Gets or sets the BitRate value..
        /// </summary>
        public double BitRate { get; set; }

        /// <summary>
        /// Gets or sets the Duration value..
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the Height value..
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [information gathered]...
        /// </summary>
        public bool InfoGathered { get; set; }

        /// <summary>
        /// Gets or sets the Path value..
        /// </summary>
        public string? Path { get => myPath; set => myPath = value; }

        /// <summary>
        /// Gets or sets the RawInfo value..
        /// </summary>
        public string? RawInfo { get; set; }

        /// <summary>
        /// Gets or sets the VideoFormat value..
        /// </summary>
        public string? VideoFormat { get; set; }

        /// <summary>
        /// Gets or sets the Width value..
        /// </summary>
        public int Width { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        private void Initialize()
        {
            InfoGathered = false;

            // first make sure we have a value for the video file setting
            if (string.IsNullOrEmpty(myPath))
            {
                throw new Exception("Could not find the location of the video file");
            }

            // Now see if the video file exists
            if (!File.Exists(myPath))
            {
                throw new Exception("The video file " + myPath + " does not exist.");
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="VideoSupport" />.
    /// </summary>
    public class VideoSupport
    {
        #region Fields

        /// <summary>
        /// Defines the thumbnailPath.
        /// </summary>
        public static string? thumbnailPath;

        /// <summary>
        /// My FFMPEG Executable path..
        /// </summary>
        private static string myFFMPEGPath = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";

        /// <summary>
        /// Defines the VLC Namespace..
        /// </summary>
        private static XNamespace vlc = "http://www.videolan.org/vlc/playlist/ns/0/";

        /// <summary>
        /// Defines the XSPF namespace..
        /// </summary>
        private static XNamespace xspf = "http://xspf.org/ns/0/";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSupport"/> class.
        /// </summary>
        public VideoSupport()
        {
            Initialise();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSupport"/> class.
        /// </summary>
        /// <param name="ffmpegExePath">The FFMPEG executable path.</param>
        public VideoSupport(string ffmpegExePath)
        {
            FFMPEGPath = ffmpegExePath;
            Initialise();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the FFMPEGPath value..
        /// </summary>
        public static string FFMPEGPath { get => myFFMPEGPath; set => myFFMPEGPath = value; }

        /// <summary>
        /// Gets or sets the WorkingPath value..
        /// </summary>
        public string? WorkingPath { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The CreateXSPF.
        /// </summary>
        /// <param name="duration">The duration<see cref="double"/>.</param>
        /// <param name="newFilename">The newFilename<see cref="string"/>.</param>
        /// <param name="XSPFilename">The XSPFilename<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string CreateXSPF(double duration, string newFilename, string XSPFilename)
        {
            newFilename = newFilename.Replace(",", " ");  // commas aren't nice in a csv
            Uri myURI = new Uri(newFilename);
            XElement playlist = new XElement(
                                    xspf + "playlist",
                                    new XAttribute("xmlns", xspf),
                                    new XAttribute(XNamespace.Xmlns + "vlc", vlc),
                                    new XAttribute("version", "1"));

            playlist.Add(new XElement(xspf + "title", "Playlist"));
            XElement trackList = new XElement(xspf + "trackList");

            playlist.Add(trackList);
            XElement extension = new XElement(xspf + "extension");
            extension.Add(new XAttribute("application", "http://www.videolan.org/vlc/playlist/0"));
            XElement trackext = new XElement(
                xspf + "extension");
            trackext.Add(new XAttribute("application", "http://www.videolan.org/vlc/playlist/0"));
            trackext.Add(new XElement(vlc + "id", "0"));
            trackext.Add(new XElement(vlc + "option", "bookmarks={name=" + newFilename.ToString() + " #0,bytes=-1,time=0}"));

            XElement track = new XElement(xspf + "track");
            track.Add(new XElement(xspf + "location", myURI.AbsoluteUri));
            trackList.Add(track);

            XElement vlcitem = new XElement(vlc + "item");
            vlcitem.Add(new XAttribute("tid", "0"));
            extension.Add(vlcitem);
            playlist.Add(extension);


            XElement durationXMLElement = new XElement(xspf + "duration", duration.ToString());
            track.Add(durationXMLElement);
            track.Add(trackext);
            if (XSPFilename == string.Empty)
            {
                XSPFilename = "T:\\xspf\\Young\\test.xspf";
            }

            XSPFilename = Support.FixImagePath(XSPFilename);

            playlist.Save(XSPFilename);
            return XSPFilename;
        }

        /// <summary>
        /// The GetDuration.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <param name="newMovie">The newMovie<see cref="Movies"/>.</param>
        public static async void GetDuration(string fileName, Movies newMovie)
        {
            MediaToolkit.Standard.Models.FfProbeOutput output = await VideoSupport.GetMetadataAsync(Support.FixImagePath(fileName));

            if (output != null)
            {
                if (double.TryParse(output.Format.Duration, out double secs))
                {
                    System.TimeSpan ts = System.TimeSpan.FromSeconds(secs);

                    newMovie.DurationSeconds = (int)ts.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// The GetDurationSeconds.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <param name="newMovie">The newMovie<see cref="Movies"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public static async Task<int> GetDurationSecondsAsync(string fileName, Movies? newMovie)
        {
            MediaToolkit.Standard.Models.FfProbeOutput output = await VideoSupport.GetMetadataAsync(Support.FixImagePath(fileName));

            int returnValue = 0;

            if (output != null)
            {
                if (double.TryParse(output.Format.Duration, out double secs))
                {
                    System.TimeSpan ts = System.TimeSpan.FromSeconds(secs);

                    if (newMovie != null) newMovie.DurationSeconds = (int)ts.TotalSeconds;
                    returnValue = (int)ts.TotalSeconds;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// The GetMetadataAsync.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{MediaToolkit.Standard.Models.FfProbeOutput}"/>.</returns>
        //public static async Task<MediaToolkit.Standard.Models.FfProbeOutput> GetMetadataAsync(string moviePath)
        //{
        //    string ffmpegFilePath = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";
        //    string? ffprobeFilePath = null;

        //    string videoPath = Support.FixImagePath(moviePath);

        //    //string dirSep = @"\";

        //    string duration = string.Empty;

        //    MediaToolkit.Standard.Tasks.Results.GetMetadataResult? metadataResult = null;
        //    MediaToolkit.Standard.Models.FfProbeOutput? output = null;

        //    if (!Support.IsWindows())
        //    {
        //        ffmpegFilePath = @"/usr/bin/ffmpeg";
        //        ffprobeFilePath = @"/usr/bin/ffprobe";
        //        //dirSep = "/";
        //    }

        //    try
        //    {
        //        var serviceProvider = new ServiceCollection().AddMediaToolkit(ffmpegFilePath, ffprobeFilePath).BuildServiceProvider();
        //        // Get metadata
        //        var service = serviceProvider.GetService<IMediaToolkitService>();
        //        var metadataTask = new FfTaskGetMetadata(videoPath);


        //        if (service != null)
        //        {
        //            metadataResult = await service.ExecuteAsync(metadataTask);
        //            // var saveThumbnailTask = new FfTaskSaveThumbnail(videoPath, "", TimeSpan.FromSeconds(0));

        //            if (metadataResult != null)
        //            {
        //                output = metadataResult.Metadata;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }

        //    return output;
        //}

        /// <summary>
        /// The GrabBookmarkImage.
        /// </summary>
        /// <param name="Movie">The Movie<see cref="Models.Movies"/>.</param>
        /// <param name="bookmark">The bookmark<see cref="Models.Bookmark"/>.</param>
        /// <param name="offset">The offset<see cref="int"/>.</param>
        public static async Task<string> GrabBookmarkImage(Models.Movies Movie, Models.Bookmark bookmark, int offset = 0)
        {
            string ImagePath = string.Empty;
            if (Movie != null && bookmark != null && bookmark.Time != null)
            {
                string ffmpegFilePath = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";
                string? ffprobeFilePath = null;
                var videoPath = Support.FixImagePath(Movie.MoviePath);
                double tempTime = bookmark.Time.Value;
                string winThumbnailpath = Path.GetDirectoryName(Movie.MoviePath) + @"\" + Path.GetFileNameWithoutExtension(Movie.MoviePath) + tempTime.ToString().Trim() + ".BMP";
                // temporarily add 2 to time
                string dirSep = @"\";
                if (Support.GetOS() != "WinNT")
                {
                    ffmpegFilePath = @"/usr/bin/ffmpeg";
                    ffprobeFilePath = @"/usr/bin/ffprobe";
                    dirSep = "/";
                }
                thumbnailPath = Path.GetDirectoryName(videoPath) + dirSep + Path.GetFileNameWithoutExtension(videoPath) + tempTime.ToString().Trim() + ".BMP";

                if (!File.Exists(thumbnailPath))
                {

                    try
                    {
                        tempTime = tempTime + offset;
                        var serviceProvider = new ServiceCollection().AddMediaToolkit(ffmpegFilePath, ffprobeFilePath).BuildServiceProvider();
                        // Get metadata
                        var service = serviceProvider.GetService<IMediaToolkitService>();
                        var metadataTask = new FfTaskGetMetadata(videoPath);
                        if (service != null)
                        {
                            var metadataResult = await service.ExecuteAsync(metadataTask);
                            var saveThumbnailTask = new FfTaskSaveThumbnail(videoPath, thumbnailPath, TimeSpan.FromSeconds(tempTime));
                            await service.ExecuteAsync(saveThumbnailTask);
                            bookmark.ImagePath = winThumbnailpath;
                            ImagePath = bookmark.ImagePath;
                            Avalonia.Media.Imaging.Bitmap? temp = bookmark.ImageBMP;
                        }
                    }
                    catch (Exception ex)                                                                 
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    bookmark.ImagePath = winThumbnailpath;
                    ImagePath = bookmark.ImagePath;
                    Avalonia.Media.Imaging.Bitmap? temp = bookmark.ImageBMP;
                }
            }

            return ImagePath;
        }

        /// <summary>
        /// Load an image from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>a file Image.</returns>
        public static System.Drawing.Image LoadImageFromFile(string fileName)
        {
            System.Drawing.Image? theImage = null;
            if (File.Exists(fileName))
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[]? img;
                    img = new byte[fileStream.Length];
                    fileStream.Read(img, 0, img.Length);
                    fileStream.Close();
                    theImage = System.Drawing.Image.FromStream(new MemoryStream(img));
                    img = null;
                }
            }

            GC.Collect();
            return theImage;
        }

        /// <summary>
        /// Load an image into a stream from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Memory stream.</returns>
        public static MemoryStream LoadMemoryStreamFromFile(string fileName)
        {
            MemoryStream? ms = null;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] fil;
                fil = new byte[fileStream.Length];
                fileStream.Read(fil, 0, fil.Length);
                fileStream.Close();
                ms = new MemoryStream(fil);
            }

            GC.Collect();
            return ms;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Get the Video info.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>returns a video file.</returns>
        public VideoFile GetVideoInfo(MemoryStream inputFile, string filename)
        {
            VideoFile? vf = null;

            if (!string.IsNullOrEmpty(WorkingPath))
            {
                // Create a temporary file for our use in ffMpeg
                string tempfile = Path.Combine(WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(filename));
                FileStream fs = File.Create(tempfile);

                // write the memory stream to a file and close our the stream so it can be used again.
                inputFile.WriteTo(fs);
                fs.Flush();
                fs.Close();
                GC.Collect();

                // Video File is a class you will see further down this post.  It has some basic information about the video

                try
                {
                    vf = new VideoFile(tempfile);
                }
                catch (Exception)
                {
                    // throw ex;
                }

                // And, without adieu, a call to our main method for this functionality.
                if (vf != null) GetVideoInfo(vf);

                try
                {
                    File.Delete(tempfile);
                }
                catch (Exception)
                {
                }
            }
            return vf;
        }

        /// <summary>
        /// Get the Video Info.
        /// </summary>
        /// <param name="input">The input.</param>
        public void GetVideoInfo(VideoFile input)
        {
            // set up the parameters for video info -- these will be passed into ffMpeg.exe
            string localParams = string.Format("-i {0}", input.Path);
            string output = RunProcess(localParams);
            input.RawInfo = output;

            // Use a regular expression to get the different properties from the video parsed out.
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            Match m = re.Match(input.RawInfo);

            if (m.Success)
            {
                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                {
                    input.Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }

            // get audio bit rate
            re = new Regex("[B|b]itrate:.((\\d|:)*)");
            m = re.Match(input.RawInfo);
            double kb = 0.0;
            if (m.Success)
            {
                double.TryParse(m.Groups[1].Value, out kb);
            }

            input.BitRate = kb;

            // get the audio format
            re = new Regex("[A|a]udio:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.AudioFormat = m.Value;
            }

            // get the video format
            re = new Regex("[V|v]ideo:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.VideoFormat = m.Value;
            }

            // get the video format
            re = new Regex("(\\d{2,3})x(\\d{2,3})");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                int.TryParse(m.Groups[1].Value, out int width);
                int.TryParse(m.Groups[2].Value, out int height);
                input.Width = width;
                input.Height = height;
            }

            input.InfoGathered = true;
        }


        /// <summary>
        /// The GetWorkingFile.
        /// </summary>
        /// <returns>Returns the working file.</returns>
        private string GetWorkingFile()
        {
            // try the stated directory
            if (File.Exists(FFMPEGPath))
            {
                return FFMPEGPath;
            }

            // oops, that didn't work, try the base directory
            if (File.Exists(Path.GetFileName(FFMPEGPath)))
            {
                return Path.GetFileName(FFMPEGPath);
            }

            // well, now we are really unlucky, let's just return null
            return null;
        }

        /// <summary>
        /// The Initialise.
        /// </summary>
        private void Initialise()
        {
            // first make sure we have a value for the ffexe file setting
            if (string.IsNullOrEmpty(FFMPEGPath))
            {
            }

            // Now see if ffmpeg.exe exists
            string workingpath = GetWorkingFile();
            if (string.IsNullOrEmpty(workingpath))
            {
                // ffmpeg doesn't exist at the location stated.
                throw new Exception("Could not find a copy of ffmpeg.exe");
            }

            FFMPEGPath = workingpath;

            // now see if we have a temporary place to work
            if (string.IsNullOrEmpty(WorkingPath))
            {
            }
        }

        /// <summary>
        /// Run the FFMPEG process.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Process output.</returns>
        private string RunProcess(string parameters)
        {
            // create a process info object so we can run our app
            ProcessStartInfo processInfo = new ProcessStartInfo(FFMPEGPath, parameters)
            {
                UseShellExecute = false,
                CreateNoWindow = true,

                // so we are going to redirect the output and error so that we can parse the return
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Create the output and streamreader to get the output
            string? output = null;
            StreamReader? streamOutput = null;

            if (processInfo != null)
            {
                // try the process
                try
                {
                    // run the process
                    Process? proc = Process.Start(processInfo);

                    if (proc != null)
                    {
                        proc.WaitForExit();

                        // get the output
                        streamOutput = proc.StandardError;

                        // now put it in a string
                        output = streamOutput.ReadToEnd();

                        proc.Close();
                    }
                }
                catch (Exception)
                {
                    output = string.Empty;
                }
                finally
                {
                    // now, if we succeded, close out the streamreader
                    if (streamOutput != null)
                    {
                        streamOutput.Close();
                        streamOutput.Dispose();
                    }
                }
            }

            return output;
        }

        #endregion
    }
}
