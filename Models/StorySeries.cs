//-----------------------------------------------------------------------
// <copyright file="StorySeries.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>03/01/2023 12:10:19 03/01/2023 12:10:19 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines the <see cref="StorySeries" />.
    /// </summary>
    public class StorySeries : ModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the StoryList.
        /// </summary>
        [NotMapped]
        public List<Story> StoryList { get; set; }

        #endregion

        #region Methods

        public void Save()
        {

        }

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="StorySeriesCollection" />.
    /// </summary>
    public class StorySeriesCollection : List<StorySeries>
    {
    }
}
