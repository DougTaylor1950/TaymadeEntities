using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;

namespace TaymadeEntities.Models
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TaymadeEntities.Models.ModelBase" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 30/04/2026 13:32 </created>
    /// </remarks>
    public class ProgramInformation : ModelBase
    {
        #region Private Fields

        /// <summary>
        /// The description
        /// </summary>
        private string? description;
        /// <summary>
        /// The information
        /// </summary>
        private string? information;
        /// <summary>
        /// The name
        /// </summary>
        private string? name;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the Description value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 30/04/2026 - 13:32 </created>
        /// </remarks>
        public string? Description
        {
            get => description;
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        /// <summary>
        /// Gets or sets the Information value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 30/04/2026 - 13:32 </created>
        /// </remarks>
        public string? Information
        {
            get => information;
            set => this.RaiseAndSetIfChanged(ref information, value);
        }

        /// <summary>
        /// Gets or sets the Name value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 30/04/2026 - 13:32 </created>
        /// </remarks>
        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        #endregion Public Properties
    }
}