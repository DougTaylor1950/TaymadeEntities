using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Svg.Skia;
using TaymadeEntities.Controls;
using TaymadeEntities.Dialogs;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.Views;
using BitMiracle.LibTiff.Classic;
using CliWrap;
using CliWrap.EventStream;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using OpenXmlPowerTools;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Windows.Input;
using TaymadeControls;
using Bookmark = TaymadeEntities.Models.Bookmark;
using FFMpegSupport = TaymadeEntities.Support.FFMpegSupport;
using PhraseEntry = TaymadeEntities.Models.PhraseEntry;
using Task = System.Threading.Tasks.Task;
using Window = Avalonia.Controls.Window;

namespace TaymadeEntities.ViewModels
{

    public class MovieViewModelBase : DialogModelBase
    {


        #region Public Fields

        /// <summary>
        /// Defines the MovieContext.
        /// </summary>
        public DBContext.SandboxEntities MovieContext = new DBContext.SandboxEntities();

        #endregion Public Fields

        #region Internal Fields

        /// <summary>
        /// Defines the currentDirector.
        /// </summary>
        internal Director? currentDirector;

        internal Movies? currentMovie;

        /// <summary>
        /// Defines the foundMissingMovies.
        /// </summary>
        internal MissingFileCollection? foundMissingMovies;

        internal FFMpegSupport? mpegSupport;

        internal MissingFile? tempMissingFile;

        #endregion Internal Fields

        #region Private Fields

        /// <summary>
        /// Defines the actorList.
        /// </summary>
        private List<Actor>? actorList;

        /// <summary>
        /// Defines the autoComplete.
        /// </summary>
        private string? autoComplete;

        /// <summary>
        /// Gets the EditSeasonCommand.
        /// </summary>
        //public ICommand EditSeasonCommand { get; }
        private string? autoCompleteToken;

        private BackgroundMovieGetter? backgroundMovieGetter = new BackgroundMovieGetter();

        /// <summary>
        /// Defines the bookmark.
        /// </summary>
        private Models.Bookmark? bookmark;

        /// <summary>
        /// Defines the byMovie.
        /// </summary>
        private bool byMovie = true;

        /// <summary>
        /// Defines the currentCastMember.
        /// </summary>
        private Cast? currentCastMember;

        /// <summary>
        /// Defines the currentEpisode.
        /// </summary>
        private EpisodeDetails? currentEpisode;

        /// <summary>
        /// Defines the currentMissingMovie.
        /// </summary>
        private MissingFile? currentMissingMovie;

        /// <summary>
        /// Defines the currentPhrase.
        /// </summary>
        private PhraseEntry? currentPhrase;

        private Models.Season? currentSeason;

        private Models.Series? currentSeries;

        /// <summary>
        /// Defines the currentMovie.
        /// </summary>
       //private Movies? currentMovie;
        /// <summary>
        /// Defines the currentSubPhrase.
        /// </summary>
        private PhraseEntry? currentSubPhrase;

        /// <summary>
        /// Defines the detailVisible.
        /// </summary>
        private bool detailVisible = true;

        /// <summary>
        /// Defines the directors.
        /// </summary>
        private ObservableCollection<Director>? directors;

        /// <summary>
        /// Defines the editVisible.
        /// </summary>
        private bool editVisible = true;

        /// <summary>
        /// Defines the episode.
        /// </summary>
        private Models.TVEpisode? episode;

        /// <summary>
        /// Defines the episodeList.
        /// </summary>
        private ObservableCollection<TVEpisode>? episodeList;

        /// <summary>
        /// Defines the filterList.
        /// </summary>
        private ObservableCollection<Models.Filter>? filterList;

        /// <summary>
        /// Defines the findBookmarkText.
        /// </summary>
        private string? findBookmarkText;

        /// <summary>
        /// Defines the hasEpisode.
        /// </summary>
        private bool hasEpisode;

        /// <summary>
        /// Defines the hasTemp.
        /// </summary>
        private bool hasTemp;

        private bool initialise;
        private bool isConverted;

        private string? missingInfo;

        /// <summary>
        /// Defines the movieList.
        /// </summary>
        private ObservableCollection<Movies>? movieList;

        private int movieProgress = 0;

        private bool mP4Converted;
        private bool mTSConverted;
        private PhraseEntry? newPhrase;

        /// <summary>
        /// Defines the newSeason.
        /// </summary>
        private Models.Season? newSeason;

        private Window? oldCaller = null;

        /// <summary>
        /// Defines the movieDetailControl.
        /// </summary>
        //private Views.MovieDetailControl? movieDetailControl;
        /// <summary>
        /// Defines the oldmovieList.
        /// </summary>
        private ObservableCollection<Movies>? oldmovieList;

        private PhraseEntry? oldPhrase = null;

        //private Movies oldCurrentMovie = null;
        private PhraseEntry? oldSubPhrase = null;

        /// <summary>
        /// Defines the phraseEntries.
        /// </summary>
        private ObservableCollection<PhraseEntry>? phraseEntries;

        /// <summary>
        /// Defines the preSorted.
        /// </summary>
        private bool preSorted = false;

        /// <summary>
        /// Defines the processOutput.
        /// </summary>
        private string? processOutput;

        /// <summary>
        /// Defines the progress.
        /// </summary>
        private string? progress;

        /// <summary>
        /// Defines the progressPercent.
        /// </summary>
        private int progressPercent = 0;

        /// <summary>
        /// Defines the resultTask.
        /// </summary>
        private Models.DialogResultButton? resultTask;

        /// <summary>
        /// The screen width
        /// </summary>
        /// <autogeneratedoc />
        private int screenWidth = 1200;

        /// <summary>
        /// The screen width list
        /// </summary>
        /// <autogeneratedoc />
        private List<int>? screenWidthList;

        /// <summary>
        /// Defines the seasonEntity.
        /// </summary>
        private Models.Season? seasonEntity;

        private ObservableCollection<Movies>? seasonMovies;

        private ObservableCollection<Models.Season>? seasonsList;

        /// <summary>
        /// Defines the seriesEntity.
        /// </summary>
        private Models.Series? seriesEntity;

        /// <summary>
        /// Defines the showVisible.
        /// </summary>
        private bool showVisible = true;

        //public DataGrid? dgImageList { get; set; }
        /// <summary>
        /// Defines the sortOrders.
        /// </summary>
        private ObservableCollection<string>? sortOrders;

        private Movies? subGridSelected;

        /// <summary>
        /// Defines the subPhrases.
        /// </summary>
        private ObservableCollection<PhraseEntry>? subPhrases;

        private string? textToken;
        private int volume = 0;
        private MovieGenre currentGenre;
        private string? findText;
        private bool? hasMovie;
        private MovieGenre currentMovieGenre;
        private int screenHeight;

        #endregion Private Fields

        #region Public Constructors

        public MovieViewModelBase()
        {
            if (Phrases == null || Phrases.Count == 0)
                Phrases = new ObservableCollection<PhraseEntry>(Models.DataController.PhraseEntries);

            SetupSortOrders();
            ScreenWidthList = new List<int>() { 400, 800, 1200, 1400, 1600, 1800 };

            ScreenWidth = 1200;

            ActorList = DataController.ActorList;
            DirectorList = new ObservableCollection<Director>(DataController.DirectorList);

            FilterList = new ObservableCollection<Models.Filter>(DataController.SandboxEntities.Filter.ToList());

            SeriesList = DataController.SeriesList;

            RemoveGenre = ReactiveCommand.Create(DoRemoveGenre);

            EditActor = ReactiveCommand.Create(DoEditActor);
            searchForMovie = ReactiveCommand.Create(SearchForMovie);
            CopyTheText = ReactiveCommand.Create<string>(CopyText);
            CutTheText = ReactiveCommand.Create<string>(CutText);
            PasteTheText = ReactiveCommand.Create<string>(PasteText);
            joinList = ReactiveCommand.Create(JoinList);
            NewCastMember = ReactiveCommand.Create(Do_AddCastMember);
            getMissingGenres = ReactiveCommand.Create(GetMissingGenres);
            ReloadBookmarks = ReactiveCommand.Create(DoReloadBookmarks);
            EditBookmark = ReactiveCommand.Create(DoEditBookmark);
            DelBookmark = ReactiveCommand.Create(DoDeleteBookmark);
            PlayBookmark = ReactiveCommand.Create(DoPlayBookmark);

            GetMissingImages = ReactiveCommand.Create(MissingImages);
            NewBookmark = ReactiveCommand.Create(Do_AddBookmark);
            NewPoster = ReactiveCommand.Create(Do_AddPoster);
            PlayFromLast = ReactiveCommand.Create(Do_PlayFromLast);
            RepeatLast = ReactiveCommand.Create(DoRepeatLast);

            _ReloadGroups = ReactiveCommand.Create(ReloadGroups);
            _MoveToSelectedRow = ReactiveCommand.Create(DoMoveToSelectedRow);
            DoEndFindMovie = ReactiveCommand.Create(EndFindMovie);

            backgroundMovieGetter.DataGetCompleted += this.BackgroundMovieGetter_DataGetCompleted;
            backgroundMovieGetter.DataGetProgress += this.BackgroundMovieGetter_DataGetProgress;
            // commands

            ShowDialog = new Interaction<MovieViewModelBase, SeriesViewModel?>();

            CreateSeasonCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MovieViewModel()
                {
                    CurrentSeries = CurrentSeries,
                    NewSeason = new Models.Season(CurrentSeries),
                    HasEpisode = false
                };

                var result = await ShowDialog.Handle(store);

                if (result != null)
                {
                    if (result.CurrentSeason != null)
                    {
                        result.CurrentSeason.Insert();
                        if (CurrentSeries.Seasons == null) CurrentSeries.Seasons = [];
                        CurrentSeries.Seasons.Add(result.CurrentSeason);
                    }
                }
            });

            AddEpisodeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MovieViewModel()
                {
                    CurrentSeries = CurrentSeries,
                    NewSeason = CurrentSeason,
                    Episode = new TVEpisode(CurrentSeason),
                    HasEpisode = true
                };

                var result = await ShowDialog.Handle(store);

