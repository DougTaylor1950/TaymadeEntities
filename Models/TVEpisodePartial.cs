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
