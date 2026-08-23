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
    /// Defines the <see cref="CliWrapCompletedEventArgs" />.
    /// </summary>
    public class CliWrapCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CliWrapCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="error">The error<see cref="Exception?"/>.</param>
        /// <param name="cancelled">The cancelled<see cref="bool"/>.</param>
        /// <param name="userState">The userState<see cref="object?"/>.</param>
        public CliWrapCompletedEventArgs(Exception? error, bool cancelled, object? userState) : base(error, cancelled, userState)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Result.
        /// </summary>
        public int Result { get; internal set; }

        /// <summary>
        /// Gets or sets the TaskName.
        /// </summary>
        public string TaskName { get; internal set; }

        public string MovieName { get; internal set; }

        public string OutputStream { get; internal set; }

        public string? BitmapFileName { get; internal set; }

        public Avalonia.Media.Imaging.Bitmap? Bitmap { get; set; }

        #endregion
    }
}
