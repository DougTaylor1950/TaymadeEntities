//-----------------------------------------------------------------------
// <copyright file="MovieProperties.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>06/05/2022 10:46:51 06/05/2022 10:46:51 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="MovieProperties" />.
    /// </summary>
    [Table("MovieProperties")]
    public class MovieProperties
    {
        #region Fields

        /// <summary>
        /// Defines the lastMoveID.
        /// </summary>
        private int? lastMoveID;

        public MovieProperties()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AutoComplete.
        /// </summary>
        public string AutoComplete { get; set; }

        /// <summary>
        /// Gets or sets the EpisodePosition.
        /// </summary>
        public int? EpisodePosition { get; set; }

        /// <summary>
        /// Gets or sets the Group.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Key]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the JSON.
        /// </summary>
        public string? JSON { get; set; }

        /// <summary>
        /// Gets or sets the LastMoveID.
        /// </summary>
        public int? LastMoveID
        {
            get => lastMoveID;
            set
            {
                if (value != null && value > 0)
                    lastMoveID = value;
            }
        }

        /// <summary>
        /// Gets or sets the LastSeason.
        /// </summary>
        public int? LastSeason { get; set; }

        /// <summary>
        /// Gets or sets the LastSeries.
        /// </summary>
        public int? LastSeries { get; set; }

        /// <summary>
        /// Gets or sets the LastStoryId.
        /// </summary>
        public int? LastStoryId { get; set; }

        /// <summary>
        /// Gets or sets the LastTab.
        /// </summary>
        public string? LastTab { get; set; }

        /// <summary>
        /// Gets the MovieSortDirection.
        /// </summary>
        [NotMapped]
        public System.ComponentModel.ListSortDirection MovieSortDirection
        {
            get
            {
                System.ComponentModel.ListSortDirection defaultDirection = System.ComponentModel.ListSortDirection.Ascending;
                if (SortDirection == -1) defaultDirection = System.ComponentModel.ListSortDirection.Descending;
                return defaultDirection;

            }
        }

        /// <summary>
        /// Gets or sets the MovieVolume.
        /// </summary>
        public int? MovieVolume { get; set; }

        /// <summary>
        /// Gets or sets the SongVolume.
        /// </summary>
        public int? SongVolume { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets or sets the SortColumns.
        /// </summary>
        public string? SortColumns { get; set; }

        /// <summary>
        /// Gets or sets the SortColumnString.
        /// </summary>
        public string? SortColumnString { get; set; }

        /// <summary>
        /// Gets or sets the SortDirection.
        /// </summary>
        public int? SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the SortedColumn.
        /// </summary>
        public int? SortedColumn { get; set; }

        /// <summary>
        /// Gets or sets the StorySort.
        /// </summary>
        public int? StorySort { get; set; }

        /// <summary>
        /// Gets or sets the SubGroup.
        /// </summary>
        public string? SubGroup { get; set; }

        /// <summary>
        /// Gets or sets the User.
        /// </summary>
        public string? User { get; set; }
        public int? DefaultFilter { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            try
            {


                //var local = DataController.SandboxEntities.Set<MovieProperties>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                //// check if local is not null
                //if (local != null)
                //{
                //    // detach
                //    DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                //}
                // set Modified flag in your entry

                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();
            }
            catch (System.Exception)
            {

                //throw;
            }
        }

        #endregion
    }
}
