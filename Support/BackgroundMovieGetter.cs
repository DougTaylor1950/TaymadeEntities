using TaymadeEntities.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using SupportCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaymadeEntities.Support.FFMpegSupport;

namespace TaymadeEntities.Support
{
    public class BackgroundMovieGetter
    {

        #region Public Fields

        public bool busy = false;

        #endregion Public Fields

        #region Private Fields

        private BackgroundWorker backgroundMovieGetter;
        private Models.PhraseEntry id;
        private Models.PhraseEntry subId;
        private Models.SandboxDBContextFactory _contentFactory = new Models.SandboxDBContextFactory();
        private ObservableCollection<Movies> movieList;

        #endregion Private Fields

        #region Public Constructors

        public BackgroundMovieGetter()
        {
            backgroundMovieGetter = new BackgroundWorker();
            //
            // backgroundWorker1
            //
            this.backgroundMovieGetter.WorkerSupportsCancellation = true;
            this.backgroundMovieGetter.DoWork += this.backgroundMovieGetter_DoWork;
            this.backgroundMovieGetter.RunWorkerCompleted += backgroundMovieGetter_RunWorkerCompleted;
            this.backgroundMovieGetter.ProgressChanged += this.BackgroundMovieGetter_ProgressChanged;

            this.backgroundMovieGetter.WorkerReportsProgress = true;
        }

        private void BackgroundMovieGetter_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            int percent = e.ProgressPercentage;
            DataGetProgressEventArgs progressEventArgs = new DataGetProgressEventArgs(percent,e.UserState);

            if (percent == 0) progressEventArgs.Progress = "started";
            if (percent == 100) progressEventArgs.Progress = "Completed";

            OnDataGetProgress(progressEventArgs);
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void DataGetCompletedEventArgsEventHandler(object sender, DataGetCompletedEventArgs e);

        // <summary>
        /// The DataGetProgressEventHandler.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataGetProgressEventArgs"/>.</param>
        public delegate void DataGetProgressEventHandler(object sender, DataGetProgressEventArgs e);

        #endregion Public Delegates

        #region Public Events

        public event DataGetCompletedEventArgsEventHandler DataGetCompleted;

        public event DataGetProgressEventHandler DataGetProgress;

        #endregion Public Events

        #region Public Properties

        public Models.PhraseEntry? LastSubID { get; set; }
        public Models.PhraseEntry? SubPhrase
        {
            get => subId;
            set
            {
                if (subId != value || LastSubID == null) LastSubID = subId;
                subId = value;
            }
        }

        public Models.PhraseEntry? Phrase
        {
            get => id;
            set
            {
                if (id != value || LastID == null) LastID = id;
                id = value;
            }
        }

        public Models.PhraseEntry? LastID { get; private set; }
        public ObservableCollection<Movies> MovieList 
        { get => movieList; set => movieList = value; }

        #endregion Public Properties

        #region Private Properties

        private DBContext.SandboxEntities _SandboxEntities { get; set; } = new DBContext.SandboxEntities();
        public bool AllowReload { get;  set; }

        #endregion Private Properties

        #region Public Methods

        public void backgroundMovieGetter_DoWork(object? sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            bw.ReportProgress(0);
            e.Result = this.LoadData(e);
            bw.ReportProgress(100);

        }

        #endregion Public Methods

        #region Internal Methods

        public void Run()
        {
            if (!backgroundMovieGetter.IsBusy)
            { 
                if (AllowReload) // allow chamge of genre
                {
                    LastID = null;
                    AllowReload = false;
                }
                if (LastID == Phrase && !(SubPhrase != null && SubPhrase != LastSubID)) 
                    return;
                else
                    backgroundMovieGetter.RunWorkerAsync();
            }
            // while (backgroundMovieGetter.IsBusy)
            //  {

            // }
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void OnDataGetComplete(DataGetCompletedEventArgs e)
        {

            DataGetCompletedEventArgsEventHandler handler = DataGetCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDataGetProgress(DataGetProgressEventArgs e)
        {

            DataGetProgressEventHandler handler = DataGetProgress;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void backgroundMovieGetter_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            busy = false;
            DataGetCompletedEventArgs completedEventArgs = new DataGetCompletedEventArgs(null, false, null)
            {
                Result = e.Result as ObservableCollection<Movies>
            };


            OnDataGetComplete(completedEventArgs);
        }
        private ObservableCollection<Models.Movies> LoadData(DoWorkEventArgs e)
        {

            busy = true;
            if (Phrase != null)
            {
                if (SubPhrase == null)
                {
                    //using (var context = _contentFactory.Create())
                    //{
                    //    List<Models.Movies> tempList = context.Movies
                    //            .Where(x => x.FilmGroup.Contains(Phrase.Id))
                    //            .Include(x => x.Casts)
                    //            .Include(b => b.Bookmarks)
                    //            .Include(d => d.Director)
                    //            .ToList();
                    List<Models.Movies>? tempList = DataController.MovieController.GetMoviesByGenre(Phrase.COMPKEY);

                    MovieList = MovieCollection.GetAndSortObservableCollection(tempList);
                    //}
                }
                else
                {
                    List<Models.Movies>? tempList = DataController.MovieController.GetMoviesByGenre(Phrase.COMPKEY, SubPhrase.COMPKEY);
                        
                    MovieList = MovieCollection.GetAndSortObservableCollection(tempList);
                }
                //MovieList = new ObservableCollection<Movies>(tempList);
            }

            else
            {

                List<Models.Movies> tempList = _SandboxEntities.Movies
                        .Include(x => x.Casts)
                        .Include(b => b.Bookmarks)
                        .Include(d => d.Director)
                        .ToList();
                //MovieList = new ObservableCollection<Movies>(tempList);
                MovieList = MovieCollection.GetAndSortObservableCollection(tempList);
            }


            return MovieList;
        }

        #endregion Private Methods
    }

    public class DataGetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CliWrapCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="error">The error<see cref="Exception?"/>.</param>
        /// <param name="cancelled">The cancelled<see cref="bool"/>.</param>
        /// <param name="userState">The userState<see cref="object?"/>.</param>
        public DataGetCompletedEventArgs(Exception? error, bool cancelled, object? userState) : base(error, cancelled, userState)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Result.
        /// </summary>
        public ObservableCollection<Movies> Result { get; internal set; }



        #endregion
    }

    public class DataGetProgressEventArgs : System.ComponentModel.ProgressChangedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGetProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progressPercentage">The progressPercentage<see cref="int"/>.</param>
        /// <param name="userState">The userState<see cref="object?"/>.</param>
        public DataGetProgressEventArgs(int progressPercentage, object? userState) : base(progressPercentage, userState)
        {
            TaskName = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Progress.
        /// </summary>
        public string? Progress { get; internal set; }

        /// <summary>
        /// Gets or sets the TaskName.
        /// </summary>
        public string TaskName { get; internal set; }

        #endregion
    }



}
