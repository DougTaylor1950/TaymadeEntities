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
    /// Defines the <see cref="CliWrapErrorEventArgs" />.
    /// </summary>
    public class CliWrapErrorEventArgs : System.EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CliWrapErrorEventArgs"/> class.
        /// </summary>
        /// <param name="ex">The ex<see cref="Exception"/>.</param>
        /// <param name="userState">The userState<see cref="object?"/>.</param>
        /// <param name="taskName">The taskName<see cref="string?"/>.</param>
        public CliWrapErrorEventArgs(Exception ex, object? userState, string? taskName) : base()
        {
            Exception = ex;
            TaskName = taskName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the TaskName.
        /// </summary>
        public string? TaskName { get; internal set; }

        public int? ErrorCode { get; set; }

        public string? ErrorString { get; set; }

        #endregion
    }
}
