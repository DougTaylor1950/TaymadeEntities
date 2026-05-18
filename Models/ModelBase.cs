//-----------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>18/05/2022 10:51:37 18/05/2022 10:51:37 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using ReactiveUI;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ModelBase" />.
    /// </summary>
    public class ModelBase : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// Defines the errorText.
        /// </summary>
        private string? errorText;

        /// <summary>
        /// Defines the isValid.
        /// </summary>
        private bool isValid = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Errors.
        /// </summary>
        [NotMapped]
        public List<ModelError>? Errors { get; set; }

        /// <summary>
        /// Gets the ErrorText.
        /// </summary>
        [NotMapped]
        public string ErrorText
        {
            get
            {
                if (Errors != null && Errors.Count > 0)
                {
                    errorText = string.Join(",", Errors.Select(x => x.Error));
                }
                else errorText = string.Empty;
                return errorText;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref errorText, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether HasErrors.
        /// </summary>
        [NotMapped]
        public bool HasErrors => !IsValid;

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsValid.
        /// </summary>
        [NotMapped]
        public bool IsValid { get => isValid; set => isValid = value; }

        #endregion

        #region Methods

        /// <summary>
        /// The ClearErrors.
        /// </summary>
        public void ClearErrors()
        {
            Errors = new List<ModelError>();
            isValid = true;
        }

        /// <summary>
        /// The SetError.
        /// </summary>
        /// <param name="v">The v<see cref="string"/>.</param>
        protected void SetError(string v)
        {
            IsValid = false;
            if (Errors == null) Errors = new List<ModelError>();

            ModelError? error = Errors.Find(x => x.Error == v);

            if (error == null)
            {
                ModelError modelError = new ModelError()
                {
                    Error = v
                };
                Errors.Add(modelError);
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ModelError" />.
    /// </summary>
    public class ModelError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Error.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the Property.
        /// </summary>
        public string? Property { get; set; }

        #endregion
    }
}