                if (result != null)
                {
                    if (result.CurrentSeason != null && result.EpisodeEntity != null)
                    {
                        result.EpisodeEntity.Insert();
                        CurrentSeason.TVEpisodes.Add(result.EpisodeEntity);
                        EpisodeEntity = result.EpisodeEntity;

                        CurrentSeason.TVEpisodes = new ObservableCollection<TVEpisode>([.. CurrentSeason.TVEpisodes.DistinctBy(x => x.Id).OrderBy(x => x.EpisodeNumber)]);
                    }
                }
            });

            DeleteEpisodeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MovieViewModel()
                {
                    CurrentSeries = CurrentSeries,
                    NewSeason = CurrentSeason,
                    Episode = EpisodeEntity,
                    HasEpisode = true
                };

                var result = await ShowDialog.Handle(store);

                if (result != null)
                {
                    if (result.CurrentSeason != null && result.EpisodeEntity != null)
                    {
                        result.CurrentSeason.TVEpisodes.Remove(result.EpisodeEntity);
                        result.EpisodeEntity.Delete();
                    }
                }
            });

            this.MpegSupport = new FFMpegSupport();
            this.MpegSupport.CliWrapCompleted += this.MpegSupport_CliWrapCompleted;
            this.MpegSupport.CliWrapProgress += this.MpegSupport_CliWrapProgress;
            this.MpegSupport.CliWrapError += this.MpegSupport_CliWrapErrored;
        }

        private void DoRemoveGenre()
        {
            if (CurrentGenre != null)
            {
                if (CurrentMovie != null)
                {
                    CurrentGenre.Delete();
                    CurrentMovie.MovieGenres.Remove(CurrentGenre);
                    CurrentMovie.Save();
                    CurrentMovie.MovieGenres = new ObservableCollection<MovieGenre>(CurrentMovie.MovieGenres);
                }
            }
        }

        private void DoMoveToSelectedRow()
        {
            MoveToSelectedRow(null);
        }

        private void MoveToSelectedRow(object value)
        {
            //throw new NotImplementedException();
        }

        #endregion Public Constructors

        // Fix for CS1503: Argument 1: cannot convert from 'int' to 'TaymadeEntities.Models.MovieIntResult'
        // Update the LINQ queries in SearchForMovie to compare Movie IDs correctly

        #region Public Properties

        public Window? mainWindow { get; set; }

        public bool MainWindowInitialised { get; set; } = false;
        public static SvgImage SpectrumImage
        {
            get
            { // SvgImage /Assets/svg/spectrum-gradient.svg}
                SvgImage background = new SvgImage()
                {
                    Source = SvgSource.Load("avares://TaymadeControls/Assets/svg/spectrum-gradient.svg", baseUri: null)
                };
                return background;
            }
        }

        /// <summary>
        /// Defines the phraseEntries.
        /// </summary>
        // private List<PhraseEntry> phraseEntries;
        public ReactiveCommand<Unit, Unit>? _ReloadGroups { get; }

        public ReactiveCommand<Unit, Unit>? _MoveToSelectedRow { get; }

        /// <summary>
        /// Gets or sets the ActorList.
        /// </summary>
        public List<Actor>? ActorList { get => actorList; set => this.RaiseAndSetIfChanged(ref actorList, value); }

        /// <summary>
        /// Gets or sets the AddEpisode.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? AddEpisode { get; set; }

        /// <summary>
        /// Gets the Accept.
        /// </summary>
        //public ReactiveCommand<Unit, Unit>? Accept { get; private set; }
        /// <summary>
        /// Gets the AddEpisodeCommand.
        /// </summary>
        public ICommand? AddEpisodeCommand { get; }

        /// <summary>
        /// Gets or sets the AutoComplete.
        /// </summary>
        public string? AutoComplete
        {
            get
            {
                if (string.IsNullOrEmpty(autoComplete))
                    autoComplete = DataController.MovieProperties.AutoComplete;
                return autoComplete;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref autoComplete, value);
                DataController.MovieProperties.AutoComplete = value;
                DataController.MovieProperties.Save();
            }
        }

        public string? AutoCompleteToken { get => autoCompleteToken; set => this.RaiseAndSetIfChanged(ref autoCompleteToken, value); }

        /// <summary>
        /// Gets or sets the AutoCompleteTokens.
        /// </summary>
        public List<string>? AutoCompleteTokens
        {
            get
            {
                List<string> tokens = AutoComplete.Split(',').ToList();
                tokens.Sort();
                return tokens;
            }
            set => AutoComplete = string.Join(',', value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether ByMovie.
        /// </summary>
        public bool ByMovie { get => byMovie; set => this.RaiseAndSetIfChanged(ref byMovie, value); }

        public ReactiveCommand<string, Unit>? CopyTheText { get; }

        /// <summary>
        /// Gets or sets the CreateSeason.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? CreateSeason { get; set; }

        /// <summary>
        /// Gets the CreateSeasonCommand.
        /// </summary>
        public ICommand? CreateSeasonCommand { get; }

        /// <summary>
        /// Gets or sets the CurrentBookmark.
        /// </summary>
        public Models.Bookmark? CurrentBookmark { get => bookmark; set => this.RaiseAndSetIfChanged(ref bookmark, value); }

        /// <summary>
        /// Gets or sets the CurrentCastMember.
        /// </summary>
        public Cast? CurrentCastMember
        {
            get => currentCastMember;
            set => this.RaiseAndSetIfChanged(ref currentCastMember, value);
        }

        /// <summary>
        /// Gets or sets the CurrentDirector.
        /// </summary>
        public Director? CurrentDirector { get => currentDirector; set => this.RaiseAndSetIfChanged(ref currentDirector, value); }

        /// <summary>
        /// Gets or sets the CurrentEpisode.
        /// </summary>
        public TaymadeEntities.Support.EpisodeDetails? CurrentEpisode { get => currentEpisode; set => this.RaiseAndSetIfChanged(ref currentEpisode, value); }

        /// <summary>
        /// Gets or sets the CurrentMissingMovie.
        /// </summary>
        public MissingFile? CurrentMissingMovie
        {
            get => currentMissingMovie;
            set => this.RaiseAndSetIfChanged(ref currentMissingMovie, value);
        }

        public MovieGenre CurrentGenre { get => currentGenre; set => this.RaiseAndSetIfChanged(ref currentGenre, value); }

        /// <summary>
        /// Gets or sets the CurrentMovie.
        /// </summary>
        public Movies? CurrentMovie
        {
            get => currentMovie;
            set
            {
                //if (!Initialise)
                //{
                this.RaiseAndSetIfChanged(ref currentMovie, value);
                if (value != null)
                {
                    EditVisible = true;
                    ShowVisible = true;
                    DetailVisible = true;
                    DataController.MovieProperties.LastMoveID = value.Id;
                    if (value != null && CurrentMovie != null)
                    {
                        //oldCurrentMovie = value;
                        if (CurrentMovie.Director == null && CurrentMovie.DirectorID == null) CurrentMovie.DirectorID = 14;
                        if (CurrentMovie.DirectorID != null)
                            CurrentMovie.Director = DirectorList.Where(d => d.Id == CurrentMovie.DirectorID).FirstOrDefault();

                        if (CurrentMovie.Director != null) CurrentDirector = CurrentMovie.Director;

                        // check for tempfile
                        HasTemp = File.Exists(value.GetTempFileName());
                        if (currentMovie.Series != null && currentMovie.Series != 2)
                        {
                            // has a series
                            if (currentMovie.SeriesEntity != null)
                            {
                                if (value.Bookmarks.Count == 0)
                                {
                                    value.Bookmarks = new ObservableCollection<Models.Bookmark>(
                                        DataController.BookmarkController.GetBookmarksByMovieId(CurrentMovie.Id)
                                        );
                                }

                                if (currentMovie.SeriesEntity.Seasons != null)
                                {
                                    if (currentMovie.SeriesEntity.Seasons.Count == 0) currentMovie.SeriesEntity.Seasons = new ObservableCollection<Models.Season>(DataController.SandboxEntities.Seasons.Where(s => s.Series == currentMovie.Series).OrderBy(s => s.SeasonNo).ToList());
                                    if (currentMovie.Episode != null && currentMovie.Episode > 0)
                                    {
                                        //int tempCastMember = currentMovie.TVEpisodes.Count;
                                    }

                                    if (currentMovie.SeasonEntity == null && currentMovie.Season != null && currentMovie.Season > 0)
                                    {
                                        CurrentMovie.SeasonEntity = currentMovie.SeriesEntity.Seasons.Where(s => s.Id == currentMovie.Season).FirstOrDefault();
                                        //CurrentSeason

                                        //CurrentMovie.SeasonEntity;
                                    }
                                    if (CurrentMovie.SeasonEntity != null)  // we have a season associated with movie
                                    {
                                        if (CurrentMovie.EpisodeEntity == null && CurrentMovie.Episode != null && CurrentMovie.Episode > 0)
                                        {
                                            if (CurrentMovie.SeasonEntity.TVEpisodes != null)
                                            {
                                                if (CurrentMovie.SeasonEntity.TVEpisodes.Count == 0)
                                                {
                                                    CurrentMovie.SeasonEntity.TVEpisodes = new ObservableCollection<TVEpisode>(DataController.SandboxEntities.TVEpisodes.Where(e => e.SeasonID == CurrentMovie.SeasonEntity.Id).ToList());
                                                }
                                                CurrentMovie.EpisodeEntity = CurrentMovie.SeasonEntity.TVEpisodes.Where(e => e.Id == CurrentMovie.Episode).FirstOrDefault();
                                            }
                                            //CurrentEpisode
                                        }
                                    }
                                }
                            }
                        }



                        this.RaisePropertyChanged(nameof(IsMP4));

                        if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
                        //MainWindow? main = Support.Support.GetMainWindow();

                        if (mainWindow != null)
                        {
                            // mainWindow.RowChanged(null, null);
                        }
                    }
                   // value.FixMovieData();
                }
                else
                {
                    EditVisible = false;
                    ShowVisible = false;
                    DetailVisible = false;
                    IsMP4 = false;
                }
            }


            //}
        }

        public MovieGenre CurrentMovieGenre
        {
            get => currentMovieGenre;
            set => this.RaiseAndSetIfChanged(ref currentMovieGenre, value);
        }

        /// <summary>
        /// Gets or sets the CurrentPhrase.
        /// </summary>
        public Models.PhraseEntry? CurrentPhrase
        {
            get => currentPhrase;
            set
            {
                this.RaiseAndSetIfChanged(ref currentPhrase, value);

                if (value != null)
                {
                    if (value != oldPhrase || oldPhrase == null)
                    {
                        CurrentSubPhrase = null;
                        oldPhrase = value;
                        SubPhrases = new ObservableCollection<PhraseEntry>(DataController.GetSubPhraseEntries(value));
                        GroupChanged();
                    }
                }
            }
        }

        // PSEUDOCODE:
        // - When CurrentSeason is set:
        //   - update backing field via RaiseAndSetIfChanged
        //   - if value not null:
        //       - query Movies matching season id and (if CurrentSeries present) matching series id
        //       - construct an ObservableCollection<Movies> from the result
        //         (do NOT call .ToList() on the ObservableCollection itself)
        //   - if value is null:
        //       - set SeasonMovies to an empty ObservableCollection to avoid null references
        //
        // FIX SUMMARY:
        // The original code called .ToList() after constructing the ObservableCollection,
        // which returned a List<Movies> and caused assignment of List to ObservableCollection (CS0029).
        // Fix by building a list first (or passing IEnumerable) and then creating the ObservableCollection.

        public Models.Season CurrentSeason
        {
            get => currentSeason;
            set
            {
                this.RaiseAndSetIfChanged(ref currentSeason, value);

                if (value != null)
                {
                    // If CurrentSeries is available, filter by both season and series.
                    if (CurrentSeries != null)
                    {
                        var moviesForSeason = DataController.SandboxEntities.Movies
                            .Where(m => m.Season == value.Id && m.Series == CurrentSeries.Id)
                            .ToList();

                        SeasonMovies = new ObservableCollection<Movies>(moviesForSeason);
                    }
                    else
                    {
                        var moviesForSeason = DataController.SandboxEntities.Movies
                            .Where(m => m.Season == value.Id);

                        SeasonMovies = new ObservableCollection<Movies>(moviesForSeason);
                    }
                }
                else
                {
                    SeasonMovies = new ObservableCollection<Movies>();
                }
            }
        }

        public Movies? CurrentSeasonMovie { get; set; }

        public Models.Series CurrentSeries
        {
            get => currentSeries;
            set
            {
                this.RaiseAndSetIfChanged(ref this.currentSeries, value);

                if (value != null)
                {
                    if (value.Id == 22) //All
                    {
                        MovieList = new ObservableCollection<Movies>
                            (
                            DataController.MovieController.GetMoviesByGenre("SER-1")
                            );
                    }
                    else if (value.Id > 0 && value.Id != 2)
                    {
                        SeasonsList = value.Seasons;
                        SeriesVisible = true;
                        // should now get movielist for selected series
                        // if 0 it is all movies in the genres season

                        List<Models.Movies> tempList = DataController.SandboxEntities.Movies
                            .Where(s => s.Series == value.Id).OrderBy(s => s.Season).ToList();
                        SeriesMovieList = new ObservableCollection<Movies>(tempList);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the CurrentSubPhrase.
        /// </summary>
        public PhraseEntry? CurrentSubPhrase
        {
            get => currentSubPhrase;
            set
            {
                this.RaiseAndSetIfChanged(ref currentSubPhrase, value);

                if (value != null)
                {
                    if (value != oldSubPhrase || oldSubPhrase == null)
                    {
                        oldSubPhrase = value;
                        GroupChanged();
                    }
                }
            }
        }

        public ReactiveCommand<string, Unit> CutTheText { get; }

        /// <summary>
        /// Gets the DelBookmark.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? DelBookmark { get; set; }

        /// <summary>
        /// Gets the DeleteEpisodeCommand.
        /// </summary>
        public ICommand DeleteEpisodeCommand { get; }

        /// <summary>
        /// Gets or sets the DeleteMember.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? DeleteMember { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DetailVisible.
        /// </summary>
        public bool DetailVisible { get => detailVisible; set => this.RaiseAndSetIfChanged(ref detailVisible, value); }

        /// <summary>
        /// Gets or sets the Directors.
        /// </summary>
        public ObservableCollection<Models.Director> DirectorList
        {
            get => directors;
            set => this.RaiseAndSetIfChanged(ref directors, value);
        }

        public ReactiveCommand<Unit, Unit> DoEndFindMovie { get; }

        public ReactiveCommand<Unit, Unit>? DoSearch { get; }

        /// <summary>
        /// Gets the EditActor.
        /// </summary>
        public ReactiveCommand<Unit, Unit> EditActor { get; }

        /// <summary>
        /// Gets the EditBookmark.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? EditBookmark { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EditVisible.
        /// </summary>
        public bool EditVisible { get => editVisible; set => this.RaiseAndSetIfChanged(ref editVisible, value); }

        /// <summary>
        /// Gets or sets the Episode.
        /// </summary>
        public Models.TVEpisode? Episode { get => episode; set => this.RaiseAndSetIfChanged(ref episode, value); }

        /// <summary>
        /// Gets or sets the EpisodeEntity.
        /// </summary>
        public TVEpisode? EpisodeEntity { get; internal set; }

        /// <summary>
        /// Gets or sets the EpisodeList.
        /// </summary>
        public ObservableCollection<Models.TVEpisode> EpisodeList { get => this.episodeList; set => this.RaiseAndSetIfChanged(ref this.episodeList, value); }

        /// <summary>
        /// Gets or sets the FilterList.
        /// </summary>
        public ObservableCollection<Models.Filter>? FilterList { get => filterList; set => this.RaiseAndSetIfChanged(ref filterList, value); }

        /// <summary>
        /// Gets or sets the FindBookmarkText.
        /// </summary>
        public string FindBookmarkText { get => findBookmarkText; set => this.RaiseAndSetIfChanged(ref findBookmarkText, value); }

        public bool FirstMovieSelected { get; set; } = false;

        /// <summary>
        /// Gets or sets the FoundMissingMovies.
        /// </summary>
        public MissingFileCollection FoundMissingMovies { get => foundMissingMovies; set => this.RaiseAndSetIfChanged(ref foundMissingMovies, value); }

        public ReactiveCommand<Unit, Unit> getMissingGenres { get; }

        public ReactiveCommand<Unit, Unit> GetMissingImages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasEpisode.
        /// </summary>
        public bool HasEpisode { get => this.hasEpisode; set => this.RaiseAndSetIfChanged(ref this.hasEpisode, value); }

        /// <summary>
        /// Gets or sets a value indicating whether HasTemp.
        /// </summary>
        public bool HasTemp { get => hasTemp; set => this.RaiseAndSetIfChanged(ref hasTemp, value); }

        //public TabItemHeader ImageSetsHeader
        //{
        //    get
        //    {
        //        return TaymadeControls.Builders.TabHeaders.imageSetsHeader;
        //    }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MovieViewModelBase"/> is initialise.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialise; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool Initialise { get => this.initialise; set => this.initialise = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is converted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is converted; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool IsConverted { get => isConverted; set => this.RaiseAndSetIfChanged(ref isConverted, value); }


        public bool IsMP4
        {
            get
            {
                if (CurrentMovie != null && CurrentMovie.MoviePath != null)
                {
                    return CurrentMovie.MoviePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
            set { }
        }

        public ReactiveCommand<Unit, Unit> joinList { get; }

        public TabItemHeader MaintenanceHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.maintenanceHeader;
            }
        }

        /// <summary>
        /// Gets or sets the missing information.
        /// </summary>
        /// <value>
        /// The missing information.
        /// </value>
        /// <autogeneratedoc />
        public string? MissingInfo { get => missingInfo; set => this.RaiseAndSetIfChanged(ref missingInfo, value); }

        public TabItemHeader MovieActorHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.movieActorsHeader;
            }
        }

        public TabItemHeader MovieActorsHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.movieActorsHeader;
            }
        }

        public TabItemHeader MovieBookmarksHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.MovieBookmarksHeader();
            }
        }

        public TabItemHeader MovieCastHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.movieCastHeader;
            }
        }

        public TabItemHeader MovieDetailsHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.movieDetailsHeader;
            }
        }

        public TabItemHeader MovieDirectorsHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.directorHeader;
            }
        }

        public TabItemHeader MovieDownloadsHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.movieDownloadsHeader;
            }
        }

        public TabItemHeader MovieEditHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.MovieEditHeader();
            }
        }

        public TabItemHeader MovieInfoHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.infoHeader;
            }
        }

        /// <summary>
        /// Gets the FindMovie.
        /// </summary>
        //public ReactiveCommand<Unit, Unit> FindMovie { get; }
        public bool MovieJoined { get; private set; }

        /// <summary>
        /// Gets the movie join identifier list.
        /// </summary>
        /// <value>
        /// The movie join identifier list.
        /// </value>
        /// <autogeneratedoc />
        public List<int>? MovieJoinIdList { get; private set; }

        /// <summary>
        /// Gets the movie join list.
        /// </summary>
        /// <value>
        /// The movie join list.
        /// </value>
        /// <autogeneratedoc />
        public List<string>? MovieJoinList { get; private set; }

        /// <summary>
        /// Gets or sets the MovieList.
        /// </summary>
        public ObservableCollection<Models.Movies>? MovieList
        {
            get => movieList;
            set => this.RaiseAndSetIfChanged(ref movieList, value);
        }

        public ObservableCollection<string>? SortOrders
        {
            get => sortOrders;
            set => this.RaiseAndSetIfChanged(ref sortOrders, value);
        }
        public TabItemHeader? MovieListHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.MovieListHeader();
            }
        }

        /// <summary>
        /// Gets or sets the MoviePath.
        /// </summary>
        public string? MoviePath { get; set; }

        public TabItemHeader? MoviePlayHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.MoviePlayHeader();
            }
        }

        //public async ObservableCollection<Movies>? GetMovieListAsync(PhraseEntry phrase)
        //{
        //}
        /// <summary>
        /// Gets or sets the movie progress.
        /// </summary>
        /// <value>
        /// The movie progress.
        /// </value>
        /// <autogeneratedoc />
        public int MovieProgress { get => movieProgress; set => this.RaiseAndSetIfChanged(ref movieProgress, value); }

        //public async ObservableCollection<Movies>? GetMovieListAsync()
        //{
        //    return null;
        //}
        /// <summary>
        /// Gets a value indicating whether [m p4 converted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [m p4 converted]; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool MP4Converted { get => mP4Converted; private set => this.RaiseAndSetIfChanged(ref mP4Converted, value); }

        /// <summary>
        /// Gets or sets the MpegSupport.
        /// </summary>
        public FFMpegSupport MpegSupport { get; set; }

        /// <summary>
        /// Gets a value indicating whether [MTS converted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [MTS converted]; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool MTSConverted { get => mTSConverted; private set => this.RaiseAndSetIfChanged(ref mTSConverted, value); }

        public TabItemHeader MusicHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.musicHeader;
            }
        }

        /// <summary>
        /// Gets the NewBookmark.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewBookmark { get; set; }

        /// <summary>
        /// Gets the NewCastMember.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewCastMember { get; private set; }

        /// <summary>
        /// Creates new phrase.
        /// </summary>
        /// <value>
        /// The new phrase.
        /// </value>
        /// <autogeneratedoc />
        public PhraseEntry? NewPhrase
        {
            get => newPhrase;
            set
            {
                newPhrase = value;
                NewSubPhrase = null;
                if (value != null)
                {
                    SubPhrases = new ObservableCollection<PhraseEntry>(DataController.GetSubPhraseEntries(value));
                }
            }
        }

        /// <summary>
        /// Gets the NewPoster.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? NewPoster { get; }

        /// <summary>
        /// Gets or sets the NewSeason.
        /// </summary>
        public Models.Season? NewSeason { get => this.newSeason; set => this.RaiseAndSetIfChanged(ref this.newSeason, value); }

        public PhraseEntry? NewSubPhrase { get; set; }

        /// <summary>
        /// Gets or sets the OldMovieList.
        /// </summary>
        public ObservableCollection<Models.Movies> OldMovieList { get => oldmovieList; set => this.RaiseAndSetIfChanged(ref oldmovieList, value); }

        public ReactiveCommand<string, Unit> PasteTheText { get; }

        /// <summary>
        /// Gets or sets the BookmarkUserControl.
        /// </summary>
        // public BookmarkUserControl? BookmarkUserControl { get => bookmarkUserControl; set => bookmarkUserControl = value; }
        /// <summary>
        /// Gets or sets the Phrases.
        /// </summary>
        public ObservableCollection<Models.PhraseEntry>? Phrases { get => phraseEntries; set => this.RaiseAndSetIfChanged(ref phraseEntries, value); }

        /// <summary>
        /// Gets the PlayBookmark.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? PlayBookmark { get; set; }

        /// <summary>
        /// Gets the PlayFromLast.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? PlayFromLast { get; }

        /// <summary>
        /// Gets or sets the ProcessOutput.
        /// </summary>
        public string ProcessOutput { get => processOutput; set => this.RaiseAndSetIfChanged(ref processOutput, value); }

        /// <summary>
        /// Gets or sets the Progress.
        /// </summary>
        public string? Progress { get => this.progress; set => this.RaiseAndSetIfChanged(ref this.progress, value); }

        /// <summary>
        /// Gets the ProgressPercent.
        /// </summary>
        public int ProgressPercent { get => progressPercent; private set => this.RaiseAndSetIfChanged(ref progressPercent, value); }

        /// <summary>
        /// Gets the ReloadBookmarks.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ReloadBookmarks { get; }

        /// <summary>
        /// Gets the RefreshMovies.
        /// </summary>
        //public ReactiveCommand<Unit, Unit> RefreshMovies { get; }
        /// <summary>
        /// Gets the RepeatLast.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? RepeatLast { get; }

        /// <summary>
        /// Gets or sets the ResultTask.
        /// </summary>
        public Models.DialogResultButton ResultTask { get => resultTask; set => resultTask = value; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        /// <value>
        /// The width of the screen.
        /// </value>
        /// <autogeneratedoc />
        public int ScreenWidth { get => screenWidth; set => this.RaiseAndSetIfChanged(ref screenWidth, value); }

        public int ScreenHeight { get => screenHeight; set => this.RaiseAndSetIfChanged(ref screenHeight, value); }


        /// <summary>
        /// Gets or sets the screen width list.
        /// </summary>
        /// <value>
        /// The screen width list.
        /// </value>
        /// <autogeneratedoc />
        public List<int> ScreenWidthList { get => screenWidthList; set => screenWidthList = value; }

        public ReactiveCommand<Unit, Unit> searchForMovie { get; }

        public ObservableCollection<Movies> SeasonMovies
        {
            get => seasonMovies;
            set => this.RaiseAndSetIfChanged(ref seasonMovies, value);
        }

        public ObservableCollection<Models.Season>
                                                                    SeasonsList
        {
            get => seasonsList;
            set => this.RaiseAndSetIfChanged(ref seasonsList, value);
        }

        /// <summary>
        /// Gets or sets the SeriesList.
        /// </summary>
        public List<Models.Series>? SeriesList { get; set; }
        public object RemoveGenre { get; private set; }

        /// <summary>
        /// Gets the EndFindMovie.
        /// </summary>
        //public ReactiveCommand<Unit, Unit> EndFindMovie { get; }
        //     public ReactiveCommand<Unit, Unit> ReloadPictures { get; private set; }
        public ObservableCollection<Movies>? SeriesMovieList { get; private set; }

        public bool SeriesVisible { get; private set; }

        /// <summary>
        /// Gets or sets the SeriesEntity.
        /// </summary>
        //public Models.Series? SeriesEntity { get => seriesEntity; set => this.RaiseAndSetIfChanged(ref seriesEntity, value); }
        /// <summary>
        /// Gets the ShowDialog.
        /// </summary>
        public Interaction<MovieViewModelBase, SeriesViewModel?> ShowDialog { get; set; }

        /// <summary>
        /// Gets or sets the season entity.
        /// </summary>
        /// <value>
        /// The season entity.
        /// </value>
        /// <autogeneratedoc />
        //public Models.Season? SeasonEntity { get => seasonEntity; set => this.RaiseAndSetIfChanged(ref seasonEntity, value); }
        /// <summary>
        /// Gets or sets a value indicating whether ShowVisible.
        /// </summary>
        public bool ShowVisible { get => showVisible; set => this.RaiseAndSetIfChanged(ref showVisible, value); }

        //private Task<int?> DoRefreshDriveAsync()
        //{
        //    var taskCompletionSource = new TaskCompletionSource<int?>();
        //    this.DoRefreshDrive(result =>
        //    {
        //        taskCompletionSource.SetResult(result);
        //    });
        //}
        /// <summary>
        /// Gets the SortFields.
        /// </summary>
        public List<String> SortFields => "Id,Name,Info,NBookmarks,%Unmarked,Duration,Added,Modified,HasChapter,Year"
                    .Split(',')
                    .ToList();

        /// <summary>
        /// Gets or sets the MovieDetailControl.
        /// </summary>
        //public MovieDetailControl? MovieDetailControl { get => movieDetailControl; set => this.RaiseAndSetIfChanged(ref movieDetailControl, value); }
        /// <summary>
        /// Gets or sets the SortOrders.
        /// </summary>

        public TabItemHeader StoriesHeader
        {
            get
            {
                return TaymadeControls.Builders.TabHeaders.storiesHeader;
            }
        }

        public Movies SubGridSelected { get => subGridSelected; set => this.RaiseAndSetIfChanged(ref subGridSelected, value); }

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

        /// <summary>
        /// Gets or sets the SeriesEntity.
        /// </summary>
        //public Models.Series? SeriesEntity { get => seriesEntity; set => this.RaiseAndSetIfChanged(ref seriesEntity, value); }
        public string? TempFileName { get; private set; }

        public string? TextToken { get => textToken; set => this.RaiseAndSetIfChanged(ref textToken, value); }

        /// <summary>
        /// Gets or sets the MovieBookmarksControl.
        /// </summary>
        //public MovieBookmarksControl? MovieBookmarksControl { get => movieBookmarksControl; set => this.RaiseAndSetIfChanged(ref movieBookmarksControl, value); }
        /// <summary>
        /// Gets or sets the MovieCastControl.
        /// </summary>
        // public MovieCastControl? MovieCastControl { get => movieCastControl; set => this.RaiseAndSetIfChanged(ref movieCastControl, value); }
        public int Volume
        {
            get => volume;
            set
            {
                this.RaiseAndSetIfChanged(ref volume, value);
                
            }
        }
        public string? FindText { get => findText; private set => this.RaiseAndSetIfChanged(ref findText, value); }
        public bool? HasMovie { get => hasMovie; internal set => this.RaiseAndSetIfChanged(ref hasMovie, value); }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The GetWindow.
        /// </summary>
        /// <returns>The <see cref="Window"/>.</returns>
        public static Window GetWindow()
        {
            if (
                Avalonia.Application.Current != null
                && Avalonia.Application.Current.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime desktopLifetime
            )
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        /// <summary>
        /// The AddActualBookmark.
        /// </summary>
        /// <param name="main">The main<see cref="MainWindow?"/>.</param>
        /// <param name="vm">The vm<see cref="MainWindowViewModel?"/>.</param>
        /// <param name="bookmark">The bookmark<see cref="Bookmark"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        //public async System.Threading.Tasks.Task AddActualBookmark(
        //    MainWindow? main,
        //    MainWindowViewModel? vm,
        //    Models.Bookmark? bookmark
        //)
        //{
        //    Models.Bookmark? newbookmark = DataController.SandboxEntities.CreateBookmark(
        //        bookmark.MovieID,
        //        bookmark.Time.Value,
        //        bookmark.Name
        //    );

        //    string imagePath = bookmark.ImagePath;

        //    if (newbookmark != null)
        //    {
        //        newbookmark.ImagePath = imagePath;
        //        bookmark = newbookmark;
        //    }
        //    //DataController.SandboxEntities.Bookmarks.Add(bookmark);

        //    //DataController.SandboxEntities.;

        //    CurrentMovie.Bookmarks.Add(bookmark);

        //    CurrentMovie.ModifiedOn = DateTime.Now;
        //    CurrentMovie.ImagesCount = CurrentMovie.Bookmarks.Count;
        //    CurrentMovie.SetPercentUnmarked();
        //    CurrentMovie.Save();

        //    CurrentBookmark = bookmark;
        //    CurrentBookmark = bookmark;
        //    // if (CurrentMovieModel != null) CurrentMovieModel.Bookmarks.Add(bookmark);

        //    UserControl movieBookmarks = main.MovieBookmarks;
        //    if (movieBookmarks != null)
        //    {
        //        DataGrid dgb = main.MovieBookmarks.dgBooks;
        //        if (dgb != null)
        //        {
        //            dgb.ItemsSource = new ObservableCollection<Models.Bookmark>(CurrentMovie.Bookmarks);
        //            dgb.SelectedItem = CurrentBookmark;
        //        }
        //    }

        //    UserControl bookmarkUserControl = main.BookmarkDetails;

        //    if (bookmarkUserControl != null)
        //    {
        //        bookmarkUserControl.DataContext = this;
        //    }

        //    if (string.IsNullOrEmpty(imagePath))
        //    {
        //        await EditTheBookmark();
        //    }
        //    else newbookmark.Save();

        //    DoReloadBookmarks();
        //}

        //        // if (viewModel != null) viewModel.CurrentMovieModel = value;
        //        this.RaiseAndSetIfChanged(ref this.movie, value);
        //    }
        //}
        /// <summary>
        /// Gets or sets the Phrases.
        /// </summary>
        //public List<Models.PhraseEntry> Phrases { get => this.phraseEntries; set => this.RaiseAndSetIfChanged(ref this.phraseEntries, value); }
        /// <summary>
        /// The DoAddNfoFile.
        /// </summary>
        public async void AddNFOFile()
        {
            //MainWindowViewModel? mvm = mainWindow.DataContext as MainWindowViewModel;
            string newFileName =
                        Path.GetFileNameWithoutExtension(CurrentMovie.MoviePath) + ".Nfo";
            if (CurrentMovie != null)
            {
                if (CurrentMovie.TMDBID == null || CurrentMovie.TMDBID == 0)
                {
                    if (CurrentMovie.Nfo == null)
                    {
                        CurrentMovie.Nfo = new Nfo(newFileName);
                        CurrentMovie.Nfo.Title = CurrentMovie.MovieName;
                        if (CurrentMovie.Year != null)
                            CurrentMovie.Nfo.Year = CurrentMovie.Year.ToString();
                    }
                    else
                    {
                        CurrentMovie.Nfo.Title = CurrentMovie.MovieName;
                        if (CurrentMovie.Year != null)
                            CurrentMovie.Nfo.Year = CurrentMovie.Year.ToString();
                        CurrentMovie.Nfo.Save();
                    }
                }
                // create full nfo from The Movie Database
                if (CurrentMovie.TMDBID != null && CurrentMovie.TMDBID > 0)
                {
                    Support.iMovie iMovie = await Support.TmdbSupport.GetMovieData(CurrentMovie.TMDBID.Value);

                    Support.NfoData nfoData = new Support.NfoData(iMovie); //TmdbSupport.GetMovieDBNFOData(CurrentMovie.TMDBID.Value);

                    newFileName = CurrentMovie.NforFileName;
                    if (nfoData != null)
                    {
                        CurrentMovie.Nfo = new Nfo(nfoData);
                        nfoData.Save(newFileName);
                    }
                }
            }
        }

        //    set
        //    {
        //        if (value != null)
        //        {
        //            DataController.MovieProperties.LastMoveID = value.Id;
        //            this.movie = value;
        //            this.MovieSetup(this.movie);
        //        }
        //        // MainWindowViewModel? viewModel = Support.GetMainWindowViewModel();
        public void AddPhrase()
        {
            if (NewPhrase != null)
            {
                string? group = CurrentMovie.FilmGroup;

                if (NewSubPhrase == null)
                {
                    if (string.IsNullOrEmpty(group) && !group.Contains(NewPhrase.Id))
                        group += NewPhrase.Id;
                    else if (!group.Contains(NewPhrase.Id))
                        group += "," + NewPhrase.Id;
                }
                else
                {
                    if (string.IsNullOrEmpty(group) && !group.Contains(NewSubPhrase.Id))
                        group += NewSubPhrase.Id;
                    else if (!group.Contains(NewSubPhrase.Id))
                        group += "," + NewSubPhrase.Id;
                }
                CurrentMovie.FilmGroup = group;
                CurrentMovie.notBuilt = true;
                CurrentMovie.BuildGenreList();

                this.RaisePropertyChanged(nameof(CurrentMovie));

                if (string.IsNullOrEmpty(CurrentMovie.PrimaryFilmGroup)) CurrentMovie.PrimaryFilmGroup = NewPhrase.Id;
            }
        }

        /// <summary>
        /// Gets or sets the Movie.
        /// </summary>
        //public Models.Movies? Movie
        //{
        //    get =>
        //        //movie = Support.GetCurrentMovie();
        //        this.movie;
        public async void AddSeasonCommand()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            if (CurrentMovie != null)
            {
                CurrentSeason = new Models.Season(CurrentMovie.SeriesEntity);
                CurrentSeries = CurrentMovie.SeriesEntity;
                HasEpisode = false;
                SeriesDialogWindow seriesDialogWindow = new SeriesDialogWindow();

                SeriesViewModel seriesViewModel = new SeriesViewModel(seriesDialogWindow);

                if (this.Caller == null) this.Caller = mainWindow;

                oldCaller = this.Caller;
                Caller = seriesDialogWindow;

                seriesDialogWindow.DataContext = this;

                await seriesDialogWindow.ShowDialog(oldCaller);
                if (
                        resultButton != null
                        && resultButton.Result == Models.DialogResultButton.ResultType.Ok
                    )
                {
                    if (NewSeason != null)
                    {
                        NewSeason.Insert();
                        CurrentMovie.Season = NewSeason.Id;
                        CurrentMovie.SeasonEntity = NewSeason;
                        CurrentMovie.SeriesEntity.Seasons.Add(NewSeason);
                        CurrentMovie.Save();
                        this.RaisePropertyChanged(nameof(CurrentMovie));
                    }
                }

                Caller = oldCaller;
            }
        }

        /// <summary>
        /// Gets or sets the MovieList.
        /// </summary>
        // public ObservableCollection<Models.Movies>? MovieList { get => movieList; set => this.RaiseAndSetIfChanged(ref movieList, value); }
        /// <summary>
        /// Adds the tv episode command.
        /// </summary>
        /// <autogeneratedoc />
        public async void AddTVEpisodeCommand()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            if (CurrentMovie != null && CurrentMovie.Season != null && (CurrentMovie.Episode == null || CurrentMovie.Episode == 0))
            {
                CurrentSeason = CurrentMovie.SeasonEntity;
                CurrentSeries = CurrentMovie.SeriesEntity;
                NewSeason = CurrentSeason;
                Episode = new TVEpisode(CurrentMovie.SeasonEntity);
                HasEpisode = true;

                SeriesDialogWindow seriesDialogWindow = new SeriesDialogWindow();

                SeriesViewModel seriesViewModel = new SeriesViewModel(seriesDialogWindow);

                if (this.Caller == null) this.Caller = mainWindow;

                oldCaller = this.Caller;
                Caller = seriesDialogWindow;

                seriesDialogWindow.DataContext = this;

                await seriesDialogWindow.ShowDialog(oldCaller);
                if (
                        resultButton != null
                        && resultButton.Result == Models.DialogResultButton.ResultType.Ok
                    )
                {
                    Episode.Insert();
                    Episode.SeasonID = CurrentMovie.Season;

                    CurrentMovie.SeasonEntity.TVEpisodes.Add(Episode);
                    CurrentMovie.Episode = Episode.Id;
                    CurrentMovie.EpisodeEntity = Episode;

                    CurrentMovie.SeasonEntity.TVEpisodes = new ObservableCollection<TVEpisode>(CurrentMovie.SeasonEntity.TVEpisodes.DistinctBy(x => x.Id).OrderBy(x => x.EpisodeNumber).ToList());

                    this.RaisePropertyChanged(nameof(CurrentMovie));
                }
            }
            Caller = oldCaller;
        }

        //public Cast CurrentCastMember { get => this.currentCastMember; set => this.RaiseAndSetIfChanged(ref this.currentCastMember, value); }
        /// <summary>
        /// The DoClearList.
        /// </summary>
        public void ClearList()
        {
            foreach (var item in MovieList)
            {
                item.IsSelected = false;
            }
        }

        //public Bookmark CurrentBookmark
        //{
        //    get => this.currentBookmark;
        //    set => this.RaiseAndSetIfChanged(ref this.currentBookmark, value);
        //}
        public async void CopyText(string param)
        {
            Window window = Support.Support.GetMainWindow();
            string saveParam = string.Empty;
            if (param == "FindText")
            {
                saveParam = FindText;
            }
            else if (param == "FindBookmarkText")
            {
                saveParam = FindBookmarkText;
            }
            try
            {
                await DoSetClipboardTextAsync(saveParam);
            }
            catch (Exception e)
            {
                //ErrorMessages?.Add(e.Message);
            }
        }

        public async Task<bool> CreateActualMovieFromPath(string filePath, PhraseEntry? phrase, PhraseEntry? subPhrase)
        {
            string directory = Path.GetDirectoryName(filePath);
            //CreateLocalMethod(directory);
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            Support.Support support = new Support.Support();
            support.ActionCompleted += Support_ActionCompleted;
            support.ProgressInformation += Support_ProgressInformation;

            bool success = await support.CreateMovie(filePath, phrase, subPhrase);

            return success;
        }

        public async void CutText(string param)
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            string saveParam = string.Empty;
            if (param == "FindText")
            {
                saveParam = FindText;
                FindText = string.Empty;
            }
            else if (param == "FindBookmarkText")
            {
                saveParam = FindBookmarkText;
                FindBookmarkText = string.Empty;
            }

            try
            {
                await DoSetClipboardTextAsync(saveParam);
            }
            catch (Exception e)
            {
                //ErrorMessages?.Add(e.Message);
            }
        }

        /// <summary>
        /// The DoDeleteMovie.
        /// </summary>
        public void DeleteMovie()
        {
            //Views.MainWindow? mainWindow = GetWindow() as Views.MainWindow;

            //if (mainWindow != null)
            //{
            //MainWindowViewModel? mvm = mainWindow.DataContext as MainWindowViewModel;
            if (CurrentMovie != null)
            {
                // need to delete bookmarks and images
                // need to delete movie file and nfo file.
                // need to delete chapter file
                // need to see if directory is empty, if so delete it. Finally delete movie record

                foreach (Bookmark bookmark in CurrentMovie.Bookmarks)
                {
                    // check for existence of image
                    if (!string.IsNullOrEmpty(bookmark.ImagePath))
                    {
                        DeleteFile(bookmark.ImagePath);

                        // delete bookmark now done in stored procedure
                        // bookmark.Delete();
                    }
                }
                // check nfo file
                string nfoFile = CurrentMovie.NforFileName;

                if (!string.IsNullOrEmpty(nfoFile))
                {
                    DeleteFile(nfoFile);
                }

                // delete metafile
                string metafilePath = FFMpegSupport.GetFFMetaDataPath(
                    CurrentMovie.MoviePath
                );
                if (!string.IsNullOrEmpty(metafilePath))
                {
                    DeleteFile(metafilePath);
                }

                if (!string.IsNullOrEmpty(CurrentMovie.MoviePath))
                {
                    DeleteFile(CurrentMovie.MoviePath);
                }

                string? directory = Path.GetDirectoryName(CurrentMovie.MoviePath);

                if (
                    !string.IsNullOrEmpty(directory)
                    && Directory.Exists(directory)
                    && IsDirectoryEmpty(directory)
                )
                {
                    Directory.Delete(directory);
                }

                CurrentMovie.LogMessage("Delete");

                CurrentMovie.Delete();
                MovieList.Remove(CurrentMovie);
            }
            // }
        }

        /// <summary>
        /// The DoDeleteMovieEntity.
        /// </summary>
        public void DeleteMovieEntity()
        {
            //Views.MainWindow? mainWindow = GetWindow() as Views.MainWindow;

            //if (mainWindow != null)
            //{
            // MainWindowViewModel? mvm = mainWindow.DataContext as MainWindowViewModel;
            if (CurrentMovie != null)
            {
                CurrentMovie.Delete();
                MovieList.Remove(CurrentMovie);
            }
            //}
        }

        public void DeletePhrase()
        {
            if (CurrentPhrase != null)
            {
                CurrentPhrase.Delete();
                Phrases = new ObservableCollection<PhraseEntry>(DataController.PhraseEntries);
                CurrentPhrase = null;
                subPhrases = null;
            }
        }

        public void DeleteSeasonCommand()
        {
            if (CurrentSeason != null)
            {
                // will need to delete episodes before delet
            }
        }

        /// <summary>
        /// Deletes the sub phrase.
        /// </summary>
        /// <autogeneratedoc />
        public void DeleteSubPhrase()
        {
            if (CurrentSubPhrase != null && SubPhrases != null)
            {
                CurrentSubPhrase.Delete();
                if (CurrentPhrase != null)
                    SubPhrases = new ObservableCollection<PhraseEntry>(DataController.GetSubPhraseEntries(CurrentPhrase));
            }
        }

        /// <summary>
        /// The DoAddPhrase.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        public void DoAddPhrase()
        {
            PhraseEntry? temp = PhraseEntry.CreatePhrase();
            if (temp != null)
            {
                CurrentPhrase = temp;

                // check phrase added to cuurrentPhrase
                if (CurrentPhrase != null)
                {
                    Phrases.Add(temp);
                    CurrentPhrase.Order = Phrases.Count();
                }
            }
        }

        /// <summary>
        /// The DoAddSubPhrase.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        public void DoAddSubPhrase()
        {
            if (CurrentPhrase != null)
            {
                PhraseEntry? newPhrase = PhraseEntry.CreateSubPhrase(CurrentPhrase.Id);
                if (newPhrase != null)
                {
                    CurrentSubPhrase = newPhrase;
                    if (CurrentSubPhrase != null) SubPhrases.Add(CurrentSubPhrase);
                }
            }
        }

        public async void DoChapters()
        {
            if (this.CurrentMovie != null && !string.IsNullOrEmpty(this.CurrentMovie.MoviePath))
            {
                {
                    //SetChapterColour = new SolidColorBrush(Color.FromRgb(100,0,0));
                    // see if we have VLC running.
                    if (FFMpegSupport.FfMpegProc != null && !FFMpegSupport.FfMpegProc.HasExited && FFMpegSupport.FfMpegProc.ProcessName.ToLower() == "vlc")
                    {
                        FFMpegSupport.FfMpegProc.Kill();
                    }

                    //this.IsBusy = true;

                    //FFMpegSupport mpegSupport = new FFMpegSupport();

                    // should wait for completion;
                    //bool success = await mpegSupport.BuildChapterFileAsync(Movie);
                    if (this.MpegSupport == null) this.MpegSupport = new();
                    this.MpegSupport.Movies = this.CurrentMovie;

                    await this.MpegSupport.BuildChapterFileAsync(this.CurrentMovie);

                    this.CurrentMovie.LogMessage("Chapters Added");

                    //Movie.HasChapters = success;
                    //Movie.Save();
                    //IsBusy = false;
                    //SetChapterColour = new SolidColorBrush(Color.FromRgb(156, 156, 156));
                }
            }
        }

        /// <summary>
        /// The DoClearSort.
        /// </summary>
        public void DoClear()
        {
            for (int i = 0; i < 10; i++)
            {
                SortOrders[i] = "";
            }
        }

        /// <summary>
        /// The DoDeleteCastMember.
        /// </summary>
        public void DoDeleteCastMember()
        {
            if (CurrentCastMember != null)
            {
                CurrentCastMember.Delete();

                if (CurrentMovie != null && CurrentMovie.Casts != null)
                {
                    CurrentMovie.Casts.Remove(CurrentCastMember);
                }
            }
        }

        /// <summary>
        /// Does the edit.
        /// </summary>
        /// <autogeneratedoc />
        public async void DoEdit()
        {
            //  bool success = await ViewModelSupport.EditMovie(CurrentMovie, this);
        }

        /// <summary>
        /// The DoEditActor.
        /// </summary>
        public async void DoEditActor()
        {
            //if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            //if (mainWindow != null)
            //{
            //    if (this.CurrentCastMember != null)
            //    {
            //        this.CurrentCastMember.Save();
            //        //mCurrentCastMember?.Actor.Save();
            //        ActorViewModel viewModel = new ActorViewModel(CurrentCastMember.Actor);

            //        //viewModel.EditingActor = false;

            //        Dialogs.ActorEditDialog actorEditDialog = new Dialogs.ActorEditDialog(
            //            viewModel
            //        );

            //        actorEditDialog.DataContext = viewModel;
            //        viewModel.Caller = actorEditDialog;

            //        await actorEditDialog.ShowDialog(mainWindow);

            //        if (
            //            viewModel.resultButton != null
            //            && viewModel.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok
            //        )
            //        {
            //            // save cast
            //            if (viewModel.CurrentActor != null)
            //                viewModel.CurrentActor.Save();
            //            // viewModel.CurrentCast.Actor.Save();
            //            CurrentCastMember.Actor = viewModel.CurrentActor;
            //            CurrentCastMember.ActorChanged = true;
            //        }
            //    }
            //}
        }

        public void DoPlayMovieFile()
        {
            if (CurrentMovie != null)
            {
                TaymadeEntities.Support.Support.PlayMovie(CurrentMovie.MoviePath, null);
            }
        }

        /// <summary>
        /// Save the created or existing phrase
        /// Check to see if the directory exists.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        public void DoSavePhrase()
        {
            if (CurrentPhrase != null)
            {
                CurrentPhrase.Save();
                string phraseId = CurrentPhrase.Id;

                if (CurrentPhrase.PhraseInfo != null && string.IsNullOrEmpty(CurrentPhrase.PhraseInfo.Path))
                {
                    // create path
                    CurrentPhrase.PhraseInfo.Path = @"K:\td1\white\" + CurrentPhrase.Id;

                    if (!Directory.Exists(CurrentPhrase.PhraseInfo.Path))
                    {
                        Directory.CreateDirectory(CurrentPhrase.PhraseInfo.Path);
                    }
                    CurrentPhrase.Save();
                }
                // rebuild the phrase list
                Phrases = DataController.GetPhraseEntries();

                CurrentPhrase = Phrases.Where(x => x.Id == phraseId).FirstOrDefault();
            }
        }

        /// <summary>
        /// The DoSaveSubPhrase.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        public void DoSaveSubPhrase()
        {
            if (CurrentSubPhrase != null)
            {
                CurrentSubPhrase.Save();

                SubPhrases = new ObservableCollection<PhraseEntry>(DataController.GetSubPhraseEntries(CurrentPhrase));
            }
        }

        /// <summary>
        /// The Do_SortMovies.
        /// </summary>
        public void DoSortMovies()
        {
            DoPreSort();

            string saveSorts = String.Join(",", SortOrders.Select(x => x.ToString()).ToArray());

            DataController.MovieProperties.SortColumns = saveSorts;
            DataController.MovieProperties.Save();

            int position = 1;
            foreach (string item in SortOrders)
            {
                if (int.TryParse(item, out int direction))
                {
                    if (item != null && Math.Abs(direction) == position)
                    {
                        int index = SortOrders.IndexOf(item);

                        string sortField = SortFields[index];

                        if (!string.IsNullOrEmpty(sortField))
                        {
                            DoOrderBy(sortField, position, direction);
                        }
                    }
                }
            }
        }

        public async void DoSubGridEdit()
        {
            //bool success = await ViewModelSupport.EditMovie(SubGridSelected, this);

            //if (success) SubGridSelected.Save();
        }

        /// <summary>
        /// Edits the episode command.
        /// </summary>
        /// <autogeneratedoc />
        public async void EditEpisodeCommand()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            if (CurrentMovie != null && CurrentMovie.Season != null && (CurrentMovie.Episode != null || CurrentMovie.Episode > 0))
            {
                CurrentSeason = CurrentMovie.SeasonEntity;
                CurrentSeries = CurrentMovie.SeriesEntity;
                NewSeason = CurrentSeason;
                Episode = CurrentMovie.EpisodeEntity;
                HasEpisode = true;

                SeriesDialogWindow seriesDialogWindow = new SeriesDialogWindow();

                SeriesViewModel seriesViewModel = new SeriesViewModel(seriesDialogWindow);

                if (this.Caller == null) this.Caller = mainWindow;

                oldCaller = this.Caller;
                Caller = seriesDialogWindow;

                seriesDialogWindow.DataContext = this;

                await seriesDialogWindow.ShowDialog(oldCaller);
                if (
                        resultButton != null
                        && resultButton.Result == Models.DialogResultButton.ResultType.Ok
                    )
                {
                    Episode.Save();
                    //CurrentMovie.SeasonEntity.TVEpisodes.Add(Episode);
                    //CurrentMovie.Episode = Episode.Id;
                    //CurrentMovie.EpisodeEntity = Episode;

                    CurrentMovie.SeasonEntity.TVEpisodes = new ObservableCollection<TVEpisode>(CurrentMovie.SeasonEntity.TVEpisodes.DistinctBy(x => x.Id).OrderBy(x => x.EpisodeNumber).ToList());
                    this.RaisePropertyChanged(nameof(CurrentMovie));
                }
                Caller = oldCaller;
            }
        }

        public async void EditSeasonCommand()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            if (CurrentSeason != null)
            {
                NewSeason = CurrentSeason;
                //SeasonEntity = CurrentMovie.SeasonEntity;
                //SeriesEntity = CurrentMovie.SeriesEntity;
                //HasEpisode = CurrentMovie.HasEpisodes.Value;
                //Episode = CurrentMovie.EpisodeEntity;
                SeriesDialogWindow seriesDialogWindow = new SeriesDialogWindow();

                SeriesViewModel seriesViewModel = new SeriesViewModel(seriesDialogWindow);

                if (this.Caller == null) this.Caller = mainWindow;

                oldCaller = this.Caller;
                Caller = seriesDialogWindow;

                seriesDialogWindow.DataContext = this;

                await seriesDialogWindow.ShowDialog(oldCaller);
                if (
                       resultButton != null
                       && resultButton.Result == Models.DialogResultButton.ResultType.Ok
                   )
                {
                    CurrentSeason.Save();
                    this.RaisePropertyChanged(nameof(CurrentMovie));
                }
            }
            Caller = oldCaller;
        }

        /// <summary>
        /// The EditTheBookmark.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task EditTheBookmark()
        {
            MainWindow? main = Support.Support.GetWindow() as MainWindow;
            if (main != null)
            {


                if (this != null && this.CurrentBookmark != null)
                {
                    BookmarkViewModel viewModel = new BookmarkViewModel();

                    // set movie property as well
                    viewModel.CurrentBookmark = this.CurrentBookmark;

                    Dialogs.EditBookmarkDialog editBookmarkDialog = new Dialogs.EditBookmarkDialog(
                    );

                    editBookmarkDialog.DataContext = viewModel;

                    bool result = await editBookmarkDialog.ShowDialog<bool>(main);

                    if (result)
                    {
                        // save movie
                        if (viewModel.CurrentBookmark != null)
                        {
                            CurrentBookmark = viewModel.CurrentBookmark;
                            CurrentBookmark.Save();
                            CurrentBookmark.Redisplay();

                            //if (
                            //    CurrentMovie != null
                            //    && CurrentMovie.Bookmarks != null
                            //)
                            //CurrentMovie.Bookmarks = viewModel.Movie.Bookmarks;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The DoEndFind.
        /// </summary>
        public void EndFindMovie()
        {
            if (CurrentPhrase == null)
            {
                //string? group = DataController.MovieProperties.Group;
                //if (!string.IsNullOrEmpty(group))
                CurrentPhrase = Support.Support.GetStoredFilmGroup();
            }
            backgroundMovieGetter.AllowReload = true;   // force a reload
            GetMovieList();

            //if (CurrentPhrase != null)
            //{
            //    List<Models.Movies> tempList = MovieContext.Movies
            //        .Where(x => x.FilmGroup.Contains(CurrentPhrase.Id))
            //        .Include(x => x.Casts)
            //        .Include(b => b.Bookmarks)
            //        .Include(d => d.Director)
            //        .ToList();
            //    //MovieList = new ObservableCollection<Movies>(tempList);
            //    MovieList = MovieCollection.GetAndSortObservableCollection(tempList);
            //}
        }

        public async void Find()
        {
            if (!string.IsNullOrEmpty(FindText))
            {
                string findValue = FindText;
                findValue = findValue.Replace(Environment.NewLine, "");
                await FindMovieNewList(FindText);
            }
        }

        /// <summary>
        /// Finds the and move to current movie.
        /// </summary>
        /// <autogeneratedoc />
        //public void FindAndMoveToCurrentMovie()
        //{
        //    DataGrid? dgMovies = Support.Support.GetDgMovies();
        //    if (dgMovies != null)
        //    {
        //        dgMovies.ItemsSource = MovieList;
        //    }

        //    if (CurrentMovie != null)
        //    {
        //        Movies movies = MovieList.Where(x => x.Id == CurrentMovie.Id).FirstOrDefault();
        //        if (movies != null)
        //        {
        //            int pos = MovieList.IndexOf(movies);

        //            CurrentMovie = movies;

        //            if (pos >= 0) MoveToSelectedRow(pos);
        //        }
        //    }
        //}

        /// <summary>
        /// Finds from movie information.
        /// </summary>
        /// <autogeneratedoc />
        public async void FindFromInfo()
        {
            if (!string.IsNullOrEmpty(FindBookmarkText))
            {
                await FindMovieNewListInfo();
            }
            else if (!string.IsNullOrEmpty(FindText))
            {
                FindBookmarkText = FindText;
                await FindMovieNewListInfo();
                FindBookmarkText = String.Empty;
            }
        }

        /// <summary>
        /// Finds the movie.
        /// </summary>
        /// <autogeneratedoc />
        public void FindMovie()
        {
            FindMovie(true);
        }

        /// <summary>
        /// The DoFindMovies.
        /// </summary>
        public void FindMovie(bool left = false)
        {
            if (!string.IsNullOrEmpty(FindText))
            {
                OldMovieList = MovieList;

                List<Movies> tempList = DataController.SandboxEntities.Movies
                    .Where(x => x.MovieName.ToLower().Contains(FindText.ToLower()))
                    .OrderBy(x => x.MovieName)
                    .ToList();
                //MovieList = MovieList = new ObservableCollection<Movies>(tempList);
                MovieList = MovieCollection.GetAndSortObservableCollection(tempList, false);

                foreach (Movies item in MovieList)
                {
                    //item.FixMovieData();

                    if (item.Dirty)
                    {
                        item.Save();
                    }
                }

                if (!left) FindText = string.Empty;
            }
            else
                MovieList = OldMovieList;
        }

        /// <summary>
        /// Finds the with bookmark.
        /// </summary>
        /// <autogeneratedoc />
        public async void FindWithBookmark()
        {
            if (!string.IsNullOrEmpty(FindBookmarkText))
            {
                await FindMovieNewListBookmarks();
            }
            else if (!string.IsNullOrEmpty(FindText))
            {
                FindBookmarkText = FindText;
                await FindMovieNewListBookmarks();
                FindBookmarkText = String.Empty;
            }
        }

        /// <summary>
        /// Finds the with bookmark no dialog.
        /// </summary>
        /// <autogeneratedoc />
        public async void FindWithBookmarkNoDialog()
        {
            if (!string.IsNullOrEmpty(FindBookmarkText))
            {
                await FindMovieNewListBookmarks(true);
            }
        }

        public ICommand GetChaptersCommand()
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                DoChapters();
            });
            return myCommand;
        }

        public ReactiveCommand<Unit, Unit> GetCommand(ReactiveCommand<Unit, Unit> model)
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                if (model != null)
                {
                    model.Execute().Subscribe();
                }
            });
            return myCommand;
        }

        public Director GetDirector()
        {
            Director returnVal = null;

            if (CurrentMovie != null && CurrentMovie.TMDBID != null)
            {
                Support.CastList castMembers = Support.TmdbSupport.GetMovieCredits(CurrentMovie.TMDBID.Value);

                var director = castMembers.Where(cl => cl.IsDirector).FirstOrDefault();

                if (director != null)
                {
                    Models.Director? mdirector = Models.DataController.SandboxEntities.Directors.Where(d => d.Name.ToLower() == director.Name.ToLower()).FirstOrDefault();
                    if (mdirector != null)
                    {
                        CurrentMovie.Director = mdirector;
                        CurrentMovie.DirectorID = mdirector.Id;
                    }
                    else
                    {
                        // create new director
                        mdirector = new Models.Director();
                        mdirector.Name = director.Name;
                        Models.DataController.SandboxEntities.Directors.Add(mdirector);
                        Models.DataController.SandboxEntities.SaveChanges();
                        CurrentMovie.Director = mdirector;
                        CurrentMovie.DirectorID = mdirector.Id;
                        DirectorList.Add(mdirector);
                    }
                }
                else { CurrentMovie.DirectorID = 14; }
            }

            return returnVal;
        }

        public async Task<Director> GetDirectorAsync()
        {
            Director returnVal = null;

            if (CurrentMovie != null && CurrentMovie.TMDBID != null)
            {
                Support.CastList castMembers = await Support.TmdbSupport.GetMovieCreditsAsync(CurrentMovie.TMDBID.Value);

                var director = castMembers.Where(cl => cl.IsDirector).FirstOrDefault();

                if (director != null)
                {
                    Models.Director? mdirector = Models.DataController.SandboxEntities.Directors.Where(d => d.Name.ToLower() == director.Name.ToLower()).FirstOrDefault();
                    if (mdirector != null)
                    {
                        CurrentMovie.Director = mdirector;
                        CurrentMovie.DirectorID = mdirector.Id;
                    }
                    else
                    {
                        // create new director
                        mdirector = new Models.Director();
                        mdirector.Name = director.Name;
                        Models.DataController.SandboxEntities.Directors.Add(mdirector);
                        Models.DataController.SandboxEntities.SaveChanges();
                        CurrentMovie.Director = mdirector;
                        CurrentMovie.DirectorID = mdirector.Id;
                        DirectorList.Add(mdirector);
                    }
                }
                else { CurrentMovie.DirectorID = 14; }
            }

            return returnVal;
        }

        public async void GetMissingGenres()
        {
            //List<MissingGenresMovies> missingGenrestemp = DataController.SandboxEntities.MissingGenresMovies.ToList();

            List<Movies>? missingGenres = DataController.MovieController.GetMoviesByGenre("PORN-1");

            //foreach (var item in missingGenres)
            //{
            //    item.SetPercentUnmarked();
            //    item.BuildGenreList();
            //    Debug.WriteLine(item.Id.ToString());
            //}

            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            // save caller for return
            Window tempWindow = Caller;

            if (missingGenres != null)
            {
                // save current movie list and currentMovie
                ObservableCollection<Movies> oldMovieList = MovieList;
                Movies oldCurrentMovie = CurrentMovie;

                MovieViewModel viewModel = new MovieViewModel();
                viewModel.MovieList = new ObservableCollection<Movies>(missingGenres); ;
                viewModel.CurrentMovie = CurrentMovie;

                //MovieListDialog movieListDialog = new MovieListDialog(viewModel);
                //viewModel.Caller = movieListDialog;
                //this.Caller = movieListDialog;

                //// show on main window
                //await movieListDialog.ShowDialog(mainWindow);

                //// restore caller and movie List
                //this.Caller = tempWindow;
                //MovieList = oldMovieList;
                //CurrentMovie = oldCurrentMovie;
                // }
            }
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 30/01/2026 30/01/2026 </created>
        /// </remarks>
        public void GetMovieList()
        {
            if (CurrentPhrase != null)
            {
                SeriesVisible = (CurrentPhrase.Id == "SER");
                if (CurrentSubPhrase == null)
                {
                    backgroundMovieGetter.Phrase = CurrentPhrase;
                    backgroundMovieGetter.SubPhrase = CurrentSubPhrase;
                    if (!backgroundMovieGetter.busy) backgroundMovieGetter.Run();
                }
                else
                {
                    backgroundMovieGetter.Phrase = CurrentPhrase;
                    backgroundMovieGetter.SubPhrase = CurrentSubPhrase;
                    if (!backgroundMovieGetter.busy) backgroundMovieGetter.Run();
                }
                // MovieList = Support.GetMovieList(this.CurrentPhrase.Id);
            }
            else
            {
                backgroundMovieGetter.Phrase = null;
                if (!backgroundMovieGetter.busy) backgroundMovieGetter.Run();
                //backgroundMovieGetter.Run();
                //MovieList = Support.GetMovieList("");
            }

            //MovieList = MovieCollection.GetAndSortObservableCollection(
            //    DataController.MovieList.ToList()
            //);

            if (
                DataController.MovieProperties.LastMoveID != null
                && DataController.MovieProperties.LastMoveID > 0
            )
            {
                if (MovieList != null)
                {
                    CurrentMovie = MovieList.Where(x => x.Id ==
                        DataController.MovieProperties.LastMoveID).FirstOrDefault();

                    if (CurrentMovie != null)
                    {
                        MoviePath = CurrentMovie.MoviePath;
                        int pos = MovieList.IndexOf(CurrentMovie);
                        if (pos >= 0) MoveToSelectedRow(pos);
                    }
                }
            }
        }

        public void GetMovieList(bool forceReload)
        {
            if (forceReload)
                backgroundMovieGetter.AllowReload = true;
            GetMovieList();
        }

        public ICommand GetSearchCommand()
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                this.Search();
            });
            return myCommand;
        }

        public async void GetSearchDirector()
        {
            Director director = await GetDirectorAsync();
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

        /// <summary>
        /// The GrabBookmarkImage.
        /// </summary>
        public void GrabBookmarkImage(Bookmark bookmark)
        {
            if (CurrentMovie != null && bookmark != null)
            {
                // Support.VideoSupport.GrabBookmarkImage(CurrentMovie, bookmark, 0);
                System.Threading.Thread.Sleep(1000);
                var bmp = bookmark.ImageBMP;
                bookmark.SetImageBMP();
            }
        }

        /// <summary>
        /// Groups the changed.
        /// </summary>
        /// <autogeneratedoc />
        public void GroupChanged()
        {
            if (CurrentPhrase != null)
            {
                if (CurrentSubPhrase != null && CurrentSubPhrase.COMPKEY != CurrentPhrase.COMPKEY)
                {
                    //MovieList = new ObservableCollection<Movies>(DataController.SandboxEntities.GetMoviesByGenre(CurrentPhrase.COMPKEY, CurrentSubPhrase.COMPKEY));
                }
                else
                {
                    GetMovieList();
                    //MovieList = new ObservableCollection<Movies>(DataController.SandboxEntities.GetMoviesByGenre(CurrentPhrase.COMPKEY));
                }
                DataController.MovieProperties.Group = CurrentPhrase.Id;
                DataController.MovieProperties.Save();

                //ComboBox? cmb = sender as ComboBox;

                //if (mainWindowViewModel == null) mainWindowViewModel = DataContext as MainWindowViewModel;

                //if (mainWindowViewModel != null)
                // {
                if (this.MovieList != null)
                //    mainWindowViewModel.MovieListControl = GetMovieListControl() as MovieListControl;

                {
                    //ComboBox? seriesCombo = GetSeriesCombo(this.MovieList);
                    //ComboBox? seasonCombo = GetSeasonCombo(this.MovieList);

                    //if (seriesCombo != null) seriesCombo.IsVisible = false;
                    //if (seasonCombo != null) seasonCombo.IsVisible = false;

                    //if (!initalise && cmb != null && cmb.SelectedItem != null)
                    //{
                    //string desc = cmb.SelectedItem as string;
                    //DoChangeGroup(cmb.SelectedItem as PhraseEntry);

                    //}
                    //initalise = false;
                }
                //}
            }
        }

        /// <summary>
        /// The IsDirectoryEmpty.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsDirectoryEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
        }

        /// <summary>
        /// The DoJoinList.
        /// </summary>
        public void JoinList()
        {
            SetupMpeg();
        }

        //public void LastMovie()
        //{
        //    // get last id from MovieProperties
        //    int? lastId = DataController.MovieProperties.LastMoveID;

        //    // set CurrentMovie to movie with lastId
        //    if (lastId != null)
        //    {
        //        CurrentMovie = MovieList.Where(x => x.Id == lastId).FirstOrDefault();
        //        DataGrid? dgMovies = Support.Support.GetDgMovies();

        //        dgMovies.ScrollIntoView(CurrentMovie, dgMovies.Columns[1]);
        //    }
        //}

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
                            //  await Support.VideoSupport.GrabBookmarkImage(CurrentMovie, bookmark, 0);

                            CurrentBookmark = bookmark;
                            //bookmark.ImagePath = string.Empty;
                            bookmark.Save();
                        }
                    }
                }
            }
        }

        public async void MoveMP4()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            // find details of current movie, create MP4 filename, check its existence
            if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MoviePath))
            {
                string? mp4FileName = Path.ChangeExtension(CurrentMovie.MoviePath, ".mp4");
                if (File.Exists(mp4FileName))
                {
                    // file exists, save current moviepath in variable, change moviepath to mp4 filename
                    string? oldMoviePath = CurrentMovie.MoviePath;
                    CurrentMovie.MoviePath = mp4FileName;
                    // save movie
                    CurrentMovie.Save();

                    // delete old movie file check with user that this desired if so delete old file
                    try
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Delete old movie file",
                            "Do you want to delete the old movie file?",
                            ButtonEnum.YesNo
                            );

                        var result = await box.ShowAsPopupAsync(mainWindow);
                        //MessageBox.IconProperty

                        if (result == ButtonResult.Yes)
                        {
                            // delete old movie file
                            DeleteFile(oldMoviePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        DeleteFile(oldMoviePath);
                    }
                }
            }
            MP4Converted = false;
        }

        public async void MoveMTS()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            // find details of current movie, create MP4 filename, check its existence
            if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MoviePath))
            {
                string? mtsFileName = Path.ChangeExtension(CurrentMovie.MoviePath, ".mts");
                if (File.Exists(mtsFileName))
                {
                    // file exists, save current moviepath in variable, change moviepath to mp4 filename
                    string? oldMoviePath = CurrentMovie.MoviePath;
                    CurrentMovie.MoviePath = mtsFileName;
                    // save movie
                    CurrentMovie.Save();

                    // delete old movie file check with user that this desired if so delete old file
                    try
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Delete old movie file",
                            "Do you want to delete the old movie file?",
                            ButtonEnum.YesNo
                            );

                        var result = await box.ShowAsPopupAsync(mainWindow);
                        //MessageBox.IconProperty

                        if (result == ButtonResult.Yes)
                        {
                            // delete old movie file
                            DeleteFile(oldMoviePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        DeleteFile(oldMoviePath);
                    }
                }
            }
            MTSConverted = false;
        }

        /// <summary>
        /// Moves to current.
        /// </summary>
        /// <autogeneratedoc />
        public void MoveToCurrent()
        {
            if (MovieList != null && CurrentMovie != null)
            {
                int pos = MovieList.IndexOf(currentMovie);
                MoveToSelectedRow(pos);
            }
        }

        /// <summary>
        /// Moves to first.
        /// </summary>
        /// <autogeneratedoc />
        public void MoveToFirst()
        {
            if (MovieList != null)
            {
                CurrentMovie = MovieList.FirstOrDefault();
                if (CurrentMovie != null)
                {
                    int pos = 0;
                    MoveToSelectedRow(pos);
                }
            }
        }

        /// <summary>
        /// Moves to last.
        /// </summary>
        /// <autogeneratedoc />
        public void MoveToLast()
        {
            if (MovieList != null)
            {
                CurrentMovie = MovieList.LastOrDefault();
                if (CurrentMovie != null)
                {
                    int pos = MovieList.IndexOf(CurrentMovie);
                    MoveToSelectedRow(pos);
                }
            }
        }

        /// <summary>
        /// Moves to selected row.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <autogeneratedoc />
        //public void MoveToSelectedRow(int? pos = null)
        //{
        //    //DataGrid? temp = BoundGrid;
        //    DataGrid? dgMovies = Support.Support.GetDgMovies();
        //    if (dgMovies != null)
        //    {
        //        //dgMovies.ItemsSource = MovieList;
        //        ObservableCollection<Movies>? movies = dgMovies.ItemsSource as ObservableCollection<Movies>;

        //        //Movies tempm = movies.Where(x => x.Id == CurrentMovie.Id).FirstOrDefault();
        //        //CurrentMovie = movies.Where(x => x.Id == CurrentMovie.Id).FirstOrDefault();
        //        if (CurrentMovie != null && movies != null && movies.Contains(CurrentMovie))
        //        {
        //            dgMovies.SelectedItem = CurrentMovie;

        //            dgMovies.ScrollIntoView(CurrentMovie, null);

        //        }
        //    }
        //}

        public async void OtherMovies()
        {
            //if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            //// save caller for return
            //Window tempWindow = Caller;

            //List<Movies> temp = ViewModelSupport.OtherMovieList(CurrentMovie);
            //if (temp != null)
            //{
            //    // save current movie list and currentMovie
            //    ObservableCollection<Movies> oldMovieList = MovieList;
            //    Movies oldCurrentMovie = CurrentMovie;

            //    MovieViewModel viewModel = new MovieViewModel();
            //    viewModel.MovieList = new ObservableCollection<Movies>(temp); ;
            //    viewModel.CurrentMovie = CurrentMovie;

            //    MovieListDialog movieListDialog = new MovieListDialog(viewModel);
            //    viewModel.Caller = movieListDialog;
            //    this.Caller = movieListDialog;

            //    // show on main window
            //    await movieListDialog.ShowDialog(mainWindow);

            //    // restore caller and movie List
            //    this.Caller = tempWindow;
            //    MovieList = oldMovieList;
            //    CurrentMovie = oldCurrentMovie;
            //    // }
            //}
        }

        /// <summary>
        /// Does the page down command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageDownCommand()
        {
            if (MovieList != null && CurrentMovie != null)
            {
                int pos = MovieList.IndexOf(currentMovie);
                pos += 15;
                if (pos < MovieList.Count)
                {
                    CurrentMovie = MovieList[pos];
                }
                else
                {
                    CurrentMovie = MovieList.Last();
                }

                MoveToSelectedRow(pos);
            }
        }

        /// <summary>
        /// Pages the end command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageEndCommand()
        {
            MoveToLast();
        }

        /// <summary>
        /// Pages the home command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageHomeCommand()
        {
            MoveToFirst();
        }

        /// <summary>
        /// Pages up command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageUpCommand()
        {
            if (MovieList != null && CurrentMovie != null)
            {
                int pos = MovieList.IndexOf(currentMovie);
                pos -= 5;
                if (pos >= 0)
                {
                    CurrentMovie = MovieList[pos];
                }
                else
                {
                    CurrentMovie = MovieList.First();
                }

                MoveToSelectedRow(pos);
            }
        }

        public async void PasteText(string param)
        {
            //string? pastedText = null;
            if (await DoGetClipboardTextAsync() is { } pastedText)
            {
                if (param == "FindText")
                {
                    FindText = pastedText;
                }
                else if (param == "FindBookmarkText")
                {
                    FindBookmarkText = pastedText;
                }
            }
        }

        public void PlayCurrentMovie()
        {
            if (CurrentMovie != null)
            {
                TaymadeEntities.Support.Support.PlayMovie(CurrentMovie.MoviePath, null);
            }
        }

        /// <summary>
        /// The Do_PlayFromLastPlayer.
        /// </summary>
        public void PlayFromLastPlayer()
        {
            if (CurrentMovie != null && CurrentMovie.Bookmarks.Count > 0)
            {
                string parameters = "ID=" + CurrentMovie.Id.ToString().Trim();

                List<Bookmark> bookmarks = CurrentMovie.Bookmarks.ToList();

                bookmarks.Sort((x, y) => x.Time.Value.CompareTo(y.Time.Value));

                CurrentBookmark = bookmarks.LastOrDefault();

                if (CurrentBookmark != null && CurrentBookmark.Time != null)
                {
                    parameters += " FROM=" + CurrentBookmark.Time.ToString().Trim();

                    if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

                    if (mainWindow != null)
                    {
                        parameters += " POSITION=" + mainWindow.Position.ToString().Replace(", -", "|");
                    }
                }

                PlayClick1(parameters);
            }
        }

        public void PlaySubGridMovie()
        {
            if (SubGridSelected != null)
            {
                TaymadeEntities.Support.Support.PlayMovie(SubGridSelected.MoviePath, null);
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        /// <autogeneratedoc />
        public void Refresh()
        {
            if (CurrentMovie != null)
            {
                //MainWindowViewModel? model = button.DataContext as MainWindowViewModel;

                CurrentMovie.SetPercentUnmarked();
                CurrentMovie.BuildGenreList();
                this.RaisePropertyChanged(nameof(CurrentMovie));
            }
        }

        /// <summary>
        /// The DoRefreshMovies.
        /// </summary>
        public void RefreshMovies()
        {
            if (CurrentPhrase == null)
            {
                string? group = DataController.MovieProperties.Group;
                if (!string.IsNullOrEmpty(group))
                    CurrentPhrase = DataController.PhraseEntries.Find(x => x.Id == group);
            }

            if (CurrentPhrase != null)
            {
                //MovieList = Support.Support.GetMovieList(CurrentPhrase.Id);

                MovieList = new ObservableCollection<Movies>(
                    DataController.MovieController.GetMoviesByGenre(CurrentPhrase.COMPKEY)
                    );

                //List<Models.Movies> tempList = MovieContext.Movies
                //    .Where(x => x.FilmGroup.Contains(CurrentPhrase.Id))
                //    .Include(x => x.Casts)
                //    .Include(b => b.Bookmarks)
                //    .Include(d => d.Director)
                //    .ToList();
                ////MovieList = new ObservableCollection<Movies>(tempList);
                //MovieList = MovieCollection.GetAndSortObservableCollection(tempList);

                DoSortMovies();
                //FindAndMoveToCurrentMovie();
            }
        }

        public void ReloadGroups()
        {
            CurrentSubPhrase = null;

            GetMovieList(true);
        }

        public void SaveCurrentMovie()
        {
            if (CurrentMovie != null) CurrentMovie.Save();
        }

        public void SaveCurrentMovieNFO()
        {
            if (CurrentMovie != null && CurrentMovie.Nfo != null) CurrentMovie.Nfo.Save();
        }

        /// <summary>
        /// Defines the movie.
        /// </summary>
        //private Models.Movies? movie;
        /// <summary>
        /// The DoSearch.
        /// </summary>
        public async void Search()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
            if (mainWindow != null)
            {
                if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MovieName))
                {
                    if (this.Caller == null) this.Caller = mainWindow;
                    oldCaller = this.Caller;
                    MovieViewModel viewModel = new MovieViewModel(CurrentMovie);

                    // add in fixing the title
                    // set viewModel year to the text in the ()
                    string name = CurrentMovie.MovieName;
                    // replace 's)' with s) to deal with decades
                    name = name.Replace("s)", "s)");

                    int pos1 = name.LastIndexOf('(');
                    int pos2 = name.LastIndexOf(')');
                    if (pos1 > 0 && pos2 > pos1)
                    {
                        string year = name.Substring(pos1 + 1, pos2 - pos1 - 1);
                        if (int.TryParse(year, out int y))
                        {
                            viewModel.Year = y;
                            CurrentMovie.Year = y;
                            CurrentMovie.Save();

                            name = name.Substring(0, pos1).Trim();
                            viewModel.MovieTitle = name;
                        }

                        // is not a year in brackets so put brackets back
                        else viewModel.MovieTitle = Path.GetFileNameWithoutExtension(CurrentMovie.MoviePath);
                    }
                    else viewModel.MovieTitle = Path.GetFileNameWithoutExtension(CurrentMovie.MoviePath);

                    //viewModel.Caller = this.Caller;
                    //Dialogs.TMDBSearchDialog searchDialog = new Dialogs.TMDBSearchDialog(viewModel);
                    //viewModel.Caller = searchDialog;
                    //await searchDialog.ShowDialog(oldCaller);
                    //if (
                    //    viewModel.resultButton != null
                    //    && viewModel.resultButton.Result == Models.DialogResultButton.ResultType.Ok
                    //)
                    //{
                    //    if (viewModel.FoundMovie != null)
                    //        GetTMDBDetailsLocal(viewModel);
                    //}
                    //this.Caller = oldCaller;
                }
            }
        }

        /// <summary>
        /// Shows the details.
        /// </summary>
        /// <autogeneratedoc />
        public void ShowDetails()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            if (mainWindow != null)
            {
                //var movieDialog = new Dialogs.MovieDetailDialog(this, false);

                //this.Caller = movieDialog;

                // movieDialog.ShowDialog(mainWindow);
            }
        }

        public void ShowSubGridDetails()
        {
            Views.MainWindow? mainWindow = Support.Support.GetMainWindow() as Views.MainWindow;

            Movies oldSelectedMovie = CurrentMovie;
            if (mainWindow != null)
            {
                this.CurrentMovie = SubGridSelected;

                //  var movieDialog = new Dialogs.MovieDetailDialog(this, false);

                //this.Caller = movieDialog;

                //movieDialog.ShowDialog(mainWindow);
            }

            CurrentMovie = oldSelectedMovie;
        }

        public void SubComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            // check sender is a combobox
            if (sender != null)
            {
                ComboBox? comboBox = sender as ComboBox;
                // see if it is the subphrase box SubGroups
                if (comboBox != null)
                    if (comboBox.Name == "SubGroups")
                    {
                        if (e.AddedItems.Count > 0)
                        {
                            CurrentSubPhrase = e.AddedItems[0] as PhraseEntry;
                            this.GetMovieList();
                        }
                    }
                    else if (comboBox.Name == "Groups")
                    {
                        if (e.AddedItems.Count > 0)
                        {
                            CurrentPhrase = e.AddedItems[0] as PhraseEntry;
                            CurrentSubPhrase = null;
                            this.GetMovieList();
                        }
                    }
            }
        }

        //public void ToBookmarks()
        //{
        //    // set currenttab to bookmark tab on mainWindow
        //    if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();
        //    if (mainWindow != null)
        //    {
        //        mainWindow.tabControl.SelectedItem = mainWindow.Bookmarks;
        //    }
        //}



        /// <summary>
        /// The DoToMP4.
        /// </summary>
        public async void ToMP4()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            MP4Converted = false;
            if (mainWindow != null)
            {
                if (CurrentMovie != null)
                {
                    {
                        FFMpegSupport mpegSupport = new FFMpegSupport();
                        //mpegSupport.ConversionComplete += MP4Complete;
                        mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                        mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;
                        if (CurrentMovie.DurationSeconds != null) mpegSupport.TotalDuration = CurrentMovie.DurationSeconds.Value;
                        mpegSupport.MovieName = CurrentMovie.MovieName;
                        // set action
                        mpegSupport.action = "CONVERT";
                        bool success = await mpegSupport.ConvertToMP4(CurrentMovie.MoviePath, CurrentMovie.Id);

                        MP4Converted = success;
                    }
                }
            }
        }

        /// <summary>
        /// The DoToMTS.
        /// </summary>
        public async void ToMTS()
        {
            MTSConverted = false;
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            if (mainWindow != null)
            {
                MainWindowViewModel? mvm = mainWindow.DataContext as MainWindowViewModel;
                if (mvm != null && CurrentMovie != null)
                {
                    {
                        FFMpegSupport mpegSupport = new FFMpegSupport();
                        //mpegSupport.ConversionComplete += MTSComplete;
                        mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                        mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;
                        if (CurrentMovie.DurationSeconds != null) mpegSupport.TotalDuration = CurrentMovie.DurationSeconds.Value;
                        mpegSupport.MovieName = CurrentMovie.MovieName;

                        bool success = await mpegSupport.ConvertToMTS(CurrentMovie);
                        MTSConverted = success;
                    }
                }
            }
        }


        /// <summary>
        /// The DoTrimMovieAsync.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> TrimMovie()
        {
            bool success = false;
            if (CurrentMovie != null)
            {
                CurrentMovie.EndBookmark.Time = CurrentMovie.DurationSeconds;
                Dialogs.TrimMovieDialog trimMovieDialog = new TrimMovieDialog(this, CurrentMovie);
                Caller = trimMovieDialog;

                if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

                if (mainWindow != null)
                {
                    await trimMovieDialog.ShowDialog(mainWindow);
                    Support.Support.DeleteTempImage();

                    if (ResultTask != null && ResultTask.Result == DialogResultButton.ResultType.Ok)
                    {
                        tempMissingFile = null;
                        {
                            FFMpegSupport mpegSupport = new FFMpegSupport();
                            //mpegSupport.ConversionComplete += MTSComplete;
                            mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                            mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;
                            mpegSupport.CliWrapError += MpegSupport_CliWrapError;
                            if (currentMovie.DurationSeconds != null) mpegSupport.TotalDuration = currentMovie.DurationSeconds.Value;
                            if (ResultTask.Seconds != null) mpegSupport.TotalDuration = ResultTask.Seconds.Value;
                            mpegSupport.MovieName = currentMovie.MovieName;

                            int val = await mpegSupport.TrimMovie(currentMovie, ResultTask.Paramater);

                            if (val == 0) HasTemp = true;
                        }
                        success = true;
                    }

                    // TrimMovie = ReactiveCommand.CreateFromTask(DoTrimMovieAsync);
                }
            }
            return success;
        }

        public void UpdateToken()
        {
            if (!string.IsNullOrEmpty(AutoCompleteToken) && !string.IsNullOrEmpty(TextToken))
            {
                AutoComplete = AutoComplete.Replace("," + AutoCompleteToken + ",", "," + TextToken + ",");

                List<string> tempList = AutoComplete.Split(',').ToList();

                tempList.Sort();

                //AutoComplete = string.Join(',', tempList);

                AutoCompleteTokens = tempList;
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// The DoSubSorts.
        /// </summary>
        /// <param name="sortFieldsList">The sortFieldsList<see cref="List{string}"/>.</param>
        /// <param name="sortDirectionsList">The sortDirectionsList<see cref="List{int}"/>.</param>
        /// <param name="eList">The eList<see cref="IOrderedEnumerable{Movies}"/>.</param>
        /// <returns>The <see cref="IOrderedEnumerable{Movies}"/>.</returns>
        internal static IOrderedEnumerable<Movies> DoSubSorts(
            List<string> sortFieldsList,
            List<int> sortDirectionsList,
            IOrderedEnumerable<Movies> eList
        )
        {
            for (int i = 0; i < sortFieldsList.Count; i++)
            {
                if (sortFieldsList[i] == "Id")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.Id);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.Id);
                }
                else if (sortFieldsList[i] == "Name")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.MovieName);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.MovieName);
                }
                else if (sortFieldsList[i] == "Info")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.Info);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.Info);
                }
                else if (sortFieldsList[i] == "NBookmarks")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.ImagesCount);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.ImagesCount);
                }
                else if (sortFieldsList[i] == "%Unmarked")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.PercentUnBookmarked);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.PercentUnBookmarked);
                }
                else if (sortFieldsList[i] == "Duration")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.DurationSeconds);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.DurationSeconds);
                }
                else if (sortFieldsList[i] == "Added")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.AddedOn);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.AddedOn);
                }
                else if (sortFieldsList[i] == "Modified")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.ModifiedOn);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.ModifiedOn);
                }
                else if (sortFieldsList[i] == "HasChapter")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.HasChapters);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.HasChapters);
                }
                else if (sortFieldsList[i] == "Year")
                {
                    if (sortDirectionsList[i] > 0)
                        eList = eList.ThenBy(x => x.Year);
                    else if (sortDirectionsList[i] < 0)
                        eList = eList.ThenByDescending(x => x.Year);
                }
            }

            return eList;
        }

        /// <summary>
        /// The DoFindMoviesByBookmark.
        /// </summary>
        internal void DoFindMoviesByBookmark()
        {
            if (!string.IsNullOrEmpty(FindBookmarkText))
            {
                OldMovieList = MovieList;
                MovieList = GetMatchingBookmarksFromDB();
            }
            else
                MovieList = OldMovieList;
        }

        /// <summary>
        /// The DoOrderBy.
        /// </summary>
        /// <param name="sortField">The sortField<see cref="string"/>.</param>
        /// <param name="position">The position<see cref="int"/>.</param>
        /// <param name="direction">The direction<see cref="int"/>.</param>
        internal void DoOrderBy(string sortField, int position, int direction)
        {
            List<Movies>? newList = null;

            List<string> sortFieldsList = new List<string>();
            List<int> sortDirectionsList = new List<int>();

            for (int i = 2; i < 11; i++)
            {
                foreach (string item in SortOrders)
                {
                    if (int.TryParse(item, out int nextdirection))
                    {
                        if (item != null && Math.Abs(nextdirection) == i)
                        {
                            int index = SortOrders.IndexOf(item);

                            string nextsortField = SortFields[index];

                            if (!string.IsNullOrEmpty(nextsortField))
                            {
                                sortFieldsList.Add(nextsortField);
                                sortDirectionsList.Add(nextdirection);
                            }
                        }
                    }
                }
            }

            if (MovieList != null)
            {
                if (sortField == "Id")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.Id);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.Id);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Name")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.MovieName);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.MovieName);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Info")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.Info);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.Info);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "NBookmarks")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.ImagesCount);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.ImagesCount);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Duration")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.DurationSeconds);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.DurationSeconds);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "HasChapter")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.HasChapters);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.HasChapters);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "%Unmarked")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.PercentUnBookmarked);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.PercentUnBookmarked);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Added")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.AddedOn);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.AddedOn);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Modified")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.ModifiedOn);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.ModifiedOn);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
                else if (sortField == "Year")
                {
                    if (direction > 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderBy(x => x.Year);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                    else if (direction < 0)
                    {
                        if (sortFieldsList.Count >= 0)
                        {
                            var eList = MovieList.OrderByDescending(x => x.Year);
                            eList = DoSubSorts(sortFieldsList, sortDirectionsList, eList);
                            newList = eList.ToList();
                            eList = null;
                        }
                    }
                }
            }

            if (newList != null)
            {
                MovieList = new ObservableCollection<Movies>(newList);
                newList = null;

                if (CurrentMovie != null)
                {
                    int pos = MovieList.IndexOf(CurrentMovie);
                    if (pos >= 0) MoveToSelectedRow(pos);
                }
            }
        }

        /// <summary>
        /// The DoPreSort.
        /// </summary>
        internal void DoPreSort()
        {
            if (!preSorted)
            {
                foreach (Movies movie in MovieList)
                {
                    movie.SetPercentUnmarked();
                }
            }

            preSorted = true;
        }

        /// <summary>
        /// The DoReloadBookmarks.
        /// </summary>
        internal void DoReloadBookmarks()
        {
            if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

            // CurrentMovie = Support.GetCurrentMovie();

            if (CurrentMovie != null && mainWindow != null)
            {
                CurrentMovie.Save();
                DataController.ReloadMovie(CurrentMovie);
                CurrentMovie.Bookmarks = new ObservableCollection<Bookmark>();
                CurrentMovie.Bookmarks = new ObservableCollection<Bookmark>(
                    DataController.BookmarkController.GetBookmarksByMovieId(CurrentMovie.Id));
                CurrentMovie.ImagesCount = CurrentMovie.Bookmarks.Count;
                CurrentMovie.SetPercentUnmarked();

                //mainWindow.SetBookmarks(CurrentMovie);
                //Support.Support.SetCurrentMovie(CurrentMovie);
            }
        }

        /// <summary>
        /// The MpegSupport_CliWrapCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapCompletedEventArgs"/>.</param>
        internal async void MpegSupport_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        {
            if (e.Result == 0)
            {
                if (e.TaskName == "JOIN")
                {
                    ProcessOutput = "Movies Joined";
                    MissingFile missing = new MissingFile() { Path = e.MovieName, IsSelected = true };
                    MissingInfo = ProcessOutput;
                    FoundMissingMovies.DeselectAll();
                    FoundMissingMovies.Add(missing);
                    CurrentMissingMovie = missing;
                    ScrollMissingIntoView();
                }
                else if (e.TaskName == "JOINMOVIES")
                {
                    ProcessOutput = "Movies Joined";
                    bool success = await CreateActualMovieFromPath(mpegSupport.OutputVideoPath.Replace('"', ' ').Trim(), null, null);
                    //else MissingInfo = "Not created";
                    if (success)
                    {
                        MovieJoined = true;
                        // need to show the buttons that temp file exists
                        HasTemp = true;
                        TempFileName = e.MovieName.Replace('"', ' ').Trim();
                    }
                }
            }
            else if (e.TaskName == "CONVERT")
            {
                ProcessOutput = "Movie Converted";
                if (!string.IsNullOrEmpty(e.MovieName)) ProcessOutput += " -- " + e.MovieName;
                IsConverted = true;
                HasTemp = true;
                TempFileName = e.MovieName;
            }
            else if (e.TaskName == "TRIM")
            {
                ProcessOutput = "Movie Trimmed";
                IsConverted = false;
            }
            if (e.TaskName == "Clear")
            {
                if (CurrentMovie != null)
                {
                    this.CurrentMovie.HasChapters = false;
                    this.CurrentMovie.Save();
                    this.Progress = "Removing Chapters Completed";

                }
            }
            if (e.TaskName == "SetChapters")
            {
                if (CurrentMovie != null)
                {
                    this.CurrentMovie.HasChapters = true;
                    this.CurrentMovie.Save();

                    this.Progress = "Set Chapters Completed";
                    if (!string.IsNullOrEmpty(this.CurrentMovie.ErrorText)) this.Progress += " Error: " + this.CurrentMovie.ErrorText;
                }
            }
            else
            {
            }
            ProgressPercent = 0;
            MovieProgress = 0;
        }

        private void ScrollMissingIntoView()
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// The MpegSupport_CliWrapError.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapErrorEventArgs"/>.</param>
        internal void MpegSupport_CliWrapError(object sender, CliWrapErrorEventArgs e)
        {
            if (e != null)
            {
                if (!string.IsNullOrEmpty(e.ErrorString)) ProcessOutput = e.ErrorString;
            }
        }

        /// <summary>
        /// Handles the CliWrapErrored event of the MpegSupport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CliWrapErrorEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <autogeneratedoc />
        internal void MpegSupport_CliWrapErrored(object sender, CliWrapErrorEventArgs e)
        {
            //throw new NotImplementedException();
            if (e != null)
            {
                string error = e.ErrorString;
                progress = error;
            }
        }

        /// <summary>
        /// The MpegSupport_CliWrapProgress.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CliWrapProgressEventArgs"/>.</param>
        internal void MpegSupport_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        {
            string? temp = e.Progress;
            string? taskName = e.TaskName;
            Progress = "Processing : " + taskName;

            if (!string.IsNullOrEmpty(temp))
            {
                if (e.ProgressPercentage == 0)
                {
                    ProcessOutput = temp;
                    MissingInfo = temp;
                }
                else
                {
                    ProcessOutput = temp + " - " + e.ProgressPercentage.ToString() + "%";
                    MissingInfo = ProcessOutput;
                    ProgressPercent = e.ProgressPercentage;
                    MovieProgress = e.ProgressPercentage;
                }
            }
        }

        /// <summary>
        /// The PlayClick.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        internal async void PlayClick1(string parameters = "")
        {
            Support.Support.GenerateInfoAndLogMessage("PlayExternal", "Movie", 0, "Preparing");

            int exitCode = -1;
            try
            {
                if (CurrentMovie != null)
                {
                    if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

                    mainWindow.WindowState = WindowState.Minimized;

                    Support.Support.GenerateInfoAndLogMessage("PlayExternal", "Movie", CurrentMovie.Id, "Preparing");
                    // LibVlcPlayer
                    // AaVLCPlayer
                    string? appPath = Support.Support.GetApplicationPathFromDB("LibVlcPlayer");

                    try
                    {
                        if (appPath != null)
                        {
                            Support.Support.GenerateInfoAndLogMessage("PlayExternal", "Movie", CurrentMovie.Id, "About to execute");

                            // Add volume parameter
                            parameters += " VOLUME=" + Volume.ToString("###").Trim();

                            // Add screen width paramater
                            parameters += " SCREENWIDTH=" + ScreenWidth.ToString("###").Trim();

                            var cmd = Cli.Wrap(appPath).WithArguments(parameters);
                            //.ExecuteBufferedAsync();

                            // loop through responses
                            await foreach (var cmdEvent in cmd.ListenAsync())
                            {
                                switch (cmdEvent)
                                {
                                    case StartedCommandEvent started:
                                        Console.WriteLine($"Process started; ID: {started.ProcessId}");
                                        Support.Support.GenerateInfoAndLogMessage("PlayExternal", "Movie", CurrentMovie.Id, "started");
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
                                            DoReloadBookmarks();

                                            CurrentBookmark = CurrentMovie.Bookmarks.Last();
                                            // need to check current bookmark has an image
                                            if (CurrentBookmark.ImagePath != null && !File.Exists(CurrentBookmark.ImagePath))
                                            {
                                                // get the image using the bookmark time
                                                // if the image does not exist, then grab image
                                                // await Support.VideoSupport.GrabBookmarkImage(CurrentMovie, CurrentBookmark, 0);

                                                //CurrentBookmark = bookmark;
                                                //bookmark.ImagePath = string.Empty;
                                                CurrentBookmark.Save();
                                            }
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
                                                        if (
                                                            CurrentBookmark != null
                                                            && CurrentBookmark.Id == bmId
                                                        )
                                                        {
                                                            CurrentBookmark.ImagePath = path;
                                                            CurrentBookmark.SetImageBMP();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ProcessOutput = output;
                                            Support.Support.GenerateInfoAndLogMessage("PlayExternal", "Movie", CurrentMovie.Id, output);
                                        }

                                        break;

                                    case StandardErrorCommandEvent stdErr:
                                        ProcessOutput = $"Err> {stdErr.Text}";

                                        break;

                                    case ExitedCommandEvent exited:
                                        ProcessOutput =
                                            $"Process exited; Code: " + exited.ExitCode.ToString();
                                        DoReloadBookmarks();
                                        CurrentMovie.SetPercentUnmarked();

                                        // see if we have a bookmark?
                                        if (CurrentBookmark == null && CurrentMovie != null)
                                        {
                                            CurrentBookmark = CurrentMovie.Bookmarks.LastOrDefault();
                                        }

                                        // check again
                                        if (CurrentBookmark != null)
                                        {
                                            //CurrentBookmark.ImagePath = path;
                                            CurrentBookmark.SetImageBMP();
                                        }

                                        if (mainWindow != null)
                                        {
                                            mainWindow.WindowState = WindowState.Maximized;
                                            mainWindow.Show();
                                        }
                                        break;
                                }
                            }
                        }
                        // exitCode = result.ExitCode;
                    }
                    catch (Exception ex1)
                    {
                        string error = ex1.ToString();
                        Support.Support.Logger.Error(ex1, error);
                        mainWindow.WindowState = WindowState.Maximized;
                    }
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();
                Support.Support.Logger.Error(e, error);
            }
        }

        /// <summary>
        /// Scrolls the missing into view.
        /// </summary>
        /// <autogeneratedoc />
        //internal void ScrollMissingIntoView()
        //{
        //    if (mainWindow == null) mainWindow = Support.Support.GetMainWindow();

        //    DataGrid? missingFiles = mainWindow.MissingFiles;
        //    if (missingFiles != null)
        //    {
        //        missingFiles.ScrollIntoView(CurrentMissingMovie, null);
        //    }
        //}

        /// <summary>
        /// The SetupMpeg.
        /// </summary>
        internal async void SetupMpeg()
        {
            MovieJoinList = MovieList?.Where(u => u.IsSelected && u.MoviePath != null).OrderBy(m => m.MovieName).Select(i => i.MoviePath).ToList();

            MovieJoinIdList = MovieList?.Where(u => u.IsSelected && u.MoviePath != null).OrderBy(m => m.MovieName).Select(i => i.Id).ToList();

            if (MovieJoinList != null && MovieJoinList.Count > 1)
            {
                mpegSupport = new FFMpegSupport();

                string? firstVideo = MovieJoinList.FirstOrDefault();

                string? firstVideoName = Path.GetFileNameWithoutExtension(firstVideo);

                string? extn = Path.GetExtension(firstVideo);

                mpegSupport.OutputVideoPath = '"' + Path.GetDirectoryName(firstVideo) + "\\" + firstVideoName + "temp" + extn + '"';

                mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;
                mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                mpegSupport.CliWrapError += MpegSupport_CliWrapErrored;

                string file = @"K:\TD1\White\Download\input.txt";
                StreamWriter streamWriter = new StreamWriter(file);

                int totalDuration = 0;

                Support.FFProbeInfo? info = null;

                foreach (string item in MovieJoinList)
                {
                    // need to find file duration

                    info = await FFMpegSupport.GetFFProbeInfo(item);

                    if (info != null && !string.IsNullOrEmpty(info.Duration))
                    {
                        totalDuration += int.Parse(info.Duration);
                    }
                    //int time = await Support.VideoSupport.GetDurationSeconds(item, null);


                    string line = ("file '" + item.Replace("\\", "/") + "'");
                    streamWriter.WriteLine(line);
                }
                //streamWriter.WriteLine("file '" + firstVideoFilePath.Replace("\\","/") + "'");
                //streamWriter.WriteLine("file '" + secondVideoFilePath.Replace("\\", "/") + "'");
                streamWriter.Flush();
                streamWriter.Close();
                string strParam = "  -fflags +genpts -f concat -safe 0 -i " + file + " -c copy " + mpegSupport.OutputVideoPath + " -y";

                mpegSupport.TotalDuration = totalDuration;
                //FFMpegSupport.do

                //mpegSupport.ConversionComplete += FilesJoined;
                mpegSupport.JoinProcess(strParam, "JOINMOVIES");
            }
        }

        /// <summary>
        /// Setups the sort orders.
        /// </summary>
        /// <autogeneratedoc />
        internal void SetupSortOrders()
        {
            string sorts = DataController.MovieProperties.SortColumns;

            if (!string.IsNullOrEmpty(sorts) && (SortOrders == null || SortOrders.Count == 0))
            {
                SortOrders = new ObservableCollection<string>(sorts.Split(',').ToList());
            }
            else if (SortOrders == null || SortOrders.Count == 0)
            {
                SortOrders = new ObservableCollection<string>();

                for (int i = 0; i < 10; i++)
                {
                    SortOrders.Add("");
                }
            }
        }

        /// <summary>
        /// Handles the ActionCompleted event of the Support control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MovieCompletedEventArgs" /> instance containing the event data.</param>
        internal void Support_ActionCompleted(object sender, MovieCompletedEventArgs e)
        {
            Exception? ex = e.Error;

            if (ex != null)
            {
                MissingInfo = "Not created " + ex.ToString();
            }
            else
            {
                MissingInfo = "created ";
                if (e.Movie != null)
                {
                    MissingInfo += e.Movie.MovieName + " : " + e.MovieId.ToString();
                }
            }
        }

        internal void Support_ProgressInformation(object sender, MovieProgressEventargs e)
        {
            MissingInfo = e.Info;
            ProgressPercent = e.ProgressPercentage;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// The DeleteFile.
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        //private static void SetHeaderText(string progress, PhraseEntry? currentPhrase)
        //{
        //    MainWindow? mainWindow = Support.Support.GetMainWindow();

        //    if (mainWindow != null)
        //    {
        //        // look for MainWindowHeaderControl in mainWindow
        //        TaymadeControls.HeaderControl2? headerControl = mainWindow.MainWindowHeaderControl;

        //        // check not null
        //        if (headerControl != null)
        //        {
        //            if (!string.IsNullOrEmpty(progress))
        //            {
        //                if (currentPhrase == null)
        //                    headerControl.Title = "Movies Loading ... " + progress;
        //                else
        //                    headerControl.Title = "Movies Loading ... " + currentPhrase.Description + " " + progress;
        //            }
        //            else
        //                headerControl.Title = "Movies";
        //        }
        //    }
        //}

        private void BackgroundMovieGetter_DataGetCompleted(object sender, DataGetCompletedEventArgs e)
        {
            if (e != null)
            {
                // SetHeaderText("", null);
                MovieList = e.Result;
                if (MovieList != null)
                {
                    CurrentMovie = MovieList.Where(x => x.Id ==
                       DataController.MovieProperties.LastMoveID).FirstOrDefault();

                    if (CurrentMovie != null)
                    {
                        MoviePath = CurrentMovie.MoviePath;
                        int pos = MovieList.IndexOf(CurrentMovie);
                        if (pos >= 0) MoveToSelectedRow(pos);
                    }
                }
            }
        }

        private void BackgroundMovieGetter_DataGetProgress(object sender, DataGetProgressEventArgs e)
        {
            // display information in movieListheader

            // check not null
            SetHeaderText(e.Progress, CurrentPhrase);
        }

        private void SetHeaderText(string? progress, PhraseEntry? currentPhrase)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// The Do_AddBookmark.
        /// </summary>
        private async void Do_AddBookmark()
        {
            MainWindow? main = GetWindow() as MainWindow;
            if (main != null)
            {
                MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (vm != null && CurrentMovie != null && CurrentMovie.Bookmarks != null)
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
            }
        }

        /// <summary>
        /// The Do_AddCastMember.
        /// </summary>
        private async void Do_AddCastMember()
        {
            MainWindow? main = GetWindow() as MainWindow;
            if (main != null)
            {
                MovieEditViewModel? vm = main.DataContext as MovieEditViewModel;

                if (vm != null && CurrentMovie != null)
                {
                    Cast tempCastMember = new Cast();
                    tempCastMember.MovieID = CurrentMovie.Id;
                    // tempCastMember.Actor = new Actor();

                    // search for actor in The Movie Database

                    ActorSearchModel castModel = new ActorSearchModel(tempCastMember.Actor);

                    Dialogs.TMDBActorSearchDialog searchDialog = new Dialogs.TMDBActorSearchDialog(
                        castModel
                    );

                    //castModel.Caller = searchDialog;

                    bool result = await searchDialog.ShowDialog<bool>(main);

                    if (result != null)
                    {
                        Person? found = castModel.FoundPerson;

                        if (found != null)  // not  cancelled or none selected
                        {
                            Actor? actor = DataController.ActorList
                                .Where(x => x.Name.ToLower() == found.Name.ToLower())
                                .FirstOrDefault();

                            if (actor != null) // we already know about this actor
                            {
                                Cast? castmember =
                                    actor.Casts
                                        .Where(x => x.MovieID == CurrentMovie.Id)
                                        .FirstOrDefault() as Cast;

                                if (castmember == null)
                                {
                                    actor.SetDetailsFromPerson(found);

                                    tempCastMember.ActorId = actor.Id;
                                    //tempCastMember.Actor = actor;

                                    tempCastMember.MovieID = CurrentMovie.Id;
                                    await tempCastMember.InsertAsync();
                                    tempCastMember.Actor = actor;
                                    tempCastMember.Movies = CurrentMovie;
                                    CurrentMovie.Casts.Add(tempCastMember);
                                }
                            }
                            else
                            {
                                actor = new Actor();
                                actor.Name = found.Name;
                                if (actor.Id == 0) actor.Insert();

                                actor.SetDetailsFromPerson(found);
                                //tempCastMember.Actor = actor;
                                DataController.ActorList.Add(actor);
                                tempCastMember.ActorId = actor.Id;
                                tempCastMember.MovieID = CurrentMovie.Id;
                                await tempCastMember.InsertAsync();
                                tempCastMember.Actor = actor;
                                tempCastMember.Movies = CurrentMovie;
                                CurrentMovie.Casts.Add(tempCastMember);
                            }
                        }
                        else if (castModel.SelectedActor != null)
                        {
                            tempCastMember.ActorId = castModel.SelectedActor.Id;
                            tempCastMember.MovieID = CurrentMovie.Id;
                            await tempCastMember.InsertAsync();
                            tempCastMember.Actor = castModel.SelectedActor;
                            tempCastMember.Movies = CurrentMovie;
                            CurrentMovie.Casts.Add(tempCastMember);
                        }
                        else if (!string.IsNullOrEmpty(castModel.FindText))
                        {
                            // we can't find the person in Internet database

                            Actor? actor = DataController.ActorController.GetActorByName(castModel.FindText.ToLower());

                            //Actor? actor = DataController.SandboxEntities.Actors
                            //    .AsNoTracking()
                            //    .Where(x => x.Name.ToLower() == castModel.FindText.ToLower())
                            //    .FirstOrDefault();

                            if (actor == null)  // found  new actor
                            {
                                actor = new Actor();
                                actor.Name = castModel.FindText;
                                actor.Insert();  // save entity
                                                 //tempCastMember.Actor = actor;
                                tempCastMember.ActorId = actor.Id;
                                tempCastMember.MovieID = CurrentMovie.Id;
                                await tempCastMember.InsertAsync();
                                tempCastMember.Actor = actor;
                                tempCastMember.Movies = CurrentMovie;
                            }

                            CurrentMovie.Casts.Add(tempCastMember);
                        }

                        // search grid and reload

                        //UserControl? movieCastControl = main.MovieCast;

                        //if (movieCastControl != null)
                        //{
                        //DataGrid? dgCast = main.MovieCast.dgCast;
                        //if (dgCast != null)
                        //    dgCast.ItemsSource = CurrentMovie.Casts;
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// The Do_AddPoster.
        /// </summary>
        private async void Do_AddPoster()
        {
            MainWindow? main = GetWindow() as MainWindow;
            if (main != null)
            {
                MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (vm != null && CurrentMovie != null)
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
            }
        }

        /// <summary>
        /// The Do_PlayFromLast.
        /// </summary>
        private void Do_PlayFromLast()
        {
            MainWindow? main = GetWindow() as MainWindow;

            if (main != null)
            {
                MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (vm != null && currentMovie != null && CurrentMovie.Bookmarks.Count > 0)
                {
                    CurrentBookmark = currentMovie.Bookmarks.Last();
                    string moviePath = currentMovie.MoviePath;

                    if (!string.IsNullOrEmpty(moviePath))
                    {
                        FFMpegSupport.PlayMovie(moviePath, CurrentBookmark);
                    }
                }
            }
        }

        /// <summary>
        /// The DoDeleteBookmark.
        /// </summary>
        private void DoDeleteBookmark()
        {
            MainWindow? main = GetWindow() as MainWindow;
            if (main != null)
            {

                if (CurrentBookmark != null && CurrentMovie != null)
                {
                    if (!string.IsNullOrEmpty(CurrentBookmark.ImagePath))
                    {
                        // image file to delete

                        string imagePath = Support.Support.FixImagePath(CurrentBookmark.ImagePath);

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    int ind = CurrentMovie.Bookmarks.ToList().IndexOf(CurrentBookmark);
                    CurrentMovie.Bookmarks.Remove(CurrentBookmark);

                    CurrentBookmark.Delete();

                    if (CurrentMovie.Bookmarks.Count > 0)
                    {
                        CurrentBookmark = CurrentMovie.Bookmarks.ToList()[ind];

                        //DataGrid dgb = main.MovieBookmarks.dgBooks;
                        //if (dgb != null)
                        //{
                        //    dgb.ItemsSource = new ObservableCollection<Bookmark>(
                        //        CurrentMovie.Bookmarks
                        //    );
                        //    dgb.SelectedItem = CurrentBookmark;
                        //}
                    }
                    else
                    {
                        CurrentBookmark = null;
                    }
                }
            }
        }

        /// <summary>
        /// The DoEditBookmark.
        /// </summary>
        private async void DoEditBookmark()
        {
            await EditTheBookmark();
        }

        private async Task<string?> DoGetClipboardTextAsync()
        {
            Window? main = Support.Support.GetMainWindow();

            string? returnString = string.Empty;

            if (main != null)
            {
                var clipboard = main.Clipboard;
                returnString = await clipboard.TryGetTextAsync();
            }

            return returnString;  // provider.GetTextAsync();
        }

        /// <summary>
        /// The DoPlayBookmark.
        /// </summary>
        private void DoPlayBookmark()
        {
            MainWindow? main = GetWindow() as MainWindow;

            if (main != null)
            {
                MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (vm != null && CurrentBookmark != null && currentMovie != null)
                {
                    string moviePath = currentMovie.MoviePath;

                    if (!string.IsNullOrEmpty(moviePath))
                    {
                        FFMpegSupport.PlayMovie(moviePath, CurrentBookmark);
                    }
                }
            }
        }

        /// <summary>
        /// The DoRepeatLast.
        /// </summary>
        private async void DoRepeatLast()
        {
            MainWindow? main = GetWindow() as MainWindow;

            if (main != null)
            {
                MainWindowViewModel? vm = main.DataContext as MainWindowViewModel;

                if (vm != null && currentMovie != null && CurrentMovie.Bookmarks.Count > 0)
                {
                    CurrentBookmark = currentMovie.Bookmarks.Last();
                    string moviePath = currentMovie.MoviePath;

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
                        //      await AddActualBookmark(main, vm, bookmark);
                    }
                }
            }
        }

        private async Task DoSetClipboardTextAsync(string? text)
        {
            // For learning purposes, we opted to directly get the reference
            // for StorageProvider APIs here inside the ViewModel.

            // For your real-world apps, you should follow the MVVM principles
            // by making service classes and locating them with DI/IoC.

            // See DepInject project for a sample of how to accomplish this.
            //if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            //    desktop.MainWindow?.Clipboard is not { } provider)
            //    throw new NullReferenceException("Missing Clipboard instance.");

            Window? main = Support.Support.GetMainWindow();

            if (main != null)
            {
                var clipboard = main.Clipboard;
                var data = new DataTransfer();
                data.Add(DataTransferItem.CreateText(text));
                await clipboard.SetDataAsync(data);
            }
        }

        private async Task FindMovieNewList(string findValue)
        {
            Window tempWindow = Caller;

            if (tempWindow == null) tempWindow = Support.Support.GetMainWindow();

            MovieViewModel viewModel = new MovieViewModel();

            ObservableCollection<Movies>? oldMovieList = viewModel.MovieList;
            Movies? oldCurrentMovie = viewModel.CurrentMovie;

            List<Movies> tempList = DataController.SandboxEntities.Movies
                    .Where(x => x.MovieName.ToLower().Contains(findValue.ToLower()))
                    .OrderBy(x => x.MovieName)
                    .ToList();
            ObservableCollection<Movies> movieList = new ObservableCollection<Movies>(tempList);
            movieList = MovieCollection.GetAndSortObservableCollection(tempList, false);

            viewModel.MovieList = new ObservableCollection<Movies>(tempList); ;
            //viewModel.CurrentMovie = FoundMovie;

            //MovieListDialog movieListDialog = new MovieListDialog(viewModel);
            //viewModel.Caller = movieListDialog;
            //this.Caller = movieListDialog;
            ////OldMovieList = MovieList;

            //// show on main window
            //await movieListDialog.ShowDialog(tempWindow);

            //// restore caller
            //this.Caller = tempWindow;
        }

        private async Task FindMovieNewListBookmarks(bool noDialog = false)
        {
            Window tempWindow = Caller;

            if (tempWindow == null) tempWindow = Support.Support.GetMainWindow();

            MovieViewModel viewModel = new MovieViewModel();

            ObservableCollection<Movies>? oldMovieList = viewModel.MovieList;
            Movies? oldCurrentMovie = viewModel.CurrentMovie;
            ObservableCollection<Movies> tempList = GetMatchingBookmarksFromDB();

            if (!noDialog)
            {
                viewModel.MovieList = new ObservableCollection<Movies>(tempList);
                //viewModel.CurrentMovie = FoundMovie;

                //MovieListDialog movieListDialog = new MovieListDialog(viewModel);
                //viewModel.Caller = movieListDialog;
                //this.Caller = movieListDialog;
                ////OldMovieList = MovieList;

                //// show on main window
                //await movieListDialog.ShowDialog(tempWindow);

                //// restore caller
                //this.Caller = tempWindow;
            }
            else
            {
                MovieList = new ObservableCollection<Movies>(tempList);
            }
        }

        private async Task FindMovieNewListInfo()
        {
            Window tempWindow = Caller;

            if (tempWindow == null) tempWindow = Support.Support.GetMainWindow();

            MovieViewModel viewModel = new MovieViewModel();

            ObservableCollection<Movies>? oldMovieList = viewModel.MovieList;
            Movies? oldCurrentMovie = viewModel.CurrentMovie;

            ObservableCollection<Movies> tempList = GetMatchingInfoFromDB();

            viewModel.MovieList = new ObservableCollection<Movies>(tempList); ;
            //viewModel.CurrentMovie = FoundMovie;

            //MovieListDialog movieListDialog = new MovieListDialog(viewModel);
            //viewModel.Caller = movieListDialog;
            //this.Caller = movieListDialog;
            ////OldMovieList = MovieList;

            //// show on main window
            //await movieListDialog.ShowDialog(tempWindow);

            //// restore caller
            //this.Caller = tempWindow;
        }

        /// <summary>
        /// Gets from TMDB.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <autogeneratedoc />
        private async void GetFromTMDB(Movies movie, int ID, bool getCast = false)
        {
            Support.iMovie iMovie = await Support.TmdbSupport.GetMovieData(ID);

            if (iMovie != null)
            {
                if (iMovie.ProductionCompanies != null && iMovie.ProductionCompanies.Count > 0)
                {
                    foreach (var item in iMovie.ProductionCompanies)
                    {
                        Models.ProductionCompany? pc = DataController.ProductionCompanies.Where(p => p.TMDBID == item.Id).FirstOrDefault();

                        if (pc == null)
                        {
                            pc = new Models.ProductionCompany()
                            {
                                CompanyName = item.Name,
                                TMDBID = item.Id
                            };

                            DataController.SandboxEntities.ProductionCompany.Add(pc);
                            DataController.SandboxEntities.SaveChanges();

                            DataController.ProductionCompanies.Add(pc);
                            movie.ProductionCompanies.Add(pc);
                        }

                        Models.ProductionCompanyMovie? productionCompanyMovie = DataController.SandboxEntities.ProductionCompanyMovie.Where(p => p.MovieId == movie.Id && p.CompanyId == pc.Id).FirstOrDefault();

                        if (productionCompanyMovie == null)
                        {
                            productionCompanyMovie = new Models.ProductionCompanyMovie()
                            {
                                MovieId = movie.Id,
                                CompanyId = pc.Id
                            };

                            DataController.SandboxEntities.ProductionCompanyMovie.Add(productionCompanyMovie);
                            DataController.SandboxEntities.SaveChanges();
                        }
                    }
                    DataController.ProductionCompanies = DataController.SandboxEntities.ProductionCompany.ToList();
                }

                // deal with languages
                if (iMovie.Languages != null && iMovie.Languages.Count > 0)
                {
                    foreach (var language in iMovie.Languages)
                    {
                        MovieLanguage? movieLanguage = movie.MovieLanguages.Where(l => l.Iso_639_1 == language.Iso_639_1).FirstOrDefault();
                        if (movieLanguage == null)
                        {
                            movieLanguage = new MovieLanguage()
                            {
                                MovieId = movie.Id,
                                Iso_639_1 = language.Iso_639_1,
                                LanguageName = language.Name
                            };

                            DataController.SandboxEntities.MovieLanguage.Add(movieLanguage);
                            DataController.SandboxEntities.SaveChanges();

                            movie.MovieLanguages.Add(movieLanguage);
                        }
                    }
                }

                //if (getCast)
                //    GetCastData(movie, iMovie);
            }
        }

        private ObservableCollection<Movies> GetMatchingBookmarksFromDB()
        {
            List<Movies> tempList = DataController.SandboxEntities.GetMoviesbyBookmarkName(FindBookmarkText);
            movieList = new ObservableCollection<Movies>(tempList);
            movieList = MovieCollection.GetAndSortObservableCollection(tempList, false);

            foreach (Movies item in movieList)
            {
                // item.FixMovieData();

                if (item.Dirty) item.Save();
            }

            FindBookmarkText = string.Empty;

            return movieList;
        }

        private ObservableCollection<Movies> GetMatchingInfoFromDB()
        {
            List<Movies>? tempList = DataController.MovieController.GetMoviesByInfo(FindText);
            if (tempList != null)
            {
                movieList = new ObservableCollection<Movies>(tempList);
                movieList = MovieCollection.GetAndSortObservableCollection(tempList, false);

                foreach (Movies item in movieList)
                {
                    //item.FixMovieData();

                    if (item.Dirty) item.Save();
                }
            }
            FindBookmarkText = string.Empty;

            return movieList;
        }

        /// <summary>
        /// Gets the TMDB details local.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <autogeneratedoc />
        private void GetTMDBDetailsLocal(MovieViewModel viewModel)
        {
            if (
                string.IsNullOrEmpty(CurrentMovie.IMDBID)
                && !string.IsNullOrEmpty(viewModel.FoundMovie.IMDBID)
            )
            {
                CurrentMovie.IMDBID = viewModel.FoundMovie.IMDBID;
            }

            if (
                CurrentMovie.Year == null
                && !string.IsNullOrEmpty(viewModel.FoundMovie.Year)
            )
            {
                if (
                    int.TryParse(
                        viewModel.FoundMovie.Year.Substring(0, 4),
                        out int year
                    )
                )
                    CurrentMovie.Year = year;
            }
            if (CurrentMovie.TMDBID == null || CurrentMovie.TMDBID < 1)
                CurrentMovie.TMDBID = viewModel.FoundMovie.ID;
            if (
                string.IsNullOrEmpty(CurrentMovie.Info)
                && !string.IsNullOrEmpty(viewModel.FoundMovie.Overview)
            )
                CurrentMovie.Info = viewModel.FoundMovie.Overview;
            //get more data
            viewModel.GetCast(viewModel.FoundMovie, CurrentMovie);
            GetFromTMDB(CurrentMovie, viewModel.FoundMovie.ID);
        }

        //                // save movie
        //                this.Movie.Save();
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// The MovieSetup.
        /// </summary>
        /// <param name="movie">The movie<see cref="Movies"/>.</param>
        private void MovieSetup(Movies movie)
        {
            if (movie != null)
            {
                movie.FixMovieData();

                this.Phrases = new ObservableCollection<PhraseEntry>(DataController.PhraseEntries);

                this.CurrentSeries = movie.SeriesEntity;

                if (this.CurrentSeries != null)
                {
                    if (movie.Series != null && movie.Series != 2)
                    {
                        if (this.CurrentSeries.Id != movie.Series) this.CurrentSeries = Models.DataController.SeriesList.Find(s => s.Id == movie.Series);
                    }

                    if (movie.Season != null && this.CurrentSeries!.Seasons != null)
                    {
                        int seasonCount = this.CurrentSeries.Seasons.Count;
                        this.CurrentSeason = this.CurrentSeries.Seasons.Where(se => se.Id == movie.Season).FirstOrDefault();

                        if (this.CurrentSeason != null)
                        {
                            if (this.Episode == null)
                            {
                                int temp = this.CurrentSeason.TVEpisodes.Count;
                                if (this.CurrentSeason.TVEpisodes.Count == 0)
                                {
                                    this.CurrentSeason.TVEpisodes = new ObservableCollection<TVEpisode>(Models.DataController.SandboxEntities.TVEpisodes.Where(te => te.SeasonID == this.CurrentSeason.Id).ToList());

                                    this.Episode = this.CurrentSeason.TVEpisodes.Where(e => e.Id == movie.Episode).FirstOrDefault();
                                }
                                else
                                {
                                    this.Episode = this.CurrentSeason.TVEpisodes.Where(e => e.Id == movie.Episode).FirstOrDefault();
                                }
                            }
                            this.EpisodeList = this.CurrentSeason.TVEpisodes;
                        }
                    }

                    if (movie.Nfo == null)
                    {
                    }
                }
            }
        }

        private async void SearchForMovie()
        {
            //SearchMoviesDialog search = new SearchMoviesDialog();

            //if (this.Caller == null) this.Caller = Support.Support.GetMainWindow();

            //this.Caller = Support.Support.GetMainWindow();

            //// will need to create new viewModel for searches

            //SearchViewModel searchModel = new SearchViewModel();

            //search.DataContext = searchModel;

            //oldCaller = this.Caller;

            //Caller = search;

            //// will fire up a dialog to do a detailed search on a movie
            //await search.ShowDialog(oldCaller);
            //if (
            //        searchModel.resultButton != null
            //        && searchModel.resultButton.Result == Models.DialogResultButton.ResultType.Ok
            //    )
            //{
            //    // do Actual search search criteria is in viewmodel
            //    List<Movies> tempList = new List<Movies>();
            //    // if we are looking for an individual movie
            //    if (searchModel.MovieId != null)
            //    {
            //        Movies? temp = DataController.SandboxEntities.Movies.Find(searchModel.MovieId);
            //        if (temp != null)
            //        {
            //            tempList.Add(temp);
            //        }
            //    }
            //    else
            //        if (!string.IsNullOrEmpty(searchModel.MovieTitle))
            //        {
            //            // see if we can find some movies
            //            tempList = DataController.SandboxEntities.GetMoviesbyTitle(searchModel.MovieTitle);
            //        }

            //    // look for actors
            //    if (!string.IsNullOrEmpty(searchModel.ActorName))
            //    {
            //        // two possible situations we have a list already or we don't
            //        List<MovieIntResult> ActorMovies = DataController.SandboxEntities.GetActorMovieIds(searchModel.ActorName);
            //        // this should have returned a list of names

            //        // Extract the Ids from ActorMovies for comparison
            //        var actorMovieIds = MovieIntResult.GetMovieIds(ActorMovies);

            //        if (tempList == null || tempList.Count == 0)
            //        {
            //            var result = from x in DataController.SandboxEntities.Movies
            //                         where actorMovieIds.Contains(x.Id)
            //                         select x;
            //            // should be a list of movies
            //            tempList = result.ToList();
            //        }
            //        else
            //        {
            //            var result = from x in tempList
            //                         where actorMovieIds.Contains(x.Id)
            //                         select x;
            //            // should be a list of movies
            //            tempList = result.ToList();
            //        }
            //    }

            //    // need to add bookmark text and with info
            //    // yet to decide whether should be cumulative or standalone
            //    // leaning towards standalone

            //    if (!string.IsNullOrEmpty(searchModel.BookmarkText))
            //    {
            //        tempList = DataController.SandboxEntities.GetMoviesbyBookmarkName(searchModel.BookmarkText);
            //    }

            //    if (!string.IsNullOrEmpty(searchModel.InfoText))
            //    {
            //        tempList = DataController.SandboxEntities.GetMoviesbyInfo(searchModel.InfoText);
            //    }

            //    if (searchModel.CurrentSeries != null)
            //    {
            //        tempList = DataController.SandboxEntities.Movies.Where(m => m.Series == searchModel.CurrentSeries.Id).ToList();
            //    }

            //    this.MovieList = new ObservableCollection<Movies>(tempList);
            //}
        }

        #endregion Private Methods

        /// <summary>
        /// The DoSearch.
        /// </summary>
        //private async void DoSearch()
        //{
        //    //Views.MainWindow mainWindow = GetWindow() as Views.MainWindow;

        //    if (this.Movie != null && !string.IsNullOrEmpty(this.Movie.MovieName))
        //    {
        //        Dialogs.TMDBSearchDialog searchDialog = new Dialogs.TMDBSearchDialog(this);

        //        await searchDialog.ShowDialog(this.Caller);

        //        this.Movie.LogMessage("Searched");

        //        if (this.resultButton != null && this.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok)
        //        {
        //            if (this.FoundMovie != null)
        //            {
        //                if (string.IsNullOrEmpty(this.Movie.IMDBID) && !string.IsNullOrEmpty(this.FoundMovie.IMDBID))
        //                {
        //                    this.Movie.IMDBID = this.FoundMovie.IMDBID;
        //                }

        //                if ((this.Movie.Year == null || this.Movie.Year == 0) && !string.IsNullOrEmpty(this.FoundMovie.Year))
        //                {
        //                    if (int.TryParse(this.FoundMovie.Year.Substring(0, 4), out int year)) this.Movie.Year = year;
        //                }

        //                if (this.Movie.TMDBID == null || this.Movie.TMDBID < 1) this.Movie.TMDBID = this.FoundMovie.ID;

        //                if (string.IsNullOrEmpty(this.Movie.Info) && !string.IsNullOrEmpty(this.FoundMovie.Overview)) this.Movie.Info = this.FoundMovie.Overview;

        //                //get more data

        //                this.GetCast(this.FoundMovie, this.Movie);

        //                if (this.Movie.Nfo != null)
        //                {
        //                    this.Movie.Nfo.SetValuesFromMovie(this.Movie);
        //                    this.Movie.Nfo.Save();
        //                }
        /// <summary>
        /// The DoChapters.
        /// </summary>

    }
}