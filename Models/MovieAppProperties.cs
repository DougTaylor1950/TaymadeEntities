//-----------------------------------------------------------------------
// <copyright file="MovieAppProperties.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>04/07/2023 15:10:22 04/07/2023 15:10:22 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="MovieAppProperties" />.
    /// </summary>
    public class MovieAppProperties
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether BoolValue.
        /// </summary>
        [NotMapped]
        public bool BoolValue
        {
            get
            {
                if (!string.IsNullOrEmpty(Property))
                {
                    if (Property.ToUpper() == "TRUE")
                        return true;
                    else
                        return false;
                }
                else return false;
            }
        }

        /// <summary>
        /// Gets or sets the Computer.
        /// </summary>
        public string? Computer { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Property.
        /// </summary>
        public string? Property { get; set; }

        /// <summary>
        /// Gets or sets the PropertyName.
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        public string? Type { get; set; }

        internal void Save()
        {
            if (Id == 0)
            {
            }
            else
            {
                EntityState state = DataController.SandboxEntities.Entry(this).State;

                var local = DataController.SandboxEntities.Set<Filter>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                // check if local is not null
                if (local != null)
                {
                    // detach
                    DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                }
                // set Modified flag in your entry
                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();
            }
        }

        #endregion
    }
}
