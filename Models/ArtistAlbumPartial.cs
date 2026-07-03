//-----------------------------------------------------------------------
// <copyright file="ArtistAlbumPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>03/09/2020 15:34:49 03/09/2020 15:34:49 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ArtistAlbum" />.
    /// </summary>
    [MetadataType(typeof(ArtistAlbumMetadata))]
    public partial class ArtistAlbum
    {
        public void Delete()
        {
            DataController.MusicController.DeleteArtistAlbum(this);
        }
        #region Methods

        /// <summary>
        /// The GetDatabaseInfo.
        /// </summary>
        public void GetDatabaseInfo()
        {
            if (Album != null) Album.GetDatabaseAlbums();
            if (Artist != null) Artist.GetArtistInfo();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        public void Insert()
        {
            DataController.MusicController.AddArtistAlbum(this);

           
        }

        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            var local = DataController.MusicEntitiesContext.Set<ArtistAlbum>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

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

    /// <summary>
    /// Defines the <see cref="ArtistAlbumMetadata" />.
    /// </summary>
    public class ArtistAlbumMetadata
    {
    }

    /// <summary>
    /// Defines the <see cref="ArtistAlbumModel" />.
    /// </summary>
    public class ArtistAlbumModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Albums.
        /// </summary>
        public virtual IEnumerable<ArtistAlbum>? Albums { get; set; }

        /// <summary>
        /// Gets or sets the Artist.
        /// </summary>
        public virtual Artist? Artist { get; set; }

        /// <summary>
        /// Gets or sets the Videos.
        /// </summary>
        public virtual IEnumerable<ArtistVideo>? Videos { get; set; }

        #endregion
    }
}
