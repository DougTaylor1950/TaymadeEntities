//-----------------------------------------------------------------------
// <copyright file="FFMpegSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>14/05/2022 14:53:01 14/05/2022 14:53:01 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using System;

    /// <summary>
    /// Defines the <see cref="ConversionCompleteEventArgs" />.
    /// </summary>
    public class ConversionCompleteEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Action.
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ExitCode.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Gets or sets the Filename.
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MovieId.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the Output.
        /// </summary>
        public string Output { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TimeTaken.
        /// </summary>
        public TimeSpan TimeTaken { get; set; }

        #endregion
    }
}
