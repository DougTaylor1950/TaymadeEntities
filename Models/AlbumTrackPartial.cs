//-----------------------------------------------------------------------
// <copyright file="AlbumTrackPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>29/06/2020 08:59:41 29/06/2020 08:59:41 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TaymadeEntities.Models
{
    /// <summary>
    /// Defines the <see cref="AlbumTrack" />.
    /// </summary>
    [MetadataType(typeof(AlbumTrackMetadata))]
    public partial class AlbumTrack
    {
        #region Fields

        /// <summary>
        /// Defines the playing.
        /// </summary>
        private string playing = "not playing";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ArtistId.
        /// </summary>
        [NotMapped]
        public int? ArtistId { get; set; }

        [NotMapped]
        public int? ArtistAlbumId { get; set; }

        [NotMapped]
        public string CommentHTML { get
            {
                if (!string.IsNullOrEmpty(Comment))
                {
                    return Comment.Replace("|", "<br/>");
                }
                else return string.Empty;
                    }
        }


        [NotMapped]
        public string TrackDuration
        {
            get
            {
                string trackDuration = string.Empty;

                if (Duration != null  && Duration.Value > 0)
                {
                    trackDuration = TimeSpan.FromMilliseconds(Duration.Value).ToString();
                }
                return trackDuration;
            }
        }

        /// <summary>
        /// Gets or sets the Error.
        /// </summary>
        [NotMapped]
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Mode.
        /// </summary>
        [NotMapped]
        public string Mode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Playing.
        /// </summary>
        [NotMapped]
        public string Playing
        {
            get => playing;
            set => playing = value;
        }

        /// <summary>
        /// Gets or sets the ReturnUrl.
        /// </summary>
        [NotMapped]
        public string ReturnUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TrackImageURL.
        /// </summary>
        [NotMapped]
        public string TrackImageURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets the TrackNoValue.
        /// </summary>
        [NotMapped]
        public int TrackNoValue
        {
            get
            {
                if (TrackNo != null)
                {
                    return TrackNo.Value;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the TrackURL.
        /// </summary>
        [NotMapped]
        public string TrackURL { get; set; } = string.Empty;

        public bool Update()
        {
            return DataController.MusicController.UpdateTrack(this);
        }

        #endregion

        public void Insert()
        {
            //var local = DataController.MusicEntitiesContext.Set<AlbumTrack>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            //// check if local is not null
            //if (local != null)
            //{
            //    // detach
            //    DataController.MusicEntitiesContext.Entry(local).State = EntityState.Detached;
            //}

            //DataController.MusicEntitiesContext.AlbumTracks.Add(this);
            //DataController.MusicEntitiesContext.SaveChanges();

            DataController.MusicController.AddTrack(this);
        }

        public void Save()
        {
            //var local = DataController.MusicEntitiesContext.Set<AlbumTrack>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            //// check if local is not null
            //if (local != null)
            //{
            //    // detach
            //    DataController.MusicEntitiesContext.Entry(local).State = EntityState.Detached;
            //}
            //// set Modified flag in your entry
            ////ModifiedOn = DateTime.Now;
            //DataController.MusicEntitiesContext.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //DataController.MusicEntitiesContext.SaveChanges();
            DataController.MusicController.Save();


            // ClearErrors();
            // TimeSpan ts = MovieDuration;
        }

    }

    public class AlbumTrackMetadata
    {
        [Display(Name = "Album Track Id")]
        public int Id { get; set; }

        [Display(Name = "Album Id")]
        public int AlbumID { get; set; }

        [Display(Name = "Track Name")]
        public string TrackName { get; set; } = string.Empty;

        [Display(Name = "Track Path")]
        public string TrackPath { get; set; } = string.Empty;

        [Display(Name = "Track No")]
        public Nullable<int> TrackNo { get; set; }

        [Display(Name = "Duration millisecs")]
        public Nullable<int> Duration { get; set; }

        [Display(Name = "Image Path")]
        public string ImagePath { get; set; } = string.Empty;

        [Display(Name = "Comment")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "Rating")]
        public Nullable<decimal> Rating { get; set; }

        [Display(Name = "MusicBrianz Id")]
        public string MusicBrainzID { get; set; } = string.Empty;

        [Display(Name = "Plex Key")]
        public string PlexKey { get; set; } = string.Empty;

        [Display(Name = "Discogs Id")]
        public string DiscogsID { get; set; } = string.Empty;

        [Display(Name = "Track Position")]
        public string TrackPosition { get; set; } = string.Empty;

        [Display(Name = "Album")]
        public virtual Album? Album { get; set; }
    }
}
