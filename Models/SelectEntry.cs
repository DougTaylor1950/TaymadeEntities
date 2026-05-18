//-----------------------------------------------------------------------
// <copyright file="SelectEntry.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>07/06/2022 22:13:01 07/06/2022 22:13:01 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    /// <summary>
    /// Defines the <see cref="SelectEntry" />.
    /// </summary>
    public class SelectEntry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string? Id { get; set; }

        #endregion

        public override string ToString()
        {

            return Description;
        }
    }
}
