//-----------------------------------------------------------------------
// <copyright file="Support.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>25/04/2022 11:57:36 25/04/2022 11:57:36 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using CliWrap;
    using DocumentFormat.OpenXml.Drawing.Charts;
    using DocumentFormat.OpenXml.Office2010.Excel;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    // using Microsoft.Office.Interop.Word;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Targets;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using TaymadeEntities.Dialogs;
    using TaymadeEntities.Models;
    using TaymadeEntities.ViewModels;
    using Application = Avalonia.Application;
    using Bitmap = System.Drawing.Bitmap;
    using Window = Avalonia.Controls.Window;

    /// <summary>
    /// Defines the <see cref="Support" />.
    /// </summary>
    public class Support
    {
        private const string tempImageFileName = "K:\\TD1\\White\\Download\\image.bmp";
        private static Logger? logger;

        #region Properties

        public FrameSetHeader? FrameSetHeader
        {
            get => frameSetHeader;
            set => frameSetHeader = value;
        }

        public static Movies? CreatedMovie { get; set; }

        public static Logger Logger
        {
            get
            {
                if (logger == null)
                {
                    string? appname = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow.Title : "App";
                    string logDirectory = "C:\\NLog\\logs\\" + appname;
                    ScopeContext.PushProperty("UserName", "Doug Taylor");

                    var config = new LoggingConfiguration();
                    var fileTarget = new FileTarget
                    {
                        Name = "file",
                        Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${environment-user}",

                        FileName = Path.Combine(logDirectory, "${date:format=yyyyMMdd}.log")
                    };
                    config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget, "*");

                    var sqlTarget = new NLog.Targets.DatabaseTarget
                    {
                        Name = "database",
                        DBProvider = "Microsoft.Data.SqlClient",
                        ConnectionString = "data source=TAYMADE-8\\sqlexpress;Initial Catalog=sandbox;Persist Security Info=True;User Id=sandbox;Password=sandbox;Encrypt=false",
                        CommandText = "INSERT INTO [MVMLogs](CreatedOn,message,level,Exception,StackTrace,Logger) VALUES (getutcdate(),@msg,@level,@exception,@trace,@logger)",
                    };
                    sqlTarget.Parameters.Add(new DatabaseParameterInfo()
                    { Name = "@msg", Layout = "${message}" });
                    sqlTarget.Parameters.Add(new DatabaseParameterInfo()

                    { Name = "@level", Layout = "${level}" });
                    sqlTarget.Parameters.Add(new DatabaseParameterInfo()

                    { Name = "@exception", Layout = "${exception}" });

                    sqlTarget.Parameters.Add(new DatabaseParameterInfo()
                    { Name = "@trace", Layout = "${stacktrace}" });

                    sqlTarget.Parameters.Add(new DatabaseParameterInfo()
                    { Name = "@logger", Layout = "${logger}" });



                    config.AddRule(LogLevel.Error, LogLevel.Fatal, sqlTarget, "*");


                    LogManager.Configuration = config;
                    LogManager.ThrowExceptions = true;
                    LogManager.ThrowConfigExceptions = true;

                    // set internal log level
                    InternalLogger.LogLevel = LogLevel.Debug;

                    InternalLogger.LogFile = @"c:\Nlog\logs\internalLog.text";

                    logger = LogManager.GetLogger("all");


                }

                return logger;
            }
        }

        public static LogEventInfo GenerateInfoLogMessage(string action, string entity, int Id, string description)
        {
            return new LogEventInfo(LogLevel.Info, "", entity + action + " : " + Id.ToString() + " : " + description);
        }

        public static void GenerateInfoAndLogMessage(string action, string entity, int Id, string? description)
        {
            try
            {


                string id = string.Empty;

                if (Id > 0) id = Id.ToString(); else id = "No Id provided";

                LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, "", entity + action + " : " + id + " : " + description);
                Support.Logger.Info(logEvent);
            }
            catch (Exception)
            {

                //throw;
            }
        }


        /// <summary>
        /// Gets or sets the FfMpegProc
        /// Gets the FfMpegProc...
        /// </summary>
        public static Process? FfMpegProc { get; set; }

        /// <summary>
        /// Gets or sets the VLCProcess.
        /// </summary>
        public static Process? VLCProcess { get; set; }

        /// <summary>
        /// Gets the Writer.
        /// </summary>
        public static System.IO.StreamWriter? Writer { get; private set; }
        public PhraseEntry? PhraseEntry { get; private set; }
        public PhraseEntry? SubPhraseEntry { get; private set; }

        #endregion

        public delegate void CompletedEventHandler(object sender, MovieCompletedEventArgs e);

        public delegate void ProgressEventHandler(object sender, MovieProgressEventargs e);

        public event CompletedEventHandler ActionCompleted;

        public event ProgressEventHandler ProgressInformation;


        #region Methods

        public static void FindAges(TaymadeEntities.Support.Word.WordProperties returnProps, string tempKeywords)
        {
            // check on ages
            string agestring = string.Empty;
            string[] codes = tempKeywords.Split(new char[] { ',', '-', ' ' });
            string localCode = string.Empty;

            // go through codes looking for year old
            foreach (string code in codes)
            {
                if (code.Contains("y"))
                {
                    localCode = code;
                    // check for ';'
                    if (localCode.Contains(";"))
                    {
                        int semiPos = localCode.IndexOf(";");
                        if (semiPos > 0)
                        {
                            // we need the code stub from this point on
                            localCode = localCode.Substring(semiPos + 1);
                        }
                    }
                    int pos = localCode.IndexOf("y");
                    string tempAge = localCode.Substring(0, pos).Replace(".", "").Trim();
                    if (agestring != string.Empty)
                    {
                        agestring += ",";
                    }
                    // need to check string length

                    if (tempAge.Length > 2)
                    {
                        int i = tempAge.Length - 1;
                        string temp = tempAge;
                        tempAge = string.Empty;
                        while (char.IsNumber(temp[i]))
                        {
                            tempAge = temp[i] + tempAge;
                            i -= 1;
                        }
                    }
                    agestring += tempAge;
                }
                else if (IsNumeric(code) && code != "69")
                {
                    int.TryParse(code, out int n);
                    if (agestring != string.Empty)
                    {
                        agestring += ",";
                    }
                    agestring += n.ToString().Trim();
                }
                else
                {
                    Regex regex = new Regex("\\d+\\b");
                    Match match = regex.Match(code);
                    if (match.Success == true && !code.Contains(":"))
                    {
                        if (match.Value != "69")
                        {
                            if (agestring != string.Empty)
                            {
                                agestring += ",";
                            }
                            agestring += match.Value;
                        }
                    }
                }
            }
            if (agestring != string.Empty)
            {
                try
                {
                    List<int> intList;
                    string output = SortAgeList(agestring, out intList);
                    returnProps.Age = output;
                    //string[] ageArray = output.Split(',');
                    if (intList.Count > 0) returnProps.LowestAge = intList.FirstOrDefault().ToString();
                }
                catch (Exception)
                {
                    returnProps.Age = agestring;

                }

            }
        }

        /// <summary>
        /// Sorts the age list.
        /// </summary>
        /// <param name="agestring">The agestring.</param>
        /// <param name="intList">The int list.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        public static string SortAgeList(string agestring, out List<int> intList)
        {

            intList = StringIntListToIntList(agestring);
            return String.Join(",", intList);
        }


        public static async void GetCastData(Movies movie, iMovie iMovie)
        {
            List<Cast> templist = new List<Cast>();

            if (iMovie != null && movie != null && movie.TMDBID != null)
            {
                iMovie.CastList = await TmdbSupport.GetMovieCreditsAsync(movie.TMDBID.Value);
                if (iMovie.CastList.Count > 0)
                {
                    // sort out cast

                    if (movie.Casts == null) movie.Casts = new List<Cast>();
                    foreach (var item in iMovie.CastList)
                    {
                        Cast? castMember = DataController.CastController.GetCastByCreditId(item.CreditId);
                        //Cast? castMember = movie.Casts.Where(x => x.credit_id == item.CreditId).FirstOrDefault();

                        if (castMember == null)
                        {
                            castMember = new Cast()
                            {
                                credit_id = item.CreditId,
                                MovieID = movie.Id,
                                CastId = item.CastID,
                                Role = item.Character
                            };

                            // see if we can find the actor in the database with the correct TMDB Id 
                            Actor? actor = DataController.ActorController.GetActorByTMDBID(item.ID);
                            // Actor? actor = DataController.SandboxEntities.Actors.Where(x => x.TMDBID == item.ID).FirstOrDefault();

                            if (actor == null)
                            {
                                // then it might be the actor does exist but has not been recorded yet
                                actor = DataController.ActorController.GetActorByName(item.Name);
                                if (actor != null)
                                {
                                    DataController.ActorController.SetDetailsFromCastMember(actor, item);
                                    //actor.SetDetailsFromCastMember(item);
                                    // DataController.ActorController.Save(actor);
                                    //actor.Save();
                                }
                                else
                                {

                                    actor = DataController.ActorController.GetOrCreateActor(item.Name);
                                    if (actor != null)
                                    {
                                        //actor.SetDetailsFromCastMember(item);
                                        if (actor.Id == 0) DataController.ActorController.AddActor(actor);
                                        else DataController.ActorController.Save(actor);
                                        // DataController.SandboxEntities.Actors.Add(actor);
                                        //  DataController.SandboxEntities.SaveChanges();
                                    }
                                }
                            }

                            if (actor != null)
                            {
                                castMember.ActorId = actor.Id;
                                castMember.Insert();
                            }

                            templist.Add(castMember);

                            //    DataController.SandboxEntities.Casts.Add(castMember);
                            //    DataController.SandboxEntities.SaveChanges();
                        }
                        else
                        {
                            //castMember.Actor;
                            //if (castMember.Actor != null) castMember.Actor.SetDetailsFromCastMember(item);
                            //actor.Save();
                        }
                    }

                    //foreach (var cast in templist)
                    //{
                    //    DataController.SandboxEntities.Casts.Add(cast);
                    //    DataController.SandboxEntities.SaveChanges();
                    //}
                }

                // take a look at directors
                if (movie.DirectorID == null && !string.IsNullOrEmpty(iMovie.DirectorName))
                {
                    FindOrCreateDirector(movie, iMovie);
                }

                if (!string.IsNullOrEmpty(movie.DirectorsName) && !string.IsNullOrEmpty(iMovie.DirectorName) && iMovie.DirectorName != movie.DirectorsName)
                {
                    // need to change director 
                    FindOrCreateDirector(movie, iMovie);
                }
            }
        }



        private static void FindOrCreateDirector(Movies movie, iMovie iMovie)
        {
            // look the director up
            Models.Director? director = Models.DataController.DirectorList.Find(x => x.Name.ToLower() == iMovie.DirectorName.ToLower());

            if (director != null)
            {
                movie.Director = director;
                movie.DirectorID = director.Id;
            }
            else
            {
                // create new director
                director = new Models.Director();
                director.Name = iMovie.DirectorName;
                Models.DataController.DirectorController.Insert(director);
                Models.DataController.DirectorController.Save();
            }

            movie.Save();
        }

        /// <summary>
        ///   <br />
        /// </summary>
        /// <param name="returnProps"></param>
        /// <param name="tempKeywords"></param>
        /// <autogeneratedoc />
        //public static void FindAges(WordProperties returnProps, string tempKeywords)
        //{
        //    // check on ages
        //    string agestring = string.Empty;
        //    string[] codes = tempKeywords.Split(new char[] { ',', '-', ' ' });

        //    // go through codes looking for year old
        //    foreach (string code in codes)
        //    {
        //        if (code.Contains("y"))
        //        {
        //            int pos = code.IndexOf("y");
        //            string tempAge = code.Substring(0, pos).Replace(".", "").Trim();
        //            if (agestring != string.Empty)
        //            {
        //                agestring += ",";
        //            }
        //            // need to check string length

        //            if (tempAge.Length > 2)
        //            {
        //                int i = tempAge.Length - 1;
        //                string temp = tempAge;
        //                tempAge = string.Empty;
        //                while (char.IsNumber(temp[i]))
        //                {
        //                    tempAge = temp[i] + tempAge;
        //                    i -= 1;
        //                }


        //            }
        //            agestring += tempAge;
        //        }

        //        if (IsNumeric(code) && code != "69")
        //        {
        //            int.TryParse(code, out int n);
        //            if (agestring != string.Empty)
        //            {
        //                agestring += ",";
        //            }
        //            agestring += n.ToString().Trim();
        //        }

        //        Regex regex = new Regex("\\d+\\b");
        //        Match match = regex.Match(code);
        //        if (match.Success == true)
        //        {
        //            if (match.Value != "69")
        //            {
        //                if (agestring != string.Empty)
        //                {
        //                    agestring += ",";
        //                }
        //                agestring += match.Value;
        //            }
        //        }
        //    }
        //    if (agestring != string.Empty)
        //    {
        //        try
        //        {
        //            List<int> intList;
        //            string output = SortAgeList(agestring, out intList);
        //            returnProps.Age = output;
        //            //string[] ageArray = output.Split(',');
        //            if (intList.Count > 0) returnProps.LowestAge = intList.FirstOrDefault().ToString();
        //        }
        //        catch (Exception)
        //        {
        //            returnProps.Age = agestring;

        //        }

        //    }
        //}

        /// <summary>
        /// Sorts the age list.
        /// </summary>
        /// <param name="agestring">The agestring.</param>
        /// <param name="intList">The int list.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        //public static string SortAgeList(string agestring, out List<int> intList)
        //{

        //    intList = Support.StringIntListToIntList(agestring);
        //    return String.Join(",", intList);
        //}


        public static void DeleteTempImage()
        {
            if (File.Exists(tempImageFileName))
            {
                File.Delete(tempImageFileName);
            }
        }

        public static void SetImageBMP(string ImagePath, out Avalonia.Media.Imaging.Bitmap? imageBMP)
        {
            imageBMP = null;
            if (!string.IsNullOrEmpty(Support.FixImagePath(ImagePath)))
            {
                string fileName = Support.FixImagePath(ImagePath);
                if (System.IO.File.Exists(fileName) && imageBMP == null)
                {
                    imageBMP = Support.GetBMP(fileName);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ActionComplete" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MovieCompletedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        protected virtual void OnActionComplete(MovieCompletedEventArgs e)
        {
            //Busy = false;
            CompletedEventHandler handler = ActionCompleted;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Called when [progress].
        /// </summary>
        /// <param name="e">The e.</param>
        /// <autogeneratedoc />
        protected virtual void OnProgress(MovieProgressEventargs e)
        {
            ProgressEventHandler handler = ProgressInformation;
            handler?.Invoke(this, e);
        }

        private void FFMpeg_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        {

            ImageSetViewModel.MissingInfo = "Completed";
            ImageSetViewModel.RootFolder.HasTempMP4 = true; // indicate temporary file 
            ImageSetViewModel.PlayFromFile(ImageSetViewModel.OutputVideoPath);
            // need to change button visibility
        }

        internal void FFMpeg_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        {
            //con.WriteLine(e.Progress);
            ImageSetViewModel.MissingInfo = e.Progress;

            if (e.ProgressPercentage > 0) ImageSetViewModel.ProgressPercent = e.ProgressPercentage;
            if (ProgressInformation != null)
            {
                MovieProgressEventargs args = new MovieProgressEventargs(e.ProgressPercentage, null)
                {
                    Info = e.Progress
                };
                OnProgress(args);
            }

        }

        private ImageSetViewModel? ImageSetViewModel { get; set; }

        public async Task<int> MakeMovieFromImages(ImageSetViewModel? imageSetViewModel)
        {
            FFMpegSupport fFMpeg = new FFMpegSupport();
            fFMpeg.CliWrapCompleted += FFMpeg_CliWrapCompleted;
            fFMpeg.CliWrapError += FFMpeg_CliWrapError;
            fFMpeg.CliWrapProgress += FFMpeg_CliWrapProgress;
            ImageSetViewModel = imageSetViewModel;

            int error = -1;
            if (imageSetViewModel != null
               && imageSetViewModel.RootFolder != null
               && imageSetViewModel.RootFolder.CurrentSubFolder != null
               && imageSetViewModel.RootFolder.CurrentSubFolder.FrameSetHeader != null
               && imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems != null
               && imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.Count > 0)
            {

                string outputDirectory = imageSetViewModel.RootFolder.TempDirectory("temp");
                string imageFileStub = outputDirectory;
                string outputFileName = outputDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(imageSetViewModel.RootFolder.CurrentSubFolder.Path) + ".mp4";

                Directory.CreateDirectory(outputDirectory);
                // clear all files in temp folder
                DeleteFilesInFolder(outputDirectory);

                imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.ReloadImageItems
                    (
                    imageSetViewModel.RootFolder.CurrentSubFolder.Path
                    );
                //this.RaisePropertyChanged(nameof(RootFolder.CurrentSubFolder.ImageItems.Count));
                // go through all the images and find maxsizes
                double absMaxWidth = 0;
                double absMaxHeight = 0;

                MovieProgressEventargs progressChangedEventArgs = null;

                imageSetViewModel.MissingInfo = "Building List";

                List<FrameSet>? frameSets = imageSetViewModel.RootFolder.CurrentSubFolder.FrameSetHeader.FrameSetList;
                int indx = 1;
                int cnt = imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.Count;

                ImageItemsCollection? images = imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems;
                double aspectRatio;
                // absMaxWidth, absMaxHeight, progressChangedEventArgs, indx,
                (int maxWidth, int maxHeight) = await GetMaxSizes(progressChangedEventArgs, images);

                progressChangedEventArgs = new MovieProgressEventargs(0, null);
                progressChangedEventArgs.Info = "Creating Images";
                //Support_ProgressInformation(null, progressChangedEventArgs);
                OnProgress(progressChangedEventArgs);
                int index = 1;
                // need to ensure the values are even 
                if (maxHeight % 2 != 0) maxHeight += 1;
                if (maxWidth % 2 != 0) maxWidth += 1;

                // then we go through all images and save them to a created temp directory 
                // resizing the images to fit 

                int count = 0;
                if (frameSets == null)
                {
                    count = imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.Count * 2;
                }
                else
                {
                    foreach (var frameset in frameSets)
                    {
                        count += (frameset.EndImage + 1 - frameset.StartImage) * (int)frameset.FrameRate;
                    }
                }

                bool success = await BuildImages(imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems, imageFileStub,
                    absMaxWidth, absMaxHeight, progressChangedEventArgs, frameSets, maxWidth, maxHeight, count);

                // use ffmpeg to build an MP4 file
                string ffMpegCommand = "";
                if (FrameSetHeader == null && FrameSetHeader.FPS != null)
                {
                    ffMpegCommand = " -framerate 1 -i " + '"' + imageFileStub + "%04d.jpg" + '"' + " -c:v libx264 -r 30 " + '"' + outputFileName + '"';
                }
                else
                {
                    ffMpegCommand = " -framerate " + FrameSetHeader.FPS.Value.ToString("0.00") + " -i " + '"' + imageFileStub + "%04d.jpg" + '"' + " -c:v libx264 -r 30 " + '"' + outputFileName + '"';
                }


                imageSetViewModel.OutputVideoPath = outputFileName;
                progressChangedEventArgs = new MovieProgressEventargs(0, null);
                progressChangedEventArgs.ProgressPercentage = 0;
                progressChangedEventArgs.Info = "Creating temp MP4";
                progressChangedEventArgs.Bitmap = null;
                progressChangedEventArgs.BitmapPath = "";
                OnProgress(progressChangedEventArgs);

                imageSetViewModel.MissingInfo = "Creating temp MP4";

                //Views.MainWindow? main = GetMainWindow();

                fFMpeg.action = "CreateMovie";
                fFMpeg.FrameCount = index * 10;

                await fFMpeg.DoCliWrapCreateMovie(ffMpegCommand);



                // now display created image file

            }


            return error;
        }

        public async Task<bool>
            BuildImages(ImageItemsCollection imageItemsCollection, string imageFileStub,
            double absMaxWidth, double absMaxHeight, MovieProgressEventargs progressChangedEventArgs,
            List<FrameSet>? frameSets, int maxWidth, int maxHeight, int count)
        {
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
                System.Drawing.Color averageColour = Support.GetAverageColorFast(image);

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
                reSizedImage = Support.ResizeImage(image, newWidth, newHeight);

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

                // update progress
                progressChangedEventArgs = new MovieProgressEventargs(0, null);
                progressChangedEventArgs.ProgressPercentage = (index * 100) / count;
                progressChangedEventArgs.Info = "building bitmaps";
                progressChangedEventArgs.Bitmap = ConvertFileToAvaloniaBitmap(tempImageFileName);
                OnProgress(progressChangedEventArgs);

                await Task.Delay(50);
                solidBrush.Dispose();
            }
            return success;
        }

        // double absMaxWidth, double absMaxHeight, MovieProgressEventargs progressChangedEventArgs, int indx,
        internal async Task<(int maxWidth, int maxHeight)>
            GetMaxSizes(MovieProgressEventargs progressChangedEventArgs, ImageItemsCollection images)
        {
            double absMaxWidth = 0;
            double absMaxHeight = 0;
            int indx = 1;
            int cnt = 1;
            foreach (ImageItem item in images)
            {
                progressChangedEventArgs = new MovieProgressEventargs(0, null);
                progressChangedEventArgs.ProgressPercentage = (indx * 100) / cnt;
                progressChangedEventArgs.Info = "building bitmaps";
                progressChangedEventArgs.Bitmap = item.ImageBMP;
                indx += 1;
                OnProgress(progressChangedEventArgs);
                await Task.Delay(200);
                //Support_ProgressInformation(null, progressChangedEventArgs);
                if (item.ImageBMP != null && item.ImageBMP.Size.Height > absMaxHeight) absMaxHeight = item.ImageBMP.Size.Height;
                if (item.ImageBMP != null && item.ImageBMP.Size.Width > absMaxWidth) absMaxWidth = item.ImageBMP.Size.Width;
            }

            // convert to integer
            int maxWidth = (int)absMaxWidth;
            int maxHeight = (int)absMaxHeight;

            // we will have a maximum size of 1024  x 1024

            double aspectRatio = absMaxWidth / absMaxHeight;
            if (maxWidth > 1200 || maxHeight > 1024)
            {
                if (maxWidth - 1200 > maxHeight - 1024)
                {
                    maxWidth = 1200;
                    maxHeight = (int)(absMaxHeight / aspectRatio);
                }
            }
            // absMaxWidth, absMaxHeight, progressChangedEventArgs, indx,
            return (maxWidth, maxHeight);
        }

        private static void DeleteFilesInFolder(string outputDirectory)
        {
            var filelist = Directory.GetFiles(outputDirectory, "*.jpg");
            foreach (var tempFile in filelist)
            {
                File.Delete(tempFile);
            }
        }

        public static System.Drawing.Color GetAverageColorFast(Bitmap bmp)
        {
            Bitmap singlePixel = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(singlePixel))
            {
                // Use Bilinear or Bicubic for accurate averaging
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, 1, 1));
            }
            return singlePixel.GetPixel(0, 0);
        }

        //public async Task<int> MakeMovieFromImages(ImageSetViewModel? imageSetViewModel)
        //{
        //    int error = -1;
        //    if (imageSetViewModel != null
        //       && imageSetViewModel.RootFolder != null
        //       && imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems != null
        //       && imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.Count > 0)
        //    {

        //        // go through all the images and find maxsizes
        //        double absMaxWidth = 0;
        //        double absMaxHeight = 0;

        //        imageSetViewModel.MissingInfo = "Building List";
        //        foreach (ImageItem item in imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems)
        //        {
        //            if (item.ImageBMP != null && item.ImageBMP.Size.Height > absMaxHeight) absMaxHeight = item.ImageBMP.Size.Height;
        //            if (item.ImageBMP != null && item.ImageBMP.Size.Width > absMaxWidth) absMaxWidth = item.ImageBMP.Size.Width;
        //        }

        //        // convert to integer
        //        int maxWidth = (int)absMaxWidth;
        //        int maxHeight = (int)absMaxHeight;

        //        // we will have a maximum size of 1024  x 1024

        //        double aspectRatio = absMaxWidth / absMaxHeight;
        //        if (maxWidth > 1200 || maxHeight > 1024)
        //        {
        //            if (maxWidth - 1200 > maxHeight - 1024)
        //            {
        //                maxWidth = 1200;
        //                maxHeight = (int)(absMaxHeight / aspectRatio);
        //            }
        //        }

        //        MovieProgressEventargs progressChangedEventArgs = new MovieProgressEventargs(0, null);
        //        progressChangedEventArgs.Info = "Creating Images";
        //        this.OnProgress(progressChangedEventArgs);

        //        int index = 1;
        //        // need to ensure the values are even 
        //        if (maxHeight % 2 != 0) maxHeight += 1;
        //        if (maxWidth % 2 != 0) maxWidth += 1;

        //        // then we go through all images and save them to a created temp directory 
        //        // resizing the images to fit 

        //        string outputDirectory = imageSetViewModel.RootFolder.TempDirectory();
        //        string imageFileStub = outputDirectory + @"\temp";
        //        string outputFileName = outputDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(imageSetViewModel.RootFolder.CurrentSubFolder.Path) + ".mp4";

        //        Directory.CreateDirectory(outputDirectory);


        //        SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.WhiteSmoke);

        //        int count = imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems.Count * 2;

        //        foreach (ImageItem item in imageSetViewModel.RootFolder.CurrentSubFolder.ImageItems)
        //        {
        //            System.Drawing.Bitmap image = new System.Drawing.Bitmap(item.ImagePath);
        //            System.Drawing.Image reSizedImage = image;

        //            Color averageColour = GetAverageColorFast(image);

        //            solidBrush = new SolidBrush(averageColour);


        //            aspectRatio = (double)image.Width / (double)image.Height;

        //            int newHeight = image.Height;
        //            int newWidth = image.Width;
        //            // check size
        //            if (absMaxWidth - image.Width >= absMaxHeight - image.Height)
        //            {

        //                newHeight = maxHeight;
        //                newWidth = (int)(maxHeight * aspectRatio);
        //            }
        //            else
        //            {
        //                newWidth = maxWidth;
        //                newHeight = (int)((double)maxWidth / aspectRatio);
        //            }

        //            reSizedImage = Support.ResizeImage(image, newWidth, newHeight);

        //            // find which dimension is furthest away

        //            int xdif = (maxWidth - reSizedImage.Width) / 2;
        //            int ydif = (maxHeight - reSizedImage.Height) / 2;
        //            // create new bitmap of max sizes
        //            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(maxWidth, maxHeight);


        //            using (Graphics g = Graphics.FromImage(newBitmap))
        //            {
        //                g.FillRectangle(solidBrush, 0, 0, maxWidth, maxHeight);
        //                g.DrawImage(reSizedImage, xdif, ydif, reSizedImage.Width, reSizedImage.Height);
        //            }

        //            newBitmap.Save(imageFileStub + index.ToString("0000") + ".jpg", ImageFormat.Jpeg);
        //            index += 1;
        //            newBitmap.Save(imageFileStub + index.ToString("0000") + ".jpg", ImageFormat.Jpeg);
        //            index += 1;

        //            newBitmap.Dispose();

        //            progressChangedEventArgs.ProgressPercentage = (index * 100) / count;
        //            progressChangedEventArgs.Info = "building bitmaps";
        //            this.OnProgress(progressChangedEventArgs);

        //        }
        //        solidBrush.Dispose();

        //        string ffMpegCommand = " -framerate 1 -i " + '"' + imageFileStub + "%04d.jpg" + '"' + " -c:v libx264 -r 25 " + '"' + outputFileName + '"';

        //        FFMpegSupport fFMpeg = new FFMpegSupport();
        //        progressChangedEventArgs.Info = "Creating temp MP4";
        //        this.OnProgress(progressChangedEventArgs);

        //        imageSetViewModel.MissingInfo = "Creating temp MP4";

        //        Views.MainWindow? main = GetMainWindow();

        //        fFMpeg.action = "CreateMovie";
        //        fFMpeg.FrameCount = index * 25;

        //        fFMpeg.CliWrapCompleted += FFMpeg_CliWrapCompleted;
        //        fFMpeg.CliWrapError += FFMpeg_CliWrapError;
        //        fFMpeg.CliWrapProgress += FFMpeg_CliWrapProgress;

        //        error = await fFMpeg.DoCliWrap(ffMpegCommand);

        //    }

        //    return error;
        //}

        //private void FFMpeg_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        //{
        //    Debug.WriteLine(e.Progress);
        //    ViewModels.MainWindowViewModel imageSetViewModel = GetMainWindowViewModel();
        //    imageSetViewModel.MissingInfo = e.Progress;

        //    if (e.ProgressPercentage > 0) imageSetViewModel.MovieProgress = e.ProgressPercentage;
        //}

        private void FFMpeg_CliWrapError(object sender, CliWrapErrorEventArgs e)
        {

        }

        //private void FFMpeg_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        //{
        //    ViewModels.MainWindowViewModel imageSetViewModel = GetMainWindowViewModel();

        //    imageSetViewModel.MissingInfo = "Completed";

        //    MainWindow main = GetMainWindow();
        //    ImageSetViewModel? imagesetViewModel = main.ImageSetControl.DataContext as ImageSetViewModel;

        //    if (imagesetViewModel != null)

        //        imagesetViewModel.RootFolder.HasTempMP4 = true;
        //}

        /// <summary>
        /// Creates the movie.
        /// </summary>
        /// <param name="movieFilename">The model filename.</param>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        /// 

        public static TimeSpan SetMovieDuration(int? value)
        {
            if (value != null)
                return TimeSpan.FromSeconds((Double)value);
            else
                return TimeSpan.Zero;
        }

        /// <summary>
        /// </summary>
        /// <param name="movieFilename">The movie filename.</param>
        /// <param name="everyNframes">The every nframes.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 31/07/2026 31/07/2026 </created>
        ///   needs testing
        /// </remarks>
        public static async Task<bool> SplitMovieIntoFrames(string? movieFilename, int? everyNframes)
        {
            bool success = false;
            string command = "";
            if (!string.IsNullOrEmpty(movieFilename) && File.Exists(movieFilename))
            {
                string framePattern = "frame_%04d.jpg";
                string folder = Path.GetDirectoryName(movieFilename);
                string destFolder = Path.Combine(folder, "temp");
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                framePattern = '"' + Path.Combine(destFolder, framePattern) + '"';

                if (everyNframes != null)
                {
                    command = "ffmpeg -i " + '"' + movieFilename + '"' + " -vf \"select='not(mod(n,N))'\" -vsync 0 " + framePattern;
                }
                else
                {
                    command = "ffmpeg -i " + '"' + movieFilename + '"' + " " + framePattern;
                }

                FFMpegSupport fFMpeg = new FFMpegSupport();
                //fFMpeg.CliWrapCompleted += FFMpeg_CliWrapCompleted;
                //fFMpeg.CliWrapError += FFMpeg_CliWrapError;
                //fFMpeg.CliWrapProgress += FFMpeg_CliWrapProgress;

                await fFMpeg.DoCliWrapCreateMovie(command);
            }

            return success;
        }

        /// <summary>
        /// Creates the movie.
        /// </summary>
        /// <param name="movieFilename">The movie filename.</param>
        /// <param name="phrase">The phrase entry.</param>
        /// <param name="subPhrase">The sub-phrase entry.</param>
        /// <param name="newTMIDB">The new TMIDB value.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        public async Task<bool> CreateMovie(string? movieFilename, PhraseEntry? phrase, PhraseEntry? subPhrase, int? newTMIDB = null)
        {
            bool success = true;
            string error = string.Empty;
            Exception exception = null;
            Models.Movies? newMovie = null;
            CreatedMovie = null;
            try
            {
                // setup progress event arguments
                MovieProgressEventargs progressArgs = new MovieProgressEventargs(0, null);

                if (!string.IsNullOrEmpty(movieFilename))
                {

                    string filmName = System.IO.Path.GetFileNameWithoutExtension(movieFilename).Replace("&", "and").Replace(",", " ");
                    newMovie = DataController.MovieController.CreateMovie(filmName, 0, movieFilename, phrase.Id);
                    // newMovie = DataController.SandboxEntities.CreateMovie(filmName);
                    if (newMovie == null)
                    {
                        throw new Exception("Failed to create movie entity");
                    }
                    int Id = newMovie.Id;
                    newMovie = DataController.MovieController.GetMoviesById(Id);

                    progressArgs.Info = "Movie entity created " + movieFilename;
                    OnProgress(progressArgs);

                    // add genre
                    SetGroupDetails(newMovie, phrase, subPhrase, newMovie.Id);

                    progressArgs.Info = "Movie initialisation finished " + movieFilename;
                    OnProgress(progressArgs);

                    //VideoSupport.GetDuration(model.FileName, newMovie);
                    // metadata.Wait();

                    if (!string.IsNullOrEmpty(movieFilename))
                    {
                        string? filename = movieFilename;
                        string fixedName = Support.FixPathBack(movieFilename);

                        string extn = System.IO.Path.GetExtension(movieFilename);
                        //string filmName = System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "and").Replace(",", " ");

                        // clean out non Latin
                        filmName = DownloadSupport.CleanText(filmName);

                        // save in movie

                        // film name limited to 100 characters.
                        if (filmName.Length <= 100)
                            newMovie.MovieName = filmName;
                        else newMovie.MovieName = filmName.Substring(0, 100);

                        string shortName = filmName;

                        // check to see if we have a ridiculosly long name, that will break the character limit for a path

                        if (shortName.Length > 40) shortName = shortName.Substring(0, 40);


                        // log change it will also give guidance if there is a failure.
                        newMovie.LogMessage("Created");
                        string XSPFilename = newMovie.CreateXSPFDirectory(phrase, shortName);

                        newMovie.Path = XSPFilename;

                        //       newMovie.DurationSeconds = await VideoSupport.GetDurationSeconds(newMovie.MoviePath, newMovie);

                        if (phrase != null)
                        {
                            string rootDirectory = DownloadSupport.MovieBasePath + phrase.Id;

                            if (DownloadSupport.CheckAndCreateDirectory(rootDirectory))
                            {
                                rootDirectory = rootDirectory + DownloadSupport.DirectorySeparator() + shortName.Trim();

                                if (DownloadSupport.CheckAndCreateDirectory(rootDirectory))
                                {
                                    // changed to use trimmed name
                                    string newName = rootDirectory + DownloadSupport.DirectorySeparator() + shortName.Trim() + extn;
                                    string fixedNewName = Support.FixPathBack(newName);

                                    //await newMovie.GetDuration(movieFilename);

                                    //if (newMovie.DurationSeconds != null)
                                    //    VideoSupport.CreateXSPF(newMovie.DurationSeconds.Value, newName, XSPFilename);

                                    //we need to create a temporary new name
                                    string tempNewName = Support.FixImagePath(newName);
                                    MoveFiles(newMovie, filename, fixedNewName, tempNewName);
                                    //      await newMovie.GetDuration();
                                    if (newMovie.DurationSeconds == 0)
                                    {
                                        newMovie.DurationSeconds = 30;
                                    }


                                    if (newTMIDB != null)
                                    {
                                        newMovie.TMDBID = newTMIDB;

                                        //iMovie iMovie = await TmdbSupport.GetMovieData(newTMIDB.Value);
                                        //if (iMovie != null)
                                        //{
                                        //    DialogModelBase.GetCastData(newMovie, iMovie);
                                        //    newMovie.Info = iMovie.Overview;

                                        //    //newMovie.Save();
                                        //}
                                    }

                                    success = newMovie.Save();
                                    error = newMovie.ErrorText;

                                    progressArgs.Info = "Movie entity saved " + newMovie.MovieName;
                                    OnProgress(progressArgs);
                                }
                            }
                        }

                        // try and get a first bookmark
                        if (newMovie.Bookmarks != null)
                        {
                            newMovie.CreateFirstBookmark();

                            progressArgs.Info = "Movie bookmark created " + newMovie.MovieName;
                            OnProgress(progressArgs);
                        }
                    }
                }



                //            //MainWindowViewModel? mainWindowModel = Support.GetMainWindowViewModel();
                //            //Views.MainWindow? main = Support.GetMainWindow();

                //            //if (main != null && mainWindowModel != null)
                //            //{
                //            //    if (success) mainWindowModel.MovieList.Add(newMovie);

                //            //    main.DoChangeGroup(viewModel.CurrentPhrase);
                //            //    main.MovieChanged(newMovie, mainWindowModel);
                //            //}

                //            //Support.SetCurrentMovie(newMovie);

                //            //if (mainWindowModel != null)
                //            //{
                //            //    mainWindowModel.CurrentMovieModel = newMovie;
                //            //}

                //            //DownloadViewModel? vm = DataContext as DownloadViewModel;



                {

                    // error = "Movie creation cancelled";
                    // exception = new Exception(error);
                    //success = false;
                }


            }
            catch (System.Exception ex)
            {
                exception = ex;
                Support.GenerateInfoAndLogMessage("creating Movie", "Movie", 0, ex.ToString());
                success = false;
            }

            //if (!string.IsNullOrEmpty(error) && exception == null)
            //{
            //    exception = new Exception(error);
            //}

            MovieCompletedEventArgs args = new MovieCompletedEventArgs(exception, false, null)
            {
                Movie = newMovie,
                PhraseEntry = phrase,
                SubPhraseEntry = subPhrase
            };

            if (newMovie != null)
            {
                args.MovieId = newMovie.Id;
                CreatedMovie = newMovie;
                args.PhraseEntry = phrase;
                args.SubPhraseEntry = subPhrase;
            }

            OnActionComplete(args);
            return success;
        }

        private static void SetGroupDetails(Movies newMovie, PhraseEntry? phrase, PhraseEntry? subPhrase, int Id)
        {
            PhraseEntry SubPhrase = subPhrase;
            string? subGenre = SubPhrase != null ? SubPhrase.COMPKEY : null;

            //    movieGenre.Insert();
            if (subPhrase != null)
            {
                newMovie.FilmGroup = phrase.Id;
                newMovie.PrimaryFilmGroup = phrase.COMPKEY;
            }
            else if (phrase != null)
            {
                newMovie.FilmGroup = phrase.Id;
                newMovie.PrimaryFilmGroup = phrase.Id;
            }
            newMovie.Added = System.DateTime.Now;
            newMovie.ModifiedOn = System.DateTime.Now;
            newMovie.HasChapters = false;
            newMovie.HasEpisodes = false;
            newMovie.Save();
            MovieGenre movieGenre = DataController.MovieController.CreateMovieGenre(newMovie.Id, phrase?.COMPKEY, subPhrase?.COMPKEY);
            newMovie.MovieGenres.Add(movieGenre);
            SetSeriesDetails(newMovie, movieGenre);
        }

        private static void SetSeriesDetails(Movies newMovie, MovieGenre? movieGenre)
        {
            if (newMovie.FilmGroup == "SER") // it is a series
            {
                // Naked attraction = SER.NAKATR-9
                // Naked News = SER.NEWS-9
                if (movieGenre != null && !string.IsNullOrEmpty(movieGenre.SubGenre))
                {
                    //if (movieGenre.SubGenre == "SER.NAKATR-9") newMovie.Series = 3;
                    //if (movieGenre.SubGenre == "SER.NEWS-9") newMovie.Series = 4;
                }
            }
            //EntityState state = DataController.SandboxEntities.Entry(newMovie).State;


        }

        /// <summary>
        /// <br />.
        /// </summary>
        /// <param name="newMovie">The new movie.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="fixedNewName">New name of the fixed.</param>
        /// <param name="tempNewName">New name of the temporary.</param>
        private static void MoveFiles(Movies newMovie, string filename, string fixedNewName, string tempNewName)
        {
            if (!File.Exists(tempNewName))
            {

                try
                {

                    if (filename != tempNewName)
                    // move file
                    {
                        // need to check directory exist and remove suprious double quotes from names

                        filename = filename.Replace('"', ' ').Trim();
                        tempNewName = tempNewName.Replace('"', ' ').Trim();

                        File.Move(filename, tempNewName);
                        SetMoviePathAndMoveOthers(newMovie, ref filename, fixedNewName, ref tempNewName);
                    }
                    else
                    {
                        newMovie.MoviePath = filename;
                        newMovie.Save();
                    }
                }
                catch (System.Exception ex)
                {
                    Support.GenerateInfoAndLogMessage("moving created file failed", "movie " + tempNewName, newMovie.Id, ex.ToString());
                }
            }
            else
                SetMoviePathAndMoveOthers(newMovie, ref filename, fixedNewName, ref tempNewName);
        }

        private static void SetMoviePathAndMoveOthers(Movies newMovie, ref string filename, string fixedNewName, ref string tempNewName)
        {
            //remove spurious double quotes
            fixedNewName = fixedNewName.Replace('"', ' ').Trim();
            newMovie.MoviePath = fixedNewName;

            // add new file to database
            if (newMovie.Id == 0)
            {
                DataController.MovieController.Add(newMovie);// SandboxEntities.Movies.Add(newMovie);
                //DataController.SandboxEntities.SaveChanges();
            }
            else
            {
                DataController.MovieController.UpdateMovie(newMovie);
            }

            // success log it. 
            Support.GenerateInfoAndLogMessage("moving created file", "movie", newMovie.Id, tempNewName);


            // get extension
            string extn = Path.GetExtension(tempNewName);

            filename = filename.Replace(extn, ".nfo");

            // see if we have an nfo file
            if (File.Exists(filename))
            {
                // we have an nfo file to move
                tempNewName = tempNewName.Replace(extn, ".nfo");
                // move it unless already there
                if (!File.Exists(tempNewName))
                {
                    File.Move(filename, tempNewName);

                }
            }

            // see about other files

            string pattern = Path.GetFileNameWithoutExtension(filename) + ".*";

            string[] others = Directory.GetFiles(Path.GetDirectoryName(filename), pattern);

            string baseNewDir = Path.GetDirectoryName(tempNewName);
            //string baseFileName = Path.GetFileNameWithoutExtension(tempNewName);
            foreach (string additionalFile in others)
            {
                tempNewName = baseNewDir + @"\" + Path.GetFileName(additionalFile);
                if (!File.Exists(tempNewName))
                {
                    File.Move(additionalFile, tempNewName);
                }

                // need to identify bookmarks charecterised by extn BMP

                extn = CheckForBookmark(newMovie, additionalFile);
            }
        }

        public static string CheckForBookmark(Movies newMovie, string additionalFile)
        {
            string? baseFileName = newMovie.BaseFileName;
            string extn = Path.GetExtension(additionalFile).ToLower();
            if (extn == ".bmp")
            {
                string bookmarkName = Path.GetFileNameWithoutExtension(additionalFile);
                // should just leave the time
                bookmarkName = bookmarkName.Replace(baseFileName, "");

                if (double.TryParse(bookmarkName, out double time))
                {
                    Bookmark? bookmark = newMovie.Bookmarks.Where(x => x.Time != null && x.Time.Value == time).FirstOrDefault();
                    if (bookmark != null) bookmark.ImagePath = additionalFile;
                    else
                    {
                        bookmark = new Bookmark()
                        {
                            MovieID = newMovie.Id,
                            ImagePath = additionalFile,
                            Time = time
                        };
                        bookmark.Insert();
                    }
                }
            }

            return extn;
        }

        public static Models.Filter? GetDefaultFilter()
        {
            Models.Filter returnValue = null;
            if (
                DataController.MovieProperties.DefaultFilter != null
                )
            {
                returnValue = DataController.SandboxEntities.Filter.Find(DataController.MovieProperties.DefaultFilter);
                if (returnValue != null)
                {
                    returnValue.FromJson(returnValue.JSON);
                }
            }
            else

                returnValue = new Models.Filter();
            return returnValue;
        }


        public static List<int> StringIntListToIntList(string agestring)
        {
            agestring = agestring.Replace(".", ",");
            try
            {
                return agestring.Split(',').Select(x => int.Parse(x)).OrderBy(x => x).ToList();

            }
            catch (Exception)
            {

                return new List<int>() { 0 };
            }
        }

        public static bool IsNumeric(string s)
        {
            if (Regex.IsMatch(s, @"^\d+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Capitalises the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>.</returns>
        public static string Capitalise(string input)
        {
            string result = Regex.Replace(input.ToLower(), @"\b(\w)", m => m.Value.ToUpper());
            result = Regex.Replace(result, @"(\s(of|in|by|and)|\'[st])\b", m => m.Value.ToLower(), RegexOptions.IgnoreCase);
            return result;
        }


        public static bool IsWindows()
        {
            string os = GetOS();
            return (os.ToString() == "WinNT");
        }

        /// <summary>
        /// The FixImagePath.
        /// </summary>
        /// <param name="filename">The filename<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FixImagePath(string filename)
        {
            string tPath = string.Empty;
            if (!string.IsNullOrEmpty(filename))


            {
                //find OS if WinNT short circuit operation
                string os = GetOS();

                string machineName = GetComputerName();
                List<MappedDrives> mappedDrives = DataController.MaintenaceController.GetDrivesByComputerName(machineName);

                // List <MappedDrives> mappedDrives = DataController.SandboxEntities.MappedDrives.Where(s => s.Computer == machineName && s.LocationType == "PATH").ToList();

                MappedDrives? mapped = mappedDrives.Where(d => filename.ToLower().Contains(d.SourceDrive)).FirstOrDefault();

                // m oving to mapped files
                if (mapped != null && mapped.SourceDrive != null && mapped.DestinationDrive != null)
                {
                    filename = filename.Replace(mapped.SourceDrive, mapped.DestinationDrive, StringComparison.OrdinalIgnoreCase);
                    tPath = filename;
                }

                else if (os.ToString() == "WinNT")
                {
                    // need to check for drive p


                    tPath = filename.Replace("/", "\\");
                    string mpath = filename.Replace("/", "\\");

                    if (mpath.Contains(@"p:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"W:\Drive-P\");
                    }

                    if (mpath.Contains(@"j:\", StringComparison.OrdinalIgnoreCase))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"W:\Drive-j\");
                    }

                    if (mpath.Contains(@"m:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"K:\DriveF\");
                    }
                }
                else
                {
                    string mpath = filename.Replace("\\", "/").ToLower();

                    tPath = filename.Replace("\\", "/");

                    if (mpath.Contains("k:/td1/white/"))
                    {
                        string stub = tPath.Substring(0, 13);
                        tPath = tPath.Replace(stub, "/home/doug/Media/White/");
                    }

                    else if (mpath.Contains("t:/white/"))
                    {
                        string stub = tPath.Substring(0, 9);
                        tPath = tPath.Replace(stub, "/home/doug/Media/White/");
                    }
                    // add xspf files to deal with
                    else if (mpath.Contains("k:/td1/xspf/"))
                    {
                        string stub = tPath.Substring(0, 12);
                        tPath = tPath.Replace(stub, "/home/doug/Media/TD1/xspf/");
                    }
                    else if (mpath.Contains("k:/td1/"))
                    {
                        string stub = tPath.Substring(0, 7);
                        tPath = tPath.Replace(stub, "/home/doug/Media/TD1/");
                    }
                    else if (mpath.Contains("t:/"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, "/home/doug/Media/TD1/");
                    }
                    else if (mpath.Contains("j:/"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Drive-J/");
                    }
                    else if (mpath.Contains("w:/drive-p/"))
                    {
                        string stub = tPath.Substring(0, 11);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Drive-P/");
                    }
                    else if (mpath.Contains("w:/drive-j/"))
                    {
                        string stub = tPath.Substring(0, 11);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Drive-J/");
                    }

                    else if (mpath.Contains("p:/"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Drive-P/");
                    }
                    else if (mpath.Contains("s:/"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Video/");
                    }
                    else if (mpath.Contains("m:/"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, "/home/doug/Media/Drive-F/");
                    }
                    else if (mpath.Contains("c:/drive_i/stories/"))
                    {
                        string stub = tPath.Substring(0, 19);
                        tPath = tPath.Replace(stub, "/home/doug/Media/stories/");
                    }

                }
            }
            return tPath;
        }

        public static string? GetApplicationPathFromDB(string AppName)
        {
            string machineName = GetComputerName();

            MappedDrives? found = DataController.MaintenaceController.GetDriveByComputerAndApplicationName(machineName, AppName);
            //MappedDrives? found = DataController.SandboxEntities.MappedDrives.Where(m => m.Computer == machineName && m.LocationType == "APP" && m.SourceDrive == AppName).FirstOrDefault();

            if (found != null) return found.DestinationDrive;
            return null;
        }

        /// <summary>
        /// The FixPathBack.
        /// </summary>
        /// <param name="filename">The filename<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FixPathBack(string? filename)
        {
            string tPath = string.Empty;
            if (!string.IsNullOrEmpty(filename))
            {

                //find OS if WinNT short circuit operation
                string os = GetOS();

                string machineName = GetComputerName();

                // add mapped drive support
                List<MappedDrives> mappedDrives = DataController.MaintenaceController.GetDrivesByComputerName(machineName);
                //List <MappedDrives> mappedDrives = DataController.SandboxEntities.MappedDrives.Where(s => s.Computer == machineName && s.Reversible == true && s.LocationType == "PATH").ToList();

                MappedDrives? mapped = mappedDrives.Where(d => filename.ToLower().Contains(d.DestinationDrive)).FirstOrDefault();

                // m oving to mapped files
                if (mapped != null && mapped.SourceDrive != null && mapped.DestinationDrive != null)
                {
                    filename = filename.Replace(mapped.DestinationDrive, mapped.SourceDrive, StringComparison.OrdinalIgnoreCase);
                    tPath = filename;
                }

                else if (os.ToString() == "WinNT")
                {
                    // no work to do
                    tPath = filename.Replace("/", "\\");
                    string mpath = filename.Replace("/", "\\").ToLower();

                    if (mpath.Contains(@"p:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"W:\Drive-P\");
                    }

                    if (mpath.Contains(@"t:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"K:\TD1\");
                    }

                    if (mpath.Contains(@"j:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"W:\Drive-j\");
                    }

                    if (mpath.Contains(@"m:\"))
                    {
                        string stub = tPath.Substring(0, 3);
                        tPath = tPath.Replace(stub, @"K:\DriveF\");
                    }
                }
                else
                {
                    string mpath = filename; //.Replace("/", "").ToLower();

                    // change to windows backslash
                    tPath = filename.Replace("/", "\\");

                    if (mpath.Contains("/home/doug/Media/White/"))
                    {
                        string stub = tPath.Substring(0, 23);
                        tPath = tPath.Replace(stub, @"K:\td1\White\");
                    }
                    //else if (mpath.Contains("t:/white"))
                    //{
                    //    string stub = tPath.Substring(0, 9);
                    //    tPath = tPath.Replace(stub, "/home/doug/Media/White/");
                    //}
                    else if (mpath.Contains("/home/doug/Media/TD1/"))
                    {
                        string stub = tPath.Substring(0, 21);
                        tPath = tPath.Replace(stub, @"K:\td1\");
                    }
                    else if (mpath.Contains("/home/doug/Media/Drive-J/"))
                    {
                        string stub = tPath.Substring(0, 25);
                        tPath = tPath.Replace(stub, @"j:\");
                    }
                    else if (mpath.Contains("/home/doug/Media/Drive-P/"))
                    {
                        string stub = tPath.Substring(0, 25);
                        tPath = tPath.Replace(stub, @"w:\drive-p");
                    }
                    //else if (mpath.Contains("p:/"))
                    //{
                    //    string stub = tPath.Substring(0, 3);
                    //    tPath = tPath.Replace(stub, "/home/doug/Media/Drive-P/");
                    //}
                    else if (mpath.Contains("/home/doug/Media/Video/"))
                    {
                        string stub = tPath.Substring(0, 23);
                        tPath = tPath.Replace(stub, @"s\");
                    }
                    else if (mpath.Contains("/home/doug/Media/Drive-F/"))
                    {
                        string stub = tPath.Substring(0, 25);
                        tPath = tPath.Replace(stub, @"M:\");
                    }
                    else if (mpath.Contains("/media/stories/"))
                    {
                        string stub = tPath.Substring(0, 25);
                        tPath = tPath.Replace(stub, @"C:\Drive_I\Stories\");
                    }
                }
            }
            return tPath.Replace("/", "\\");
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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

        public static Bitmap? ConvertAvaloniaBMPToSystem(Avalonia.Media.Imaging.Bitmap sourceBMP)
        {
            Bitmap destImage = null;
            using (MemoryStream imageStream = new MemoryStream())
            {
                sourceBMP.Save(imageStream);
                imageStream.Seek(0, SeekOrigin.Begin);
                destImage = new System.Drawing.Bitmap(imageStream);

            }
            return destImage;
        }

        public static Avalonia.Media.Imaging.Bitmap ResizeImage(Avalonia.Media.Imaging.Bitmap sourceBMP, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);
            System.Drawing.Bitmap sourceImage = null;
            using (MemoryStream imageStream = new MemoryStream())
            {
                sourceBMP.Save(imageStream);
                imageStream.Seek(0, SeekOrigin.Begin);
                sourceImage = new System.Drawing.Bitmap(imageStream);

            }

            destImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

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
                    graphics.DrawImage(sourceImage, destRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return Support.ConvertFileToAvaloniaBitmap(destImage);
        }

        //internal static System.Drawing.Color CalculateAverageColor(Bitmap bm)
        //{
        //    int width = bm.Width;
        //    int height = bm.Height;
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;
        //    int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
        //    int dropped = 0; // keep track of dropped pixels
        //    long[] totals = new long[] { 0, 0, 0 };
        //    int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

        //    BitmapData srcData = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
        //    int stride = srcData.Stride;
        //    IntPtr Scan0 = srcData.Scan0;

        //    unsafe
        //    {
        //        byte* p = (byte*)(void*)Scan0;

        //        for (int y = 0; y < height; y++)
        //        {
        //            for (int x = 0; x < width; x++)
        //            {
        //                int idx = (y * stride) + x * bppModifier;
        //                red = p[idx + 2];
        //                green = p[idx + 1];
        //                blue = p[idx];
        //                if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
        //                {
        //                    totals[2] += red;
        //                    totals[1] += green;
        //                    totals[0] += blue;
        //                }
        //                else
        //                {
        //                    dropped++;
        //                }
        //            }
        //        }
        //    }

        //    int count = width * height - dropped;
        //    int avgR = (int)(totals[2] / count);
        //    int avgG = (int)(totals[1] / count);
        //    int avgB = (int)(totals[0] / count);

        //    return System.Drawing.Color.FromArgb(avgR, avgG, avgB);
        //}

        /// <summary>
        /// The GetApplicationPath.
        /// </summary>
        /// <param name="app">The app<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetApplicationPath(string app)
        {
            string aPath = string.Empty;

            string localApp = app.ToLower();

            string? tempPath = null;
            string os = GetOS();
            if (os.ToString() == "WinNT")

            {
                switch (localApp)
                {
                    case "chrome":
                        tempPath = GetApplicationPathFromDB("chrome");
                        if (tempPath != null)
                            aPath = tempPath;
                        else
                            aPath = @"""C:\Program Files (x86)\Google\Chrome\Application\chrome.exe""";
                        break;
                    case "word":
                        tempPath = GetApplicationPathFromDB("word");
                        if (tempPath != null)
                            aPath = tempPath;
                        else
                        {
                            string computer = GetComputerName();
                            if (computer.ToLower() == "taymade-8")
                                aPath = @"""C:\Program Files (x86)\Microsoft Office\root\Office16\WINWORD.EXE""";
                            else
                                aPath = @"""C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE""";
                        }
                        break;
                    case "acrobat":
                        aPath = @"""C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe""";
                        break;
                }
            }
            else
            {
                switch (localApp)
                {
                    case "chrome":
                        aPath = "/usr/bin/firefox";
                        break;
                    case "word":
                        aPath = "/usr/bin/libreoffice";
                        break;
                }
            }

            // if (!string.IsNullOrEmpty(aPath)) aPath = '"' + aPath + '"';

            return aPath;
        }


        public static string GetApplicationVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public static string GetComputerName()
        {
            return System.Environment.MachineName;
        }

        /// <summary>
        /// The GetBMP.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <returns>The <see cref="Avalonia.Media.Imaging.Bitmap?"/>.</returns>
        public static Avalonia.Media.Imaging.Bitmap? GetBMP(string fileName)
        {
            Avalonia.Media.Imaging.Bitmap? retBMP = null;

            if (System.IO.File.Exists(fileName))// && GetOS() == "WinNT")
            {
                // load the image bytes into memory so the on-disk file is not locked
                var fileBytes = File.ReadAllBytes(fileName);

                // create Avalonia Bitmaps from in-memory stream
                using (var ms = new MemoryStream(fileBytes, writable: false))
                {
                    retBMP = new Avalonia.Media.Imaging.Bitmap(ms);
                }
            }
            return retBMP;
        }

        private static Avalonia.Media.Imaging.Bitmap? ConvertFileToAvaloniaBitmap(string? fileName, Avalonia.Media.Imaging.Bitmap? retBMP)
        {
            try
            {
                retBMP = ConvertFileToAvaloniaBitmap(fileName);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return retBMP;
        }

        public static Avalonia.Media.Imaging.Bitmap? ConvertFileToAvaloniaBitmap(string? fileName)
        {
            Avalonia.Media.Imaging.Bitmap? retBMP = null;
            try
            {
                retBMP = GetBMP(fileName);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return retBMP;
        }

        public static Avalonia.Media.Imaging.Bitmap? ConvertFileToAvaloniaBitmap(System.Drawing.Bitmap? sBitmap)
        {
            Avalonia.Media.Imaging.Bitmap? retBMP = null;

            if (sBitmap != null)
            {
                try
                {
                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        sBitmap?.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                        memory.Position = 0;
                        retBMP = new Avalonia.Media.Imaging.Bitmap(memory);
                    }
                    sBitmap?.Dispose();
                    sBitmap = null;
                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return retBMP;
        }

        public static Avalonia.Media.Imaging.Bitmap? GetBMPFromBitmap(System.Drawing.Image sBitmap)
        {
            Avalonia.Media.Imaging.Bitmap? retBMP = null;
            try
            {
                using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                {
                    sBitmap?.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;

                    retBMP = new Avalonia.Media.Imaging.Bitmap(memory);
                }
                sBitmap?.Dispose();
                sBitmap = null;

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return retBMP;
        }

        public static Avalonia.Media.Imaging.Bitmap? AddRectangle(Avalonia.Media.Imaging.Bitmap source, Rectangle rect)
        {
            Avalonia.Media.Imaging.Bitmap? retBMP = null;

            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                source.Save(memory);
                memory.Position = 0;

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(memory);

                Graphics g = Graphics.FromImage(bitmap);

                g.DrawRectangle(Pens.Yellow, rect);

                bitmap.Save(tempImageFileName);

                //bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);



                //memory.Position = 0;
                //memory.Seek(0, SeekOrigin.Begin);

                retBMP = new Avalonia.Media.Imaging.Bitmap(tempImageFileName);
            }



            return retBMP;
        }

        /// <summary>
        /// The GetMainWindow.
        /// </summary>
        /// <returns>The <see cref="Views.MainWindow"/>.</returns>
        public static Window? GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow as Window;
            }
            return null;
        }

        /// <summary>
        ///// Gets the movie list control.
        ///// </summary>
        ///// <returns></returns>
        ///// <autogeneratedoc />
        //public static Views.MovieListControl? GetMovieListControl()
        //{
        //    Views.MainWindow? main = GetMainWindow();
        //    if (main == null) { return null; }
        //    else
        //    {
        //        Views.MovieListControl? mvlc = main.MovieList;

        //        return mvlc;
        //    }
        //}

        //public static DataGrid? GetDgMovies()
        //{
        //    Views.MovieListControl? mvlc = GetMovieListControl();

        //    DataGrid? dgMovies = null;

        //    if (mvlc != null)
        //    {
        //        dgMovies = mvlc.dgMovies;
        //    }

        //    return dgMovies;
        //}

        //public static void SetScreen(Window window)
        //{
        //    if (window.Screens.ScreenCount > 1 && DataController.ShowOnAlternateScreen())
        //    {
        //        if (GetScreenId() != null)
        //        {
        //            if (GetScreenId() > 0)
        //                window.Position = new PixelPoint(-800, 50);
        //        }
        //    }
        //    window.WindowState = WindowState.Maximized;
        //}

        private static int? screenId = null;
        private static object absMaxWidth;
        private FrameSetHeader? frameSetHeader;

        /// <summary>Gets the screen identifier.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        //public static int? GetScreenId()
        //{

        //    MainWindowViewModel imageSetViewModel = GetMainWindowViewModel();

        //    if (imageSetViewModel != null && int.TryParse(imageSetViewModel.CurrentScreen.Id, out int screenid))
        //    {
        //        screenId = screenid;
        //    }

        //    return screenId;
        //}

        /// <summary>Gets the main window view model.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        //public static ViewModels.MainWindowViewModel? GetMainWindowViewModel()
        //{
        //    Views.MainWindow? main = Support.GetMainWindow();

        //    if (main != null)
        //    {
        //        MainWindowViewModel? viewModel = main.DataContext as MainWindowViewModel;

        //        return viewModel;
        //    }
        //    else return null;
        //   }

        /// <summary>
        ///   Gets the curent movie.
        /// <returns>
        ///   <br />
        /// </returns>
        //public static Models.Movies GetCurrentMovie()
        //{
        //    MainWindowViewModel? viewModel = GetMainWindowViewModel();

        //    if (viewModel != null)
        //    {
        //        return viewModel.CurrentMovie;
        //    }
        //    else return null;
        //}

        /// <summary>
        /// Gets the last movie.
        /// </summary>
        /// <returns></returns>
        /// <autogeneratedoc />
        public static Models.Movies? GetLastMovie()
        {
            int? LastMovieID = DataController.MovieProperties.LastMoveID;
            if (DataController.MovieList != null)
            {
                Movies? current = DataController.MovieList.Where(x => x.Id == LastMovieID).FirstOrDefault();

                return current;
            }
            else return null;
        }

        public static ObservableCollection<Models.Movies> GetMovieList(string id)
        {
            ObservableCollection<Movies> movieList = null;
            if (!string.IsNullOrEmpty(id))

            {
                List<Models.Movies> tempList = DataController.SandboxEntities.Movies
                        .Where(x => x.FilmGroup.Contains(id))
                        //.Include(x => x.Casts)
                        .Include(b => b.Bookmarks)
                        //.Include(d => d.Director)
                        .ToList();
                movieList = MovieCollection.GetAndSortObservableCollection(tempList);
            }
            else
            {
                List<Models.Movies> tempList = DataController.SandboxEntities.Movies
                        // .Include(x => x.Casts)
                        .Include(b => b.Bookmarks)
                        // .Include(d => d.Director)
                        .ToList();
                movieList = MovieCollection.GetAndSortObservableCollection(tempList);
            }

            //MovieList = new ObservableCollection<Movies>(tempList);

            return movieList;
        }

        public static async Task<ObservableCollection<Models.Movies>> GetMovieListAsyncTask(string id)
        {
            List<Models.Movies> tempList = await DataController.SandboxEntities.Movies
                    .Where(x => x.FilmGroup.Contains(id))
                    // .Include(x => x.Casts)
                    .Include(b => b.Bookmarks)
                    //  .Include(d => d.Director)
                    .ToListAsync();
            //MovieList = new ObservableCollection<Movies>(tempList);
            ObservableCollection<Movies> movieList = MovieCollection.GetAndSortObservableCollection(tempList);

            return movieList;
        }

        public static PhraseEntry? GetStoredFilmGroup()
        {
            string? group = DataController.MovieProperties.Group;
            PhraseEntry? currentPhrase = null;
            if (!string.IsNullOrEmpty(group))
                currentPhrase = DataController.PhraseEntries.Find(x => x.Id == group);

            return currentPhrase;
        }


        /// <summary>
        /// Sets the last movie to current.
        /// </summary>
        /// <autogeneratedoc />
        //public static void SetLastMovieToCurrent()
        //{
        //    MainWindowViewModel? viewModel = GetMainWindowViewModel();

        //    if (viewModel != null)
        //    {
        //        viewModel.CurrentMovie = GetLastMovie();
        //    }
        //}

        /// <summary>Sets the current movie.</summary>
        /// <param name="movie">The movie.</param>
        //public static void SetCurrentMovie(Models.Movies movie)
        //{
        //    MainWindowViewModel? viewModel = GetMainWindowViewModel();

        //    if (viewModel != null)
        //    {
        //        viewModel.CurrentMovie = movie;
        //    }
        //}

        /// <summary>
        /// The GetMainWindowViewModel.
        /// </summary>
        /// <returns>The <see cref="ViewModels.MainWindowViewModel"/>.</returns>
        //        public static ViewModels.MainWindowViewModel GetMainWindowViewModel()
        //        {
        //            ViewModels.MainWindowViewModel? model = null;

        //#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        //            Window window = GetWindow() as Views.MainWindow;
        //#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        //            if (window != null)
        //            {
        //                model = window.DataContext as ViewModels.MainWindowViewModel;
        //            }

        //            return model;
        //        }

        /// <summary>
        /// The GetOS.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetOS()
        {
            if (OperatingSystem.IsLinux()) return "Linux";
            else if (OperatingSystem.IsWindows()) return "WinNT";
            else return "Unknown";


#pragma warning disable CS8602 // Dereference of a possibly null reference.
            // return AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        /// <summary>
        /// The GetTime.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetTime()
        {
            string retVal = string.Empty;
            if (Writer != null) Writer.WriteLine("get-time");
            return retVal;
        }

        /// <summary>
        /// The GetWindow.
        /// </summary>
        /// <returns>The <see cref="Window"/>.</returns>
        public static Window GetWindow()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        /// <summary>
        /// The PlayMovie.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        /// <param name="currentBookmark">The currentBookmark<see cref="Bookmark"/>.</param>
        public static void PlayMovie(string moviePath, Bookmark? currentBookmark)
        {
            string mPath = FixImagePath(moviePath);
            string localPath = mPath;
            Uri uri = new Uri(mPath);
            string path = string.Empty;
            string os = Support.GetOS();

            if (os == "WinNT")
            {
                path = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
            }
            else
            {
                path = "/snap/bin/vlc";
            }
            ProcessStartInfo psi = new ProcessStartInfo(path);
            if (currentBookmark != null && currentBookmark.Time != null && currentBookmark.Time.Value > 0)
            {
                psi.Arguments = '"' + mPath + '"' + " --start-time=" + currentBookmark.Time.ToString();
            }
            else psi.Arguments = '"' + mPath + '"';
            VLCProcess = Process.Start(psi);
        }

        public static void PlayTrack(string trackPath)
        {
            string mPath = FixImagePath(trackPath);
            string os = Support.GetOS();
            string path = string.Empty;
            if (os == "WinNT")
            {
                path = @"C:\Program Files\ffmpeg\bin\ffplay.exe";
            }
            else
            {
                path = "/snap/bin/vlc";
            }
            ProcessStartInfo psi = new ProcessStartInfo(path);
            psi.Arguments = '"' + mPath + '"';
            VLCProcess = Process.Start(psi);
        }

        /// <summary>
        /// The PlayMovieControlled.
        /// </summary>
        /// <param name="moviePath">The moviePath<see cref="string"/>.</param>
        /// <param name="bookmark">The bookmark<see cref="Bookmark"/>.</param>
        public static void PlayMovieControlled(string moviePath, Bookmark? bookmark = null)
        {
            string arguments = string.Empty;
            if (bookmark != null && bookmark.Time != null && bookmark.Time.Value > 0)
            {
                arguments = '"' + moviePath + '"' + " --start-time=" + bookmark.Time.ToString();
            }
            else arguments = '"' + moviePath + '"';
            arguments += " --extraintf rc ";

            string VLCFilePath;

            string os = Support.GetOS();
            if (os == "WinNT")
            {
                VLCFilePath = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
            }
            else
            {
                VLCFilePath = "/snap/bin/vlc";
            }

            ProcessStartInfo psi = FFMpegSupport.GenerateStartInfo(arguments, VLCFilePath, false, false, ProcessWindowStyle.Normal, true);
            FfMpegProc = Process.Start(psi);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Writer = FfMpegProc.StandardInput;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        /// <summary>
        /// The SentenceCase.
        /// </summary>
        /// <param name="input">The input<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SentenceCase(string input)
        {

            // start by converting entire string to lower case
            string lowerCase = input.ToLower();
            // matches the first sentence of a string, as well as subsequent sentences
            Regex r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            // MatchEvaluator delegate defines replacement of setence starts to uppercase
            string result = r.Replace(lowerCase, s => s.Value.ToUpper());
            return result;
        }

        internal static void OpenTextEditor(string file, object value)
        {
            // open textpad or gedit depending on OS
            string os = GetOS();
            string editorPath = @"C:\Program Files\TextPad 8\TextPad.exe";

            if (os == "Linux")
            {
                editorPath = "/usr/bin/gedit";
            }
            else if (os == "WinNT")
            {
                editorPath = @"C:\Program Files\TextPad 8\TextPad.exe";
            }
            else
            {
                editorPath = "/usr/bin/gedit";
            }

            // use cliwrap to open the editor
            Cli.Wrap(editorPath)
                .WithArguments(new[] { file })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

        }

        public static string FormatFileSize(long totalSize)
        {
            return totalSize
                switch
            {
                < 1024 => $"{totalSize} B",
                < 1048576 => $"{totalSize / 1024.0:F2} KB"
                    ,
                < 1073741824 => $"{totalSize / 1048576.0:F2} MB"
                    ,
                _ => $"{totalSize / 1073741824.0:F2} GB"
            };
        }

        internal async Task<int> CreateVideoFromFrameSet(ImageSetViewModel imageSetviewModel, MovieImage currentSubFolder,
            FrameSet currentFrameSet)
        {
            string frameSetName = "FrameSet" + currentFrameSet.Index.ToString("000").Trim();
            string outputDirectory = Path.Combine(currentSubFolder.Path, frameSetName);
            string imageFileStub = outputDirectory;
            double aspectRatio = 0;
            double absMaxWidth = 0;
            double absMaxHeight = 0;
            int result = -1;

            ImageSetViewModel = imageSetviewModel;
            FFMpegSupport fFMpeg = new FFMpegSupport();
            fFMpeg.CliWrapCompleted += FFMpeg_CliWrapCompleted;
            fFMpeg.CliWrapError += FFMpeg_CliWrapError;
            fFMpeg.CliWrapProgress += FFMpeg_CliWrapProgress;

            MovieProgressEventargs progressChangedEventArgs = null;

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            DeleteFilesInFolder(outputDirectory);

            if (currentSubFolder.FrameSetHeader == null)
            {
                currentSubFolder.FrameSetHeader = new FrameSetHeader()
                {
                    MovieImageId = currentSubFolder.Id,
                    MaxXSize = 0,
                    MaxYSize = 0
                };
            }

            if (currentSubFolder.FrameSetHeader != null)
            {
                //currentSubFolder.jsonRead = false;
                //currentSubFolder.FromJson();
                int maxWidth = currentSubFolder.FrameSetHeader.MaxXSize;
                int maxHeight = currentSubFolder.FrameSetHeader.MaxYSize;

                if (currentSubFolder.FrameSetHeader.MaxXSize == 0 || currentSubFolder.FrameSetHeader.MaxYSize == 0)
                {
                    ImageItemsCollection? images = currentSubFolder.ImageItems;


                    //(absMaxWidth, absMaxHeight, progressChangedEventArgs, indx,
                    (maxWidth, maxHeight) =
                        await GetMaxSizes(progressChangedEventArgs, images);
                    currentSubFolder.FrameSetHeader.MaxXSize = maxWidth;
                    currentSubFolder.FrameSetHeader.MaxYSize = maxHeight;
                    //currentSubFolder.ToJson();
                    currentSubFolder.Save();
                }

                ImageItemsCollection? imageItems = new ImageItemsCollection();
                for (int i = currentFrameSet.StartImage - 1; i < currentFrameSet.EndImage; i++)
                {
                    imageItems.Add(currentSubFolder.ImageItems[i]);
                }

                if (maxHeight % 2 != 0) maxHeight += 1;
                if (maxWidth % 2 != 0) maxWidth += 1;

                // then we go through all images and save them to a created temp directory 
                // resizing the images to fit 
                SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.WhiteSmoke);

                int count = imageItems.Count;


                // now process images for this video

                bool success = await BuildImages(imageItems, imageFileStub, absMaxWidth, absMaxHeight, progressChangedEventArgs, null, maxWidth, maxHeight
                     , count);
                string outputFileName = imageFileStub + "\\" + System.IO.Path.GetFileNameWithoutExtension(currentSubFolder.Path) + ".mp4";

                string ffMpegCommand = "";
                //FFMpegSupport fFMpeg = new FFMpegSupport();
                if (FrameSetHeader == null || FrameSetHeader.FPS == null)
                    ffMpegCommand = " -framerate 3 -i " + '"' + imageFileStub + "\\" + "%04d.jpg" + '"' + " -c:v libx264 -r 20 " + '"' + outputFileName + '"';
                else
                    ffMpegCommand = " -framerate " + FrameSetHeader.FPS.Value.ToString("0.00") + " -i " + '"' + imageFileStub + "\\" + "%04d.jpg" + '"' + " -c:v libx264 -pix_fmt yuv420p " + '"' + outputFileName + '"';

                // if outputfile exists delete it
                if (File.Exists(outputFileName)) File.Delete(outputFileName);


                fFMpeg.action = "CreateMovie";
                fFMpeg.FrameCount = imageItems.Count;

                result = await fFMpeg.DoCliWrapCreateMovie(ffMpegCommand);
                currentFrameSet.HasMovie = (result == 0);
            }
            return result;
        }
        #endregion

    }

    public static class PathExtensions
    {
        public static string GetLastPathSegment(this string path)
        {
            string lastPathSegment = path
                .Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();

            return lastPathSegment;
        }
    }
}



