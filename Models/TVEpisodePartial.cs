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
            DataController.SandboxEntities.Entry(this).State = EntityState.Deleted;
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        internal void Insert()
        {
            DataController.SandboxEntities.Set<TVEpisode>().Add(this);
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            var local = DataController.SandboxEntities.Set<TVEpisode>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DataController.SandboxEntities.SaveChanges();
        }

        #endregion
    }
}
