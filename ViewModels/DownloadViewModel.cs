//-----------------------------------------------------------------------
// <copyright file="DownloadViewModel.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>12/05/2022 12:39:23 12/05/2022 12:39:23 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.ViewModels
{
    using Avalonia.Controls;
    using TaymadeEntities.Models;
    using TaymadeEntities.Support;
    using TaymadeEntities.Views;
    using DynamicData;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reactive;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MsBox.Avalonia;
    using MsBox.Avalonia.Enums;
    using TaymadeEntities.Dialogs;
    using FileSupport;

    /// <summary>
    /// Defines the <see cref="DownloadViewModel" />.
    /// </summary>
    public class DownloadViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the SortDirection.
        /// </summary>
        public int SortDirection = -1;

        /// <summary>
        /// Defines the currentMovie.
        /// </summary>
        private Movies currentMovie;

        private UnboundGridData? currentRow;

        private Movies? foundMovie;

        /// <summary>
        /// Defines the foundMovieList.
        /// </summary>
        private ObservableCollection<Movies>? foundMovieList;

        private bool hasNewMovie;

        /// <summary>
        /// Defines the hasTemp.
        /// </summary>
        private bool hasTemp = false;

        /// <summary>
        /// Defines the headerColumn.
        /// </summary>
        private string? headerColumn = "Name";

        private bool isSelected;

        /// <summary>
        /// Defines the JoinListCombo.
        /// </summary>
        private ComboBox? JoinListCombo = null;

        /// <summary>
        /// Defines the movieJoinList.
        /// </summary>
        private List<string>? movieJoinList = new();

        /// <summary>
        /// Defines the mpegSupport.
        /// </summary>
        private FFMpegSupport? mpegSupport;

        /// <summary>
        /// Defines the progress.
        /// </summary>
        private string progress;

        private ProgramInformation? procInfo = new ProgramInformation()
        {
            Information = "Info ... "
        };

        /// <summary>
        /// Defines the progressPercent.
        /// </summary>
        private int progressPercent;

        public Window Caller { get; set; }

        /// <summary>
        /// Defines the unbounds.
        /// </summary>
        private ObservableCollection<UnboundGridData>? unbounds;
        private PhraseEntry currentPhrase;
        private PhraseEntry currentSubPhrase;
        private List<PhraseEntry> phraseList;
        private List<PhraseEntry> subPhraseList;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadViewModel"/> class.
        /// </summary>
        public DownloadViewModel()
        {
            DownloadSupport instance = DownloadSupport.GetInstance();
            ProcessRoot();

            UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;
            if (unboundGridDatas != null)
                Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);
            else Unbounds = new ObservableCollection<UnboundGridData>(Unbounds);
            SetUpCommands();

            this.SetupProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadViewModel"/> class.
        /// </summary>
        /// <param name="window">The window<see cref="DownloadedFiles"/>.</param>
        //public DownloadViewModel(DownloadedFiles window) : base()
        //{
        //    Caller = window;
        //    SetUpCommands();
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadViewModel"/> class.
        /// </summary>
        /// <param name="unboundData">The unboundData<see cref="Support.UnboundGridDataCollection"/>.</param>
        public DownloadViewModel(UnboundGridDataCollection unboundData) : base()
        {
            SetUpCommands();

            Unbounds = new ObservableCollection<UnboundGridData>(unboundData);

            this.SetupProperties();
        }

        private void SetupProperties()
        {
            DownProperties = DataController.GetDownloadProperties();
            if (DownProperties != null)
            {
                if (DownProperties.SortDirection != null)
                {
                    SortDirection = DownProperties.SortDirection.Value;
                }
                if (DownProperties.SortedColumn != null)
                {
                    if (DownProperties.SortedColumn == 0) HeaderColumn = "Name";
                    else if (DownProperties.SortedColumn == 1) HeaderColumn = "Creation Time";
                    else if (DownProperties.SortedColumn == 2) HeaderColumn = "File Length";
                }
            }
            else
            {
                // set headercolum and sort direction HeaderColumn = "Creation Time" && SortDirection = -1
                HeaderColumn = "Creation Time";
                SortDirection = -1;
            }
            SortList();

            //this.MoveToLastItem();
        }

        /// <summary>
        /// Moves to last item.
        /// </summary>
        /// <autogeneratedoc />
        public void MoveToLastItem()
        {
            // if we have downloadproperties and a lastunboundindex then set currentrow to that index
            if (DownProperties != null && DownProperties.LastUnboundIndex != null && Unbounds != null && Unbounds.Count > 0)
            {
                int index = DownProperties.LastUnboundIndex.Value;
                // is the id of the last currentrow
                CurrentRow = Unbounds.Where(u => u.Id == index).LastOrDefault();
                // if unboundgrid is not null then scrolintoview the currentrow
                if (Unbounds != null && CurrentRow != null)
                {
                    // get observablecollection from unboundgrid itemsources
                    ObservableCollection<UnboundGridData> unboundCollection = Unbounds;
                    if (unboundCollection != null)
                    {
                        // get the index of the currentrow in the observablecollection

                        //CurrentRow = unboundCollection.Where(u => u.Id == index).LastOrDefault();
                        UnboundGrid.ScrollIntoView(CurrentRow, null);
                    }
                }
            }
        }

        public ProgramInformation? ProcessInfo { get => procInfo; set => this.RaiseAndSetIfChanged(ref procInfo, value); }

        public void ShowErrors()
        {
        }

        public void LastItem()
        {
            MoveToLastItem();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the AddToList.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? AddToList { get; private set; }

        /// <summary>
        /// Gets the ClearList.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ClearList { get; private set; }

        public ReactiveCommand<Unit, Unit>? EditMovieCommand { get; set; }

        /// <summary>
        /// Gets the create.
        /// </summary>
        /// <value>
        /// The create.
        /// </value>
        /// <autogeneratedoc />
        public ReactiveCommand<Unit, bool>? Create { get; private set; }

        public ReactiveCommand<Unit, Unit>? LastLoadCommand { get; private set; }
        public ReactiveCommand<Unit, Unit>? LastItemCommand { get; private set; }
        public List<Movies> CreatedMovieList { get; set; }

        /// <summary>
        /// Gets the CurrentMovie.
        /// </summary>
        public Movies CurrentMovie
        {
            get => currentMovie;
            set
            {
                this.RaiseAndSetIfChanged(ref currentMovie, value);
            }
        }

        /// <summary>
        /// Gets or sets the CurrentRow.
        /// </summary>
        public UnboundGridData? CurrentRow
        {
            get => currentRow;
            set
            {
                this.RaiseAndSetIfChanged(ref currentRow, value);
                IsSelected = (value != null);
                if (value != null)
                {
                    if (DownProperties != null)
                    {
                        DownProperties.LastUnboundIndex = value.Id;
                        DownProperties.Update();
                    }

                    //if (ProcessInfo != null)
                    //{
                    //    ProcessInfo.Information = value.FileName;
                    //}
                }
            }
        }

        /// <summary>
        /// Gets the delete.
        /// </summary>
        /// <value>
        /// The delete.
        /// </value>
        /// <autogeneratedoc />
        public ReactiveCommand<Unit, Unit>? Delete { get; private set; }

        public ReactiveCommand<Unit, Unit>? DeleteEntry { get; private set; }

        public ReactiveCommand<Unit, Unit> DeleteTemp { get; private set; }
        //public DataGrid DgMovies { get; internal set; }

        public DataGrid UnboundGrid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the DoDuplicates.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? DoDuplicates { get; private set; }

        /// <summary>
        /// Gets the DoRefresh.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? DoRefresh { get; private set; }

        public DownloadProperties DownProperties { get; private set; }

        /// <summary>
        /// Gets the EndFindFile.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? EndFindFile { get; private set; }

        /// <summary>
        /// Gets the FindFile.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? FindFile { get; private set; }

        public Movies? FoundMovie { get => foundMovie; set => this.RaiseAndSetIfChanged(ref foundMovie, value); }

        /// <summary>
        /// Gets or sets the FoundMovieList.
        /// </summary>
        public ObservableCollection<Movies>? FoundMovieList
        {
            get => foundMovieList;
            set => this.RaiseAndSetIfChanged(ref foundMovieList, value);
        }

        /// <summary>
        /// Gets the duration of the get.
        /// </summary>
        /// <value>
        /// The duration of the get.
        /// </value>
        /// <autogeneratedoc />
        public ReactiveCommand<Unit, Unit>? GetDuration { get; private set; }

        public bool HasNewMovie
        {
            get => hasNewMovie;
            set
            {
                this.RaiseAndSetIfChanged(ref hasNewMovie, value);
                if (this.Caller != null && Caller is MainWindow)
                {
                    MainWindow? main = Caller as MainWindow;
                    //  main?.SetHasNewMovie(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether HasTemp.
        /// </summary>
        public bool HasTemp
        {
            get => hasTemp;
            set
            {
                this.RaiseAndSetIfChanged(ref hasTemp, value);
                if (this.Caller != null && Caller is MainWindow)
                {
                    MainWindow? main = Caller as MainWindow;
                    //  main?.SetHasTemp(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the HeaderColumn
        /// Gets the HeaderColumn..
        /// </summary>
        public string? HeaderColumn { get => headerColumn; set => headerColumn = value; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (this.Caller != null && Caller is MainWindow)
                {
                    MainWindow main = Caller as MainWindow;
                    //main.SetIsSelected(value);
                }
                //   DownLoadDialog.SetIsSelected(value);
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        /// <summary>
        /// Gets the JoinList.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? JoinList { get; private set; }

        /// <summary>
        /// Gets the MoveTemp.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? MoveTemp { get; private set; }

        /// <summary>
        /// Gets or sets the MovieJoinList.
        /// </summary>
        public List<string>? MovieJoinList { get => movieJoinList; set => this.RaiseAndSetIfChanged(ref movieJoinList, value); }

        /// <summary>
        /// Gets the Play.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? Play { get; private set; }

        /// <summary>
        /// Gets the PlayTemp.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? PlayTemp { get; private set; }

        /// <summary>
        /// Gets or sets the ProcessOutput.
        /// </summary>
        public string ProcessOutput { get => progress; set => this.RaiseAndSetIfChanged(ref progress, value); }

        /// <summary>
        /// Gets the progress percent..
        /// </summary>
        public int ProgressPercent { get => progressPercent; set => this.RaiseAndSetIfChanged(ref progressPercent, value); }

        /// <summary>
        /// Gets the rename.
        /// </summary>
        /// <value>
        /// The rename.
        /// </value>
        /// <autogeneratedoc />
        public ReactiveCommand<Unit, bool>? Rename { get; private set; }

        /// <summary>
        /// Gets or sets the ResultTask.
        /// </summary>
        public DialogResultButton ResultTask { get; set; }

        public ReactiveCommand<Unit, Unit>? DeleteEntity { get; private set; }
        public ReactiveCommand<Unit, Unit> ShowDetails { get; private set; }

        /// <summary>
        /// Gets the ToMP4.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ToMP4 { get; private set; }

        public ReactiveCommand<Unit, Unit>? ResetTimeStamps { get; private set; }

        /// <summary>
        /// Gets the ToMTS.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? ToMTS { get; private set; }

        /// <summary>
        /// Gets the TrimMovie.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? TrimMovie { get; private set; }

        /// <summary>
        /// Gets or sets the Unbounds.
        /// </summary>
        public ObservableCollection<UnboundGridData>? Unbounds
        {
            get => unbounds;
            set
            {
                this.RaiseAndSetIfChanged(ref unbounds, value);
                if (this.Caller != null)
                {
                    MainWindow? main = this.Caller as MainWindow;
                    //if (main != null && main.UnboundsCount != null && value != null)
                    //{
                    //    main.UnboundsCount.Text = value.Count.ToString();
                }
            }
            //this.RaisePropertyChanged("Unbounds"));
        }


        //public DownloadedFiles DownLoadDialog { get; internal set; }
        public string? FindText { get; set; }
        public PhraseEntry CurrentPhrase
        {
            get => currentPhrase;
            set
            {
                this.RaiseAndSetIfChanged(ref currentPhrase, value);
                if (value != null && value.PhraseID == 1)
                {
                    // set subphrase list to the list of subphrases for this phrase
                    var subPhrases = DataController.SandboxEntities.PhraseEntry.Where(pe => pe.PhraseID == 9 && pe.Id.Contains(value.Id)).ToList();
                    // Do something with the subPhrases, e.g., display them in the UI
                    // You can create a new ObservableCollection for subphrases if needed
                    SubPhraseList = subPhrases;
                    // HasSubPhrases = true;
                }
            }
        }
        public PhraseEntry CurrentSubPhrase
        {
            get => currentSubPhrase;
            set => this.RaiseAndSetIfChanged(ref currentSubPhrase, value);
        }

        public List<PhraseEntry> PhraseList
        {
            get
            {
                if (phraseList == null || phraseList.Count == 0)
                {
                    phraseList = DataController.SandboxEntities.PhraseEntry.Where(p => p.PhraseID == 1).ToList();
                }
                return phraseList;
            }

            internal set => phraseList = value;
        }

        public List<PhraseEntry> SubPhraseList
        {
            get => subPhraseList;
            set => subPhraseList = value;
        }

        #endregion Properties

        #region Methods

        public async Task DoMovieEdit()
        {
            //if (FoundMovie != null)
            //{
            //    await DoMovieEdit(FoundMovie);
            //}
        }

        //public async Task DoMovieEdit(Movies? movie)
        //{
        //    MainWindow? main = Support.GetMainWindow() as MainWindow;

        //    MovieEditViewModel mvm = new(movie);
        //    mvm.CurrentMovie = movie;
        //    Dialogs.MovieEditDialog editor = new(mvm);
        //    editor.DataContext = mvm;
        //    //Window main = GetWindow();
        //    mvm.Caller = editor;
        //    //Dialogs.DialogResultButton result;

        //    editor.OkButtonPanelEditMovie.OkButton.Command = mvm.Accept;
        //    editor.OkButtonPanelEditMovie.CancelButton.Command = mvm.Cancel;

        //    if (main != null)
        //    {
        //        await editor.ShowDialog(main);
        //        if (mvm.resultButton != null && mvm.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok)
        //        {
        //            // save movie
        //            if (mvm.CurrentMovie != null)
        //            {
        //                if (mvm.CurrentMovie.SeriesEntity != null)
        //                {
        //                    if (mvm.CurrentMovie.Series == null)
        //                    {
        //                        mvm.CurrentMovie.Series = mvm.CurrentMovie.SeriesEntity.Id;
        //                    }
        //                }
        //                if (mvm.CurrentMovie.Director != null)
        //                {
        //                    if (mvm.CurrentMovie.DirectorID == null) mvm.CurrentMovie.DirectorID = mvm.CurrentMovie.Director.Id;
        //                }
        //                mvm.CurrentMovie.MoviePath = Support.FixPathBack(mvm.CurrentMovie.MoviePath);
        //                mvm.CurrentMovie.Save();
        //            }
        //            if (mvm.CurrentMovie.SeriesEntity != null)
        //            {
        //                mvm.CurrentMovie.SeriesEntity.Save();
        //            }
        //        }
        //    }
        //}
        //}

        //public void DoShowDetails()
        //{
        //    Views.MainWindow? mainWindow = Support.Support.GetMainWindow() as Views.MainWindow;

        //   // Window temp = Caller;

        //    if (mainWindow != null && FoundMovie != null)
        //    {
        //        MovieViewModel viewModel = new(FoundMovie);
        //        var movieDialog = new Dialogs.MovieDetailDialog(viewModel, false);

        //       // this.Caller = movieDialog;
        //        //movieDialog.DataContext = mvm;
        //        // movieDialog.CurrentMovieModel = mvm;

        //        movieDialog.ShowDialog(mainWindow);

        //    }
        //}

        public void DoTextEdit()
        {
            // get current row and then edit text file using TextPad
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                string file = CurrentRow.FileName;
                if (File.Exists(file))
                {
                    // open text editor
                    Support.OpenTextEditor(file, null);
                }
                else
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Warning", "File does not exist: " + file,
                        ButtonEnum.Ok);
                    box.ShowAsync();
                }
            }
        }

        public async void EditMovie()
        {
            //if (Support.CreatedMovie != null)
            //    await DoMovieEdit(Support.CreatedMovie);
            //else
            //{
            //    var box = MessageBoxManager.GetMessageBoxStandard("Warning", "No Created Movie Found",
            //    ButtonEnum.Ok);

            //    var result = await box.ShowAsync();

            //    // clear out flags
            //    HasNewMovie = false;
            //    Support.CreatedMovie = null;
            //}
        }

        public async void GetIMDBData()
        {
            if (CurrentRow != null && CurrentRow.TIMDB != null)
            {
                //iMovie iMovie = await TmdbSupport.GetMovieData(CurrentRow.TIMDB.Value);
                //if (iMovie != null)
                //{
                //    GetCastData(Support.CreatedMovie, iMovie);
                //    CurrentRow.Overview = iMovie.Overview;
                //    CurrentRow.Year = iMovie.ReleaseDate.Year;

                //    CurrentRow.Save();
                //}
            }
        }

        public void LastLoad()
        {
            // check download list exits if not get it and sort by created time
            //UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;

            //if (unboundGridDatas != null)
            //Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);
            // set headercolum and sort direction HeaderColumn = "Creation Time" && SortDirection = -1
            HeaderColumn = "Creation Time";
            SortDirection = -1;
            SortList();
            // set downloadproperties
            DownProperties = DataController.GetDownloadProperties();
            if (DownProperties != null)
            {
                DownProperties.SortDirection = SortDirection;
                {
                    SortDirection = DownProperties.SortDirection.Value;
                }
                if (DownProperties.SortedColumn != null)
                {
                    if (HeaderColumn == "Name") DownProperties.SortedColumn = 0;
                    else if (HeaderColumn == "Creation Time") DownProperties.SortedColumn = 1;
                    else if (HeaderColumn == "File Length") DownProperties.SortedColumn = 2;
                }
                DownProperties.Update();
            }
        }

        public void MoveToSelectedRow(int pos)
        {
            //DataGrid? temp = BoundGrid;

            //if (DgMovies != null)
            //{
            //    DgMovies.ItemsSource = foundMovieList;
            //    ObservableCollection<Movies> movies = DgMovies.ItemsSource as ObservableCollection<Movies>;

            //    //Movies tempm = movies.Where(x => x.Id == CurrentMovie.Id).FirstOrDefault();
            //    if (FoundMovie != null)
            //    {
            //        DgMovies.ScrollIntoView(FoundMovie, null);
            //    }
            //}
        }

        //public async void OtherMovies()
        //{
        //    Views.MainWindow? mainWindow = Support.GetMainWindow() as Views.MainWindow;

        //    // save caller for return
        //    Window tempWindow = Caller;

        //    List<Movies> temp = ViewModelSupport.OtherMovieList(FoundMovie);
        //    if (temp != null)
        //    {
        //        MovieViewModel viewModel = new();

        //        ObservableCollection<Movies>? oldMovieList = viewModel.MovieList;
        //        Movies? oldCurrentMovie = viewModel.CurrentMovie;

        //        viewModel.MovieList = new ObservableCollection<Movies>(temp); ;
        //        viewModel.CurrentMovie = FoundMovie;

        //        MovieListDialog movieListDialog1 = new(viewModel);
        //        MovieListDialog movieListDialog = movieListDialog1;
        //        viewModel.Caller = movieListDialog;
        //        //this.Caller = movieListDialog;

        //        // show on main window
        //        await movieListDialog.ShowDialog(mainWindow);

        //        // restore caller
        //        this.Caller = tempWindow;

        //        // }

        //    }
        //}

        /// <summary>
        /// Does the page down command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageDownCommand()
        {
            if (FoundMovieList != null && FoundMovie != null)
            {
                int pos = FoundMovieList.IndexOf(FoundMovie);
                pos += 5;
                if (pos < FoundMovieList.Count)
                {
                    FoundMovie = FoundMovieList[pos];
                }
                else
                {
                    FoundMovie = FoundMovieList.Last();
                    pos = FoundMovieList.Count - 1;
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
            if (FoundMovieList != null && FoundMovie != null)
            {
                FoundMovie = FoundMovieList.Last();
            }
            MoveToSelectedRow(FoundMovieList.Count - 1);
        }

        /// <summary>
        /// Pages the home command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageHomeCommand()
        {
            if (FoundMovieList != null && FoundMovie != null)
            {
                FoundMovie = FoundMovieList.First();
            }
            MoveToSelectedRow(0);
        }

        /// <summary>
        /// Pages up command.
        /// </summary>
        /// <autogeneratedoc />
        public void PageUpCommand()
        {
            if (FoundMovieList != null && FoundMovie != null)
            {
                int pos = FoundMovieList.IndexOf(FoundMovie);
                pos -= 5;
                if (pos >= 0)
                {
                    FoundMovie = FoundMovieList[pos];
                }
                else
                {
                    FoundMovie = FoundMovieList.First();
                    pos = 0;
                }

                MoveToSelectedRow(pos);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="clearFind">The clearFind<see cref="bool"/>.</param>
        public void Refresh(bool clearFind = true)
        {
            this.BuildList();

            FoundMovieList = new ObservableCollection<Movies>();

            if (clearFind) FindText = string.Empty;
           // else DoFindFile();
        }

        private void BuildList()
        {
            Unbounds = new ObservableCollection<UnboundGridData>(
                               DataController.SandboxEntities.UnboundGridData.OrderByDescending(x => x.CreationTime).ToList()
                               );
            SortList();
            this.RaisePropertyChanged(nameof(Unbounds));
        }

        public void RefreshFile()
        {
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                if (File.Exists(CurrentRow.FileName))
                {
                    CurrentRow.Refresh();
                    CurrentRow.Save();
                    UpdateMainWindow(CurrentRow, " --refreshed");
                }
                else
                {
                    Unbounds.Remove(CurrentRow);
                    CurrentRow.Delete();
                }
            }
        }

        public async void SearchIMDB()
        {
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                // Window oldCaller = Caller;
                Movies temp = new()
                {
                    MoviePath = CurrentRow.FileName,
                    MovieName = CurrentRow.Name
                };

                // MovieViewModel viewModel = new(temp);

                // set viewModel year to the text in the ()
                string name = CurrentRow.Name;
                // replace 's)' with s) to deal with decades
                name = name.Replace("s)", "s)");

                int pos1 = name.LastIndexOf('(');
                int pos2 = name.LastIndexOf(')');
                if (pos1 > 0 && pos2 > pos1)
                {
                    string year = name.Substring(pos1 + 1, pos2 - pos1 - 1);
                    //if (int.TryParse(year, out int y))
                    //{
                    //    viewModel.Year = y;
                    //    CurrentRow.Year = y;
                    //    CurrentRow.Save();

                    //    name = name.Substring(0, pos1).Trim();
                    //    viewModel.MovieTitle = name;
                    //}

                    // is not a year in brackets so put brackets back
                    //else viewModel.MovieTitle = Path.GetFileNameWithoutExtension(CurrentRow.FileName);
                }
                //else viewModel.MovieTitle = Path.GetFileNameWithoutExtension(CurrentRow.FileName);

                //Dialogs.TMDBSearchDialog searchDialog = new Dialogs.TMDBSearchDialog(viewModel);
                //viewModel.Caller = searchDialog;
                //await searchDialog.ShowDialog(oldCaller);
                //if (
                //    viewModel.resultButton != null
                //    && viewModel.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok
                //)
                //{
                //    if (viewModel.FoundMovie != null)
                //    {
                //        CurrentRow.TIMDB = viewModel.FoundMovie.ID;

                //        iMovie iMovie = await TmdbSupport.GetMovieData(viewModel.FoundMovie.ID);
                //        if (iMovie != null)
                //        {
                //            //GetCastData(Support.CreatedMovie, iMovie);
                //            CurrentRow.Overview = iMovie.Overview;
                //            CurrentRow.Year = iMovie.ReleaseDate.Year;

                //            CurrentRow.Save();
                //        }

                //    }
                //}
                //this.Caller = oldCaller;
            }
        }

        //public void ShowBookmarks()
        //{
        //    Views.MainWindow? mainWindow = Support.GetMainWindow() as Views.MainWindow;

        //    Window temp = Caller;

        //    if (mainWindow != null && FoundMovie != null)
        //    {
        //        MovieViewModel viewModel = new MovieViewModel(FoundMovie);
        //        var movieDialog = new BookmarkDialog(viewModel);

        //        this.Caller = movieDialog;
        //        //movieDialog.DataContext = mvm;
        //        // movieDialog.CurrentMovieModel = mvm;
        //        viewModel.Caller = movieDialog;
        //        movieDialog.ShowDialog(mainWindow);

        //        this.Caller = temp;
        //    }
        //}

        //public void ShowCast()
        //{
        //    Views.MainWindow? mainWindow = Support.GetMainWindow() as Views.MainWindow;

        //    Window temp = Caller;

        //    if (mainWindow != null && FoundMovie != null)
        //    {
        //        MovieViewModel viewModel = new MovieViewModel(FoundMovie);
        //        var movieDialog = new CastListDialog(viewModel);

        //        this.Caller = movieDialog;
        //        //movieDialog.DataContext = mvm;
        //        // movieDialog.CurrentMovieModel = mvm;
        //        viewModel.Caller = movieDialog;
        //        movieDialog.ShowDialog(mainWindow);

        //        this.Caller = temp;
        //    }
        //}

        /// <summary>
        /// The SortingColumns.
        /// </summary>
        /// <param name="source">The source<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataGridColumnEventArgs"/>.</param>
        public void SortingColumns(object source, DataGridColumnEventArgs e)
        {
            HeaderColumn = e.Column.Header.ToString().Trim();
            SortDirection = SortDirection * -1;
            SortList();

            //SortList();
            // set downloadproperties
            DownProperties = DataController.GetDownloadProperties();
            if (DownProperties != null)
            {
                DownProperties.SortDirection = SortDirection;
                //{
                //    SortDirection = DownProperties.SortDirection.Value;
                //}
                if (DownProperties.SortedColumn != null)
                {
                    if (HeaderColumn == "Name") DownProperties.SortedColumn = 0;
                    else if (HeaderColumn == "Creation Time") DownProperties.SortedColumn = 1;
                    else if (HeaderColumn == "File Length") DownProperties.SortedColumn = 2;
                }
                DownProperties.Update();
            }
        }

        /// <summary>
        /// The SortList.
        /// </summary>
        public void SortList()
        {
            if (HeaderColumn == "Creation Time" && SortDirection == 1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderBy(s => s.CreationTime).ToList());
            else if (HeaderColumn == "Creation Time" && SortDirection == -1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderByDescending(s => s.CreationTime).ToList());
            else if (HeaderColumn == "File Length" && SortDirection == 1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderBy(s => s.FileLength).ToList());
            else if (HeaderColumn == "File Length" && SortDirection == -1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderByDescending(s => s.FileLength).ToList());
            else if (HeaderColumn == "Name" && SortDirection == 1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderBy(s => s.Name).ToList());
            else if (HeaderColumn == "Name" && SortDirection == -1)
                Unbounds = new ObservableCollection<UnboundGridData>(Unbounds.OrderByDescending(s => s.Name).ToList());
        }

        /// <summary>
        /// The DoFindFile.
        /// </summary>
        //internal void DoFindFile()
        //{
        //    if (!string.IsNullOrEmpty(FindText))
        //    {
        //        //DownloadSupport instance = DownloadSupport.GetInstance();
        //        ProcessRoot();
        //        //if (instance.UnboundGridDatas != null)
        //        //{
        //       // var retValue = DataController.SandboxEntities.GetUnboundGridDatabyFileName(FindText);
        //        //var gridlist = Unbounds.Where(x => x.FileName!.ToLower().Contains(FindText.ToLower()));
        //        Unbounds = new ObservableCollection<UnboundGridData>(retValue);

        //        GetFoundMoviesList(FindText);
        //    }
        //}

        /// <summary>
        /// The GetFoundMoviesList.
        /// </summary>
        /// <param name="search">The search<see cref="string"/>.</param>
        internal void GetFoundMoviesList(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var movies = DataController.SandboxEntities.GetMoviesbyTitle(search);
                FoundMovieList = new ObservableCollection<Movies>(movies);
            }
        }

        /// <summary>
        /// The DoAccept.
        /// </summary>
        private new void DoAccept()
        {
            // ResultTask = new DialogResultButton() { Result = DialogResultButton.ResultType.Ok };
            //Caller.Close();
        }

        /// <summary>
        /// The DoAddToList.
        /// </summary>
        private void DoAddToList()
        {
            //if (DownLoadDialog != null && CurrentRow != null)
            //{
            //    JoinListCombo = DownLoadDialog.SelectedMovies;

            //    if (!string.IsNullOrEmpty(CurrentRow.FileName) && MovieJoinList != null)
            //    {
            //        MovieJoinList.Add(CurrentRow.FileName);
            //        if (JoinListCombo != null)
            //            JoinListCombo.ItemsSource = new ObservableCollection<string>(MovieJoinList);
            //    }
            //}
        }

        /// <summary>
        /// The DoCancel.
        /// </summary>
        //private new void DoCancel()
        //{
        //    ResultTask = new DialogResultButton() { Result = DialogResultButton.ResultType.Cancel };
        //    Caller.Close();
        //}

        /// <summary>
        /// The DoClearList.
        /// </summary>
        private void DoClearList()
        {
            foreach (var item in Unbounds)
            {
                item.IsSelected = false;
            }
        }

        /// <summary>
        /// Does the create.
        /// </summary>
        /// <returns></returns>
        /// <autogeneratedoc />
        //private async Task<bool> DoCreate()
        //{
        //    bool success = false;
        //    if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
        //    {
        //        CurrentRow.Save();
        //        Support support = new Support();

        //        support.ActionCompleted += Support_ActionCompleted;
        //        support.ProgressInformation += Support_ProgressInformation;

        //        int? newTMIDB = null;
        //        // see if we have a TIMDB ID
        //        if (CurrentRow.TIMDB != null)
        //        {
        //            newTMIDB = CurrentRow.TIMDB;
        //        }



        //        success = await support.CreateMovie(CurrentRow.FileName, Support.GetMainWindow(), newTMIDB);
        //        if (success)
        //        {
        //            CurrentRow.Delete();
        //            Unbounds.Remove(CurrentRow);

        //            ProcessRoot();

        //            SetDownloads();

        //            // Support.CreatedMovie will contain the new Movie
        //            // might be an idea to have a created movie list and append the new movies to it?

        //            if (Support.CreatedMovie != null)
        //            {
        //                HasNewMovie = (Support.CreatedMovie != null);

        //                if (CreatedMovieList == null) CreatedMovieList = new List<Movies>();

        //                CreatedMovieList.Add(Support.CreatedMovie);

        //                FoundMovieList = new ObservableCollection<Movies>(CreatedMovieList.OrderByDescending(x => x.Id).ToList());
        //                FoundMovie = Support.CreatedMovie;
        //                // 
        //                Support.CreatedMovie.GetDuration(Support.CreatedMovie.MoviePath);

        //                // add moviegenre to CrteatedMovie 
        //                if (Support.CreatedMovie.MovieGenres != null)
        //                {

        //                }
        //            }
        //        }

        //    }
        //    return success;
        //}

        private void Support_ProgressInformation(object sender, MovieProgressEventargs e)
        {

        }

        private void Support_ActionCompleted(object sender, MovieCompletedEventArgs e)
        {
            if (e.Movie != null)
            {
                Support.GenerateInfoAndLogMessage("Created", "Movie", 0, e.Movie.MoviePath);

                // create moviegenre for movie
                if (e.PhraseEntry != null && e.Movie != null && e.SubPhraseEntry == null)
                {
                    MovieGenre movieGenre = new MovieGenre()
                    {
                        MovieId = e.Movie.Id,
                        Genre = e.PhraseEntry.COMPKEY
                    };
                    movieGenre.Insert();
                }
                else if (e.PhraseEntry != null && e.Movie != null && e.SubPhraseEntry != null)
                {
                    MovieGenre movieGenre = new MovieGenre()
                    {
                        MovieId = e.Movie.Id,
                        Genre = e.PhraseEntry.COMPKEY,
                        SubGenre = e.SubPhraseEntry.COMPKEY
                    };
                    movieGenre.Insert();
                }

            }
            else
            {
                Support.GenerateInfoAndLogMessage("Failed to Create Movie", "Movie", 0, e.Movie.MoviePath);
            }

        }

        private void DoDeleteEntry()
        {
            if (CurrentRow != null)
            {
                this.DeleteEntryMoveOneRow();
            }
        }

        private void DoDelete()
        {
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                // log change

                Support.GenerateInfoAndLogMessage("Deleted", "Downloaded File", 0, CurrentRow.FileName);

                // delete file

                string filename = Support.FixImagePath(CurrentRow.FileName);

                // delete to Recycle Bin if possible and file exists, use Support.FileSupport.MovetoRecycleBin

                if (File.Exists(filename))
                    FileOperationAPIWrapper.MoveToRecycleBin(filename);

                this.DeleteEntryMoveOneRow();

                //DownloadSupport.ProcessRoot();
                //SetDownloads();
            }
            else
            {
                Unbounds.Remove(CurrentRow);
                CurrentRow.Delete();
            }
        }

        private void DeleteEntryMoveOneRow()
        {
            // get index of current row
            int index = Unbounds.IndexOf(CurrentRow);
            // remove from list and delete

            Unbounds.Remove(CurrentRow);
            if (CurrentRow != null) CurrentRow.Delete();
            // get new current row
            if (Unbounds.Count > 0)
            {
                if (index < Unbounds.Count)
                    CurrentRow = Unbounds[index];
                else
                    CurrentRow = Unbounds[Unbounds.Count - 1];
            }
            else
                CurrentRow = null;
        }

        private void DoDeleteTemp()
        {
            if (CurrentRow != null)
            {
                //CurrentMovie = new Movies();
                //CurrentMovie.MoviePath = CurrentRow.FileName;

                if (CurrentRow.FileName.Contains("temp."))

                // AvalonMVVM.Support.Support.PlayMovie(CurrentRow.FileName, null);
                {
                    File.Delete(CurrentRow.FileName);
                }
            }
        }

        private async void DoDuration()
        {
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                //int time = await VideoSupport.GetDurationSecondsAsync(CurrentRow.FileName, null);
                //CurrentRow.DurationSeconds = time;
            }
        }

        /// <summary>
        /// The DoEndFile.
        /// </summary>
        private void DoEndFile()
        {
            this.FindText = string.Empty;
            //if (this.DownLoadDialog != null)  this.DownLoadDialog._findText.Text = string.Empty;
            Refresh();
        }

        public ICommand GetJoinList()
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                DoJoinList();
            });
            return myCommand;
        }

        /// <summary>
        /// The DoJoinList.
        /// </summary>
        public void DoJoinList()
        {
            SetupMpeg();
        }

        /// <summary>
        /// The DoMoveTemp.
        /// </summary>
        private void DoMoveTemp()
        {
            if (CurrentRow != null)
            {
                if (File.Exists(CurrentRow.FileName))
                {
                    CurrentMovie = new Movies();
                    CurrentMovie.MoviePath = CurrentRow.FileName;

                    if (File.Exists(CurrentMovie.GetTempFileName()))
                    {
                        string newFile = CurrentMovie.GetTempFileName();
                        string oldFile = CurrentRow.FileName;

                        File.Delete(oldFile);
                        File.Move(newFile, oldFile);

                        RefreshGrid();
                    }
                }
            }
        }

        /// <summary>
        /// The DoPlay.
        /// </summary>
        private void DoPlay()
        {
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                if (File.Exists(CurrentRow.FileName))
                {
                    Support.PlayMovie(CurrentRow.FileName, null);
                    if (CurrentRow.DurationSeconds == 0)
                    {
                        DoDuration();
                        //   int totalDuration = VideoSupport.GetDurationSeconds(CurrentRow.FileName, null);
                        //  CurrentRow.DurationSeconds = totalDuration;
                        CurrentRow.Save();
                    }

                    // get datagrid from CurrentRow
                }
                else
                {
                    unbounds.Remove(CurrentRow);
                    CurrentRow.Delete();
                }
            }
        }

        /// <summary>
        /// The DoPlayTemp.
        /// </summary>
        private void DoPlayTemp()
        {
            if (CurrentRow != null)
            {
                CurrentMovie = new Movies();
                CurrentMovie.MoviePath = CurrentRow.FileName;

                if (CurrentMovie.MoviePath.Contains("temp."))

                    Support.PlayMovie(CurrentRow.FileName, null);
                else
                {
                    if (File.Exists(CurrentMovie.GetTempFileName()))
                        Support.PlayMovie(CurrentMovie.GetTempFileName(), null);
                }
            }
        }

        private async Task<bool> DoRename()
        {
            bool success = false;
            if (CurrentRow != null && !string.IsNullOrEmpty(CurrentRow.FileName))
            {
                {
                    // replace any commas as they can mess up XSPF files.
                    EntryDialogModel dialogModel = new EntryDialogModel()
                    {
                        EntryTypeValue = EntryDialogModel.EntryType.Text,
                        EntryText = CurrentRow.FileName?.Replace(",", "-"),
                        MaxStringLength = 150
                    };

                    dialogModel.EntryText = Regex.Replace(CurrentRow.FileName, @"[^\u0000-\u007F]+", string.Empty).Trim();

                    // dialogModel.EntryText = model.FileName;
                    // Regex.Replace(model.FileName, @"[^\u0020-\u007E]", string.Empty);

                    // create dialog
                    EntryDialog entryDialog = new EntryDialog(dialogModel);

                    //   dialogModel.Caller = entryDialog;

                    MainWindow main = Support.GetMainWindow();
                    DialogResultButton result = await entryDialog.ShowDialog<DialogResultButton>(main);
                    if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                    {
                        string? oldname = CurrentRow.FileName;
                        string? newName = dialogModel.EntryText;

                        if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(oldname) && !File.Exists(newName))
                        {
                            File.Move(oldname, newName);
                            CurrentRow.FileName = newName;
                            CurrentRow.Save();
                        }

                        Refresh(false);

                        MoveToLastItem();

                        UnboundGridData? unboundGridData = Unbounds.Where(x => x.FileName == newName).FirstOrDefault();
                        if (unboundGridData != null)
                        {
                            //DataGrid grid = this.FindControl<DataGrid>("Unbound");

                            //if (grid != null)
                            //{
                            //    grid.ItemsSource= downloadViewModel.Unbounds;
                            //    grid.SelectedItem = unboundGridData;
                            //}
                        }
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// The DoToMP4.
        /// </summary>
        private async void DoToMP4()
        {
            if (CurrentRow != null)
            {
                {
                    FFMpegSupport mpegSupport = new FFMpegSupport();
                    mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                    mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;

                    //int totalDuration = await VideoSupport.GetDurationSecondsAsync(CurrentRow.FileName, null);
                    //mpegSupport.TotalDuration = totalDuration;

                    //mpegSupport.ConvertToMP4(CurrentRow.FileName, CurrentRow.MovieId);
                }
            }
        }

        /// <summary>
        /// The DoToMTS.
        /// </summary>
        private async void DoToMTS()
        {
            if (CurrentRow != null)
            {
                {
                    FFMpegSupport mpegSupport = new FFMpegSupport();
                    mpegSupport.ConversionComplete += MTSComplete;

                    mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                    mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;

                    //int totalDuration = await VideoSupport.GetDurationSecondsAsync(CurrentRow.FileName, null);
                    //mpegSupport.TotalDuration = totalDuration;

                    //Models.Movies movie = new Models.Movies()
                    //{
                    //    MoviePath = CurrentRow.FileName,
                    //    Id = 0
                    //};
                    //mpegSupport.ConvertToMTS(movie);
                }
            }
        }

        /// <summary>
        /// The DoTrimMovie.
        /// </summary>
        private async void DoTrimMovie()
        {
            //if (CurrentRow != null)
            //{
            //    CurrentMovie = new Movies();
            //    CurrentMovie.MoviePath = CurrentRow.FileName;
            //    CurrentMovie.MovieName = CurrentRow.FileName;
            //    int time = await VideoSupport.GetDurationSecondsAsync(CurrentRow.FileName, null);
            //    CurrentMovie.DurationSeconds = time;

            //    if (currentMovie != null)
            {
                //    if (currentMovie.EndBookmark != null)
                //    {
                //        currentMovie.EndBookmark.Time = currentMovie.DurationSeconds;
                //    }
                //    Dialogs.TrimMovieDialog trimMovieDialog = new Dialogs.TrimMovieDialog(this, currentMovie);
                //    //this.Caller = trimMovieDialog;

                //    MainWindow? main = Support.GetMainWindow();

                //    if (main != null)
                //    {
                //        DialogResultButton result = await trimMovieDialog.ShowDialog<DialogResultButton>(main);

                //        if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                //        {
                //            {
                //                FFMpegSupport mpegSupport = new FFMpegSupport();
                //                //mpegSupport.ConversionComplete += MTSComplete;
                //                mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                //                mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;
                //                if (currentMovie.DurationSeconds != null) mpegSupport.TotalDuration = currentMovie.DurationSeconds.Value;
                //                if (result.Seconds != null) mpegSupport.TotalDuration = result.Seconds.Value;
                //                mpegSupport.MovieName = currentMovie.MovieName;
                //                if (this.Caller != null)
                //                {
                //                    MainWindow? mainWindow = this.Caller as MainWindow;
                //                    if (mainWindow != null)
                //                    {
                //                        // mainWindow.StopWatcher();
                //                    }
                //                }
                //                int val = await mpegSupport.TrimMovie(currentMovie, result.Paramater);
                //            }
                //        }
                //        HasTemp = File.Exists(CurrentMovie.GetTempFileName());

                //        if (!string.IsNullOrEmpty(FindText)) DoFindFile();

                //        //TrimMovie = ReactiveCommand.Create(DoTrimMovieAsync);
                //    }
                //}
                // }
            }
        }

        /// <summary>
        /// The FilesJoined.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ConversionCompleteEventArgs"/>.</param>
        //private void FilesJoined(object sender, ConversionCompleteEventArgs e)
        //{
        //    if (e.ExitCode == 0)
        //    {
        //        ProcessRoot();
        //        //UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;
        //        //if (unboundGridDatas != null)
        //        //    Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);

        //        if (Caller != null)
        //        {
        //            MenuItem join = Caller.FindControl<MenuItem>("JoinMovies");
        //            if (join != null) join.IsEnabled = true;
        //        }

        //        DoClearList();
        //    }
        //    else
        //    {
        //    }
        //}

        /// <summary>
        /// The FindDuplicates.
        /// </summary>
        private void FindDuplicates()
        {
            //UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;
            // order by file length and then order by name to make it easier to find duplicates
            if (Unbounds == null || Unbounds.Count == 0) return;
            var sorted = Unbounds.OrderBy(x => x.FileLength).ThenBy(x => x.Name).ToList();
            //unboundGridDatas.Sort((x, y) => x.FileLength.CompareTo(y.FileLength)).;
            UnboundGridDataCollection dupes = new UnboundGridDataCollection();
            dupes.Clear();

            for (int i = 0; i < sorted.Count - 2; i++)
            {
                if (sorted[i].FileLength == sorted[i + 1].FileLength)
                {
                    // if filelength is the same check the name
                    if (string.Equals(sorted[i].Name, sorted[i + 1].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // we have a duplicate so add to list
                        if (!dupes.Contains(sorted[i]))
                            dupes.Add(sorted[i]);
                        if (!dupes.Contains(sorted[i + 1]))
                            dupes.Add(sorted[i + 1]);
                    }
                    else // delete the entry if the creationdate is the same
                        if (sorted[i].CreationTime == sorted[i + 1].CreationTime)
                        {
                            sorted[i + 1].Delete();
                        }

                    dupes.Add(sorted[i]);
                    dupes.Add(sorted[i + 1]);
                }
            }

            if (dupes != null && dupes.Count > 0)
                Unbounds = new ObservableCollection<UnboundGridData>(dupes);
        }

        /// <summary>
        /// The MP4Complete.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ConversionCompleteEventArgs"/>.</param>
        private void MP4Complete(object sender, ConversionCompleteEventArgs e)
        {
            ProcessRoot();
        }

        /// <summary>
        /// The MpegSupport_CliWrapCompleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CliWrapCompletedEventArgs" /> instance containing the event data.</param>
        private void MpegSupport_CliWrapCompleted(object sender, CliWrapCompletedEventArgs e)
        {
            if (e.Result == 0)
            {
                ProcessRoot();
                //UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;
                //if (unboundGridDatas != null)
                //    Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);

                if (e.TaskName == "JOIN")
                {
                    //if (Caller != null)
                    //{
                    //    MenuItem join = Caller.FindControl<MenuItem>("JoinMovies");
                    //    if (join != null) join.IsEnabled = true;
                    //}

                    ProcessOutput = "Movies Joined";
                    DoClearList();
                }
                else if (e.TaskName == "CONVERT")
                {
                    ProcessOutput = "Movie Converted";
                    if (!string.IsNullOrEmpty(e.MovieName)) ProcessOutput += " -- " + e.MovieName;
                    ProcessRoot();
                }
                else if (e.TaskName == "TRIM")
                {
                    ProcessOutput = "Movie Corrected";
                    if (!string.IsNullOrEmpty(e.MovieName)) ProcessOutput += " -- " + e.MovieName;
                    Refresh();
                    if (this.Caller != null)
                    {
                        MainWindow? mainWindow = this.Caller as MainWindow;
                        if (mainWindow != null)
                        {
                            //     mainWindow.WatcherWork();
                        }
                    }
                }

                ProgressPercent = 0;
            }
            else
            {
            }
        }

        /// <summary>
        /// The MpegSupport_CliWrapErrored.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CliWrapErrorEventArgs" /> instance containing the event data.</param>
        private void MpegSupport_CliWrapErrored(object sender, CliWrapErrorEventArgs e)
        {
        }

        /// <summary>
        /// The MpegSupport_CliWrapProgress.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CliWrapProgressEventArgs" /> instance containing the event data.</param>
        private void MpegSupport_CliWrapProgress(object sender, CliWrapProgressEventArgs e)
        {
            string? temp = e.Progress;

            if (!string.IsNullOrEmpty(temp))
            {
                if (e.ProgressPercentage == 0)
                {
                    ProcessOutput = temp;
                }
                else
                {
                    ProcessOutput = temp + " - " + e.ProgressPercentage.ToString() + "%";
                    ProgressPercent = e.ProgressPercentage;
                }
            }
        }

        /// <summary>
        /// The MTSComplete.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ConversionCompleteEventArgs"/>.</param>
        private void MTSComplete(object sender, ConversionCompleteEventArgs e)
        {
            ProcessRoot();
        }

        /// <summary>
        /// The RefreshGrid.
        /// </summary>
        private void RefreshGrid()
        {
            //DownloadSupport instance = DownloadSupport.GetInstance();
            ProcessRoot();

            //UnboundGridDataCollection? unboundGridDatas = DownloadSupport.GetInstance().UnboundGridDatas;
            //if (unboundGridDatas != null)
            //    Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);
            SortList();
           // DoFindFile();
        }

        private void SetDownloads()
        {
            //DownloadSupport instance = DownloadSupport.GetInstance();
            UnboundGridDataCollection? unboundGridDatas = new UnboundGridDataCollection(Unbounds);

            if (HeaderColumn != null && unboundGridDatas != null)
            {
                unboundGridDatas.SortByColumn(HeaderColumn, SortDirection);
                Unbounds = new ObservableCollection<UnboundGridData>(unboundGridDatas);
            }
        }

        /// <summary>
        /// The SetUpCommands.
        /// </summary>
        private void SetUpCommands()
        {
            ToMTS = ReactiveCommand.Create(DoToMTS);
            ToMP4 = ReactiveCommand.Create(DoToMP4);
            ResetTimeStamps = ReactiveCommand.Create(DoResetTimeStamps);
            TrimMovie = ReactiveCommand.Create(DoTrimMovie);
            Play = ReactiveCommand.Create(DoPlay);
            Delete = ReactiveCommand.Create(DoDelete);
            DeleteEntry = ReactiveCommand.Create(DoDeleteEntry);
            //Create = ReactiveCommand.CreateFromTask(DoCreate);
            Rename = ReactiveCommand.CreateFromTask(DoRename);
            GetDuration = ReactiveCommand.Create(DoDuration);
            PlayTemp = ReactiveCommand.Create(DoPlayTemp);
            MoveTemp = ReactiveCommand.Create(DoMoveTemp);
            DeleteTemp = ReactiveCommand.Create(DoDeleteTemp);
            ClearList = ReactiveCommand.Create(DoClearList);
            AddToList = ReactiveCommand.Create(DoAddToList);
            EditMovieCommand = ReactiveCommand.Create(EditMovie);
            JoinList = ReactiveCommand.Create(DoJoinList);
            //FindFile = ReactiveCommand.Create(DoFindFile);
            EndFindFile = ReactiveCommand.Create(DoEndFile);
            DoDuplicates = ReactiveCommand.Create(FindDuplicates);
            DoRefresh = ReactiveCommand.Create(RefreshGrid);
            LastLoadCommand = ReactiveCommand.Create(LastLoad);
            LastItemCommand = ReactiveCommand.Create(LastItem);
            //Accept = ReactiveCommand.Create(DoAccept);
            //Cancel = ReactiveCommand.Create(DoCancel);
            //ShowDetails = ReactiveCommand.Create(DoShowDetails);
            DeleteEntity = ReactiveCommand.Create(DoDeleteEntity);
        }

        private void DoDeleteEntity()
        {
            if (FoundMovie != null)
            {
                Support.GenerateInfoAndLogMessage("Deleted", "Movie", FoundMovie.Id, FoundMovie.MovieName);
                FoundMovie.Delete();
                FoundMovieList?.Remove(FoundMovie);
                FoundMovie = null;
            }
        }

        private async void DoResetTimeStamps()
        {
            if (CurrentRow != null && !String.IsNullOrEmpty(CurrentRow.FileName))
            {
                {
                    FFMpegSupport mpegSupport = new FFMpegSupport();
                    mpegSupport.CliWrapProgress += MpegSupport_CliWrapProgress;
                    mpegSupport.CliWrapCompleted += MpegSupport_CliWrapCompleted;

                    //int totalDuration = await VideoSupport.GetDurationSecondsAsync(CurrentRow.FileName, null);
                    //mpegSupport.TotalDuration = totalDuration;

                    //mpegSupport.ResetTimestamps(CurrentRow.FileName);
                }
            }
        }

        /// <summary>
        /// The SetupMpeg.
        /// </summary>
        private async void SetupMpeg()
        {
            MovieJoinList = Unbounds?.Where(u => u.IsSelected.Value && u.FileName != null).OrderBy(m => m.Name).Select(i => i.FileName).ToList();

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

                foreach (string item in MovieJoinList)
                {
                    // need to find file duration
                    //int time = await VideoSupport.GetDurationSecondsAsync(item, null);
                    //totalDuration += time;

                    string line = ("file '" + item.Replace("\\", "/") + "'");
                    streamWriter.WriteLine(line);
                }
                //streamWriter.WriteLine("file '" + firstVideoFilePath.Replace("\\","/") + "'");
                //streamWriter.WriteLine("file '" + secondVideoFilePath.Replace("\\", "/") + "'");
                streamWriter.Flush();
                streamWriter.Close();
                string strParam = "  -fflags +genpts -f concat -safe 0 -i " + file + " -c copy " + mpegSupport.OutputVideoPath;

                mpegSupport.TotalDuration = totalDuration;
                //FFMpegSupport.do

                //mpegSupport.ConversionComplete += FilesJoined;
                mpegSupport.JoinProcess(strParam);
            }
        }

        internal void AddNewUnbound(UnboundGridData unboundGridData)
        {
            //this.Unbounds = null;
            UnboundGridData? find = this.Unbounds.Where(u => u.Id == unboundGridData.Id).FirstOrDefault();
            if (find == null)
                Unbounds.Insert(0, unboundGridData);
            // sleep for a second
            System.Threading.Thread.Sleep(1000);
            this.ProcessRoot();
            // rebuild entire list and check in database to make sure we have the latest data, this is needed as we can have multiple threads adding to the list at the same time and we want to make sure we have the latest data and not just add to the list which can cause issues with duplicates and sorting
            find = this.Unbounds.Where(u => u.Id == unboundGridData.Id).FirstOrDefault();
            if (find == null)
                Unbounds.Insert(0, unboundGridData);

            CurrentRow = unboundGridData;
            //this.RaiseAndSetIfChanged(ref unbounds, Unbounds);

            this.RaisePropertyChanged("Unbounds");
            UnboundGrid.ScrollIntoView(CurrentRow, null);
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 15/02/2026 15/02/2026 </created>
        /// </remarks>
        internal async void ProcessRoot()
        {
            // load from database and order by creation time desc if empty
            if (Unbounds == null || Unbounds.Count < 10) Unbounds = new ObservableCollection<UnboundGridData>
            (
                DataController.SandboxEntities.UnboundGridData.OrderByDescending(u => u.CreationTime).ToList()
            );

            // find the Download directory and get all the files in it and
            // add to the list if they are not already in the list,
            // this is needed as we can have multiple threads adding to the list
            // at the same time and we want to make sure we have the latest data
            // and not just add to the list which can cause issues with duplicates and sorting
            string sep = DownloadSupport.DirectorySeparator();
            string baseRoot = Support.FixImagePath(DownloadSupport.MovieBasePath + @"Download");

            // check to see if we can find the directory
            if (Directory.Exists(baseRoot))
            {
                // get all the files in the directory and add to the list
                // if they are not already in the list,
                // we only want to add video files so we need to check the extension of the file
                // and only add if it is a video file,
                // we can check for common video file extensions such as
                // .mp4, .avi, .mkv, .flv, .mpeg, .mts, .mov, .mpg, .webm, .rm, .wmv
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

                        UnboundGridData? unboundGridData = Unbounds.Where(u => u.FileName?.ToLower() == file.ToLower()).FirstOrDefault();
                        // will be null if not present in the list,
                        // this can happen if we have multiple threads adding to the list at the same time
                        // or if we have added a file to the directory that is not in the database,
                        // we want to make sure we add it to the database and list if it is not already there
                        if (unboundGridData == null)
                        {
                            // not using the create method here as we want to add all the details of the file
                            // such as length and creation time and we want to make sure we have the latest data
                            // from the file system and ensure it is one hit to avoid issues with multiple threads
                            unboundGridData = new UnboundGridData();
                            unboundGridData.Folder = searchFolder;
                            unboundGridData.FileName = file;
                            string dataFileName = searchFolder + sep + filename;
                            unboundGridData.FilePath = file;

                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(dataFileName);
                            unboundGridData.FileLength = fileInfo.Length;
                            unboundGridData.CreationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm");
                            unboundGridData.FileInfo = fileInfo;
                            unboundGridData.Insert();  // now checks for previous insert
                            if (Unbounds != null)
                                Unbounds.Add(unboundGridData);
                        }
                    }
                }
            }
            else
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Warning",
                    "Directory " + baseRoot + " Cannot be found or disk is offline",
                    ButtonEnum.Ok);
                var result = await box.ShowAsync();
            }
        }

        internal void MoveToCurrentItem()
        {
            this.RaisePropertyChanged("Unbounds");
            UnboundGrid.ScrollIntoView(CurrentRow, null);
            UpdateMainWindow(CurrentRow);
        }

        public void UpdateMainWindow(UnboundGridData unboundItem, string additional = "")
        {
            MainWindow? mainWindow = Support.GetMainWindow();
            if (mainWindow != null)
            {
                // see if mainwindow has a as a control DownloadsHeaderControl
                // if so provide information in its subtitlea property about the number of movies in the download list and the total size of those movies. // if not then do nothing. DownloadsHeaderControl? downloadsHeaderControl = mainWindow.FindControl<DownloadsHeaderControl>("DownloadsHeader"); if (downloadsHeaderControl != null) { DownloadViewModel? viewModel = this.DataContext as DownloadViewModel; if (viewModel != null && viewModel.Unbounds != null) { long totalSize = viewModel.Unbounds.Sum(x => x.FileLength); int count = viewModel.Unbounds.Count; downloadsHeaderControl.Subtitle = $"{count} Movies - {Support.FormatFileSize(totalSize)}"; } }

                //if (mainWindow.DownloadsHeaderControl != null)
                //{
                //    //                DownloadViewModel? viewModel = this.DataContext as DownloadViewModel;
                //    if (this.Unbounds != null)
                //    {
                //        long totalSize = this.Unbounds.Sum(x => x.FileLength.Value);
                //        int count = this.Unbounds.Count;
                //        mainWindow.DownloadsHeaderControl.SubtitleA = $"Number of Moviies = {count} " +
                //            $"Total size of Movies = {Support.FormatFileSize(totalSize)}";

                //    }

                //    // in subtitleB put details of createdItem if it exists.

                //    if (unboundItem != null)
                //    {
                //        mainWindow.DownloadsHeaderControl.SubtitleB = $"Last Added: " +
                //            $"{unboundItem.FileName} ({Support.FormatFileSize(unboundItem.FileLength.Value)})"
                //            + " " + additional;
                //    }
                //}
            }
        }

        #endregion Methods
    }
}