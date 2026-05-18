using System;

namespace AvalonMVVM.Models
{
    /// <summary>
    /// </summary>
    /// <seealso cref="AvalonMVVM.Models.ModelBase" />
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

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            try
            {
                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();

            }
            catch (Exception)
            {

                // throw;
            }
        }

        #endregion Internal Methods
    }
}