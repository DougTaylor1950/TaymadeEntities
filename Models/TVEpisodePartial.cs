//-----------------------------------------------------------------------
// <copyright file="TVEpisodePartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>13/11/2020 12:52:32 13/11/2020 12:52:32 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="TVEpisode" />.
    /// </summary>
    public partial class TVEpisode
    {
        #region Fields

        /// <summary>
        /// Defines the month.
        /// </summary>
        private string? month;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AirDateString
        /// Gets the AirDateString..
        /// </summary>
        [NotMapped]
        public string AirDateString
        {
            get
            {
                if (AirDate != null)
                {
                    return AirDate.Value.ToString("dd-MMM-yyyy");
                }
                else
                    return "missing";
            }
            set
            {
                if (value != null)
                {
                    if (DateTime.TryParse(value, out DateTime invalue))
                    {
                        AirDate = invalue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Month.
        /// </summary>
        [NotMapped]
        public string Month
        {
            get
            {
                if (AirDate != null && string.IsNullOrEmpty(month))
                {
                    month = AirDate.Value.ToString("MMM");
                }
                return month;
            }

            set => month = value;
        }

        #endregion

        #region Methods

        public async static Task<TVEpisode>? CreateTVEpisodeForMovie(int movieId, Avalonia.Controls.Window window)
        {

            TVEpisode? returnValue = null;
            using TaymadeEntities.ViewModels.EntryDialogModel entryDialogModel =
                    new TaymadeEntities.ViewModels.EntryDialogModel(TaymadeEntities.ViewModels.EntryDialogModel.EntryType.Text);
            using TaymadeEntities.Dialogs.EntryDialog entryDialog = new TaymadeEntities.Dialogs.EntryDialog(entryDialogModel);

            using var _context = new TaymadeEntities.DBContext.SandboxEntities();
            {
                _context.SaveChanges();
                Movies? movie = _context.Movies.Find(movieId);

                if (movie != null && movie.Season != null)
                {
                    if (movie.Series == 2)
                    {
                        Series? series = await Series.AddSeriesToMovie(movie.Id, window);
                        if (series == null) return null;
                        movie.Series = series.Id;
                    }
                    if (movie?.SeriesEntity == null)
                    {
                        // get series entity for this movie
                        Series series = DataController.MovieController.GetSeriesById(movie.Series);
                        movie.SeriesEntity = series;
                    }

                    if (movie.SeriesEntity == null) return null;

                    if (movie.Season == null || movie.Season == 0)
                    {
                        Season? season = await Season.AddSeasonToMovie(movie.Id, window);
                        if (season == null) return null;
                        movie.Season = season.Id;
                        movie.SeasonEntity = season;
                    }

                    if (movie.SeasonEntity == null)
                    {
                        // get season entity for this movie
                        TaymadeEntities.Models.Season season =
                            DataController.MovieController.GetSeasonById(movie.Season);
                        movie.SeasonEntity = season;
                    }

                    if (movie.SeasonEntity == null) return null;

                    if (movie.SeasonEntity.TVEpisodes == null)
                    {
                        movie.SeasonEntity.TVEpisodes =
                            DataController.MovieController.GetTVEpisodesBySeasonID(movie.Season.Value);
                    }


                    if (movie.SeriesEntity == null)
                    {
                        // get series entity for this movie
                        Series series = DataController.MovieController.GetSeriesById(movie.Series);
                        movie.SeriesEntity = series;
                    }

                    TVEpisode episode = new TVEpisode()
                    {
                        ShowID = movie.SeriesEntity?.TMID,
                        SeasonID = movie.Season,
                        MovieId = movie.Id,
                        SeasonNumber = movie.SeasonEntity?.SeasonNo,
                        EpisodeNumber = movie.SeasonEntity?.TVEpisodes?.Count + 1
                    };

                    entryDialog.Title = "Get Name of Episode";
                    entryDialogModel.EntryText = movie.MovieName;

                    DialogResultButton result = await entryDialog.ShowDialog<DialogResultButton>(window);
                    if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                    {
                        string? episodeName = result.Parameter;
                        episode.Name = episodeName;

                        entryDialogModel.EntryText = movie.MovieName;
                        entryDialog.Title = "Get Episode Overview";

                        episode.Overview = result.Parameter;

                        _context.TVEpisodes.Add(episode);
                        _context.SaveChanges();

                        //episode.Insert();
                        movie.Episode = episode.Id;
                        returnValue = episode;
                        //_context.Movies.Update(movie);
                        _context.SaveChanges();
                        //  movie.EpisodeEntity = episode;
                        // movie.Save();

                    }
                    // vm.FoundMovie = movie;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return EpisodeNumber.ToString() + " - " + Name;
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        internal void Delete()
        {
            DataController.MovieController.DeleteTVEpisode(this);
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        public void Insert()
        {
            DataController.MovieController.AddTVEpisode(this);
        }

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(Name) && Name.Length > 50)
            {
                Name = Name.Substring(0, 50);
            }



            if (!string.IsNullOrEmpty(Overview) && Overview.Length > 200)
            {
                Overview = Overview.Substring(0, 200);
            }

            if (AirDate == null)
            {
                AirDate = DateTime.Now;
            }


            DataController.MovieController.UpdateTVEpisode(this);
        }

        #endregion
    }
}
