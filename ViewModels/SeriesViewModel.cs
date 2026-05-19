//-----------------------------------------------------------------------
// <copyright file="SeriesViewModel.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>28/04/2022 12:14:55 28/04/2022 12:14:55 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.ViewModels
{
    using Avalonia.Controls;
    using TaymadeEntities.Models;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;

    /// <summary>
    /// Defines the <see cref="SeriesViewModel" />.
    /// </summary>
    public class SeriesViewModel : MovieEditViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the caller.
        /// </summary>
        private Window? caller;

        /// <summary>
        /// Defines the currentEpisode.
        /// </summary>
        private Support.EpisodeDetails? currentEpisode;

        /// <summary>
        /// Defines the currentSeason.
        /// </summary>
        private Support.Season? currentSeason;

        /// <summary>
        /// Defines the episodeList.
        /// </summary>
        private ObservableCollection<TVEpisode>? episodeList;



        /// <summary>
        /// Defines the seasonEntity.
        /// </summary>
        private Season? seasonEntity;

        /// <summary>
        /// Defines the seasonList.
        /// </summary>
        private List<Season>? seasonList;

        /// <summary>
        /// Defines the CurrentSeries.
        /// </summary>
        //private Series? seriesEntity;

        /// <summary>
        /// Defines the showVisible.
        /// </summary>
        private bool showVisible = false;
        private string? seriesName = "<new>";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesViewModel"/> class.
        /// </summary>
        public SeriesViewModel()
        {
            SetupCommands();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesViewModel"/> class.
        /// </summary>
        /// <param name="window">The window<see cref="Window"/>.</param>
        public SeriesViewModel(Window window)
        {
            caller = window;
            SetupCommands();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the AddEpisode.
        /// </summary>
        public new ReactiveCommand<Unit, Unit> AddEpisode { get; set; }

        public ReactiveCommand<Unit, Unit> ToMovie { get; set; }
        public ReactiveCommand<Unit, Unit> PlayEpisodeMovie { get; set; }
        public ReactiveCommand<Unit, Unit> SearchForMovie { get; set; }
        public ReactiveCommand<Unit, Unit> CreateEpisode { get; set; }

        public ReactiveCommand<Unit, Unit> CreateSeason { get; set; }

        public ReactiveCommand<Unit, Unit> SaveEpisode { get; set; }

        public ReactiveCommand<Unit, Unit> SaveSeason { get; set; }

        /// <summary>
        /// Gets or sets the AddSeason.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddSeason { get; set; }

        /// <summary>
        /// Gets or sets the CurrentEpisode.
        /// </summary>
        public Support.EpisodeDetails? CurrentSupportEpisode { get => currentEpisode; set => currentEpisode = value; }

        /// <summary>
        /// Gets or sets the CurrentSeason.
        /// </summary>
        //public new Support.Season? CurrentSeason
        //{
        //    get => currentSeason;
        //    set
        //    {
        //        this.RaiseAndSetIfChanged(ref currentSeason, value);
        //        // if not null set seasonMovies

        //        if (value != null)
        //        {
        //            this.SeasonMovies = new ObservableCollection<Movies>(
        //                DataController.SandboxEntities.Movies.Where( m => m.Season == value.).ToList()
        //                );
        //        }


        //    }
        //}

        /// <summary>
        /// Gets or sets the EpisodeEntity.
        /// </summary>
        public new TVEpisode? CurrentEpisode { get; internal set; }

        /// <summary>
        /// Gets or sets the EpisodeList.
        /// </summary>
        //public ObservableCollection<TVEpisode>? EpisodeList { get => episodeList; set => this.RaiseAndSetIfChanged(ref episodeList, value); }

        /// <summary>
        /// Gets or sets the NewSeries.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewSeries { get; set; }

        /// <summary>
        /// Gets or sets the PlayMovie
        /// Gets the PlayMovie..
        /// </summary>
        public ReactiveCommand<Unit, Unit> PlayMovie { get; set; }

        /// <summary>
        /// Gets or sets the SeasonEntity.
        /// </summary>
        public Models.Season? SeasonEntity { get => seasonEntity; set => this.RaiseAndSetIfChanged(ref seasonEntity, value); }

        /// <summary>
        /// Gets or sets the SeasonList.
        /// </summary>
        public List<Models.Season>? SeasonList { get => seasonList; set => this.RaiseAndSetIfChanged(ref seasonList, value); }

        /// <summary>
        /// Gets or sets the SeriesEntity.
        /// </summary>
       // public Models.Series? SeriesEntity { get => seriesEntity; set => this.RaiseAndSetIfChanged(ref seriesEntity, value); }

        /// <summary>
        /// Gets or sets the SeriesList.
        /// </summary>
       // public List<Models.Series>? SeriesList { get; set; }

        /// <summary>
        /// Gets or sets the SeriesName.
        /// </summary>
        public string? SeriesName { get => seriesName; set => this.RaiseAndSetIfChanged(ref seriesName, value); }

        /// <summary>
        /// Gets or sets a value indicating whether ShowVisible.
        /// </summary>
        public new bool ShowVisible { get => showVisible; set => this.RaiseAndSetIfChanged(ref showVisible, value); }

        #endregion

        #region Methods

        /// <summary>
        /// The DoAddEpisode.
        /// </summary>
        private void DoAddEpisode()
        {

            if (CurrentSeason != null && CurrentSupportEpisode != null)
            {
                TVEpisode? newEpisode = CurrentSeason.TVEpisodes.Where(s => s.EpisodeNumber == CurrentSupportEpisode.EpisodeNumber).FirstOrDefault();

                if (newEpisode != null)
                {
                    // refresh data
                    newEpisode.TMID = CurrentSupportEpisode.Id;
                    newEpisode.Name = CurrentSupportEpisode.Name;
                }
                else
                {
                    newEpisode = new TVEpisode();
                    newEpisode.TMID = CurrentSupportEpisode.Id;
                    newEpisode.Name = CurrentSupportEpisode.Name;
                    newEpisode.EpisodeNumber = CurrentSupportEpisode.EpisodeNumber;

                    newEpisode.AirDate = CurrentSupportEpisode.AirDate;
                    newEpisode.Overview = CurrentSupportEpisode.Overview;
                    newEpisode.SeasonID = CurrentSeason.Id;
                    newEpisode.SeasonNumber = CurrentSupportEpisode.SeasonNumber;
                    newEpisode.ShowID = CurrentSupportEpisode.ShowId;

                    newEpisode.Insert();
                    CurrentSeason.TVEpisodes.Add(newEpisode);
                }

            }
        }

        /// <summary>
        /// The DoAddSeason.
        /// </summary>
        private void DoAddSeason()
        {
            if (CurrentSeries != null)
            {
                if (CurrentSeason != null)
                {
                    Season? newSeason = CurrentSeries.Seasons.Where(s => s.SeasonNo != null && s.SeasonNo == CurrentSeason.SeasonNo).FirstOrDefault();

                    if (newSeason != null)
                    {
                        // refresh data
                        newSeason.TMDBID = CurrentSeries.TMID;

                    }

                    else
                    {
                        NewSeason = new Season(CurrentSeries);

                        NewSeason.ShowId = CurrentSeries.TMID;
                        NewSeason.Series = CurrentSeries.Id;
                        NewSeason.SeasonNo = CurrentSeason.SeasonNo;
                        NewSeason.Name = CurrentSeason.Name;

                        NewSeason.Insert();

                    }

                    CurrentSeries.Seasons.Add(NewSeason);
                }
            }
        }

        /// <summary>
        /// The DoCreateSeries.
        /// </summary>
        private void DoCreateSeries()
        {
            string newSeriesName = "<new>";

            // check to see if name supplied

            if (!string.IsNullOrEmpty(this.SeriesName)) newSeriesName = SeriesName;

            Series? newSeries = DataController.SandboxEntities.CreateSeries(newSeriesName);


            if (newSeries != null)
            {
                newSeries.Save();

                this.SeriesList.Add(newSeries);

                List<Support.TVShow> shows = Support.TmdbSupport.SearchTVList(newSeriesName);

                // see if we have found anything

                if (shows != null)
                {
                    if (shows.Count == 1)
                    {
                        newSeries.TMID = shows[0].ShowID;
                        newSeries.Save();
                    }
                }
            }


        }

        /// <summary>
        /// The Run_Click.
        /// </summary>
        private void Run_Click()
        {
            //if (EpisodeEntity != null)
            //{
            //    int? movieid = EpisodeEntity.MovieId;

            //    string moviePath = string.Empty;

            //    if (movieid != null && movieid.Value > 0)
            //    {
            //        // the Episode should really have a movie at this stage
            //        if (EpisodeEntity.Movie == null)
            //            EpisodeEntity.Movie = DataController.SandboxEntities.Movies.Find(movieid.Value);

            //        // check yet again or bail if not there
            //        if (EpisodeEntity.Movie != null)
            //        {
            //            moviePath = this.PlayMovieFromPath();
            //        }
            //    }
            //}
        }

        private string PlayMovieFromPath()
        {
            string moviePath = Support.Support.FixImagePath(CurrentEpisode.Movie.MoviePath);
            if (!string.IsNullOrEmpty(moviePath))
            {
                Uri uri = new(moviePath);

                string path = string.Empty;

                string os = Support.Support.GetOS();

                if (os == "WinNT")
                {
                    path = @"C:\Program Files(x86)\VideoLAN\VLC\vlc.exe";
                }
                else
                {
                    path = "/snap/bin/vlc";
                }

                ProcessStartInfo psi = new(path)
                { Arguments = '"' + moviePath + '"' };

                Process? proc = Process.Start(psi);
            }

            return moviePath;
        }

        /// <summary>
        /// The SetUpButtons.
        /// </summary>
        private void SetUpButtons()
        {
        }

        /// <summary>
        /// The SetupCommands.
        /// </summary>
        private void SetupCommands()
        {
            PlayMovie = ReactiveCommand.Create(Run_Click);
            PlayEpisodeMovie = ReactiveCommand.Create(DoPlayEpisodeMovie);
            AddSeason = ReactiveCommand.Create(DoAddSeason);
            AddEpisode = ReactiveCommand.Create(DoAddEpisode);
            ToMovie = ReactiveCommand.Create(DoToMovie);
            CreateEpisode = ReactiveCommand.Create(DoCreateEpisode);
            CreateSeason = ReactiveCommand.Create(DoCreateSeason);
            SaveEpisode = ReactiveCommand.Create(DoSaveEpisode);
            SaveSeason = ReactiveCommand.Create(DoSaveSeason);
           
            NewSeries = ReactiveCommand.Create(DoCreateSeries);
            SearchForMovie = ReactiveCommand.Create(DoSearchForMovie);
            SetUpButtons();
        }

        private void DoToMovie()
        {
            if (CurrentEpisode != null && CurrentEpisode.Movie != null)
            {
                CurrentEpisode.Movie.Season = CurrentEpisode.SeasonID;
                CurrentEpisode.Movie.Episode = CurrentEpisode.EpisodeNumber;
                CurrentEpisode.Movie.EpisodeNumber = CurrentEpisode.EpisodeNumber;
                CurrentEpisode.Movie.Info = CurrentEpisode.Overview;
                CurrentEpisode.Movie.Save();
            }
        }

        private void DoPlayEpisodeMovie()
        {
            if (CurrentEpisode != null && CurrentEpisode.MovieId > 0)
            {
                //if (CurrentEpisode.Movie == null)
                CurrentEpisode.Movie =
                        DataController.SandboxEntities.Movies.Find(CurrentEpisode.MovieId);
                if (CurrentEpisode.Movie != null)
                {
                    Support.Support.PlayMovie(CurrentEpisode.Movie.MoviePath, null);
                }
            }

        }

        private void DoSearchForMovie()
        {
            // look for a movie with the same series id as the current series same season id as the current season and the same episode number;

            if (CurrentSeries != null)
            {
                Movies? foundMovie = null;
                List<Movies>? movies = DataController.SandboxEntities.Movies.Where(m => m.Series == this.CurrentSeries.Id).ToList();
                if (movies != null && CurrentSeason != null)
                {
                    movies = movies.Where(s => s.Season == CurrentSeason.Id).ToList();

                    if (!movies.Any() && CurrentEpisode != null && CurrentEpisode.EpisodeNumber != null)
                    {
                        foundMovie = movies.Where(e => e.EpisodeNumber == CurrentEpisode.EpisodeNumber).ToList().FirstOrDefault();

                        // now set the value if not null
                        if (foundMovie != null)
                        {
                            CurrentEpisode.MovieId = foundMovie.Id;
                            CurrentEpisode.Save();
                        }
                        else
                        {
                            foundMovie = this.LookByName(movies);
                        }
                    }
                    else
                    {
                        foundMovie = this.LookByName(movies);
                    }
                }
            }

            //&& m.Season.Value == this.CurrentSeason.SeasonNo.Value && m.EpisodeNumber.Value = CurrentEpisode.EpisodeNumber.Value).FirstOrDefault();
        }

        private Movies? LookByName(List<Movies> movies)
        {
            if (CurrentSeason != null && CurrentEpisode != null)
            {
                string pathId = "S" + CurrentSeason.SeasonNo.ToString().PadLeft(2, '0').Trim() + "E" + CurrentEpisode.EpisodeNumber.ToString().PadLeft(2, '0').Trim();

                Movies? foundMovie = DataController.SandboxEntities.Movies.Where(m => m.MovieName.ToLower().Contains(CurrentEpisode.Name.ToLower())).FirstOrDefault();
                if (foundMovie != null)
                {
                    this.SetDetails(foundMovie);
                }
                else
                {
                    foundMovie = DataController.SandboxEntities.Movies.Where(
                        m => m.MoviePath.Contains(pathId)
                    && m.MoviePath.Contains(CurrentSeries.Path)
                    ).FirstOrDefault();
                    if (foundMovie != null)
                    {
                        this.SetDetails(foundMovie);
                    }
                }

                return foundMovie;
            }
            else return null;
        }

        private void SetDetails(Movies? foundMovie)
        {
            CurrentEpisode.MovieId = foundMovie.Id;
            CurrentEpisode.Save();
            // fix up movie
            foundMovie.Series = CurrentSeries.Id;
            foundMovie.Season = CurrentSeason.Id;
            foundMovie.EpisodeNumber = CurrentEpisode.EpisodeNumber;
            foundMovie.Episode = CurrentEpisode.Id;
            foundMovie.Info = CurrentEpisode.Overview;
            foundMovie.Save();
        }

        private void DoCreateSeason()
        {
            if (CurrentSeries != null)
            {
                CurrentSeason = new Season()
                {
                    Series = CurrentSeries.Id
                };
                CurrentSeason.Insert();
                CurrentSeries.Seasons.Add(CurrentSeason);
                CurrentSeason.SeasonNo = CurrentSeries.Seasons.Count;
                CurrentSeason.Save();
            }

        }

        private void DoSaveSeason()
        {
            if (CurrentSeason != null) CurrentSeason.Save();
        }

        private void DoSaveEpisode()
        {
            //if (EpisodeEntity != null)
            //{
            //    EpisodeEntity.Save();
            //}
        }

        private void DoCreateEpisode()
        {
            if (CurrentSeason != null && CurrentSeries != null)
            {
                //EpisodeEntity = new TVEpisode()
                //{
                //    ShowID = CurrentSeries.TMID,
                //    SeasonNumber = CurrentSeason.SeasonNo,
                //    SeasonID = CurrentSeason.Id
                //};
                //EpisodeEntity.Insert();
                //CurrentSeason.TVEpisodes.Add(EpisodeEntity);
            }
        }

        #endregion
    }
}
