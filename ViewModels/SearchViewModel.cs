using System;
using System.Collections.Generic;
using TaymadeEntities.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using ReactiveUI;

namespace TaymadeEntities.ViewModels
{
    /// <summary>
    /// </summary>
    /// <seealso cref="AvalonMVVM.ViewModels.DialogModelBase" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 18/02/2026 09:44 </created>
    /// </remarks>
    public class SearchViewModel : ViewModelBase, IDisposable
    {
        #region Private Fields

        private string? actorName;
        private string? movieTitle;
        private string? bookmarkText;
        private string? infoText;
        private int? movieId;
        private Series? currentSeries;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public SearchViewModel() { }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the actor.
        /// </summary>
        /// <value>
        /// The name of the actor.
        /// </value>
        public string? ActorName
        {
            get => actorName;
            set => this.RaiseAndSetIfChanged(ref actorName, value);
        }

        public string? BookmarkText
        {
            get => bookmarkText;
            set => this.RaiseAndSetIfChanged(ref bookmarkText, value);
        }

        public string? InfoText
        { 
            get => infoText; 
            set => this.RaiseAndSetIfChanged(ref infoText, value); 
        }

        public int? MovieId 
        {
            get => movieId; 
            set => this.RaiseAndSetIfChanged(ref  movieId, value); 
        }

        public List<Series> SeriesList
        { get => DataController.SeriesList; }

        public Series? CurrentSeries 
        { 
            get => currentSeries; 
            set => this.RaiseAndSetIfChanged(ref currentSeries, value); 
        }


        /// <summary>
        /// Gets or sets the MovieTitle value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 18/02/2026 - 09:45 </created>
        /// </remarks>
        public string? MovieTitle
        {
            get => movieTitle;
            set => this.RaiseAndSetIfChanged(ref movieTitle, value);
        }
        public int Year { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (SeriesList != null) SeriesList.Clear();
                    //SeriesList = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SearchViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Properties
    }
}