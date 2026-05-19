using Avalonia;
using Avalonia.Controls;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.Views;
using ReactiveUI;
using Splat.ModeDetection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace TaymadeEntities.ViewModels
{
    public class MovieEditViewModel : MovieViewModel, IDisposable
    {

        #region Private Fields

        private List<string>? autoCompleteList;
        private Actor? currentActor;
        private Bookmark currentBookmark;
        private Cast currentCastMember;
        private MovieGenre? currentGenre;
        private new Movies? currentMovie;
        private Models.Season currentSeason;
        private Series? currentSeries;
        private ObservableCollection<Director>? directorList;
        private bool disposedValue;
        private TVEpisode newEpisode;
        private PhraseEntry? newPhrase;
        private Models.Season? newSeason;
        private PhraseEntry? newSubPhrase;
        private ObservableCollection<PhraseEntry>? phraseEntries;
        private string? progress;
        private int progressPercent;
        private ObservableCollection<Series>? seriesList;
        private ObservableCollection<PhraseEntry>? subPhrases;
        private List<string> genderList;

        #endregion Private Fields

        #region Public Constructors

        public MovieEditViewModel()
        { }

        public MovieEditViewModel(Movies? currentMovie)
        {
            CurrentMovie = currentMovie;
            AddPhrase = ReactiveCommand.Create(DoAddPhrase);

            NewBookmark = ReactiveCommand.Create(Do_AddBookmark);
            EditBookmark = ReactiveCommand.Create(DoEditBookmark);
            PlayBookmark = ReactiveCommand.Create(DoPlayBookmark);
            DelBookmark = ReactiveCommand.Create(DeleteBookmark);
            Grab = ReactiveCommand.Create(DoGrab);
            AddText = ReactiveCommand.Create(AddTextToList);
            DelImage = ReactiveCommand.Create(DeleteImage);
            GetMissingImages = ReactiveCommand.Create(MissingImages);
            NewPoster = ReactiveCommand.Create(Do_AddPoster);
            NewCastMember = ReactiveCommand.Create(Do_AddCastMember);
            PlayFromLast = ReactiveCommand.Create(Do_PlayFromLast);
            RepeatLast = ReactiveCommand.Create(DoRepeatLast);
            ReloadBookmarks = ReactiveCommand.Create(DoReloadBookmarks);
            // setup phrases property in this view model
            Phrases = new ObservableCollection<PhraseEntry>(DataController.PhraseEntries);

            // setup SeriesList property in this view model from DataController
            SeriesList = new ObservableCollection<Series>(
                DataController.SandboxEntities.Series.ToList()
                );

            GenderList = new List<string>()
            {
                " Unknown",
                " Female",
                " Male"

            };
        }

        #endregion Public Constructors

        #region Public Properties

        public ReactiveCommand<Unit, Unit> AddPhrase { get; set; }

        public ReactiveCommand<Unit, Unit>? AddText { get; set; }

        /// <summary>
        /// Gets or sets the AutoCompleteList.
        /// </summary>
        public List<string>? AutoCompleteList
        {
            get
            {
                if (autoCompleteList == null)
                {
                    string? ac = DataController.SandboxEntities.MovieProperties.FirstOrDefault()?.AutoComplete;

                    if (!string.IsNullOrEmpty(ac))
                        autoCompleteList = ac.Split(',').ToList();
                }

                return autoCompleteList;
            }
            set => autoCompleteList = value;
        }

        public bool ByMovie { get; internal set; }

        public Actor? CurrentActor
        {
            get => currentActor;
            set
            {
                //currentActor?.Dispose();
                this.RaiseAndSetIfChanged(ref currentActor, value);
            }
        }

        public Bookmark CurrentBookmark
        {
            get => currentBookmark;
            set
            {
                this.RaiseAndSetIfChanged(ref currentBookmark, value);
                // this.RaisePropertyChanged(nameof(CurrentBookmarkImageBMP));
            }
        }

        public Avalonia.Media.Imaging.Bitmap? CurrentBookmarkImageBMP
        {
            get
            {
                if (CurrentBookmark != null && CurrentBookmark.ImageBMP != null)
                {
                    this.RaisePropertyChanged(nameof(CurrentBookmarkImageBMP));
                    return CurrentBookmark.ImageBMP;
                }
                return null;
            }
        }

        public MovieGenre? CurrentGenre { get => currentGenre; set => this.RaiseAndSetIfChanged(ref currentGenre, value); }

        public new Movies? CurrentMovie
        {
            get => currentMovie;
            set
            {
                currentMovie = value;
            }
        }

        public Models.Season CurrentSeason
        {
            get => currentSeason;
            set => this.RaiseAndSetIfChanged(ref currentSeason, value);
        }

        public Models.Series? CurrentSeries
        {
            get => currentSeries;
            set => this.RaiseAndSetIfChanged(ref currentSeries, value);
        }

        public ReactiveCommand<Unit, Unit>? DelBookmark { get; set; }

        /// <summary>
        /// Gets or sets the DelImage.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? DelImage { get; set; }

        public ObservableCollection<Director>? DirectorList
        {
            get => directorList;
            set => this.RaiseAndSetIfChanged(ref directorList, value);
        }

        public ReactiveCommand<Unit, Unit>? EditBookmark { get; set; }

        public int Found { get; internal set; }

        public List<string> GenderList
        {
            get => genderList;
            internal set => genderList = value;
        }

        public ReactiveCommand<Unit, Unit> GetMissingImages { get; set; }

        /// <summary>
        /// Gets or sets the Grab.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Grab { get; set; }

        public bool HasEpisode { get; internal set; }

        /// <summary>
        /// Gets the NewBookmark.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewBookmark { get; set; }

        public ReactiveCommand<Unit, Unit> NewCastMember { get; set; }

        public Models.TVEpisode NewEpisode
        {
            get => newEpisode;
            set => this.RaiseAndSetIfChanged(ref newEpisode, value);
        }

        public PhraseEntry? NewPhrase
        {
            get => newPhrase;
            set => this.RaiseAndSetIfChanged(ref newPhrase, value);
        }

        public ReactiveCommand<Unit, Unit>? NewPoster { get; }

        public Models.Season? NewSeason
        {
            get => newSeason;
            set => this.RaiseAndSetIfChanged(ref newSeason, value);
        }

        public PhraseEntry? NewSubPhrase
        {
            get => newSubPhrase;
            set => this.RaiseAndSetIfChanged(ref newSubPhrase, value);
        }

        /// <summary>
        /// Gets or sets the BookmarkUserControl.
        /// </summary>
        // public BookmarkUserControl? BookmarkUserControl { get => bookmarkUserControl; set => bookmarkUserControl = value; }
        /// <summary>
        /// Gets or sets the Phrases.
        /// </summary>
        public ObservableCollection<Models.PhraseEntry>? Phrases
        {
            get => phraseEntries;
            set => this.RaiseAndSetIfChanged(ref phraseEntries, value);
        }

        public ReactiveCommand<Unit, Unit>? PlayBookmark { get; set; }

        /// <summary>
        /// Gets the NewPoster.
        /// </summary>
        /// <summary>
        /// Gets the PlayFromLast.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? PlayFromLast { get; }

        /// <summary>
        /// Gets or sets the Progress.
        /// </summary>
        public string? Progress { get => this.progress; set => this.RaiseAndSetIfChanged(ref this.progress, value); }

        /// <summary>
        /// Gets the ProgressPercent.
        /// </summary>
        public int ProgressPercent { get => progressPercent; private set => this.RaiseAndSetIfChanged(ref progressPercent, value); }

        public ReactiveCommand<Unit, Unit>? ReloadBookmarks { get; }

        /// <summary>
        /// Gets the RepeatLast.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? RepeatLast { get; }

        public ObservableCollection<Series>? SeriesList
        {
            get => seriesList;
            set => this.RaiseAndSetIfChanged(ref seriesList, value);
        }

        /// <summary>
        /// Gets or sets the SubPhrases.
        /// </summary>
        public ObservableCollection<Models.PhraseEntry>? SubPhrases
        {
            get => subPhrases;
            set
            {
                this.RaiseAndSetIfChanged(ref subPhrases, value);
                this.NewSubPhrase = null;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void AddNewEpisodeCommand()
        {
            // will create the new tv episode for editing
            TVEpisode? newEpisode = new TVEpisode();
            if (CurrentMovie != null &&
                CurrentMovie.SeriesEntity != null && CurrentMovie.SeasonEntity != null)
            {
                newEpisode.ShowID = CurrentMovie.SeriesEntity.TMID;
                newEpisode.SeasonID = CurrentMovie.SeasonEntity.Id;
                if (CurrentMovie != null)
                {
                    newEpisode.MovieId = CurrentMovie.Id;
                    newEpisode.Overview = CurrentMovie.Info;
                }
            }
            this.NewEpisode = newEpisode;
        }

        public void AddSeasonCommand()
        {
            if (CurrentMovie != null && CurrentSeries != null)
            {
                NewSeason = new Models.Season();
                NewSeason.Series = CurrentSeries.Id;
                NewSeason.ShowId = CurrentSeries.TMID;
            }
        }

        public void DeleteBookmark()
        {
            if (CurrentBookmark != null && CurrentBookmark.Movies != null)
            {
                if (CurrentBookmark.Movies.DeleteBookmark(CurrentBookmark))
                {
                    CurrentBookmark = null;
                }
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DoPlayBookmark()
        {
            if (CurrentBookmark != null && CurrentBookmark.Movies != null)
            {
                string moviePath = CurrentBookmark.Movies.MoviePath;

                if (!string.IsNullOrEmpty(moviePath))
                {
                    FFMpegSupport.PlayMovie(moviePath, CurrentBookmark);
                }
            }
        }

        public void EditActor()
        {
            // will open the edit actor dialog for the current cast member
        }

        public Director GetDirector()
        {
            Director returnVal = null;

            //if (CurrentMovie != null && CurrentMovie.TMDBID != null)
            //{
            //    Support.CastList castMembers = Support.TmdbSupport.GetMovieCredits(CurrentMovie.TMDBID.Value);

            //    var director = castMembers.Where(cl => cl.IsDirector).FirstOrDefault();

            //    if (director != null)
            //    {
            //        Models.Director? mdirector = Models.DataController.SandboxEntities.Directors.Where(d => d.Name.ToLower() == director.Name.ToLower()).FirstOrDefault();
            //        if (mdirector != null)
            //        {
            //            CurrentMovie.Director = mdirector;
            //            CurrentMovie.DirectorID = mdirector.Id;
            //        }
            //        else
            //        {
            //            // create new director
            //            mdirector = new Models.Director();
            //            mdirector.Name = director.Name;
            //            Models.DataController.SandboxEntities.Directors.Add(mdirector);
            //            Models.DataController.SandboxEntities.SaveChanges();
            //            CurrentMovie.Director = mdirector;
            //            CurrentMovie.DirectorID = mdirector.Id;
            //            DirectorList.Add(mdirector);
            //        }
            //    }
            //    else { CurrentMovie.DirectorID = 14; }
            //}

            return returnVal;
        }

        //public Cast CurrentCastMember
        //{
        //    get => currentCastMember;
        //    set =>this.RaiseAndSetIfChanged(ref currentCastMember, value);
        //}
        public void GetSearchDirector()
        {
            GetDirector();
        }

        /// <summary>
        /// Gets the TMDB details.
        /// </summary>
        /// <autogeneratedoc />
        public void GetTMDBDetails()
        {
            if (CurrentMovie != null && (CurrentMovie.TMDBID != null && CurrentMovie.TMDBID > 0))
            {
                GetFromTMDB(CurrentMovie, CurrentMovie.TMDBID.Value, true);
            }
        }

        public async void MissingImages()
        {
            if (CurrentMovie != null)
            {
                if (CurrentMovie.Bookmarks != null && CurrentMovie.Bookmarks.Count > 0)
                {
                    foreach (Bookmark bookmark in CurrentMovie.Bookmarks)
                    {
                        if (bookmark.ImagePath != null && !File.Exists(bookmark.ImagePath))
                        {
                            // get the image using the bookmark time
                            // if the image does not exist, then grab image
                           // await Support.VideoSupport.GrabBookmarkImage(CurrentMovie, bookmark, 0);

                            CurrentBookmark = bookmark;
                            //bookmark.ImagePath = string.Empty;
                            bookmark.Save();
                        }
                    }
                }
            }
        }

        public void NullOutVariables()
        {
            //this.CurrentBookmark = null;
            this.CurrentMovie = null;
            this.CurrentActor?.ImageBMP?.Dispose();
            //this.CurrentActor = null;
            //this.CurrentGenre = null;
            //this.DirectorList = null;
            this.SeriesList = null;
            //this.NewPhrase = null;
            //this.NewSubPhrase = null;
            //this.Phrases = null;
            //this.SubPhrases = null;
        }

        public void RemoveGenre()
        {
            if (CurrentMovie != null)
            {
                //CurrentMovie.Genre = null;
                //CurrentMovie.GenreID = null;
            }
        }

        /// <summary>
        /// Saves the episode command.
        /// </summary>
        /// <autogeneratedoc />
        public void SaveEpisodeCommand()
        {
            // Will save the new episode and add it to the CurrentSeries Episode List
            if (CurrentMovie != null && CurrentMovie.EpisodeEntity != null)
            {
                CurrentMovie.EpisodeEntity.Save();
            }
        }

        public void SaveNewEpisodeCommand()
        {
            if (NewEpisode != null)
            {
                if (CurrentMovie != null
                    && CurrentMovie.SeasonEntity != null
                    && CurrentMovie.SeasonEntity.TVEpisodes != null)
                {
                    CurrentMovie.SeasonEntity.TVEpisodes.Add(NewEpisode);
                    // rebuild list to trigger UI update
                    CurrentMovie.SeasonEntity.TVEpisodes = new ObservableCollection<TVEpisode>(CurrentMovie.SeasonEntity.TVEpisodes);
                    NewEpisode.MovieId = CurrentMovie.Id;
                    NewEpisode.SeasonID = CurrentMovie.SeasonEntity.Id;
                    HasEpisode = true;
                }
                NewEpisode.Insert();
                CurrentMovie.Episode = NewEpisode.Id;
                CurrentMovie.Save();
            }
        }

        public void SaveNewSeasonCommand()
        {
            if (NewSeason != null)
            {
                NewSeason.Save();

                if (CurrentSeries != null && CurrentSeries.Seasons != null)
                {
                    CurrentSeries.Seasons.Add(NewSeason);
                }
            }
        }

        public void SaveSeasonCommand()
        {
            if (CurrentMovie != null && CurrentMovie.SeasonEntity != null)
            {
                CurrentMovie.SeasonEntity.Save();
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// The DoReloadBookmarks.
        /// </summary>
        internal void DoReloadBookmarks()
        {
            Window mainWindow = Support.Support.GetMainWindow() as Window;

            // CurrentMovie = Support.GetCurrentMovie();

            if (CurrentMovie != null && mainWindow != null)
            {
                CurrentMovie.Save();
                DataController.ReloadMovie(CurrentMovie);
                CurrentMovie.Bookmarks = new ObservableCollection<Bookmark>();
                CurrentMovie.Bookmarks = new ObservableCollection<Bookmark>(DataController.SandboxEntities.Bookmarks
                    .Where(m => m.MovieID == CurrentMovie.Id)
                    .ToList());
                CurrentMovie.ImagesCount = CurrentMovie.Bookmarks.Count;
                CurrentMovie.SetPercentUnmarked();

                // mainWindow.SetBookmarks(CurrentMovie);
                //Support.Support.SetCurrentMovie(CurrentMovie);
            }
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    NullOutVariables();
                    //  CurrentActor?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddTextToList()
        {
            // throw new NotImplementedException();
        }

        private void DeleteImage()
        {
            if (CurrentBookmark != null && CurrentMovie != null && !string.IsNullOrEmpty(CurrentBookmark.ImagePath))
            {
                string imagepath = Support.Support.FixImagePath(CurrentBookmark.ImagePath);

                if (File.Exists(imagepath))
                {
                    File.Delete(imagepath);
                }

                CurrentBookmark.ImagePath = string.Empty;

                CurrentBookmark.SetImageBMP();
                CurrentBookmark.ImageBMP = null;
            }
        }
        private async void Do_AddBookmark()
        {
            //MainWindow? main = GetWindow() as MainWindow;
            //if (main != null)
            //{
            //    MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

            if (CurrentMovie != null && CurrentMovie.Bookmarks != null)
            {
                Bookmark oldBookmark = CurrentMovie.GetLastBookmark();

                double time = 1;

                int mId = CurrentMovie.Id;
                Bookmark bookmark = new Bookmark()
                {
                    Name = "<new>",
                    Time = time,
                    MovieID = mId,
                    Type = "BOOKMARK"
                };

                //  await AddActualBookmark(main, vm, bookmark);
            }
            //  }
        }

        public void Do_AddCastMember()
        {
            if (CurrentMovie != null)
            {
                Cast tempCastMember = new Cast();
                tempCastMember.MovieID = CurrentMovie.Id;
            }
        }

        /// <summary>
        /// The Do_AddPoster.
        /// </summary>
        private async void Do_AddPoster()
        {
            //MainWindow? main = GetWindow() as MainWindow;
            //if (main != null)
            //{
            //    MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

            if (CurrentMovie != null)
            {
                int mId = CurrentMovie.Id;
                Bookmark bookmark = new Bookmark()
                {
                    Name = "Poster",
                    Time = 10,
                    MovieID = mId,
                    Type = "BOOKMARK"
                };

                //  await AddActualBookmark(main, vm, bookmark);
            }
            // }
        }

        /// <summary>
        /// The Do_PlayFromLast.
        /// </summary>
        private void Do_PlayFromLast()
        {
            if (currentMovie != null && CurrentMovie.Bookmarks.Count > 0)
            {
                CurrentBookmark = currentMovie.Bookmarks.Last();
                string moviePath = currentMovie.MoviePath;

                if (!string.IsNullOrEmpty(moviePath))
                {
                    FFMpegSupport.PlayMovie(moviePath, CurrentBookmark);
                }
            }
            // }
        }

        private void DoAddPhrase()
        {
        }

        /// <summary>
        /// Adds the tv episode command.
        /// </summary>
        /// <autogeneratedoc />
        private void DoEditBookmark()
        {
            _ = DoEditBookmarkActual(CurrentBookmark);
        }

        private async Task DoEditBookmarkActual(Bookmark bookmark)
        {
            // set movie property as well
            //this.Movie = CurrentMovie;

            Dialogs.EditBookmarkDialog editBookmarkDialog = new Dialogs.EditBookmarkDialog();

            BookmarkViewModel bookmarkViewModel = new BookmarkViewModel();
            bookmarkViewModel.CurrentBookmark = bookmark;

            editBookmarkDialog.DataContext = bookmarkViewModel;

            //editBookmarkDialog.DataContext = this;
            //this.Caller = editBookmarkDialog;

            Window? main = Support.Support.GetMainWindow() as Window;

            DialogResultButton resultButton = await editBookmarkDialog.ShowDialog<DialogResultButton>(main as Window);

            if (
                resultButton != null
                && resultButton.Result == DialogResultButton.ResultType.Ok
            )
            {
                // save movie
                if (this.CurrentBookmark != null)
                {
                    this.CurrentBookmark.Save();

                    //vm.CurrentBookmark = viewModel.CurrentBookmark;
                    this.CurrentBookmark.Redisplay();

                    //if (
                    //    CurrentMovie != null
                    //    && CurrentMovie.Bookmarks != null

                    //    )
                    // CurrentMovie.Bookmarks = this.Movie.Bookmarks;
                }
            }
            editBookmarkDialog.DataContext = null;
            bookmarkViewModel.CurrentBookmark = null;
            bookmarkViewModel = null;
            editBookmarkDialog = null;
        }

        private void DoGrab()
        {
            //if (CurrentMovie != null && CurrentBookmark != null)
            //{
            //    VideoSupport.GrabBookmarkImage(CurrentMovie, CurrentBookmark, 0);
            //    System.Threading.Thread.Sleep(1000);
            //    var bmp = CurrentBookmark.ImageBMP;
            //    CurrentBookmark.SetImageBMP();
            //}
        }
        /// <summary>
        /// The DoRepeatLast.
        /// </summary>
        private async void DoRepeatLast()
        {
            //MainWindow? main = GetWindow() as MainWindow;

            //if (main != null)
            {
                //  MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (CurrentMovie != null && CurrentMovie.Bookmarks.Count > 0)
                {
                    CurrentBookmark = CurrentMovie.Bookmarks.Last();
                    string moviePath = CurrentMovie.MoviePath;

                    Bookmark bookmark = new Bookmark();
                    if (!string.IsNullOrEmpty(CurrentBookmark.Name))
                    {
                        string bookmarkname = CurrentBookmark.Name;

                        int i = bookmarkname.Length - 1;

                        while (char.IsDigit(bookmarkname[i]))
                        {
                            i--;
                        }

                        // check length changed

                        if (i < bookmarkname.Length)
                        {
                            string digits = bookmarkname.Substring(i + 1);

                            bookmarkname = bookmarkname.Substring(0, i + 1);

                            if (int.TryParse(digits, out int num))
                            {
                                num += 1;
                                bookmarkname += num.ToString().Trim();
                            }
                        }

                        int mId = CurrentMovie.Id;
                        bookmark.Name = bookmarkname;
                        bookmark.MovieID = mId;
                        bookmark.Time = CurrentBookmark.Time + 10;
                        bookmark.Type = "BOOKMARK";
                        //await AddActualBookmark(main, vm, bookmark);
                    }
                }
            }
        }

        /// <summary>
        /// Gets from TMDB.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <autogeneratedoc />
        private async void GetFromTMDB(Movies movie, int ID, bool getCast = false)
        {
            //Support.Support.iMovie iMovie = await Support.TmdbSupport.GetMovieData(ID);

            //if (iMovie != null)
            //{
            //    if (iMovie.ProductionCompanies != null && iMovie.ProductionCompanies.Count > 0)
            //    {
            //        //foreach (var item in iMovie.ProductionCompanies)
            //{
            //    Models.ProductionCompany? pc = DataController.ProductionCompanies.Where(p => p.TMDBID == item.Id).FirstOrDefault();

            //    if (pc == null)
            //    {
            //        pc = new Models.ProductionCompany()
            //        {
            //            CompanyName = item.Name,
            //            TMDBID = item.Id
            //        };

            //        DataController.SandboxEntities.ProductionCompany.Add(pc);
            //        DataController.SandboxEntities.SaveChanges();

            //        DataController.ProductionCompanies.Add(pc);
            //        movie.ProductionCompanies.Add(pc);
            //    }

            //    Models.ProductionCompanyMovie? productionCompanyMovie = DataController.SandboxEntities.ProductionCompanyMovie.Where(p => p.MovieId == movie.Id && p.CompanyId == pc.Id).FirstOrDefault();

            //if (productionCompanyMovie == null)
            //{
            //    productionCompanyMovie = new Models.ProductionCompanyMovie()
            //    {
            //        MovieId = movie.Id,
            //        CompanyId = pc.Id
            //    };

            //    DataController.SandboxEntities.ProductionCompanyMovie.Add(productionCompanyMovie);
            //    DataController.SandboxEntities.SaveChanges();
            //}
            //    }
            //    DataController.ProductionCompanies = DataController.SandboxEntities.ProductionCompany.ToList();
            //}

            // deal with languages
            //if (iMovie.Languages != null && iMovie.Languages.Count > 0)
            //{
            //    //foreach (var language in iMovie.Languages)
            //{
            //    MovieLanguage? movieLanguage = movie.MovieLanguages.Where(l => l.Iso_639_1 == language.Iso_639_1).FirstOrDefault();
            //    //if (movieLanguage == null)
            //    //{
            //    //    movieLanguage = new MovieLanguage()
            //    //    {
            //    //        MovieId = movie.Id,
            //    //        Iso_639_1 = language.Iso_639_1,
            //    //        LanguageName = language.Name
            //    //    };

            //    //    DataController.SandboxEntities.MovieLanguage.Add(movieLanguage);
            //    //    DataController.SandboxEntities.SaveChanges();

            //    //    movie.MovieLanguages.Add(movieLanguage);
            //    //}
            //}
            //        }

            //if (getCast)
            //    Support.Support.GetCastData(movie, iMovie);
            //      }
        }

        #endregion Private Methods

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieEditViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}