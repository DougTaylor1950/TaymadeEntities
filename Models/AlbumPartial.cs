//-----------------------------------------------------------------------
// <copyright file="AlbumPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>09/07/2020 09:38:05 09/07/2020 09:38:05 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using Microsoft.EntityFrameworkCore;
    // using MusicBrainzSupport;
    // using PagedList;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MusicBrainzSupport;
    using AvalonMVVM.Support;


    /// <summary>
    /// Defines the <see cref="Album" />.
    /// </summary>
    [MetadataType(typeof(AlbumMetadata))]
    public partial class Album
    {
        private string sortName;
        #region Fields

        /// <summary>
        /// Defines the includeHTML.
        /// </summary>
        private bool includeHTML = false;
        private Avalonia.Media.Imaging.Bitmap? imageBMP;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the ImageBMP.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.Imaging.Bitmap? ImageBMP
        {
            get
            {
                if (!string.IsNullOrEmpty(Support.FixImagePath(this.AlbumPhotPath)))
                {
                    string fileName = Support.FixImagePath(this.AlbumPhotPath);
                    if (System.IO.File.Exists(fileName) && imageBMP == null)
                    {
                        imageBMP = Support.GetBMP(fileName);
                    }
                }
                return imageBMP;
            }
            set => imageBMP = value; //this.RaiseAndSetIfChanged(ref imageBMP, value);
        }
        /// <summary>
        /// Gets or sets the AlbumImageUrl.
        /// </summary>
        [NotMapped]
        public string AlbumImageUrl { get; set; }

        [NotMapped]
        public string SortName
        {
            get
            {
                sortName = AlbumName.Trim();
                while (sortName[0] < '0')
                {
                    sortName = sortName.Substring(1, sortName.Length - 1);
                }

                return sortName;
            }

            set
            {
                sortName = value;
            }
        }

        /// <summary>
        /// Gets or sets the ArtistId.
        /// </summary>
        [NotMapped]
        public int? ArtistId { get; set; }

        /// <summary>
        /// Gets or sets the DCHTML.
        /// </summary>
        [NotMapped]
        public string DCHTML { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncludeHTML.
        /// </summary>
        [NotMapped]
        public bool IncludeHTML { get => includeHTML; set => includeHTML = value; }



        /// <summary>
        /// Gets or sets the ReturnUrl.
        /// </summary>
        [NotMapped]
        public string ReturnUrl { get; set; }

        [NotMapped]
        public MBAlbum MBAlbum { get;  set; }

        [NotMapped]
        public DCAlbumDetails DCAlbum { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get Album info from online Databases Discogs and MusicBrainz.
        /// </summary>
        public async void GetDatabaseAlbums()
        {
            // get MusicBrianz info
            if (!string.IsNullOrEmpty(MusicBrainzID) && MBAlbum == null)
            {
                    MBAlbum = await MusicBrainzSupport.MusicBrainz.GetAlbumAsync(MusicBrainzID);
            }

            if (!string.IsNullOrEmpty(DiscogsID) && DCAlbum == null)
            {
                //string url = "https://api.discogs.com/masters/" + this.DiscogsID;

                // we can have two forms of ID url or simple ID
                if (DiscogsID.Length < 15)
                {
                    DCAlbum = MusicBrainzSupport.Discogs.GetAlbumDetailsFromId(DiscogsID);
                }
                else
                {
                    DCAlbum = MusicBrainzSupport.Discogs.GetAlbumDetailsFromUrl(DiscogsID);

                    if (!string.IsNullOrEmpty(DiscogsReleaseID) && DiscogsReleaseID != DiscogsID)
                    {
                        DCAlbumDetails releaseAlbum = MusicBrainzSupport.Discogs.GetAlbumDetailsFromUrl(DiscogsReleaseID);
                        if (releaseAlbum != null)
                        {
                            DCAlbum.Tracks = releaseAlbum.Tracks;
                        }
                    }
                }

                if (IncludeHTML && DCAlbum != null && !string.IsNullOrEmpty(DCAlbum.Uri))
                {
                    DCHTML = Discogs.GetUrl(DCAlbum.Uri);
                }
            }
        }

        /// <summary>
        /// The NextTrack.
        /// </summary>
        /// <param name="trackNo">The trackNo<see cref="int"/>.</param>
        /// <returns>The <see cref="AlbumTrack"/>.</returns>
        public AlbumTrack NextTrack(int trackNo)
        {
            AlbumTrack? nextTrack = null;
            SortTracks();
            foreach (AlbumTrack item in AlbumTracks)
            {
                if (item.TrackNo >= trackNo)
                {
                    nextTrack = item;
                    break;
                }
            }
            return nextTrack;
        }

        /// <summary>
        /// The SortTracks.
        /// </summary>
        public void SortTracks()
        {
            List<AlbumTrack> tracks = AlbumTracks.ToList();
            tracks.Sort((x, y) => x.TrackNoValue.CompareTo(y.TrackNoValue));
            AlbumTracks = tracks;
        }

        /// <summary>
        /// The SortTracks.
        /// </summary>
        /// <param name="artistAlbumId">The artistAlbumId<see cref="int?"/>.</param>
        public void SortTracks(int? artistAlbumId)
        {
            List<AlbumTrack> tracks = AlbumTracks.ToList();
            tracks.Sort((x, y) => x.TrackNoValue.CompareTo(y.TrackNoValue));
            AlbumTracks = tracks;

            if (artistAlbumId != null)
            {
                foreach (AlbumTrack track in AlbumTracks)
                {
                    track.ArtistAlbumId = artistAlbumId;
                }
            }
        }

        internal void Insert()
        {
            DataController.SandboxEntities.Album.Add(this);
            DataController.SandboxEntities.SaveChanges();
        }

        internal void Save()
        {
            var local = DataController.MusicEntitiesContext.Set<Album>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

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


           // ClearErrors();
           // TimeSpan ts = MovieDuration;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="AlbumMetadata" />.
    /// </summary>
    public class AlbumMetadata
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AlbumName.
        /// </summary>
        [Required(ErrorMessage = "Album Name must be defined")]
        [Display(Name = "Album Name")]
        public string? AlbumName { get; set; }

        /// <summary>
        /// Gets or sets the AlbumPath.
        /// </summary>
        [Required(ErrorMessage = "Album Path must be defined")]
        [Display(Name = "Album Path")]
        public string? AlbumPath { get; set; }

        /// <summary>
        /// Gets or sets the AlbumPhotPath.
        /// </summary>
        [Display(Name = "Album Image Path")]
        public string? AlbumPhotPath { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets the DiscNo.
        /// </summary>
        [Display(Name = "Disc No")]
        public Nullable<int> DiscNo { get; set; }

        /// <summary>
        /// Gets or sets the DiscogsID.
        /// </summary>
        [Display(Name = "Discogs ID")]
        public string? DiscogsID { get; set; }

        /// <summary>
        /// Gets or sets the DiscogsReleaseID.
        /// </summary>
        [Display(Name = "Discogs Release ID")]
        public string? DiscogsReleaseID { get; set; }

        /// <summary>
        /// Gets or sets the Genre.
        /// </summary>
        public string? Genre { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Display(Name = "Album ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the MusicBrainzID.
        /// </summary>
        [Display(Name = "MusicBrainz ID")]
        public string? MusicBrainzID { get; set; }

        /// <summary>
        /// Gets or sets the musicBrainzReleaseGroupID.
        /// </summary>
        [Display(Name = "MusicBrainz Release ID")]
        public string? musicBrainzReleaseGroupID { get; set; }

        /// <summary>
        /// Gets or sets the PlayListPath.
        /// </summary>
        [Display(Name = "PlayList Path")]
        public string? PlayListPath { get; set; }

        /// <summary>
        /// Gets or sets the PlexKey.
        /// </summary>
        [Display(Name = "Plex Key")]
        public string? PlexKey { get; set; }

        /// <summary>
        /// Gets or sets the PrimaryArtistID.
        /// </summary>
        [Display(Name = "MusicBrainz Artist ID")]
        public Nullable<int> PrimaryArtistID { get; set; }

        /// <summary>
        /// Gets or sets the Rating.
        /// </summary>
        [Display(Name = "Rating")]
        public Nullable<decimal> Rating { get; set; }

        /// <summary>
        /// Gets or sets the WIKIPageID.
        /// </summary>
        [Display(Name = "Wiki Page ID")]
        public string? WIKIPageID { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        [Display(Name = "Release Year")]
        public Nullable<int> Year { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="AlbumViewModel" />.
    /// </summary>
    //public class AlbumViewModel
    //{
    //    #region Properties
    //    /// <summary>
    //    /// Gets or sets the Page.
    //    /// </summary>
    //    public int? Page { get; set; }

    //    /// <summary>
    //    /// Gets or sets the ActorGroups.
    //    /// </summary>
    //    public List<AlbumGroup> AlbumGroups { get; set; }

    //    /// <summary>
    //    /// Gets or sets the Alphas.
    //    /// </summary>
    //    public List<string> Alphas { get; set; }

    //    /// <summary>
    //    /// Gets or sets the SelectedGroup.
    //    /// </summary>
    //    public AlbumGroup SelectedGroup { get; set; }

    //    #endregion
    //}

    public class AlbumGroup
    {
        public string? Key { get; set; }

       // public IPagedList<Album> AlbumList { get; set; }
    }

}
