//-----------------------------------------------------------------------
// <copyright file="GroupMembers.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>10/06/2022 16:34:06 10/06/2022 16:34:06 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="GroupMembers" />.
    /// </summary>
    [Table("GroupMembers")]
    public class GroupMembers : ModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the artist.
        /// </summary>
        private Artist? artist;

        /// <summary>
        /// Defines the artistGroup.
        /// </summary>
        private Artist? artistGroup;

        /// <summary>
        /// Defines the artistId.
        /// </summary>
        private int artistId;

        /// <summary>
        /// Defines the groupId.
        /// </summary>
        private int groupId;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Artist.
        /// </summary>
        [ForeignKey("ArtistId")]
        public Artist? Artist { get => artist; set => this.RaiseAndSetIfChanged(ref artist, value); }

        /// <summary>
        /// Gets or sets the ArtistGroup.
        /// </summary>
        [ForeignKey("GroupId")]
        public Artist? ArtistGroup { get => artistGroup; set => this.RaiseAndSetIfChanged(ref artistGroup, value); }

        /// <summary>
        /// Gets or sets the ArtistId.
        /// </summary>
        public int ArtistId { get => artistId; set => this.RaiseAndSetIfChanged(ref artistId, value); }

        /// <summary>
        /// Gets or sets the GroupId.
        /// </summary>
        public int GroupId { get => groupId; set => this.RaiseAndSetIfChanged(ref groupId, value); }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Key]
        public new int Id { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The Insert.
        /// </summary>
        internal void Insert()
        {
            DataController.MusicEntitiesContext.GroupMembers.Add(this);
            DataController.MusicEntitiesContext.SaveChanges();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            var local = DataController.MusicEntitiesContext.Set<GroupMembers>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.MusicEntitiesContext.Entry(local).State = EntityState.Detached;
            }


            // set Modified flag in your entry
            //ModifiedOn = DateTime.Now;
            DataController.MusicEntitiesContext.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DataController.MusicEntitiesContext.SaveChanges();
        }

        #endregion
    }
}
