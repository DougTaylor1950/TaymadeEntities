//-----------------------------------------------------------------------
// <copyright file="StoryProperties.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>18/07/2022 22:15:23 18/07/2022 22:15:23 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="StoryProperties" />.
    /// </summary>
    public class StoryProperties : ModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the lastStoryId.
        /// </summary>
        private int? lastStoryId;

        /// <summary>
        /// Defines the sortDirection.
        /// </summary>
        private int? sortDirection;

        /// <summary>
        /// Defines the storySortColumn.
        /// </summary>
        private string? storySortColumn;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Gets or sets the LastStoryId.
        /// </summary>
        public int? LastStoryId { get => lastStoryId; set => this.RaiseAndSetIfChanged(ref lastStoryId, value); }

        public string? Macro { get; set; }

        /// <summary>
        /// Gets or sets the SortDirection.
        /// </summary>
        public int? SortDirection { get => sortDirection; set => this.RaiseAndSetIfChanged(ref sortDirection, value); }

        /// <summary>
        /// Gets or sets the StorySortColumn.
        /// </summary>
        public string? StorySortColumn { get => storySortColumn; set => this.RaiseAndSetIfChanged(ref storySortColumn, value); }

        public DateTime? LastScan { get; set; }

        internal void Save()
        {
            try
            {


                var local = DataController.SandboxEntities.Set<StoryProperties>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                // check if local is not null
                if (local != null)
                {
                    // detach
                    DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                }
                // set Modified flag in your entry



                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();

            }
            catch (Exception)
            {


            }
        }

        [NotMapped]
        public System.ComponentModel.ListSortDirection StorySortDirection
        {
            get
            {
                System.ComponentModel.ListSortDirection defaultDirection = System.ComponentModel.ListSortDirection.Ascending;
                if (SortDirection == -1) defaultDirection = System.ComponentModel.ListSortDirection.Descending;
                return defaultDirection;

            }

            set
            {
                if (value == System.ComponentModel.ListSortDirection.Ascending)
                {
                    SortDirection = 1;
                }
                else
                {
                    SortDirection = -1;
                }
            }
        }

        #endregion
    }
}
