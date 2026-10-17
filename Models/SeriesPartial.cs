using Avalonia.Controls;
using System;
using TaymadeEntities.DBContext;

namespace TaymadeEntities.Models
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TaymadeEntities.Models.ModelBase" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 18/02/2026 21:03 </created>
    /// </remarks>
    public partial class Series
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        public void Update()
        {
            this.Save();
        }


        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 05/10/2026 05/10/2026 </created>
        /// </remarks>
        public async static Task<Series> AddSeriesToMovie(int id, Window? window = null)
        {
            Series newSeries = null;
            using TaymadeEntities.ViewModels.EntryDialogModel entryDialogModel =
                    new TaymadeEntities.ViewModels.EntryDialogModel(TaymadeEntities.ViewModels.EntryDialogModel.EntryType.List);
            using TaymadeEntities.Dialogs.EntryDialog entryDialog = new TaymadeEntities.Dialogs.EntryDialog(entryDialogModel);

            using var _context = new TaymadeEntities.DBContext.SandboxEntities();
            {
                Movies? movie = _context.Movies.Find(id);
                entryDialog.Title = "Set Series Value";
                entryDialogModel.ItemList = DataController.SeriesController.GetSeriesList();

                if (window == null) window = Support.Support.GetMainWindow() as Window;

                DialogResultButton result = await entryDialog.ShowDialog<DialogResultButton>(window);
                if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                {
                    Series? series = result.ListValue as Series;
                    newSeries = series;
                    if (series != null)
                    {
                        movie.Series = series.Id;
                        _context.SaveChanges();
                    }
                }
            }
            return newSeries;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            try
            {
                using SandboxEntities _context = new SandboxEntities();
                if (Id == 0)
                {
                    _context.Series.Add(this);
                    _context.SaveChanges();
                }
                else
                {
                    Series? tempSeries = _context.Series.Find(this.Id);
                    if (tempSeries == null) return;
                    MoveToTemp(tempSeries);
                    _context.SaveChanges();
                }
                // DataController.SandboxEntities.SaveChanges();

            }
            catch (Exception)
            {

                // throw;
            }
        }

        private void MoveToTemp(Series series)
        {
            series.Intro = this.Intro;
            series.Name = this.Name;
            series.Outro = this.Outro;
            series.Path = this.Path;
            series.TMID = this.TMID;
            series.Overview = this.Overview;
        }

        public void CreateFromSeries(Series? series)
        {
            if (series == null) return;
            this.Id = series.Id;
            this.Intro = series.Intro;
            this.Name = series.Name;
            this.Outro = series.Outro;
            this.Path = series.Path;
            this.TMID = series.TMID;
            this.Overview = series.Overview;
        }

    }

        #endregion Internal Methods
}

