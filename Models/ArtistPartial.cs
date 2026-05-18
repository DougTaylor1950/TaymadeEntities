//-----------------------------------------------------------------------
// <copyright file="ArtistPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>09/07/2020 10:05:10 09/07/2020 10:05:10 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    //using MusicBrainzSupport;
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MusicBrainzSupport;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="Artist" />.
    /// </summary>
    [MetadataType(typeof(ArtistMetadata))]
    public partial class Artist
    {
        #region Properties

        /// <summary>
        /// Gets or sets the DCArtist.
        /// </summary>
        [NotMapped]
        public DCArtist? DCArtist { get; set; }

        /// <summary>
        /// Gets or sets the MBArtist.
        /// </summary>
        [NotMapped]
        public MBArtist? MBArtist { get; set; }

        [NotMapped]
        public string? DCHtml { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The GetArtistInfo.
        /// </summary>
        public void GetArtistInfo()
        {
            if (MBArtist == null && !string.IsNullOrEmpty(MusicBrainzID))
            {
                MBArtist = MusicBrainzSupport.MusicBrainz.GetArtist(MusicBrainzID);
            }

            if (DCArtist == null && !string.IsNullOrEmpty(DiscogsID))
            {
                DCArtist = Discogs.GetArtistDetailsFromId(DiscogsID);

                if (DCArtist != null && !string.IsNullOrEmpty(DCArtist.Uri))
                {
                    DCHtml = Discogs.GetUrl(DCArtist.Uri);
                }
            }
        }

        internal void Save()
        {
            var local = DataController.MusicEntitiesContext.Set<Artist>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.MusicEntitiesContext.Entry(local).State = EntityState.Detached;
            }
            // stop group members being saved
            foreach (var item in GroupMembers)
            {
                DataController.MusicEntitiesContext.Entry(item).State = EntityState.Detached;
            }

            // set Modified flag in your entry
            //ModifiedOn = DateTime.Now;
            DataController.MusicEntitiesContext.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DataController.MusicEntitiesContext.SaveChanges();
        }

        internal void Insert()
        {


            DataController.MusicEntitiesContext.Add(this);
            DataController.MusicEntitiesContext.SaveChanges();
        }

        #endregion
    }

    public class ArtistMetadata
    {
        [Display(Name="Artist ID")]
        public int Id { get; set; }

        [Display(Name = "Artist Name")]
        [Required(ErrorMessage = "Artist Name must be defined")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Artist Image Path")]
        public string PhotoPath { get; set; } = string.Empty;

        [Display(Name = "MusicBrianz ID")]
        public string MusicBrainzID { get; set; } = string.Empty;

        [Display(Name = "Plex Key")]
        public string PlexKey { get; set; } = string.Empty;

        [Display(Name = "Artist Path")]
        [Required(ErrorMessage = "Artist Path must be defined")]
        public string ArtistPath { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Artist Type")]
        public string ArtistType { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [Display(Name = "Died on")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DeathDate { get; set; }

        [Display(Name = "Is Soloist")]
        public Nullable<bool> Soloist { get; set; }

        [Display(Name = "Wiki Page ID")]
        public string WIKIPageID { get; set; } = string.Empty;

        [Display(Name = "Discogs ID")]
        public string DiscogsID { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [Display(Name = "Artist Albums")]
        public virtual ICollection<ArtistAlbum>? ArtistAlbums { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        //[Display(Name = "Group Members")]
        //public virtual ICollection<GroupMember> GroupMembers { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        //[Display(Name = "Group Members")]
        //public virtual ICollection<GroupMember> GroupMembers1 { get; set; }
    }
}
