//-----------------------------------------------------------------------
// <copyright file="DownloadSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>12/05/2022 12:53:50 12/05/2022 12:53:50 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using TaymadeEntities.Models;
    using MsBox.Avalonia.Enums;
    using MsBox.Avalonia;
    using ReactiveUI;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using TaymadeEntities.ViewModels;

    /// <summary>
    /// Defines the <see cref="DownloadSupport" />.
    /// </summary>
    public class DownloadSupport : ModelBase
    {
        #region Constants

        /// <summary>
        /// Defines the MovieBasePath.
        /// </summary>
        public const string MovieBasePath = @"K:\TD1\White\";

        #endregion

        #region Fields

        private static DownloadSupport? Instance;

        /// <summary>
        /// Defines the unboundGridDatas.
        /// </summary>
        private UnboundGridDataCollection? unboundGridDatas;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the UnboundGridDatas.
        /// </summary>
        public UnboundGridDataCollection? UnboundGridDatas
        {
            get => unboundGridDatas;
            set => this.RaiseAndSetIfChanged(ref unboundGridDatas, value);
        }

        #endregion

        private DownloadSupport()
        {

        }

        #region Methods

        public static DownloadSupport GetInstance()
        {
            if (Instance == null)
                Instance = new DownloadSupport();
            return Instance;
        }

        /// <summary>
        /// The CheckAndCreateDirectory.
        /// </summary>
        /// <param name="directory">The directory<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CheckAndCreateDirectory(string directory)
        {
            bool returnBool = true;

            // directory will always be in local form
            directory = Support.FixImagePath(directory);

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (System.Exception)
            {
                returnBool = false;
            }

            return returnBool;
        }



        /// <summary>
        /// The CleanText.
        /// </summary>
        /// <param name="messy">The messy<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string CleanText(string messy)
        {
            string clean = Regex.Replace(messy, @"[\p{L}-[a-zA-Z]]+", "");
            clean = clean.Replace("&", " and ").Replace(",", " ");
            return clean;
        }

        /// <summary>
        /// The DirectorySeparator.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string DirectorySeparator()
        {
            string sep = @"\";

            if (Support.GetOS() != "WinNT") sep = "/";
            return sep;
        }

        /// <summary>
        /// The ProcessMovieFiles.
        /// </summary>
        /// <param name="movies">The movies<see cref="List{Models.Movies}"/>.</param>
        /// <param name="item">The item<see cref="string"/>.</param>
        /// <param name="processDirectories">The processDirectories<see cref="bool"/>.</param>
        public static void ProcessMovieFiles(DownloadViewModel viewModel, List<Models.Movies> movies, string item = @"K:\TD1\White\Download", bool processDirectories = false)
        {

            string searchFolder = item.Replace("\\", "/");
            string[] movieFiles = System.IO.Directory.GetFiles(searchFolder, "*.*");
            foreach (string file in movieFiles)
            {
                string extention = Path.GetExtension(file).ToLower();
                if (extention == ".wmv" || extention == ".mp4" || extention == ".avi" || extention == ".flv"
                    || extention == ".mpeg" || extention == ".mts" || extention == ".mov" || extention == ".mpg"
                    || extention == ".webm" || extention == ".rm" || extention == ".mkv")
                {
                    string filename = Path.GetFileName(file).Replace("\\", "/");
                    // dataGridViewRow.Cells[1].Value = filename;
                    Movies? movie = movies.Find(x => x.MoviePath.ToLower().Contains(filename.ToLower()));
                    if (movie == null)
                    {
                        UnboundGridData unboundGridData = new UnboundGridData();


                        unboundGridData.Folder = searchFolder;

                        // get files in folder and look for Movie Files
                        searchFolder = item.Replace("\\", "/");

                        unboundGridData.FileName = file;
                        GetFileInfo(searchFolder, filename, unboundGridData);

                        //DownloadSupport instance = DownloadSupport.GetInstance();

                        if (viewModel != null && viewModel.Unbounds != null)
                            viewModel.Unbounds.Add(unboundGridData);

                    }
                }
            }
            if (processDirectories)
            {
                string[] directories = System.IO.Directory.GetDirectories(item + "\\", "*.*");
                foreach (string dir1 in directories)
                {
                    ProcessMovieFiles(viewModel, movies, dir1, true);
                }
            }
            else
            {
                //dgUnScannedMovies.Sort(dgUnScannedMovies.Columns[lastDGColumn], dgColumnDirection);
            }
        }

        private static void GetFileInfo(string searchFolder, string filename, UnboundGridData unboundGridData)
        {
            string dataFileName = searchFolder.Replace("/", "\\") + @"\" + filename;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(dataFileName);

            unboundGridData.FileLength = fileInfo.Length;
            unboundGridData.FileInfo = fileInfo;

            unboundGridData.CreationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm");
        }

        internal static System.IO.FileInfo? GetFileInfo(string filePath)
        {
            string dataFileName = filePath;
            if (!File.Exists(filePath))
                return null;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(dataFileName);

            long fileLength = fileInfo.Length;
            string creationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm");

            return fileInfo;
        }

        internal static void GetFileInfo(UnboundGridData unboundGridData)
        {
            string dataFileName = unboundGridData.FileName;
            if (File.Exists(unboundGridData.FileName))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(dataFileName);

                unboundGridData.FileLength = fileInfo.Length;
                unboundGridData.FileInfo = fileInfo;

                unboundGridData.CreationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm");
            }
        }

        /// <summary>
        /// The ProcessRoot.
        /// </summary>
        public static async void ProcessRootx(DownloadViewModel viewModel)
        {
            // generate collection
            string sep = DirectorySeparator();

            string baseRoot = Support.FixImagePath(MovieBasePath + @"Download");

            if (Directory.Exists(baseRoot))   // check to see if we can find the directory
            {
                string[] files = System.IO.Directory.GetFiles(baseRoot + sep, "*.*");
                string searchFolder = baseRoot; //.Replace("\\", "/");
                foreach (string file in files)
                {
                    string extention = Path.GetExtension(file).ToLower();
                    if (extention == ".wmv" || extention == ".mp4" || extention == ".avi" || extention == ".flv"
                        || extention == ".mpeg" || extention == ".mts" || extention == ".mov" || extention == ".mpg"
                        || extention == ".webm" || extention == ".rm" || extention == ".mkv")
                    {
                        string filename = Path.GetFileName(file).Replace("\\", "/").ToLower();
                        if (viewModel.Unbounds == null || viewModel.Unbounds.Count < 50)
                        {  // generate collection from database if not already generated
                            viewModel.Unbounds = new System.Collections.ObjectModel.ObservableCollection<UnboundGridData>(
                                DataController.SandboxEntities.UnboundGridData.ToList());
                        }
                        UnboundGridData? unboundGridData = viewModel.Unbounds.Where(u => u.FileName.ToLower() == file.ToLower()).FirstOrDefault();
                        if (unboundGridData == null)
                        {
                            unboundGridData = new UnboundGridData();
                            unboundGridData.Folder = searchFolder;
                            unboundGridData.FileName = file;
                            string dataFileName = searchFolder + sep + filename;
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(dataFileName);
                            unboundGridData.FileLength = fileInfo.Length;
                            unboundGridData.CreationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm");
                            unboundGridData.FileInfo = fileInfo;
                            if (unboundGridData.Insert() && viewModel.Unbounds != null)
                                viewModel.Unbounds.Add(unboundGridData);
                        }
                        //DownloadSupport instance = DownloadSupport.GetInstance();

                    }
                }
            }
            else
            {
                var box = MessageBoxManager
            .GetMessageBoxStandard("Warning", "Directory " + baseRoot + " Cannot be found or disk is offline",
                ButtonEnum.Ok);

                var result = await box.ShowAsync();
            }
        }

        #endregion
    }
}
