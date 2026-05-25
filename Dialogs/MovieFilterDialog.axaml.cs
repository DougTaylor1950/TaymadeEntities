namespace TaymadeEntities.Dialogs
{
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using TaymadeEntities.Models;
    using TaymadeEntities.Support;
    using TaymadeEntities.ViewModels;
    using Microsoft.EntityFrameworkCore;
    using SupportCore;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public partial class MovieFilterDialog : ViewModelBase
    {

        public MainWindowViewModel? MVVM { get; set; }


        public bool ByView { get; set; }

        public MovieFilterDialog()
        {

            InitializeComponent();


            this.DataContextChanged += this.MovieFilterDialog_DataContextChanged;

            this.Opened += MovieFilterDialog_Opened;

        }

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 18/02/2026 18/02/2026 </created>
        /// </remarks>
        private void MovieFilterDialog_DataContextChanged(object? sender, EventArgs e)
        {
            if (this.DataContext != null && this.DataContext is MainWindowViewModel)
                MVVM = this.DataContext as MainWindowViewModel;
        }

        public MovieFilterDialog(MainWindowViewModel mvvm, Filter filter, bool byView = false)
        {
            InitializeComponent();

            ByView = byView;

            this.MVVM = mvvm;

            if (filter == null)
                Support.GetDefaultFilter();
            else MVVM.CurrentFilter = filter;

            if (this.MVVM.CurrentPhrase == null)
            {
                if (DataController.MovieProperties != null && !string.IsNullOrEmpty(DataController.MovieProperties.Group))
                {
                    MVVM.TempPhrase = DataController.PhraseEntries.Find(g => g.Id == DataController.MovieProperties.Group);

                    if (MVVM.TempPhrase != null && MVVM != null) MVVM.CurrentPhrase = MVVM.TempPhrase;
                }
            }
            else MVVM.TempPhrase = this.MVVM.CurrentPhrase;

            this.DataContext = this.MVVM;
            this.ByView = byView;
        }

        private void MovieFilterDialog_Opened(object? sender, System.EventArgs e)
        {

        }


        #region Methods

        /// <summary>
        /// The DoAccept.
        /// </summary>
        internal void Accept(object? sender, RoutedEventArgs e)
        {
            MVVM.ResultTask = new Dialogs.DialogResultButton();
            DoFilterList();
            this.Close();

        }

        private async void DoFilterList()
        {
            // check filter exists
            if (MVVM != null && MVVM.CurrentFilter != null)
            {
                // initialise temporary list, if by bookmark is included use separate query
                List<Movies>? tempList = null;

                if (MVVM.CurrentFilter.UseBookmark)
                {
                    tempList = DataController.SandboxEntities.GetMoviesbyBookmarkName(MVVM.CurrentFilter.BookmarkText);
                }
                else tempList = DataController.SandboxEntities.Movies.Include(m => m.Casts).ToList();

               
                List<Movies>? subList = null;
                

                // director list

                if (MVVM.CurrentFilter.UseDirector && MVVM.CurrentFilter.CurrentDirector != null)
                {
                    tempList = tempList.Where(m => m.DirectorID == MVVM.CurrentFilter.CurrentDirector.Id).ToList();
                }

                if (MVVM.CurrentFilter.HasChapter)
                {
                    if (MVVM.CurrentFilter.NotHasChapter)
                    {
                        tempList = tempList.Where(m => m.HasChapters == null || !m.HasChapters.Value).ToList();


                    }
                    else tempList = tempList.Where(m => m.HasChapters != null && !m.HasChapters.Value).ToList();

                }

                // by current film group 
                if (MVVM != null && MVVM.CurrentFilter.UseCurrentPhrase && MVVM.TempPhrase != null)
                {
                    //tempList = tempList.Where(m => m.FilmGroup.Contains(MVVM.TempPhrase.Id, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    subList = DataController.SandboxEntities.GetMoviesByGenre(MVVM.TempPhrase.COMPKEY);
                    tempList = MovieCollection.ListsIntersection(tempList, subList);
                }

                if (MVVM != null && MVVM.CurrentFilter.UseSecondaryFilter && MVVM.CurrentFilter.SecondaryPhrase != null)
                {
                    // tempList = tempList.Where(m => m.FilmGroup.Contains(MVVM.CurrentFilter.SecondaryPhrase.Id, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    subList = DataController.SandboxEntities.GetMoviesByGenre(MVVM.CurrentFilter.SecondaryPhrase.COMPKEY);
                    tempList = MovieCollection.ListsIntersection(tempList, subList);
                }

                if (MVVM != null && MVVM.CurrentFilter.UseTertiaryFilter && MVVM.CurrentFilter.TertiaryPhrase != null)
                {
                    //tempList = tempList.Where(m => m.FilmGroup.Contains(MVVM.CurrentFilter.TertiaryPhrase.Id, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    subList = DataController.SandboxEntities.GetMoviesByGenre(MVVM.CurrentFilter.TertiaryPhrase.COMPKEY);
                    tempList = MovieCollection.ListsIntersection(tempList, subList);
                }

                if (MVVM.CurrentFilter.HasEpisode)
                {
                    if (MVVM.CurrentFilter.NotHasEpisode)
                    {
                        tempList = tempList.Where(m => m.HasEpisodes == null || !m.HasEpisodes.Value).ToList();
                    }
                    else tempList = tempList.Where(m => m.HasEpisodes != null && !m.HasEpisodes.Value).ToList();
                }


                // by movie name
                if (MVVM.CurrentFilter.FilterByName && MVVM != null)
                {
                    TextBox? filterMovieName = this.FindControl<TextBox>("FilterMovieName");

                    if (filterMovieName != null)
                    {
                        string filterName = filterMovieName.Text;
                        if (!string.IsNullOrEmpty(filterName))
                            tempList = tempList.Where(m => m.MovieName.Contains(filterName, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

                if (MVVM.CurrentFilter.UseSeries && MVVM != null && MVVM.CurrentSeries != null)
                {
                    tempList = tempList.Where(m => m.Series == MVVM.CurrentSeries.Id).ToList();
                }

                if (MVVM.CurrentFilter.UseDuration && MVVM != null && !string.IsNullOrEmpty(MVVM.CurrentFilter.DurationFilter))
                {
                    // a duration filter will be comparator followed by a duration in seconds separated by spaces

                    string[] actions = MVVM.CurrentFilter.DurationFilter.Trim().Split(' ');

                    if (actions.Length == 2)
                    {
                        int durationSecs = 0;

                        if (actions[1].Contains(":"))
                        {
                            // timespan 
                            if (TimeSpan.TryParse(actions[1], out TimeSpan ts))
                            {
                                durationSecs = (int)ts.TotalSeconds;
                            }
                            else
                            {
                                MVVM.MovieList = new System.Collections.ObjectModel.ObservableCollection<Movies>(tempList);
                                return;
                            }
                        }

                        else if (int.TryParse(actions[1], out durationSecs))
                        {
                        }
                        else
                        {
                            MVVM.MovieList = new System.Collections.ObjectModel.ObservableCollection<Movies>(tempList);
                            return;
                        }

                        tempList = FilterDurations(tempList, actions, durationSecs);
                    }
                }


                if (MVVM.CurrentFilter.UseAdded && MVVM != null && !string.IsNullOrEmpty(MVVM.CurrentFilter.AddedFilter))
                {
                    // an Added filter will be comparator followed by a added value as a date separated by spaces

                    string[] actions = MVVM.CurrentFilter.AddedFilter.Trim().Split(' ');

                    if (actions.Length == 2)
                    {
                        DateTime dateValue = DateTime.MinValue;

                        if (actions[1].Contains(":") || actions[1].Contains("-"))
                        {
                            // DateTime 
                            if (DateTime.TryParse(actions[1], out dateValue))
                            {

                            }
                            else
                            {
                                MVVM.MovieList = new System.Collections.ObjectModel.ObservableCollection<Movies>(tempList);
                                return;
                            }
                        }

                        if (dateValue > DateTime.MinValue)
                        {
                            tempList = FilterByAddedOn(tempList, actions, dateValue);

                        }
                    }
                }


                if (MVVM.CurrentFilter.UseModified && MVVM != null && !string.IsNullOrEmpty(MVVM.CurrentFilter.ModifiedFilter))
                {
                    // a modified filter will be comparator followed by a onModified value as a date separated by spaces

                    string[] actions = MVVM.CurrentFilter.ModifiedFilter.Trim().Split(' ');

                    if (actions.Length == 2)
                    {
                        DateTime dateValue = DateTime.MinValue;

                        if (actions[1].Contains(":") || actions[1].Contains("-"))
                        {
                            // DateTime 
                            if (DateTime.TryParse(actions[1], out dateValue))
                            {

                            }
                            else
                            {
                                MVVM.MovieList = new System.Collections.ObjectModel.ObservableCollection<Movies>(tempList);
                                return;
                            }
                        }

                        if (dateValue > DateTime.MinValue)
                        {
                            if (dateValue > DateTime.MinValue)
                            {
                                tempList = FilterByModifiedOn(tempList, actions, dateValue);

                            }
                        }
                    }
                }

                // check to see if we are filtering by actor and we have a name
                if (MVVM != null && MVVM.CurrentFilter.UseActor && !string.IsNullOrEmpty(MVVM.CurrentFilter.ActorName))
                {
                    // get a llist of actors that match the name.
                    MVVM.CurrentFilter.CurrentActorList = DataController.ActorList.Where(a => a.Name.Contains(MVVM.CurrentFilter.ActorName, StringComparison.OrdinalIgnoreCase)).ToList();

                    // if we have found some actors then continue
                    if (MVVM.CurrentFilter.CurrentActorList != null)
                    {
                        List<Movies> mList = new List<Movies>();

                        // go through all the movies still in list 
                        foreach (Movies item in tempList)
                        {
                            // check we have a cast list it may not have been populated
                            if (item.Casts != null && item.Casts.Count == 0) item.Casts = DataController.SandboxEntities.Casts.Where(m => m.MovieID == item.Id).ToList();

                            // for all possible actors see if in cast list
                            foreach (Actor actor in MVVM.CurrentFilter.CurrentActorList)
                            {

                                Cast? tempCast = item.Casts.Where(x => x.ActorId == actor.Id).FirstOrDefault();

                                // if present add movie to new temporary list and stop adding for this movie
                                if (tempCast != null)
                                {
                                    mList.Add(item);
                                    break;
                                }

                            }
                        }
                        // push temporary list into return list
                        tempList = mList;
                    }
                }

                if (ByView)
                {
                    //Views.MainWindow? mainWindow = Support.GetMainWindow() as Views.MainWindow;

                    //// save caller for return
                   
                    //// save current movie list and currentMovie
                    //ObservableCollection<Movies> oldMovieList = MVVM.MovieList;
                    //Movies oldCurrentMovie = MVVM.CurrentMovie;

                    //MovieViewModel viewModel = new MovieViewModel();
                    //viewModel.MovieList = new ObservableCollection<Movies>(tempList); ;
                    //viewModel.CurrentMovie = MVVM.CurrentMovie;

                    //MovieListDialog movieListDialog = new MovieListDialog(viewModel);
                    //viewModel.Caller = movieListDialog;
                    //MVVM.Caller = movieListDialog;

                    //// show on main window
                    //await movieListDialog.ShowDialog(mainWindow);

                    //// restore caller and movie List 
                   
                    //MVVM.MovieList = oldMovieList;
                    //MVVM.CurrentMovie = oldCurrentMovie;
                    //// }


                }
                // populate main view model movielist with new observable list
                else MVVM.MovieList = new System.Collections.ObjectModel.ObservableCollection<Movies>(tempList);
            }
        }

        private List<Movies> FilterByModifiedOn(List<Movies> tempList, string[] actions, DateTime dateValue)
        {
            if (actions[0] == "==")
            {
                tempList = tempList.Where(m => m.ModifiedOn == dateValue).ToList();
            }

            if (actions[0] == ">")
            {
                tempList = tempList.Where(m => m.ModifiedOn > dateValue).ToList();
            }

            if (actions[0] == ">=")
            {
                tempList = tempList.Where(m => m.ModifiedOn >= dateValue).ToList();
            }

            if (actions[0] == "<")
            {
                tempList = tempList.Where(m => m.ModifiedOn < dateValue).ToList();
            }

            if (actions[0] == "<=")
            {
                tempList = tempList.Where(m => m.ModifiedOn <= dateValue).ToList();
            }

            return tempList;
        }

        private List<Movies> FilterByAddedOn(List<Movies> tempList, string[] actions, DateTime dateValue)
        {
            if (actions[0] == "==")
            {
                tempList = tempList.Where(m => m.Added == dateValue).ToList();
            }

            if (actions[0] == ">")
            {
                tempList = tempList.Where(m => m.Added > dateValue).ToList();
            }

            if (actions[0] == ">=")
            {
                tempList = tempList.Where(m => m.Added >= dateValue).ToList();
            }

            if (actions[0] == "<")
            {
                tempList = tempList.Where(m => m.Added < dateValue).ToList();
            }

            if (actions[0] == "<=")
            {
                tempList = tempList.Where(m => m.Added <= dateValue).ToList();
            }

            return tempList;
        }

        private static List<Movies> FilterDurations(List<Movies> tempList, string[] actions, int durationSecs)
        {
            if (actions[0] == "==")
            {
                tempList = tempList.Where(m => m.DurationSeconds == durationSecs).ToList();
            }

            if (actions[0] == ">")
            {
                tempList = tempList.Where(m => m.DurationSeconds > durationSecs).ToList();
            }

            if (actions[0] == ">=")
            {
                tempList = tempList.Where(m => m.DurationSeconds >= durationSecs).ToList();
            }

            if (actions[0] == "<")
            {
                tempList = tempList.Where(m => m.DurationSeconds < durationSecs).ToList();
            }

            if (actions[0] == "<=")
            {
                tempList = tempList.Where(m => m.DurationSeconds <= durationSecs).ToList();
            }

            return tempList;
        }


        /// <summary>
        /// The DoCancel.
        /// </summary>
        internal void Cancel(object? sender, RoutedEventArgs e)
        {
            MVVM.ResultTask = new Dialogs.DialogResultButton();
            MVVM.ResultTask.Result = DialogResultButton.ResultType.Cancel;
            this.Close();
        }

        private void ClearElements(object? sender, RoutedEventArgs e)
        {
            MVVM.CurrentFilter = new Filter();
        }

        internal void Save(object? sender, RoutedEventArgs e)
        {
            MVVM.CurrentFilter.ToJson();
            MVVM.CurrentFilter.Update();
        }

        internal void InsertFilter(object? sender, RoutedEventArgs e)
        {
            try
            {
                MVVM.CurrentFilter.ToJson();
                MVVM.CurrentFilter.Insert();
                MVVM.CurrentFilter.ToJson();
                MVVM.CurrentFilter.Update();
                MVVM.FilterList.Add(MVVM.CurrentFilter);
            }
            catch (Exception ex)
            {

                Support.GenerateInfoAndLogMessage("Creating Filter", "", 0, ex.ToString());
            }
        }
        #endregion
    }
}
