//-----------------------------------------------------------------------
// <copyright file="SeasonPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>06/05/2022 13:44:43 06/05/2022 13:44:43 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Season" />.
    /// </summary>
    public partial class Season
    {
        #region Methods

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return SeasonNo.ToString() + " - " + Name + "." + Year.ToString();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        internal void Delete()
        {
            DataController.MovieController.DeleteSeason(this);
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        public void Insert()
        {
            DataController.MovieController.AddSeason(this);
        }

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            DataController.MovieController.UpdateSeason(this);
        }

        #endregion
    }
}
