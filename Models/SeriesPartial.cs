using Avalonia.Controls;
using System;

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
            DataController.MovieController.UpdateSeries(this);
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
        public async static Task<Series> AddSeriesToMovie(int id, Window? window= null)
        {
            Series newSeries = null;
            using TaymadeEntities.ViewModels.EntryDialogModel entryDialogModel =
                    new TaymadeEntities.ViewModels.EntryDialogModel(TaymadeEntities.ViewModels.EntryDialogModel.EntryType.List);
            using TaymadeEntities.Dialogs.EntryDialog entryDialog = new TaymadeEntities.Dialogs.EntryDialog(entryDialogModel);

            using var _context = new TaymadeEntities.DBContext.SandboxEntities();
            {
                Movies? movie = _context.Movies.Find(id);
                entryDialog.Title = "Set Series Value";
                entryDialogModel.ItemList = DataController.MovieController.GetSeriesList();

                if (window == null)  window = Support.Support.GetMainWindow() as Window;

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
                DataController.MovieController.UpdateSeries(this);
                // DataController.SandboxEntities.SaveChanges();

            }
            catch (Exception)
            {

                // throw;
            }
        }

        #endregion Internal Methods
    }
}