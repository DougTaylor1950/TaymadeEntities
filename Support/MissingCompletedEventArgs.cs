//-----------------------------------------------------------------------
// <copyright file="Support.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>25/04/2022 11:57:36 25/04/2022 11:57:36 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using TaymadeEntities.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class MissingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        #region Constructors


        public MissingCompletedEventArgs(Exception? error, bool cancelled, object? userState) : base(error, cancelled, userState)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Result.
        /// </summary>
        public int Result { get;  set; }


        public List<MissingFile>? Missing { get; set; }

        public List<string>? Paths { get; set; }
        #endregion
    }

    public class MovieCompletedEventArgs : AsyncCompletedEventArgs
    {
        public MovieCompletedEventArgs(Exception? error, bool cancelled, object? userState) : base(error, cancelled, userState)
        {
        }

        public int Result { get; set; }

        public Movies? Movie { get; set; }

        public int? MovieId { get; set; }

        public PhraseEntry? PhraseEntry { get; set; }

        public PhraseEntry? SubPhraseEntry { get; set; }
    }
}

