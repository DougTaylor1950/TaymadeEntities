//-----------------------------------------------------------------------
// <copyright file="MappedDrives.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>04/07/2023 15:09:57 04/07/2023 15:09:57 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    /// <summary>
    /// Defines the <see cref="MappedDrives" />.
    /// </summary>
    public class MappedDrives : ModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Computer.
        /// </summary>
        public string? Computer { get; set; }

        /// <summary>
        /// Gets or sets the DestinationDrive.
        /// </summary>
        public string? DestinationDrive { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        //public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the Reversible.
        /// </summary>
        public bool? Reversible { get; set; }

        /// <summary>Gets or sets the SourceDrive.</summary>
        public string? SourceDrive { get; set; }

        /// <summary>Gets or sets the type of the location.</summary>
        /// <value>The type of the location.</value>
        /// 
        public string? LocationType { get; set; }

        #endregion
    }
}
