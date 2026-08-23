//-----------------------------------------------------------------------
// <copyright file="FFMpegSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>14/05/2022 14:53:01 14/05/2022 14:53:01 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using Avalonia.Controls;
    using CliWrap;
    using CliWrap.EventStream;
   // using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TaymadeEntities.Models;
    using TaymadeEntities.ViewModels;

    /// <summary>
    /// Defines the <see cref="FFMpegSupport" />.
    /// </summary>
    public class FFMpegSupport :IDisposable
    {
        #region Constants

        /// <summary>
        /// Defines the OutputPath.
        /// </summary>
        private const string OutputPath = @"t:\white\download\output.txt";

        private const string LoggingLevel = " -loglevel repeat+level+verbose ";

        private const string ReportOn = " -report";
        #endregion

        #region Fields

        /// <summary>
        /// Defines the action.
        /// </summary>
        public string action = string.Empty;

        public CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Defines the Busy.
        /// </summary>
        public bool Busy = false;

        /// <summary>
        /// Defines the elapsedTime.
        /// </summary>
        private static int? elapsedTime;

        /// <summary>
        /// Defines the FFmpegFilePath.
        /// </summary>
        private static string FFmpegFilePath = string.Empty;

        /// <summary>
        /// Defines the ffMpegProc.
        /// </summary>
        private static Process? ffMpegProc;

        /// <summary>
        /// Defines the instance.
        /// </summary>
        private static FFMpegSupport? instance = null;

        /// <summary>
        /// Defines the psi.
        /// </summary>
        private static ProcessStartInfo? psi;

        /// <summary>
        /// Defines the vLCFilePath.
        /// </summary>
        private static string vLCFilePath = string.Empty;

        /// <summary>
        /// Defines the BackgroundWorker.
        /// </summary>
        private System.ComponentModel.BackgroundWorker? BackgroundWorker = null;

        /// <summary>
        /// Defines the extn.
        /// </summary>
        private string extn = string.Empty;

        /// <summary>
        /// Defines the movieId1.
        /// </summary>
        private int movieId1;

        /// <summary>
        /// Defines the onCompletedDelegate.
        /// </summary>
        private SendOrPostCallback onCompletedDelegate;

        /// <summary>
        /// Defines the outputStream.
        /// </summary>
        private StreamWriter? outputStream;

        /// <summary>
        /// Defines the outputVideoPath.
        /// </summary>
        private string outputVideoPath = string.Empty;

        /// <summary>
        /// Defines the processStart.
        /// </summary>
        private DateTime processStart;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FFMpegSupport"/> class.
        /// </summary>
        public FFMpegSupport()
        {
            string os = Support.GetOS();

            if (os == "WinNT")
            {
                VLCFilePath = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
                FFmpegFilePath = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";
                FFProbeFilePath = @"C:\Program Files\FFMpeg\bin\ffprobe.exe";
            }
            else
            {
                VLCFilePath = "/snap/bin/vlc";
                FFmpegFilePath = @"/usr/bin/ffmpeg";
                FFProbeFilePath = @"/usr/bin/ffprobe";
            }

            psi = new ProcessStartInfo(FFmpegFilePath);

            elapsedTime = -1;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The CliWrapCompletedEventHandler.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapCompletedEventArgs"/>.</param>
        public delegate void CliWrapCompletedEventHandler(object sender, CliWrapCompletedEventArgs e);

        /// <summary>
        /// The CliWrapErrorEventHandler.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapErrorEventArgs"/>.</param>
        public delegate void CliWrapErrorEventHandler(object sender, CliWrapErrorEventArgs e);

        /// <summary>
        /// The CliWrapProgressEventHandler.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapProgressEventArgs"/>.</param>
        public delegate void CliWrapProgressEventHandler(object sender, CliWrapProgressEventArgs e);

        /// <summary>
        /// The ConversionCompleteEventHandler
        /// </summary>
        /// <param name="sender">The sender<see cref="Object"/></param>
        /// <param name="e">The e<see cref="ConversionCompleteEventArgs"/></param>
        public delegate void ConversionCompleteEventHandler(Object sender, ConversionCompleteEventArgs e);

        #endregion

        #region Events

        /// <summary>
        /// Defines the CliWrapCompleted.
        /// </summary>
        public event CliWrapCompletedEventHandler CliWrapCompleted;

        /// <summary>
        /// Defines the CliWrapError.
        /// </summary>
        public event CliWrapErrorEventHandler CliWrapError;

        /// <summary>
        /// Defines the CliWrapProgress.
        /// </summary>
        public event CliWrapProgressEventHandler CliWrapProgress;

        /// <summary>
        /// Defines the ConversionComplete.
        /// </summary>
        public event ConversionCompleteEventHandler ConversionComplete;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ErrorString.
        /// </summary>
        public static string ErrorString { get; set; }

        public int FrameCount { get; set; }

        /// <summary>
        /// Gets the ExitCode.
        /// </summary>
        public static int ExitCode { get; private set; }

        /// <summary>
        /// Gets or sets the FfMpegProc.
        /// </summary>
        public static Process? FfMpegProc { get => ffMpegProc; set => ffMpegProc = value; }

        /// <summary>
        /// Gets or sets the ProcessOutput.
        /// </summary>
        public static string? ProcessOutput { get; set; }

        /// <summary>
        /// Gets or sets the VLCFilePath.
        /// </summary>
        public static string VLCFilePath { get => vLCFilePath; set => vLCFilePath = value; }

        /// <summary>
        /// Gets the FFProbeFilePath.
        /// </summary>
        public static string FFProbeFilePath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the LogFile.
        /// </summary>
        public string LogFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MovieName.
        /// </summary>
        public string MovieName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Movies.
        /// </summary>
        public Movies Movies { get; set; }

        /// <summary>
        /// Gets or sets the Output.
        /// </summary>
        public string Output { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the OutputVideoPath.
        /// </summary>
        public string OutputVideoPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Progress.
        /// </summary>
        public TimeSpan Progress { get; set; }

        /// <summary>
        /// Gets or sets the TotalDuration.
        /// </summary>
        public int TotalDuration { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Writer.
        /// </summary>
        public StreamWriter? Writer { get; set; } = null;

        #endregion

        #region Methods

        public void Cancel()
        {
            cts.Cancel();
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }

        public  async Task<string> GrabImage(string? moviePath, string? bookmarkImagePath
            , double? seconds)
        {
            string outimagename = string.Empty;

            TimeSpan ts = TimeSpan.FromSeconds(seconds.Value);

            string command = " -ss " + ts.ToString() + " -i " + '"' + moviePath + '"' + " -frames:v 1 -q:v 2  -update 1 " + '"' +bookmarkImagePath + '"' + " -y";

            int errorcode = await DoCliWrap(command);

            if (errorcode == 0 )
            outimagename = bookmarkImagePath; 

            return outimagename;
        }


        /// <summary>
        /// The GenerateStartInfo.
        /// </summary>
        /// <param name="arguments">The arguments<see cref="string"/>.</param>
        /// <param name="filename">The filename<see cref="string"/>.</param>
        /// <param name="createNoWindow">The createNoWindow<see cref="bool"/>.</param>
        /// <param name="useShellExecute">The useShellExecute<see cref="bool"/>.</param>
        /// <param name="WindowStyle">The WindowStyle<see cref="ProcessWindowStyle"/>.</param>
        /// <param name="redirectStdInput">The redirectStdInput<see cref="bool"/>.</param>
        /// <param name="redirectStdOutput">The redirectStdOutput<see cref="bool"/>.</param>
        /// <param name="redirectStdError">The redirectStdError<see cref="bool"/>.</param>
        /// <returns>The <see cref="ProcessStartInfo"/>.</returns>
        public static ProcessStartInfo GenerateStartInfo(string arguments,
            string filename,
            bool createNoWindow = false,
            bool useShellExecute = false,
            ProcessWindowStyle WindowStyle = ProcessWindowStyle.Normal,
            bool redirectStdInput = false,
            bool redirectStdOutput = false,
            bool redirectStdError = false)
        {
            //windows case
            if (Path.DirectorySeparatorChar == '\\')
            {
                return new ProcessStartInfo
                {
                    Arguments = arguments,
                    FileName = filename,
                    CreateNoWindow = createNoWindow,
                    RedirectStandardInput = redirectStdInput,
                    RedirectStandardOutput = redirectStdOutput,
                    RedirectStandardError = redirectStdError,
                    UseShellExecute = useShellExecute,
                    WindowStyle = WindowStyle
                };
            }
            else //linux case: -nostdin options doesn't exist at least in debian ffmpeg
            {
                return new ProcessStartInfo
                {
                    Arguments = arguments,
                    FileName = filename,
                    CreateNoWindow = createNoWindow,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = useShellExecute,
                    WindowStyle = WindowStyle
                };
            }
        }

        /// <summary>
        /// The GetFFMetaDataPath.
        /// </summary>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetFFMetaDataPath(string source)
        {
            InitialiseVariables();
            source = Support.FixImagePath(source);
            string? path = Path.GetDirectoryName(source);

            string FFMetaDataFile = path + @"\FFMETADATAFILE.txt";

            // fix for linux
            FFMetaDataFile = Support.FixImagePath(FFMetaDataFile);
            return FFMetaDataFile;
        }

        public static async Task<int?> GetMovieDurationAsync(string tempMoviePath = "")
        {
            int? time = null;
            if (string.IsNullOrEmpty(tempMoviePath)) tempMoviePath = tempMoviePath;
            TaymadeEntities.Support.FFProbeInfo? info = await FFMpegSupport.GetFFProbeInfo(tempMoviePath);
            if (info != null && info.Duration != null)
            {
                {
                    TimeSpan duration = TimeSpan.Parse(info.Duration);
                    time = (int)duration.TotalSeconds;
                    
                }
            }

            return time;
        }

        public static async Task<FFProbeInfo?> GetFFProbeInfo(string filename)
        {
            FFProbeInfo? info = new FFProbeInfo();
            if (string.IsNullOrEmpty(filename)) return null;
            if (string.IsNullOrEmpty(FFProbeFilePath))
            {
                FFProbeFilePath = FFProbeFilePath = @"C:\Program Files\FFMpeg\bin\ffprobe.exe";
            }

            var cmd = Cli.Wrap(FFProbeFilePath)
                .WithArguments('"' + filename + '"');

            try
            {
                await foreach (var cmdEvent in cmd.ListenAsync(System.Text.Encoding.Default))
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            Console.WriteLine($"Process started; ID: {started.ProcessId}");
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            // process received data 
                            string output = stdOut.Text;

                            if (output.Contains("Percent="))
                            {
                                ProcessOutput = output;
                            }
                            else if (output.Contains("New Bookmark"))
                            {
                                //DoReloadBookmarks();
                                //CurrentBookmark = CurrentMovieModel.Bookmarks.Last();
                            }
                            else if (output.Contains("Bookmark Image"))
                            {
                                int pos = output.IndexOf("Id=");
                                if (pos >= 0)
                                {
                                    string id = output.Substring(pos + 3);
                                    pos = id.IndexOf("|");
                                    if (pos >= 0)
                                    {
                                        string path = id.Substring(pos + 6);
                                        id = id.Substring(0, pos);

                                        if (int.TryParse(id, out int bmId))
                                        {
                                            //if (CurrentBookmark.Id == bmId)
                                            //{
                                            //    CurrentBookmark.ImagePath = path;
                                            //    CurrentBookmark.SetImageBMP();
                                            //}
                                        }
                                    }
                                }
                            }
                            else
                                ProcessOutput = output;

                            break;
                        case StandardErrorCommandEvent stdErr:

                            //CliWrapProgressEventArgs cliWrapProgress = new CliWrapProgressEventArgs(0, null)
                            //{
                            //    Progress = ProcessOutput,
                            //    TaskName = action
                            //};
                            //OnCliWrapProgress(cliWrapProgress);
                            ProcessOutput = stdErr.Text;
                            if (ProcessOutput.Contains("Duration:"))
                            {
                                int pos = ProcessOutput.IndexOf("Duration:");
                                int endpos = ProcessOutput.IndexOf(",");
                                info.Duration = ProcessOutput.Substring(pos + "Duration: ".Length, endpos - (pos + "Duration: ".Length));
                            }
                            break;
                        case ExitedCommandEvent exited:
                            ExitCode = exited.ExitCode;
                            //errorCode = ExitCode;
                            ProcessOutput = $"Process exited; Code: " + exited.ExitCode.ToString();
                            //CliWrapCompletedEventArgs eventArgs = new CliWrapCompletedEventArgs(null, false, null)
                            //{
                            //    Result = ExitCode,
                            //    TaskName = action,
                            //    MovieName = MovieName
                            //};

                            //OnCliWrapComplete(eventArgs);
                            //DoReloadBookmarks();
                            //CurrentMovieModel.SetPercentUnmarked();
                            break;


                    }
                }

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
            return info;
        }

        /// <summary>
        /// The PlayMovie.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        /// <param name="bookmark">The bookmark<see cref="Bookmark"/>.</param>
        public static void PlayMovie(string moviePath, Bookmark? bookmark = null)
        {
            string arguments = string.Empty;
            InitialiseVariables();

            moviePath = Support.FixImagePath(moviePath);

            //string volumeoff = " --mmdevice-volume=0.01 ";

            // arguments += volumeoff;

            //string marquee = " --sub-source=marq{marquee=" + '"' +"%H:%M:%S" + '"' + ",position=9,color=0xFFFF00,size=12} ";

            if (bookmark != null && bookmark.Time != null && bookmark.Time.Value > 0)
            {
                arguments += '"' + moviePath + '"' + " --start-time=" + bookmark.Time.ToString();
            }
            else arguments += '"' + moviePath + '"';



            psi = GenerateStartInfo(arguments, VLCFilePath);


            FfMpegProc = Process.Start(psi);
        }

        /// <summary>
        /// Trims the movie parameter.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="outputVideoPath">The bookmarkImagePath video path.</param>
        /// <param name="convert">if set to <c>true</c> [convert].</param>
        /// <returns>.</returns>
        public static string TrimMovieParameter(Movies movie, string outputVideoPath, bool convert = true)
        {
            string strParam = " -i \"" + movie.MoviePath + '"';

            if (movie.StartBookmark != null)
            { 
                strParam += "\" -ss " + movie.StartBookmark.FormattedTime;
            }
            if (convert && movie.EndBookmark != null)
                strParam += " -to " + movie.EndBookmark.FormattedTime;
            else
            {
                // calc duration;
                if (movie.StartBookmark != null && movie.EndBookmark != null)
                {
                    TimeSpan start = TimeSpan.Parse(movie.StartBookmark.FormattedTime);
                    TimeSpan end = TimeSpan.Parse(movie.EndBookmark.FormattedTime);

                    TimeSpan ts = end.Subtract(start);

                    string formattedTime = ts.ToString(@"hh\:mm\:ss");
                    strParam += " -autoexit -t " + formattedTime;
                }
            }

            if (convert)
                strParam += " -c:v copy -c:a copy \"" + outputVideoPath + "\"";

            return strParam;
        }

        /// <summary>
        /// The BuildChapterFile.
        /// </summary>
        /// <param name="movie">The movie<see cref="Movies"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> BuildChapterFileAsync(Movies movie)
        {
            bool exitValue = true;
            if (movie.Bookmarks.Count > 0)
            {
                string chapterFile = string.Empty;
                string fixedPath = Support.FixImagePath((movie.MoviePath));

                string metafilePath = GetFFMetaDataPath(movie.MoviePath);
                if (!string.IsNullOrEmpty(metafilePath))
                {
                    // check we have a valid path
                    if (File.Exists(metafilePath))
                    {
                        File.Delete(metafilePath);
                    }

                    // get new file

                    await GetMapDataFileAsync(fixedPath);

                    System.Threading.Thread.Sleep(2000);

                    if (File.Exists(metafilePath))
                    {
                        using (StreamReader streamReader = new StreamReader(metafilePath))
                        {
                            chapterFile = streamReader.ReadToEnd();
                            streamReader.Close();
                        }

                        int chapterPos = chapterFile.IndexOf("[CHAPTER]");

                        // if none zero we need to truncate at this point
                        if (chapterPos > 0)
                        {
                            chapterFile = chapterFile.Substring(0, chapterPos - 1);
                            chapterFile = chapterFile.TrimEnd('\r', '\n');
                            chapterFile += Environment.NewLine;

                            // we need to clear out existing
                            exitValue = await ClearMetaDataAsync(movie.MoviePath);
                            // check to see it is okay.
                            if (exitValue == false) return exitValue;
                        }

                        chapterFile += Environment.NewLine + Environment.NewLine;    // add two line breaks
                                                                                     // go through imagelist

                        double start = 0;
                        Bookmark? movieImage2 = null;


                        List<Bookmark> bookmarks = movie.Bookmarks.Where(x => x.Type.Trim().ToLower() == "bookmark").OrderBy(x => x.Time).ToList();

                        for (int i = 0; i < bookmarks.Count - 1; i++)
                        {




                            Bookmark movieImage1 = bookmarks[i];
                            if (movieImage1.Type.Trim() == "BOOKMARK")
                            {
                                int index = i;
                                movieImage2 = bookmarks[index + 1];



                                if (index == 0 && movieImage1.Time > 0)
                                { start = movieImage1.Time.Value; }

                                chapterFile += "[CHAPTER]" + Environment.NewLine;
                                chapterFile += "TIMEBASE=1/1000" + Environment.NewLine;   // time is in seconds
                                chapterFile += "START=" + (start * 1000).ToString() + Environment.NewLine;
                                start = movieImage2.Time!.Value;
                                chapterFile += "END=" + (start * 1000 - 1).ToString() + Environment.NewLine;
                                chapterFile += "title=" + movieImage1.Name + Environment.NewLine + Environment.NewLine;
                            }
                        }

                        // add closing

                        string lastTitle = "End";
                        if (movieImage2 != null && !string.IsNullOrEmpty(movieImage2.Name))
                        {
                            lastTitle = movieImage2.Name;
                        }

                        if (movie.DurationSeconds == null)
                        {
                            TaymadeEntities.Support.FFProbeInfo? info = await FFMpegSupport.GetFFProbeInfo(movie.MoviePath);
                            if (info != null  && !string.IsNullOrEmpty(info.Duration))
                            {
                                movie.DurationSeconds = int.Parse(info.Duration);
                            }
                           
                            
                        }
                        chapterFile += "[CHAPTER]" + Environment.NewLine;
                        chapterFile += "TIMEBASE=1/1000" + Environment.NewLine;   // time is in seconds
                        chapterFile += "START=" + (start * 1000).ToString() + Environment.NewLine;
                        start = movie.DurationSeconds!.Value;

                        chapterFile += "END=" + (start * 1000 - 1).ToString() + Environment.NewLine;
                        chapterFile += "title=" + lastTitle + Environment.NewLine + Environment.NewLine;


                        chapterFile += "[STREAM]" + Environment.NewLine + "title =" + movie.MovieName + Environment.NewLine + Environment.NewLine;



                        using (StreamWriter streamWriter = new StreamWriter(metafilePath))
                        {
                            streamWriter.WriteLine(chapterFile);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        System.Threading.Thread.Sleep(1000);
                        action = "SetChapters";
                        exitValue = await SetMetadataAsync(movie.MoviePath);

                        if (!exitValue)
                        {

                        }
                    }
                }
            }
            else
            {
                exitValue = false;
            }

            return exitValue;
        }

        /// <summary>
        /// The ClearMetadata.
        /// </summary>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ClearMetadata(string source)
        {
            bool exitValue = true;
            string extn = Path.GetExtension(source);
            string FFMetaDataFile = GetFFMetaDataPath(source);

            try
            {



                string output = Support.FixImagePath(Path.GetDirectoryName(source) + @"\" + Path.GetFileNameWithoutExtension(source) + "1" + extn);

                if (File.Exists(output))
                {
                    File.Delete(output);
                }

                // Process? process = null;

                try
                {
                    source = Support.FixImagePath(source);

                    // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
                    string param = "  -i \"" + source + "\" -c copy  -map_chapters -1  \"" + output + "\"";
                    //action = "METADATA";

                    psi = GenerateStartInfo(param, FFmpegFilePath, false, false, ProcessWindowStyle.Normal);
                    FfMpegProc = Process.Start(psi);

                    // wait until it has finished
                    do
                    {
                        if (FfMpegProc != null) FfMpegProc.Refresh();
                        Debug.Write("-");
                    }
                    while (FfMpegProc != null && !FfMpegProc.HasExited && !FfMpegProc.WaitForExit(500));
                }
                catch (Exception)
                {

                    output = string.Empty;
                    exitValue = false;
                }

                if (File.Exists(output))
                {
                    if (File.Exists(source)) File.Delete(source);

                    File.Move(output, source);
                }
                else exitValue = false;

            }
            catch (Exception)
            {

                exitValue = false;
            }

            return exitValue;
        }

        /// <summary>
        /// The ClearMetaDataAsync.
        /// </summary>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> ClearMetaDataAsync(string source)
        {
            bool exitValue = true;
            string extn = Path.GetExtension(source);
            string FFMetaDataFile = GetFFMetaDataPath(source);

            try
            {

                action = "CLEAR";
                string output = Support.FixImagePath(Path.GetDirectoryName(source) + @"\" + Path.GetFileNameWithoutExtension(source) + "1" + extn);

                if (File.Exists(output))
                {
                    File.Delete(output);
                }


                //Process process = null;

                try
                {
                    source = Support.FixImagePath(source);

                    // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
                    string param = "  -i \"" + source + "\" -c copy  -map_chapters -1  \"" + output + "\"";
                    //action = "METADATA";


                    ExitCode = await DoCliWrap(param);

                    if (ExitCode == 0)
                    {
                        exitValue = true;
                    }
                    else exitValue = false;
                }
                catch (Exception)
                {

                    output = string.Empty;
                    exitValue = false;
                }

                if (File.Exists(output))
                {
                    if (File.Exists(source)) File.Delete(source);

                    File.Move(output, source);
                }
                else exitValue = false;

            }
            catch (Exception)
            {

                exitValue = false;
            }
            return exitValue;
        }

        /// <summary>
        /// The ConvertTo.
        /// </summary>
        /// <param name="file">The file<see cref="string"/>.</param>
        /// <param name="newType">The newType<see cref="string"/>.</param>
        public void ConvertTo(string file, string newType)
        {
            // ffmpeg -i moviePath.mp4 -c:v mpeg2video -qscale:v 2 -c:a mp2 -b:a 192k bookmarkImagePath.mts


            string extn = Path.GetExtension(file);
            outputVideoPath = file.Replace(extn, newType);

            string strParam = " -i \"" + file + "\" -q 0 \"" + outputVideoPath + "\"";
            action = "CONVERT";

            DoFFMpeg(strParam);
        }

        /// <summary>
        /// The ConvertToMP4.
        /// </summary>
        /// <param name="file">The file<see cref="string"/>.</param>
        /// <param name="MovieId">The MovieId<see cref="int"/>.</param>
        public async Task<bool> ConvertToMP4(string? file, int MovieId)
        {
            // ffmpeg -i moviePath.mp4 -c:v mpeg2video -qscale:v 2 -c:a mp2 -b:a 192k bookmarkImagePath.mts
            //string Path_FFMPEG = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";

            bool success = false;

            if (!string.IsNullOrEmpty(file))
                try
                {

                    MovieName = file;
                    action = "CONVERT";
                    extn = Path.GetExtension(file);
                    outputVideoPath = file.Replace(extn, ".mp4");
                    LogFile = file.Replace(extn, ".log");
                    movieId1 = MovieId;

                    string param = "  -i \"" + file + "\" -c:v mpeg2video -qscale:v 2 -c:a mp2 \"" + outputVideoPath + "\" -y";




                    int exitCode = await DoCliWrap(param);

                    success = (ExitCode == 0);

                    //BackgroundWorker = new System.ComponentModel.BackgroundWorker();
                    //BackgroundWorker.WorkerReportsProgress = true;
                    //BackgroundWorker.WorkerSupportsCancellation = true;
                    //BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                    //BackgroundWorker.DoWork += BackgroundWorker_DoWork;
                    //BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    //BackgroundWorker.RunWorkerAsync(file);
                }
                catch (Exception)
                {
                    //throw;
                }
            return success;
        }

        

        /// <summary>
        /// The ConvertToMTS.
        /// </summary>
        /// <param name="movie">The movie<see cref="Movies"/>.</param>
        public async Task<bool> ConvertToMTS(Movies movie)
        {
            bool success = false;
            // ffmpeg -i moviePath.mp4 -c:v mpeg2video -qscale:v 2 -c:a mp2 -b:a 192k bookmarkImagePath.mts

            //string Path_FFMPEG = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";

            InitialiseVariables();

            if (movie != null && !string.IsNullOrEmpty(movie.MoviePath))
            {
                movieId1 = movie.Id;
                string file = movie.MoviePath;
                MovieName = file;
                string extn = Path.GetExtension(file);

                outputVideoPath = file.Replace(extn, ".mts");

                string strParam = " -i \"" + file + "\" -q 0 \"" + outputVideoPath + "\"";
                action = "CONVERT";

                if (File.Exists(outputVideoPath)) File.Delete(OutputVideoPath);
                try
                {
                    int exitCode = await DoCliWrap(strParam);

                    success = (ExitCode == 0);
                    
                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                }
            }
            return success;
        }

        /// <summary>
        /// The DoCliWrapPlay.
        /// </summary>
        /// <param name="param">The param<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public async Task<int> DoCliWrapPlay(string param)
        {
            string filepath = FFmpegFilePath.Replace("ffmpeg", "ffplay");

            int errorCode = 0;

            var cmd = Cli.Wrap(filepath)
                .WithArguments(param);

            try
            {



                await foreach (var cmdEvent in cmd.ListenAsync(System.Text.Encoding.Default))
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            Console.WriteLine($"Process started; ID: {started.ProcessId}");
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            // process received data 
                            string output = stdOut.Text;

                            if (output.Contains("Percent="))
                            {
                                ProcessOutput = output;
                            }
                            else if (output.Contains("New Bookmark"))
                            {
                                //DoReloadBookmarks();
                                //CurrentBookmark = CurrentMovieModel.Bookmarks.Last();
                            }
                            else if (output.Contains("Bookmark Image"))
                            {
                                int pos = output.IndexOf("Id=");
                                if (pos >= 0)
                                {
                                    string id = output.Substring(pos + 3);
                                    pos = id.IndexOf("|");
                                    if (pos >= 0)
                                    {
                                        string path = id.Substring(pos + 6);
                                        id = id.Substring(0, pos);

                                        if (int.TryParse(id, out int bmId))
                                        {
                                            //if (CurrentBookmark.Id == bmId)
                                            //{
                                            //    CurrentBookmark.ImagePath = path;
                                            //    CurrentBookmark.SetImageBMP();
                                            //}
                                        }
                                    }
                                }
                            }
                            else
                                ProcessOutput = output;

                            break;
                        case StandardErrorCommandEvent stdErr:

                            CliWrapProgressEventArgs cliWrapProgress = new CliWrapProgressEventArgs(0, null)
                            {
                                Progress = ProcessOutput,
                                TaskName = action
                            };
                            OnCliWrapProgress(cliWrapProgress);
                            break;
                        case ExitedCommandEvent exited:
                            ExitCode = exited.ExitCode;
                            errorCode = ExitCode;
                            ProcessOutput = $"Process exited; Code: " + exited.ExitCode.ToString();
                            CliWrapCompletedEventArgs eventArgs = new CliWrapCompletedEventArgs(null, false, null)
                            {
                                Result = ExitCode,
                                TaskName = action,
                                MovieName = MovieName
                            };

                            OnCliWrapComplete(eventArgs);
                            //DoReloadBookmarks();
                            //CurrentMovieModel.SetPercentUnmarked();
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                Support.GenerateInfoAndLogMessage("FFMpeg", "Movie", 0, ex.ToString());
                ProcessOutput = $"Process errored ; " + ex.Message + " see log file";
                CliWrapErrorEventArgs cliWrapProgress = new CliWrapErrorEventArgs(ex, null, action);



                OnCliWrapError(cliWrapProgress);
                errorCode = -1;  // indicate there has been an error. 
                //throw;
            }
            return errorCode;
        }


        public async Task<bool> ResetTimestamps(string moviePath)
        {
            bool exitValue = false;

            // ffmpeg -i "moviePath" -fps_mode drop -c copy "bookmarkImagePath"

            string source = moviePath;
            string extn = Path.GetExtension(source);

            string output = Support.FixImagePath(Path.GetDirectoryName(source) + @"\" + Path.GetFileNameWithoutExtension(source) + "temp" + extn);


            string parameter = " -i " + '"' + source + '"' + " -fps_mode drop -c copy " + '"' + output + '"';

            try
            {
                int ErrorCode = await DoCliWrap(parameter);
                exitValue = (ErrorCode == 0);
            }
            catch (Exception)
            {

                throw;
            }

            return exitValue;
        }

        ///// <summary>
        ///// The GetChapterFileAsync.
        ///// </summary>
        ///// <param name="movie">The movie<see cref="Movies"/>.</param>
        ///// <returns>The <see cref="Task{bool}"/>.</returns>
        //public async Task<bool> GetChapterFileAsync(Movies movie)
        //{
        //    bool exitValue = true;
        //    if (movie.Bookmarks.Count > 0)
        //    {
        //        string chapterFile = string.Empty;
        //        string fixedPath = Support.FixImagePath((movie.MoviePath));

        //        string metafilePath = GetFFMetaDataPath(movie.MoviePath);
        //        if (!string.IsNullOrEmpty(metafilePath))
        //        {

        //            // check we have a valid path

        //            if (File.Exists(metafilePath))
        //            {
        //                File.Delete(metafilePath);
        //            }

        //            // get new file

        //            exitValue = await GetMapDataFileAsync(fixedPath);

        //            if (exitValue)
        //            {

        //                System.Threading.Thread.Sleep(2000);

        //                if (File.Exists(metafilePath))
        //                {
        //                    using (StreamReader streamReader = new StreamReader(metafilePath))
        //                    {
        //                        chapterFile = streamReader.ReadToEnd();
        //                        streamReader.Close();
        //                    }

        //                    string seperator = "[CHAPTER]";
        //                    char[] delims = new[] { '\r', '\n', '=' };

        //                    int chapterPos = chapterFile.IndexOf(seperator);

        //                    if (chapterPos > 0)
        //                    {
        //                        chapterFile = chapterFile.Substring(chapterPos);


        //                        string[] chapters = chapterFile.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);

        //                        // might be an idea to move the parsing to using Chapters

        //                        foreach (string chapter in chapters)
        //                        {
        //                            Chapter chapter1 = new Chapter(chapter);

        //                            //string[] elements = chapter.Split(delims, StringSplitOptions.RemoveEmptyEntries);

        //                            //double time = 0;

        //                            //if (elements.Length == 8)
        //                            //{
        //                            //if (elements[0] == "TIMEBASE" && elements[1] == "1/1000" && elements[2] == "START")
        //                            //{
        //                            //    time = int.Parse(elements[3]);
        //                            //    time = time / 1000;
        //                            //}

        //                            //string name = elements[7];


        //                            if (movie != null && movie.Bookmarks != null && chapter1.Found)
        //                            {
        //                                Bookmark? bookmark = movie.Bookmarks.Where(x => x.TruncTime == chapter1.Time).FirstOrDefault();

        //                                if (bookmark == null)
        //                                {
        //                                    bookmark = new Bookmark()
        //                                    {
        //                                        Time = chapter1.Time,
        //                                        Name = chapter1.Title,
        //                                        MovieID = movie.Id,
        //                                        Type = "BOOKMARK"
        //                                    };

        //                                    bookmark.Insert();
        //                                    movie.Bookmarks.Add(bookmark);

        //                                }
        //                                else
        //                                {
        //                                    bookmark.Time = chapter1.Time;
        //                                    bookmark.Save();
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //                // }

        //            }
        //        }
        //    }
        //    else
        //    {
        //        exitValue = false;
        //    }

        //    return exitValue;
        //}

        /// <summary>
        /// The GetMapDataFile.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        public void GetMapDataFile(string moviePath)
        {
            string extn = Path.GetExtension(moviePath);
            string FFMetaDataFile = GetFFMetaDataPath(moviePath);
            string output = Path.GetDirectoryName(moviePath) + @"\" + Path.GetFileNameWithoutExtension(moviePath) + "1" + extn;

            // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
            string param = "  -i \"" + moviePath + "\" -f ffmetadata \"" + FFMetaDataFile + "\"";
            //action = "METADATA";"METADATA";

            psi = GenerateStartInfo(param, FFmpegFilePath, false, false, ProcessWindowStyle.Normal);

            FfMpegProc = Process.Start(psi);

            if (FfMpegProc != null)
                // wait until it has finished
                do
                {
                    FfMpegProc.Refresh();
                    Debug.Write("-");
                }
                while (!FfMpegProc.HasExited && !FfMpegProc.WaitForExit(500));
        }

        /// <summary>
        /// The GetMapDataFileAsync.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> GetMapDataFileAsync(string moviePath)
        {
            string extn = Path.GetExtension(moviePath);
            string FFMetaDataFile = GetFFMetaDataPath(moviePath);
            string output = Path.GetDirectoryName(moviePath) + @"\" + Path.GetFileNameWithoutExtension(moviePath) + "1" + extn;

            // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
            string param = "  -i \"" + moviePath + "\" -f ffmetadata \"" + FFMetaDataFile + "\"";

            int exitCode = await DoCliWrap(param);

            return (exitCode == 0);
        }

        /// <summary>
        /// The JoinProcess.
        /// </summary>
        /// <param name="strParam">The strParam<see cref="string"/>.</param>
        /// <param name="actionOverride">The actionOverride<see cref="string"/>.</param>
        public async void JoinProcess(string strParam, string actionOverride = "JOIN")
        {
            try
            {
                action = actionOverride;
                int ErrorCode = await DoCliWrap(strParam);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
        }

        /// <summary>
        /// The SetMetadata.
        /// </summary>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool SetMetadata(string source)
        {
            bool exitValue = true;
            string extn = Path.GetExtension(source);
            string FFMetaDataFile = GetFFMetaDataPath(source);

            try
            {



                string output = Support.FixImagePath(Path.GetDirectoryName(source) + @"\" + Path.GetFileNameWithoutExtension(source) + "1" + extn);

                if (File.Exists(output))
                {
                    File.Delete(output);
                }

                //Process process = null;

                try
                {
                    source = Support.FixImagePath(source);

                    // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
                    string param = "  -i \"" + source + "\" -i \"" + FFMetaDataFile + "\" -map_metadata l -codec copy  \"" + output + "\"";
                    //action = "METADATA";

                    //DoCliWrap(param);
                    psi = GenerateStartInfo(param, FFmpegFilePath, false, false, ProcessWindowStyle.Normal);
                    FfMpegProc = Process.Start(psi);

                    if (FfMpegProc != null)
                        // wait until it has finished
                        do
                        {
                            FfMpegProc.Refresh();
                            Debug.Write("-");
                        }
                        while (!FfMpegProc.HasExited && !FfMpegProc.WaitForExit(500));

                    // null out process

                    //if (ExitCode == 0)
                    //{
                    //    exitValue = true;
                    //}
                    //else exitValue = false;
                }
                catch (Exception)
                {

                    output = string.Empty;
                    exitValue = false;
                }

                if (File.Exists(output))
                {
                    if (File.Exists(source)) File.Delete(source);

                    File.Move(output, source);
                }
                else exitValue = false;

            }
            catch (Exception)
            {

                exitValue = false;
            }

            return exitValue;
        }

        /// <summary>
        /// The SetMetadataAsync.
        /// </summary>
        /// <param name="source">The source<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public async Task<bool> SetMetadataAsync(string source)
        {
            bool exitValue = true;
            string extn = Path.GetExtension(source);
            string FFMetaDataFile = GetFFMetaDataPath(source);
            string? appPath = Support.GetApplicationPathFromDB("AvalonMVM");

            try
            {



                string output = Support.FixImagePath(Path.GetDirectoryName(source) + @"\" + Path.GetFileNameWithoutExtension(source) + "1" + extn);

                if (File.Exists(output))
                {
                    File.Delete(output);
                }

                //Process process = null;

                try
                {
                    source = Support.FixImagePath(source);
                        
                    // -i INPUT -i FFMETADATAFILE -map_metadata 1 -codec copy OUTPUT
                    string param = "  -i \"" + source + "\" -i \"" + FFMetaDataFile + "\" -map_metadata l -codec copy  \"" + output + "\"" + ReportOn;
                    
                    action = "SetChapters";

                   ExitCode = await DoCliWrap(param);

                    if (ExitCode == 0)
                    {
                        exitValue = true;
                        if (File.Exists(output))
                        {
                            if (File.Exists(source)) File.Delete(source);

                            File.Move(output, source);

                            // delete all log files

                            if (!string.IsNullOrEmpty(appPath))
                            {
                                string[] logs = Directory.GetFiles(appPath, "ffmpeg*.log");

                                foreach (var item in logs)
                                {
                                    File.Delete(item);
                                }
                            }

                        }
                        else exitValue = false;
                    }
                    else exitValue = false;

                }
                catch (Exception ex1)
                {
                    string errorlog = "Error Log file in " + appPath + " " + ex1.ToString();
                    output = errorlog;
                    exitValue = false;
                }



            }
            catch (Exception)
            {

                exitValue = false;
            }

            return exitValue;
        }

        /// <summary>
        /// The TrimMovie.
        /// </summary>
        /// <param name="movie">The movie<see cref="Movies"/>.</param>
        /// <param name="parameter">The parameter<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public async Task<int> TrimMovie(Movies movie, string parameter = "")
        {
            // ffmpeg -i moviePath.mp4 -c:v mpeg2video -qscale:v 2 -c:a mp2 -b:a 192k bookmarkImagePath.mts
            // ffmpeg -i moviePath.mp4 -ss 01:10:27 -to 02:18:51 -c:v copy -c:a copy bookmarkImagePath.mp4

            //string Path_FFMPEG = @"C:\Program Files\FFMpeg\bin\ffmpeg.exe";

            string extn = Path.GetExtension(movie.MoviePath);

            string inputVideoPath = Support.FixImagePath(movie.MoviePath); 

            outputVideoPath = Support.FixImagePath(movie.MoviePath.Replace(extn, "temp" + extn));
            // -c:v libx264 -preset slow -crf 22 
            string strParam = " -i " + '"' + inputVideoPath + '"'; //+ parameter +  " -c:v libx264 -preset slow -crf 22 -c:a copy " + '"' + outputVideoPath + '"';

            //string; //strParam = TrimMovieParameter(movie, outputVideoPath, true);

            if (!string.IsNullOrEmpty(parameter))
            {
                strParam += " " + parameter + " -c:v libx264 -preset slow -crf 22 -c:a copy " + '"' + outputVideoPath + '"';
            }

            action = "TRIM";

            try
            {

                int ErrorCode = await DoCliWrap(strParam);
                return ErrorCode;

            }
            catch (Exception ex)
            {
                string error = ex.ToString();

                return -1;
            }
        }

        /// <summary>
        /// The InitializeDelegates.
        /// </summary>
        protected virtual void InitializeDelegates()
        {
        }

        /// <summary>
        /// The OnCliWrapComplete.
        /// </summary>
        /// <param name="e">The e<see cref="CliWrapCompletedEventArgs"/>.</param>
        protected virtual void OnCliWrapComplete(CliWrapCompletedEventArgs e)
        {
            Busy = false;
            CliWrapCompletedEventHandler handler = CliWrapCompleted;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The OnCliWrapError.
        /// </summary>
        /// <param name="e">The e<see cref="CliWrapErrorEventArgs"/>.</param>
        protected virtual void OnCliWrapError(CliWrapErrorEventArgs e)
        {
            Busy = false;
            CliWrapErrorEventHandler handler = CliWrapError;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The OnCliWrapProgress.
        /// </summary>
        /// <param name="e">The e<see cref="CliWrapProgressEventArgs"/>.</param>
        protected virtual void OnCliWrapProgress(CliWrapProgressEventArgs e)
        {
            CliWrapProgressEventHandler handler = CliWrapProgress;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The OnConversionComplete.
        /// </summary>
        /// <param name="e">The e<see cref="ConversionCompleteEventArgs"/>.</param>
        protected virtual void OnConversionComplete(ConversionCompleteEventArgs e)
        {
            Busy = false;
            ConversionCompleteEventHandler handler = ConversionComplete;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The DoFFMpeg.
        /// </summary>
        /// <param name="strParam">The strParam<see cref="string"/>.</param>
        /// <returns>The <see cref="Process"/>.</returns>
        private static Process DoFFMpeg(string strParam)
        {
            InitialiseVariables();
            Process ffmpeg = new Process();
            try
            {
                // processStart = DateTime.Now;

                ProcessStartInfo ffmpeg_StartInfo = new ProcessStartInfo(FFmpegFilePath, strParam);
                ffmpeg_StartInfo.UseShellExecute = true;
                ffmpeg_StartInfo.RedirectStandardError = false;
                ffmpeg_StartInfo.RedirectStandardOutput = false;
                ffmpeg.StartInfo = ffmpeg_StartInfo;
                ffmpeg_StartInfo.CreateNoWindow = false;
                //ffmpeg.Exited += new EventHandler(Process_Exited);
                ffmpeg.EnableRaisingEvents = true;
                ffmpeg.Start();
                //ffmpeg.WaitForExit();
                //ffmpeg.WaitForExit(5000000);
                //StreamReader myStreamReader = ffmpeg.StandardError;
                //ffmpeg.WaitForExit();

                //RefreshUnbound_Click(null, null);

                return ffmpeg;

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// The DoFFMpegWait.
        /// </summary>
        /// <param name="strParam">The strParam<see cref="string"/>.</param>
        /// <param name="forTime">The forTime<see cref="int"/>.</param>
        private static void DoFFMpegWait(string strParam, int forTime = 0)
        {
            InitialiseVariables();
            try
            {
                //processStart = DateTime.Now;
                Process ffmpeg = new Process();
                ProcessStartInfo ffmpeg_StartInfo = new ProcessStartInfo(FFmpegFilePath, strParam);
                ffmpeg_StartInfo.UseShellExecute = true;
                ffmpeg_StartInfo.RedirectStandardError = false;
                ffmpeg_StartInfo.RedirectStandardOutput = false;
                ffmpeg.StartInfo = ffmpeg_StartInfo;
                ffmpeg_StartInfo.CreateNoWindow = false;
                //ffmpeg.Exited += new EventHandler(Process_Exited);
                //ffmpeg.EnableRaisingEvents = true;
                ffmpeg.Start();
                if (forTime == 0)
                {
                    ffmpeg.WaitForExit();
                }
                else
                {
                    ffmpeg.WaitForExit(forTime);
                }
                //ffmpeg.WaitForExit(5000000);
                //StreamReader myStreamReader = ffmpeg.StandardError;
                //ffmpeg.WaitForExit();

                //RefreshUnbound_Click(null, null);

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
        }

        /// <summary>
        /// The initialise.
        /// </summary>
        private static void InitialiseVariables()
        {
            if (instance == null)
            {
                instance = new FFMpegSupport();
            }
        }

        /// <summary>
        /// The BackgroundWorker_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.DoWorkEventArgs"/>.</param>
        private void BackgroundWorker_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            InitialiseVariables();
            string? file = e.Argument!.ToString();
            string strParam = " -i \"" + file + "\" -q 0 \"" + outputVideoPath + "\" -report -progress \"" + LogFile + "\"  ";

            try
            {
                Busy = true;
                processStart = DateTime.Now;
                ffMpegProc = new Process();
                ProcessStartInfo ffmpeg_StartInfo = new ProcessStartInfo(FFmpegFilePath, strParam);
                ffmpeg_StartInfo.UseShellExecute = false;
                ffmpeg_StartInfo.RedirectStandardError = false;
                ffmpeg_StartInfo.RedirectStandardOutput = false;
                ffMpegProc.OutputDataReceived += Ffmpeg_OutputDataReceived;
                ffMpegProc.StartInfo = ffmpeg_StartInfo;
                ffmpeg_StartInfo.CreateNoWindow = false;
                ffMpegProc.Exited += Process_Exited;
                ffMpegProc.EnableRaisingEvents = true;
                ffMpegProc.Start();
                // bookmarkImagePath = ffmpeg.StandardError.ReadToEnd();
                //ffmpeg.BeginOutputReadLine();


                //ffmpeg.WaitForExit();
                //ffmpeg.WaitForExit(5000000);
                //StreamReader myStreamReader = ffmpeg.StandardError;
                //ffmpeg.WaitForExit();

                //RefreshUnbound_Click(null, null);

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }

            while (Busy)
            {
                try
                {


                    System.Threading.Thread.Sleep(20000);
                    string data = string.Empty;
                    //using (StreamReader sr = new StreamReader(LogFile))
                    //{
                    //    data = sr.ReadToEnd();
                    //    sr.Close();
                    //}
                    using (FileStream stream = File.Open(LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            while (!reader.EndOfStream)
                            {
                                data = reader.ReadToEnd();
                                int pos = data.LastIndexOf("out_time=");
                                string value = data.Substring(pos);
                                pos = value.IndexOf((char)10);
                                if (pos > 0)
                                {
                                    value = value.Substring(9, pos - 9);
                                    if (TimeSpan.TryParse(value, out TimeSpan progres))
                                    {
                                        Progress = progres;
                                        Debug.WriteLine(Progress.ToString());

                                        elapsedTime = (int)progres.TotalSeconds;
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                    string error = ex.ToString();
                }
            }
        }

        /// <summary>
        /// The BackgroundWorker_ProgressChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.ProgressChangedEventArgs"/>.</param>
        private void BackgroundWorker_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
        }

        /// <summary>
        /// The BackgroundWorker_RunWorkerCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/>.</param>
        private void BackgroundWorker_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
        }

        internal static async Task<int> DoCliWrap(string commandPath, string param)
        {
            int errorCode = 0;

           // var cts = new CancellationTokenSource();
            //TimeSpan cancelDelay = TimeSpan.FromSeconds(15000);

           // cts.CancelAfter(cancelDelay);

            var cmd = Cli.Wrap(commandPath)
                .WithArguments(param);
            try
            {
                CommandResult result = await cmd.ExecuteAsync();

                errorCode = result.ExitCode;
            }
            catch (Exception ex)
            {

                string error = ex.ToString();
            }

               

            return errorCode;
        }

        /// <summary>
        /// The DoCliWrap.
        /// </summary>
        /// <param name="param">The param<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        internal async Task<int> DoCliWrap(string param)
        {
            int errorCode = 0;

            

            // timeout extended as it was cancelling on setting chapters
            TimeSpan cancelDelay = TimeSpan.FromSeconds(400);

            if (action.ToUpper() == "METADATA") cancelDelay = TimeSpan.FromSeconds(1500);

            if (action.ToLower() == "join") cancelDelay = TimeSpan.FromSeconds(1500);
            if (action.ToLower() == "convert") cancelDelay = TimeSpan.FromSeconds(6000);

            if (action.ToLower() == "trim" || action.ToLower() == "createmovie") cancelDelay = TimeSpan.FromSeconds(15000);


            cts.CancelAfter(cancelDelay);

            var cmd = Cli.Wrap(FFmpegFilePath)
                .WithArguments(param);
            
            try
            {

                await foreach (var cmdEvent in cmd.ListenAsync(System.Text.Encoding.Default, cts.Token))
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            Console.WriteLine($"Process started; ID: {started.ProcessId}");
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            // process received data 
                            string output = stdOut.Text;

                            if (output.Contains("Percent="))
                            {
                                ProcessOutput = output;
                            }
                            else if (output.Contains("Frame="))
                            {
                                ProcessOutput = output;
                            }
                            else if (output.Contains("New Bookmark"))
                            {
                                //DoReloadBookmarks();
                                //CurrentBookmark = CurrentMovieModel.Bookmarks.Last();
                            }
                            else if (output.Contains("Bookmark Image"))
                            {
                                int pos = output.IndexOf("Id=");
                                if (pos >= 0)
                                {
                                    string id = output.Substring(pos + 3);
                                    pos = id.IndexOf("|");
                                    if (pos >= 0)
                                    {
                                        string path = id.Substring(pos + 6);
                                        id = id.Substring(0, pos);

                                        if (int.TryParse(id, out int bmId))
                                        {
                                            //if (CurrentBookmark.Id == bmId)
                                            //{
                                            //    CurrentBookmark.ImagePath = path;
                                            //    CurrentBookmark.SetImageBMP();
                                            //}
                                        }
                                    }
                                }
                            }
                            else
                                ProcessOutput = output;

                            break;
                        case StandardErrorCommandEvent stdErr:
                            int PercentProgress = 0;
                            if (action == "CreateMovie")
                            {
                                int? frame = ExtractFrame(stdErr.Text);

                                if (frame != null && FrameCount > 0)
                                {
                                    PercentProgress = (frame.Value * 100) / FrameCount;
                                }
                            }
                            else
                            {
                                int? seconds = ExtractTime(stdErr.Text);

                                if (seconds != null && Movies != null && Movies.DurationSeconds != null)
                                {
                                    PercentProgress = (seconds.Value * 100) / Movies.DurationSeconds.Value;
                                }
                                else if (seconds != null && TotalDuration > 0)
                                {
                                    PercentProgress = (seconds.Value * 100) / TotalDuration;
                                }
                            }

                            ProcessOutput = $"Err> {stdErr.Text}";

                            if (ProcessOutput.Contains("Error")) ErrorString = ProcessOutput;
                            Debug.WriteLine(stdErr.Text);
                            CliWrapProgressEventArgs cliWrapProgress = new CliWrapProgressEventArgs(PercentProgress, null)
                            {
                                Progress = ProcessOutput,
                                TaskName = action
                            };
                            OnCliWrapProgress(cliWrapProgress);
                            break;
                        // cancellation token triggered
                        
                            

                        case ExitedCommandEvent exited:
                            //if (exited.)
                            //{
                            //    ProcessOutput = "Process cancelled by user";
                            //    CliWrapErrorEventArgs cliWrapProgress1 = new CliWrapErrorEventArgs(new OperationCanceledException("Process cancelled by user"), null, action);
                            //    cliWrapProgress1.ErrorString = ProcessOutput;
                            //    OnCliWrapError(cliWrapProgress1);
                            //    errorCode = -1;  // indicate there has been an error. 
                            //}
                            //break;
                            ExitCode = exited.ExitCode;
                            errorCode = ExitCode;
                            if (string.IsNullOrEmpty(MovieName) && action == "JOIN") MovieName = OutputVideoPath;
                            if (string.IsNullOrEmpty(MovieName) && action == "JOINMOVIES") MovieName = OutputVideoPath;
                            if (string.IsNullOrEmpty(MovieName) && action == "CONVERT") MovieName = OutputVideoPath;
                            ProcessOutput = $"Process exited; Code: " + exited.ExitCode.ToString();
                            CliWrapCompletedEventArgs eventArgs = new CliWrapCompletedEventArgs(null, false, null)
                            {
                                Result = ExitCode,
                                TaskName = action,
                                MovieName = MovieName
                            };


                            OnCliWrapComplete(eventArgs);
                            //DoReloadBookmarks();
                            //CurrentMovieModel.SetPercentUnmarked();
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                Support.GenerateInfoAndLogMessage("FFMpeg", "Movie", 0, ex.ToString());
                ProcessOutput = $"Process errored ; " + ex.Message + " see log file";
                if (!string.IsNullOrEmpty(ErrorString)) ProcessOutput = "Process Errored  : " + ErrorString;
                CliWrapErrorEventArgs cliWrapProgress = new CliWrapErrorEventArgs(ex, null, action);
                cliWrapProgress.ErrorString = ErrorString;


                OnCliWrapError(cliWrapProgress);
                errorCode = -1;  // indicate there has been an error. 
                //throw;
            }
            return errorCode;
        }

        internal async Task<int> DoCliWrapCreateMovie(string param)
        {
            int errorCode = 0;

            // timeout extended as it was cancelling on setting chapters
            TimeSpan cancelDelay = TimeSpan.FromSeconds(400);

            cancelDelay = TimeSpan.FromSeconds(15000);
            cts.CancelAfter(cancelDelay);
            var cmd = Cli.Wrap(FFmpegFilePath)
                .WithArguments(param);

            try
            {
                await foreach (var cmdEvent in cmd.ListenAsync(System.Text.Encoding.Default, cts.Token))
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            Console.WriteLine($"Process started; ID: {started.ProcessId}");
                            // start viewer
                           // PlayFromFile(OutputVideoPath);
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            // process received data 
                            string output = stdOut.Text;

                            if (output.Contains("Percent="))
                            {
                                ProcessOutput = output;
                            }
                            else if (output.Contains("Frame="))
                            {
                                ProcessOutput = output;
                            }
                            else
                                ProcessOutput = output;

                            break;
                        case StandardErrorCommandEvent stdErr:
                            int PercentProgress = 0;
                            if (action == "CreateMovie")
                            {
                                int? frame = ExtractFrame(stdErr.Text);
                                if (frame != null && FrameCount > 0)
                                {
                                    PercentProgress = (frame.Value * 100) / FrameCount;
                                }
                            }
                            //else
                            //{
                            //    int? seconds = ExtractTime(stdErr.Text);
                            //    if (seconds != null && Movies != null && Movies.DurationSeconds != null)
                            //    {
                            //        PercentProgress = (seconds.Value * 100) / Movies.DurationSeconds.Value;
                            //    }
                            //    else if (seconds != null && TotalDuration > 0)
                            //    {
                            //        PercentProgress = (seconds.Value * 100) / TotalDuration;
                            //    }
                            //}

                            ProcessOutput = $"Err> {stdErr.Text}";

                            if (ProcessOutput.Contains("Error")) ErrorString = ProcessOutput;
                            Debug.WriteLine(stdErr.Text);
                            CliWrapProgressEventArgs cliWrapProgress = new CliWrapProgressEventArgs(PercentProgress, null)
                            {
                                Progress = ProcessOutput,
                                TaskName = action
                            };
                            OnCliWrapProgress(cliWrapProgress);
                            break;
                        // cancellation token triggered
                        case ExitedCommandEvent exited:
                            
                            ExitCode = exited.ExitCode;
                            errorCode = ExitCode;
                            if (string.IsNullOrEmpty(MovieName) && action == "JOIN") MovieName = OutputVideoPath;
                            if (string.IsNullOrEmpty(MovieName) && action == "JOINMOVIES") MovieName = OutputVideoPath;
                            if (string.IsNullOrEmpty(MovieName) && action == "CONVERT") MovieName = OutputVideoPath;
                            ProcessOutput = $"Process exited; Code: " + exited.ExitCode.ToString();
                            CliWrapCompletedEventArgs eventArgs = new CliWrapCompletedEventArgs(null, false, null)
                            {
                                Result = ExitCode,
                                TaskName = action,
                                MovieName = MovieName
                            };


                            OnCliWrapComplete(eventArgs);
                            
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Support.GenerateInfoAndLogMessage("FFMpeg", "Movie", 0, ex.ToString());
                ProcessOutput = $"Process errored ; " + ex.Message + " see log file";
                if (!string.IsNullOrEmpty(ErrorString)) ProcessOutput = "Process Errored  : " + ErrorString;
                CliWrapErrorEventArgs cliWrapProgress = new CliWrapErrorEventArgs(ex, null, action);
                cliWrapProgress.ErrorString = ErrorString;


                OnCliWrapError(cliWrapProgress);
                errorCode = -1;  // indicate there has been an error. 
                //throw;
            }
            return errorCode;
        }

        private async Task PlayFromFile(string tempFileName)
        {
            if (!string.IsNullOrEmpty(tempFileName))
            {
                if (File.Exists(tempFileName))
                {
                    using PlayerViewModel playerViewModel = new PlayerViewModel(tempFileName, true);
                    using TaymadeEntities.PlayerDialog playerDialog = new PlayerDialog(playerViewModel);

                    Window? main = Support.GetWindow();
                    if (main != null)
                    {
                        await playerDialog.ShowDialog(main);
                    }
                    
                }
            }
        }

        /// <summary>
        /// The errorDataReceived.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataReceivedEventArgs"/>.</param>
        private void errorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Output += e.Data;
        }

        /// <summary>
        /// The ExtractTime.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <returns>The <see cref="int?"/>.</returns>
        private int? ExtractTime(string text)
        {
            int? returnValue = null;

            if (text.Contains("time="))
            {
                int pos = text.IndexOf("time=");
                if (pos >= 0)
                {
                    string sub = text.Substring(pos + 5);

                    pos = sub.IndexOf(" ");

                    if (pos >= 0)
                    {
                        sub = sub.Substring(0, pos).Trim();

                        if (TimeSpan.TryParse(sub, out TimeSpan result))
                        {
                            returnValue = (int)result.TotalSeconds;
                        }
                    }

                }
            }

            return returnValue;
        }

        private int? ExtractFrame(string text)
        {
            int? returnValue = null;

            if (text.Contains("frame="))
            {
                int pos = text.IndexOf("frame=");
                if (pos >= 0)
                {
                    string sub = text.Substring(pos + 6).Trim();

                    pos = sub.IndexOf(" ");

                    if (pos >= 0)
                    {
                        sub = sub.Substring(0, pos).Trim();

                        if (int.TryParse(sub, out int result))
                        {
                            returnValue = (int)result;
                        }
                    }

                }
            }

            return returnValue;
        }


        /// <summary>
        /// The Ffmpeg_OutputDataReceived.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataReceivedEventArgs"/>.</param>
        private void Ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(Output))
            {
                Output = "";
            }

            string error = string.Empty;
            if (!String.IsNullOrEmpty(e.Data))
            {
                error = e.Data;
                Console.WriteLine(error);
                if (error.Contains("time="))
                {
                    int pos = error.IndexOf("time=");
                    if (pos > 0)
                    {
                        string time = error.Substring(pos + 5, 12);
                        if (TimeSpan.TryParse(time, out TimeSpan progress))
                        {
                            Progress = progress;
                        }
                    }
                }
            }

            //Debug.WriteLine(error);

            Output += error;
        }

        /// <summary>
        /// The Process_Exited.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.EventArgs"/>.</param>
        private void Process_Exited(object? sender, System.EventArgs e)
        {
            // eventHandled = true;
            Console.WriteLine(
                $"Exit time    : {FfMpegProc?.ExitTime}\n" +
                $"Exit code    : {FfMpegProc?.ExitCode}\n" +
                $"Elapsed time : {elapsedTime}");

            TimeSpan timeTaken = new TimeSpan(0);

            int exitCode = -1;

            if (processStart > DateTime.MinValue && FfMpegProc != null)
            {
                timeTaken = FfMpegProc.ExitTime.Subtract(processStart);

                exitCode = FfMpegProc.ExitCode;

                // if (FfMpegProc.StartInfo.RedirectStandardError && FfMpegProc.StandardError != null) Output = myStreamReader.ReadToEnd();
            }

            if (outputStream != null)
            {
                outputStream.Flush();
                outputStream.Close();

                if (exitCode == 0)
                {
                    if (File.Exists(OutputPath))
                    {
                        File.Delete(OutputPath);
                    }
                }

            }

            FfMpegProc?.Close();
            FfMpegProc?.Dispose();
            FfMpegProc = null;

            ConversionCompleteEventArgs eventArgs = new ConversionCompleteEventArgs();
            eventArgs.Filename = OutputVideoPath;
            eventArgs.TimeTaken = timeTaken;
            eventArgs.Action = action;
            eventArgs.Output = Output;
            eventArgs.ExitCode = exitCode;
            eventArgs.MovieId = movieId1;


            OnConversionComplete(eventArgs);
        }

        #endregion
    }

    public class FFProbeInfo
    {
        public string Duration { get; internal set; }
    }
}
