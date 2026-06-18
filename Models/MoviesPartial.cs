//-----------------------------------------------------------------------
// <copyright file="MoviesPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>27/04/2022 17:16:40 27/04/2022 17:16:40 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Avalonia.Media;
    using TaymadeEntities.Support;
    using TaymadeEntities.ViewModels;
    //using DocumentFormat.OpenXml.Office2010.Excel;
    // using DocumentFormat.OpenXml.Office2010.ExcelAc;
    using DynamicData.Binding;
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Avalonia.Controls;

    /// <summary>
    /// Defines the <see cref="MovieMetaData" />.
    /// </summary>
    public class MovieMetaData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ImagePath.
        /// </summary>
        [MaxLength(250, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the MovieName.
        /// </summary>
        [MaxLength(100, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? MovieName { get; set; }

        /// <summary>
        /// Gets or sets the MoviePath.
        /// </summary>
        [MaxLength(250, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? MoviePath { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Defines the <see cref="Movies" />.
    /// </summary>
    public partial class Movies
    {
        #region Fields

        /// <summary>
        /// Defines the pbs.
        /// </summary>
        internal string pbs = string.Empty;

        /// <summary>
        /// Defines the backColour.
        /// </summary>
        private ISolidColorBrush backColour = Avalonia.Media.Brushes.GhostWhite;

        /// <summary>
        /// Defines the director.
        /// </summary>
        //private Director? director;

        private string baseFileName;

        /// <summary>
        /// Defines the directorsName.
        /// </summary>
        private string directorsName;

        /// <summary>
        /// Defines the doneColour.
        /// </summary>
        private ISolidColorBrush doneColour = Avalonia.Media.Brushes.White;

        /// <summary>
        /// Defines the imageBMP.
        /// </summary>
        private Avalonia.Media.Imaging.Bitmap? imageBMP;

        /// <summary>
        /// Defines the movieDuration.
        /// </summary>
        private TimeSpan movieDuration = TimeSpan.MinValue;

        /// <summary>
        /// Defines the nfo.
        /// </summary>
        private Nfo nfo;

        /// <summary>
        /// Defines the nforFileName.
        /// </summary>
        private string nforFileName;

        /// <summary>
        /// Defines the seriesEntity.
        /// </summary>
        private Series? seriesEntity;
        #endregion Fields

        #region Properties

        public bool notBuilt = true;

        private MovieExtn movieExtension;

        private ObservableCollection<MovieLanguage> movieLanguages;

        private ObservableCollection<ProductionCompany> productionCompanies;

        /// <summary>
        /// Gets or sets the BackColour.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.ISolidColorBrush BackColour { get => backColour; set => this.RaiseAndSetIfChanged(ref backColour, value); }

        [NotMapped]
        public string BaseFileName
        {
            get
            {
                if (string.IsNullOrEmpty(baseFileName))
                {
                    baseFileName = System.IO.Path.GetFileNameWithoutExtension(MoviePath);
                }
                return baseFileName;
            }
            private set => baseFileName = value;
        }

        /// <summary>
        /// Gets or sets the DirectorsName.
        /// </summary>
        [NotMapped]
        public string DirectorsName
        {
            get
            {
                if (string.IsNullOrEmpty(directorsName) && DirectorID != null && DirectorID.Value > 0)
                {
                    if (Director == null)
                    {
                        Director = DataController.DirectorList.Find(x => x.Id == DirectorID);
                    }
                    if (Director != null) directorsName = Director.Name;
                }
                return directorsName;
            }

            set => this.RaiseAndSetIfChanged(ref directorsName, value);
        }

        /// <summary>
        /// Gets or sets the DoneColour.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.ISolidColorBrush DoneColour { get => doneColour; set => this.RaiseAndSetIfChanged(ref doneColour, value); }

        /// <summary>
        /// Gets or sets the ImageBMP.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.Imaging.Bitmap? ImageBMP
        {
            get
            {
                if (!string.IsNullOrEmpty(FixImagePath()))
                {
                    string fileName = FixImagePath();
                    if (System.IO.File.Exists(fileName) && imageBMP == null)
                    {
                        imageBMP = Support.GetBMP(fileName);

                        //     imageBMP = new Avalonia.Media.Imaging.Bitmap(memory);
                        //this.RaiseAndSetIfChanged(ref imageBMP, imageBMP);
                        //}
                    }
                }
                return imageBMP;
            }
            set => imageBMP = value; //this.RaiseAndSetIfChanged(ref imageBMP, value);
        }

        /// <summary>
        /// Gets or sets the MovieDuration.
        /// </summary>
        [NotMapped]
        public TimeSpan MovieDuration
        {
            get
            {
                SetMovieDurationValue();
                return movieDuration;
            }

            set =>
                //movieDuration = value;
                this.RaiseAndSetIfChanged(ref movieDuration, value);
        }

        [NotMapped]
        public MovieExtn MovieExtension
        {
            get
            {
                if (!string.IsNullOrEmpty(Json) && movieExtension == null)
                {
                    movieExtension = new MovieExtn(Json);
                    movieExtension.Parent = this;
                }

                if (movieExtension == null)
                {
                    movieExtension = new MovieExtn() { Rating = 1 };
                }

                return movieExtension;
            }

            set
            {
                movieExtension = value;
                if (value != null) Json = movieExtension.Serialise();
            }
        }

        public ObservableCollection<MovieLanguage> MovieLanguages
        {
            get
            {
                if (movieLanguages == null)
                {
                    movieLanguages = new ObservableCollection<MovieLanguage>(DataController.SandboxEntities.MovieLanguage.Where(l => l.MovieId == Id).ToList());
                }
                return movieLanguages;
            }

            set => movieLanguages = value;
        }

        /// <summary>
        /// Gets or sets the Nfo.
        /// </summary>
        [NotMapped]
        public Nfo Nfo
        {
            get
            {
                if (nfo == null && !string.IsNullOrEmpty(NforFileName))
                {
                    nfo = new Nfo(NforFileName);
                }
                return nfo;
            }
            set => this.RaiseAndSetIfChanged(ref nfo, value);
        }

        /// <summary>
        /// Gets the NforFileName.
        /// </summary>
        [NotMapped]
        public string NforFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(MoviePath) && string.IsNullOrEmpty(nforFileName))
                {
                    string temp = System.IO.Path.GetDirectoryName(MoviePath) + @"\" + System.IO.Path.GetFileNameWithoutExtension(MoviePath) + ".nfo";
                    nforFileName = Support.FixImagePath(temp);
                }

                return nforFileName;
            }
            private set => nforFileName = value;
        }

        /// <summary>
        /// Gets or sets the PercentUnBookmarked.
        /// </summary>
        [NotMapped]
        public string PercentUnBookmarked
        {
            get => pbs;

            set => this.RaiseAndSetIfChanged(ref pbs, value);
        }

        /// <summary>
        /// Gets the Process.
        /// </summary>
        [NotMapped]
        public Process? Process { get; private set; }

        public ObservableCollection<ProductionCompany> ProductionCompanies
        {
            get
            {
                if (productionCompanies == null)
                {
                    var temp = DataController.SandboxEntities.ProductionCompanyMovie.Where(p => p.MovieId == Id).Select(m => m.CompanyId).ToList();
                    productionCompanies = new ObservableCollection<ProductionCompany>(DataController.ProductionCompanies.Where(c => temp.Contains(c.Id)).ToList());
                }
                return productionCompanies;
            }

            set => this.RaiseAndSetIfChanged(ref productionCompanies, value);
        }
        /// <summary>
        /// Gets the RunTime.
        /// </summary>
        [NotMapped]
        public string RunTime
        {
            get
            {
                if (MovieDuration.TotalSeconds > 0)
                {
                    return MovieDuration.ToString();
                }
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Gets or sets the SeriesEntity.
        /// </summary>
        [NotMapped]
        public Series? SeriesEntity
        {
            get
            {
                if (seriesEntity == null && Series != null && Series != 2)
                {
                    seriesEntity = new Series(Series);
                }
                return seriesEntity;
            }

            set => seriesEntity = value;
        }

        /// <summary>
        /// Gets the UnbookmarkedTime.
        /// </summary>
        [NotMapped]
        public string UnbookmarkedTime => TimeFromLastBookMarkToEnd().ToString();

        #endregion Properties

        #region Methods

        [NotMapped]
        private string? ChangedFields { get; set; }

        public static string GetTempFileName(string MoviePath)
        {
            string extn = System.IO.Path.GetExtension(MoviePath);

            string filename = System.IO.Path.GetFileNameWithoutExtension(MoviePath);

            string moviePath = System.IO.Path.GetDirectoryName(MoviePath) + @"\" + filename + "temp" + extn;

            return moviePath;
        }

        /// <summary>
        /// The SetMovieDuration.
        /// </summary>
        /// <param name="value">The value<see cref="int?"/>.</param>
        /// <returns>The <see cref="TimeSpan"/>.</returns>
        public static TimeSpan SetMovieDuration(int? value)
        {
            if (value != null)
                return TimeSpan.FromSeconds((Double)value);
            else
                return TimeSpan.Zero;
        }

        public void BuildGenreList()
        {
            string[] genres = FilmGroup.Split(new char[] { ',' });

            if (genres.Length > MovieGenres.Count && notBuilt)
            {
                notBuilt = false;
                foreach (var item in genres)
                {
                    PhraseEntry? genrePhrase = DataController.PhraseEntries.Find(p => p.Id == item);
                    PhraseEntry? subGenrePhrase = null;
                    string compKey = string.Empty;

                    if (genrePhrase == null) genrePhrase = DataController.SubPhraseEntries.Find(p => p.Id == item);

                    if (genrePhrase != null)
                    {
                        MovieGenre? movieGenre = null;

                        if (genrePhrase.PhraseID == 1)
                        {
                            compKey = genrePhrase.COMPKEY;
                        }
                        else if (genrePhrase.PhraseID == 9)
                        {
                            string[] elements = genrePhrase.Id.Split(new char[] { '.' });

                            if (elements.Length > 1)
                            {
                                subGenrePhrase = DataController.PhraseEntries.Find(p => p.Id == elements[0]);
                                if (subGenrePhrase != null)
                                {
                                    compKey = subGenrePhrase.COMPKEY;
                                }
                            }
                        }

                        // only add if new genre
                        movieGenre = MovieGenres.Where(m => m.MovieId == Id && m.Genre == compKey).FirstOrDefault();

                        if (movieGenre == null)
                        {
                            movieGenre = new MovieGenre() { MovieId = Id, Genre = compKey };
                            if (subGenrePhrase != null)
                            {
                                movieGenre.SubGenre = genrePhrase.COMPKEY;
                            }
                            movieGenre.Insert();
                            // MovieGenres.Add(movieGenre);
                            this.RaisePropertyChanged(nameof(MovieGenres));
                        }
                        if (movieGenre != null && subGenrePhrase != null)
                        {
                            movieGenre.SubGenre = genrePhrase.COMPKEY;
                            movieGenre.Save();
                        }
                    }
                }
            }

            this.GetMovieGenreList();
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 01/02/2026 01/02/2026 </created>
        /// </remarks>
        private void GetMovieGenreList()
        {
            this.MovieGenres.Clear();
            this.MovieGenres = new ObservableCollection<MovieGenre>(DataController.SandboxEntities.MovieGenre.Where(g => g.MovieId == this.Id).ToList());
        }

        public void CreateFirstBookmark()
        {
            if (Bookmarks == null) Bookmarks = new ObservableCollection<Bookmark>();
            Bookmark? bookmark = Bookmarks.FirstOrDefault();
            if (bookmark == null)
            {
                try
                {


                    bookmark = new Bookmark();
                    bookmark.Name = MovieName;
                    bookmark.Time = 10;
                    bookmark.Type = "BOOKMARK";
                    bookmark.MovieID = Id;
                    //VideoSupport videoSupport = new VideoSupport();
                    //VideoSupport.GrabBookmarkImage(this, bookmark);

                    //bookmark.ImagePath = VideoSupport.thumbnailPath;
                    //if (Id > 0)
                    //{
                    //    DataController.SandboxEntities.Bookmarks.Add(bookmark);
                    //    DataController.SandboxEntities.SaveChanges();
                    //    SetPercentUnmarked();
                    //}

                    ImagePath = bookmark.ImagePath;
                    Save();
                }
                catch (Exception ex)
                {

                    string error = ex.ToString();
                }
            }
        }

        public string CreateXSPFDirectory(PhraseEntry phrase, string filmName)
        {
            string XSPFilenameDir;
            if (phrase != null)
            {
                XSPFilenameDir = @"k:\td1\xspf\" + phrase.Id;
            }
            else
                XSPFilenameDir = @"k:\td1\xspf\missing\";
            // need to check xspf directory exists
            DownloadSupport.CheckAndCreateDirectory(XSPFilenameDir);

            string XSPFilename = XSPFilenameDir + @"\" + filmName + ".xspf";

            path = XSPFilename;
            return XSPFilename;
        }

        public bool DeleteBookmark(Bookmark bookmark)
        {
            bool success = false;
            if (bookmark != null && this.Bookmarks.Contains(bookmark))
            {
                // delete image file
                if (!string.IsNullOrEmpty(bookmark.ImagePath))
                {
                    // image file to delete

                    string imagePath = Support.FixImagePath(bookmark.ImagePath);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                this.Bookmarks.Remove(bookmark);
                imagesCount -= 1;
                bookmark.Delete();
                success = true;
            }

            return success;
        }
        /// <summary>
        /// The EditMovie.
        /// </summary>
        /// <param name="main">The main<see cref="Views.MainWindow"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task EditMovie(Window main)
        {
            ViewModels.MovieEditViewModel? mvm = new ViewModels.MovieEditViewModel(this);

            Dialogs.MovieEditDialog editor = new Dialogs.MovieEditDialog(mvm);
            editor.DataContext = mvm;
            //Window main = GetWindow();
            mvm.Caller = editor;
            //Dialogs.DialogResultButton result;

            bool result = await editor.ShowDialog<bool>(main);

            if (result)
            {
                // save movie

                if (mvm.CurrentMovie != null)
                {
                    if (mvm.CurrentMovie.SeriesEntity != null)
                    {
                        if (mvm.CurrentMovie.Series == null)
                        {
                            mvm.CurrentMovie.Series = mvm.CurrentMovie.SeriesEntity.Id;
                        }
                    }

                    if (mvm.CurrentMovie.Director != null)
                    {
                        if (mvm.CurrentMovie.DirectorID == null) mvm.CurrentMovie.DirectorID = mvm.CurrentMovie.Director.Id;
                    }

                    mvm.CurrentMovie.MoviePath = Support.FixPathBack(mvm.CurrentMovie.MoviePath);

                    mvm.CurrentMovie.Save();

                    if (mvm.CurrentSeries != null)
                    {
                        mvm.CurrentSeries.Save();
                    }
                }
            }
        }

        /// <summary>
        /// The FixImagePath.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string FixImagePath()
        {
            return Support.FixImagePath(ImagePath);
        }

        /// <summary>
        /// The FixLinuxPath.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string FixLinuxPath()
        {
            return Support.FixImagePath(MoviePath);
        }

        /// <summary>
        /// The FixMovieData.
        /// </summary>
        public async void FixMovieData()
        {
            //if (Series == null) Series = 2;
            try
            {
                if (Bookmarks.Count == 0)
                {
                    Bookmarks = new ObservableCollection<Bookmark>(
                        DataController.BookmarkController.GetBookmarksByMovieId(Id) ?? new List<Bookmark>());
                    //Models.DataController.SandboxEntities.Bookmarks.Where(x => x.MovieID == Id).ToList());
                }

                if (Bookmarks.Count > 0)
                {
                    if (ImagesCount != bookmarks.Count)
                    {
                        ImagesCount = bookmarks.Count;

                        //Save();
                    }
                    this.RaisePropertyChanged(nameof(ImagesCount));
                }

                if (string.IsNullOrEmpty(ImagePath) && Bookmarks.Count > 0)
                {
                    Models.Bookmark? bookmark = Bookmarks.FirstOrDefault(x => !string.IsNullOrEmpty(x.ImagePath));
                    if (bookmark != null) ImagePath = bookmark.ImagePath;
                    ImagesCount = Bookmarks.Count;
                }

                if (Episode != null && EpisodeEntity == null) this.GetEpisodeEntity();

                if (EpisodeEntity != null)
                {
                    EpisodeNumber = EpisodeEntity.EpisodeNumber;
                }

                if (!System.IO.File.Exists(Support.FixImagePath(MoviePath)))
                {
                    BackColour = Avalonia.Media.Brushes.Red;
                }

                int count = Casts.Count;
                if (count == 0)
                {

                    List<Cast> casts = await DataController.CastController.GetCastsByMovieIdAsync(Id);
                    Casts = new ObservableCollection<Cast>(
                        casts);
                    this.RaisePropertyChanged(nameof(Casts));
                }

                SetPercentUnmarked();

                DataController.MovieController.UpdateMovie(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetDuration.
        /// </summary>
        public async Task GetDuration(string tempMoviePath = "")
        {
            if (string.IsNullOrEmpty(tempMoviePath)) tempMoviePath = MoviePath;
            int? time = await FFMpegSupport.GetMovieDurationAsync(tempMoviePath);
            if (time.HasValue)
            {
                DurationSeconds = time;
            }

            //TaymadeEntities.Support.FFProbeInfo? info = await FFMpegSupport.GetFFProbeInfo(tempMoviePath);
            ////int time = await VideoSupport.GetDurationSeconds(tempMoviePath, this);
            //if (info != null && info.Duration != null)
            //{
            //    {
            //        TimeSpan duration = TimeSpan.Parse(info.Duration);
            //        int time = (int)duration.TotalSeconds;
            //        DurationSeconds = time;
            //    }
            //}
        }

        public string? GetTempFileName()
        {
            if (!string.IsNullOrEmpty(MoviePath))
            {
                string extn = System.IO.Path.GetExtension(MoviePath);

                string moviePath = string.Empty;

                string filename = System.IO.Path.GetFileNameWithoutExtension(MoviePath);

                if (!MoviePath.Contains("temp.", StringComparison.OrdinalIgnoreCase))
                {
                    moviePath = System.IO.Path.GetDirectoryName(MoviePath) + @"\" + filename + "temp" + extn;
                }
                else moviePath = MoviePath;

                return moviePath;
            }
            return null;
        }

        public string GetTempImageName()
        {
            string extn = System.IO.Path.GetExtension(MoviePath);

            string filename = System.IO.Path.GetFileNameWithoutExtension(MoviePath);

            string moviePath = System.IO.Path.GetDirectoryName(MoviePath) + @"\" + filename + "10.bmp";

            return moviePath;
        }
        /// <summary>
        /// The SetDoneColour.
        /// </summary>
        public void SetDoneColour()
        {
            double percent = 0;
            if (Bookmarks != null && Bookmarks.Count > 0 && DurationSeconds != null && DurationSeconds > 0)
            {
                Bookmark? bookmark = Bookmarks.LastOrDefault();
                if (bookmark != null && bookmark.Time != null && bookmark.Time > 0)
                {
                    percent = 1.0 - ((double)bookmark.Time.Value / (double)DurationSeconds.Value);
                }
            }
            if (percent < 0.1) DoneColour = Avalonia.Media.Brushes.White;
            else if (percent < 0.2) DoneColour = Avalonia.Media.Brushes.Green;
            else if (percent < 0.3) DoneColour = Avalonia.Media.Brushes.PaleGreen;
            else if (percent < 0.4) DoneColour = Avalonia.Media.Brushes.GreenYellow;
            else if (percent < 0.5) DoneColour = Avalonia.Media.Brushes.YellowGreen;
            else if (percent < 0.6) DoneColour = Avalonia.Media.Brushes.Yellow;
            else if (percent < 0.7) DoneColour = Avalonia.Media.Brushes.Goldenrod;
            else if (percent < 0.8) DoneColour = Avalonia.Media.Brushes.Orange;
            else if (percent < 0.9) DoneColour = Avalonia.Media.Brushes.OrangeRed;
            else if (percent < 1) DoneColour = Avalonia.Media.Brushes.Red;
        }

        /// <summary>
        /// The SetMovieDurationValue.
        /// </summary>
        public void SetMovieDurationValue()
        {
            if (movieDuration == TimeSpan.MinValue && DurationSeconds != null)
            {
                MovieDuration = SetMovieDuration(DurationSeconds);

                //this.RaiseAndSetIfChanged(ref movieDuration, value);
            }
            else if ((int)movieDuration.TotalSeconds != DurationSeconds && DurationSeconds != null)
            {
                MovieDuration = SetMovieDuration(DurationSeconds);
                // this.RaiseAndSetIfChanged(ref movieDuration, value);
                // this.RaisePropertyChanged("MovieDuration");
            }
            else if (movieDuration == TimeSpan.MinValue)
            {
                MovieDuration = SetMovieDuration(0);
                //movieDuration = TimeSpan.FromSeconds((Double)0);
            }
        }

        /// <summary>
        /// The SetPercentUnmarked.
        /// </summary>
        public void SetPercentUnmarked()
        {
            double pbm = 0;
            TimeSpan time = TimeFromLastBookMarkToEnd();
            if (time > TimeSpan.MinValue)
            {
                pbm = time.TotalSeconds / MovieDuration.TotalSeconds * 100;
                PercentUnBookmarked = pbm.ToString("000.0") + "%";

                if (Dirty) Save();

                //SetDoneColour();
            }
        }

        /// <summary>
        /// The TimeFromLastBookMarkToEnd.
        /// </summary>
        /// <returns>The <see cref="TimeSpan"/>.</returns>
        public TimeSpan TimeFromLastBookMarkToEnd()
        {
            TimeSpan filmDuration = TimeSpan.FromSeconds(0);
            if (DurationSeconds != null)
            {
                filmDuration = TimeSpan.FromSeconds(DurationSeconds.Value);
                Bookmark? last = Bookmarks.LastOrDefault();
                imagesCount = Bookmarks.Count;
                if (last != null && last.Time != null)
                {
                    TimeSpan lastTime = TimeSpan.FromSeconds(last.Time.Value);
                    TimeSpan left = filmDuration.Subtract(lastTime);
                    return left;
                }
                else
                    return filmDuration;
            }
            else
                return filmDuration;
        }

        public bool Delete()
        {
            return DataController.MovieController.DeleteMovie(Id);


            //DataController.SandboxEntities.Movies.Remove(this);
            //DataController.SandboxEntities.SaveChanges();
        }
        public Bookmark GetLastBookmark()
        {
            return Bookmarks.LastOrDefault();
        }
        public bool Insert()
        {
            bool success = true;
            try
            {
                int tmpYear = 0;
                if (Year != null) tmpYear = Year.Value;
                Movies? temp = DataController.SandboxEntities.CreateMovie(MovieName, tmpYear, MoviePath, FilmGroup);
                if (temp != null)
                {
                    this.Id = temp.Id;
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                success = false;
                //throw;
            }
            return success;
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 31/01/2026 31/01/2026 </created>
        /// </remarks>
        public void LoadChildren()
        {
            try
            {
                Casts = new ObservableCollection<Cast>(
                    DataController.CastController.GetCastsByMovieId(this.Id)
                    );
                Bookmarks = new ObservableCollection<Bookmark>(
                    DataController.BookmarkController.GetBookmarksByMovieId(this.Id));
                //DataController.SandboxEntities.Bookmarks.Where(b => b.MovieID == this.Id).OrderBy(b => b.Id).ToList());
                Director = DataController.SandboxEntities.Directors.Where(d => d.Movies.Contains(this)).FirstOrDefault();

                // set bookmark count
                if (Bookmarks != null) ImagesCount = Bookmarks.Count;

            }
            catch (Exception)
            {
            }
        }

        /// <summary>Logs the message.</summary>
        /// <param name="action">The action.</param>
        public void LogMessage(string action)
        {
            Support.GenerateInfoAndLogMessage(action, "Movie", Id, MovieName);
        }

        /// <summary>
        /// The Save.
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            bool success = true;
            if (HasChapters == null) HasChapters = false;
            if (HasEpisodes == null) HasEpisodes = false;
            if (Series == null) Series = 2;

            //if (dirty)
            //{
            if (MovieExtension == null)
            {
                MovieExtension = new MovieExtn();
                MovieExtension.Rating = 0;
                Json = MovieExtension.Serialise();
            }
            else Json = MovieExtension.Serialise();
            try
            {
                this.ErrorText = "";

                if (Id == 0)
                {
                    success = Insert();
                }

                if (SeriesEntity != null && (Series == null || Series != SeriesEntity.Id))
                {
                    Series = SeriesEntity.Id;
                }

                //if (Director != null)
                //{
                // Director.SaveAsync();
                //}

                ModifiedOn = DateTime.Now;
                success = await DataController.MovieController.UpdateMovieAsync(this);

                ClearErrors();
                LogMessage("Saved " + ChangedFields);
                ChangedFields = string.Empty;
                Dirty = false;                  // clear modified flag;
                //success = (saved == 1);
            }
            catch (Exception ex)
            {
                string msg = "error Saving movie : " + Id.ToString() + " : " + MovieName;
                Support.Logger.Error(ex, msg);

                MVMLogs logs = new MVMLogs(ex, "database", "Error");

                if (Errors == null) Errors = new System.Collections.Generic.List<ModelError>();

                ModelError error = new ModelError()
                {
                    Error = ex.Message,
                    Property = "save"
                };
                Errors.Add(error);
                success = false;
            }
            //}
            return success;
        }

        public bool Save()
        {
            bool success = false;
            if (HasChapters == null) HasChapters = false;
            if (HasEpisodes == null) HasEpisodes = false;
            if (Series == null) Series = 2;

            //if (dirty)
            //{
            if (MovieExtension == null)
            {
                MovieExtension = new MovieExtn();
                MovieExtension.Rating = 0;
                Json = MovieExtension.Serialise();
            }
            else Json = MovieExtension.Serialise();
            try
            {
                this.ErrorText = "";

                if (Id == 0)
                {
                    success = DataController.MovieController.Add(this);
                }

                if (SeriesEntity != null && (Series == null || Series != SeriesEntity.Id))
                {
                    Series = SeriesEntity.Id;
                }

                //if (Director != null)
                //{
                //    Director.Save();
                //}

                //EntityState state = DataController.SandboxEntities.Entry(this).State;
                //if (state == EntityState.Detached) DataController.SandboxEntities.Movies.Attach(this);

                //var local = DataController.SandboxEntities.Set<Movies>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                //// check if local is not null
                //if (local != null)
                //{
                //    // detach
                //    //DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                //}
                // set Modified flag in your entry
                ModifiedOn = DateTime.Now;
                //DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //int saved = DataController.SandboxEntities.SaveChanges();

                success = DataController.MovieController.UpdateMovie(this);

                ClearErrors();
                LogMessage("Saved " + ChangedFields);
                ChangedFields = string.Empty;
                Dirty = false;                  // clear modified flag;
                                                //success = (saved == 1  );
            }
            catch (Exception ex)
            {
                string msg = "error Saving movie : " + Id.ToString() + " : " + MovieName;
                Support.Logger.Error(ex, msg);

                MVMLogs logs = new MVMLogs(ex, "database", "Error");

                if (Errors == null) Errors = new System.Collections.Generic.List<ModelError>();

                ModelError error = new ModelError()
                {
                    Error = ex.Message,
                    Property = "save"
                };
                Errors.Add(error);
                success = false;
            }
            //}
            return success;
        }

        public void ReloadCasts()
        {
            Casts = new ObservableCollection<Cast>(
                DataController.CastController.GetCastsByMovieId(this.Id).ToList()
                );
            //using var ctx = DataController.SandboxEntities;
            //Casts = new ObservableCollection<Cast>(
            //        ctx.Casts.Where(c => c.MovieID == this.Id).OrderBy(c => c.ActorId).ToList());
        }

        #endregion Methods
    }
}