//-----------------------------------------------------------------------
// <copyright file="SeasonPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>06/05/2022 13:44:43 06/05/2022 13:44:43 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Season" />.
    /// </summary>
    public partial class Season
    {
        #region Methods

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return SeasonNo.ToString() + " - " + Name + "." + Year.ToString();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        internal void Delete()
        {
            var local = DataController.SandboxEntities.Set<Season>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        internal void Insert()
        {
            DataController.SandboxEntities.Set<Season>().Add(this);
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            var local = DataController.SandboxEntities.Set<Season>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DataController.SandboxEntities.SaveChanges();
        }

        #endregion
    }
}
