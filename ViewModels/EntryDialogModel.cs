//-----------------------------------------------------------------------
// <copyright file="EntryDialogModel.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>21/05/2022 10:26:50 21/05/2022 10:26:50 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.ViewModels
{
    using ReactiveUI;
    using System;
    using System.Reactive;
    using System.IO;
    using TaymadeEntities.Support;

    /// <summary>
    /// Defines the <see cref="EntryDialogModel" />.
    /// </summary>
    public class EntryDialogModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the entryTypeValue.
        /// </summary>
        private EntryType entryTypeValue = EntryType.Text;
        private string? entryText;
        private TimeSpan entryTime;

        public EntryDialogModel()
        {
            //Accept = ReactiveCommand.Create(DoAccept);
            //Cancel = ReactiveCommand.Create(DoCancel);
            Hyphenate = ReactiveCommand.Create(DoHyphenate);
            Underscore = ReactiveCommand.Create(DoUnderscore);
            Shorten = ReactiveCommand.Create(DoShorten);
        }

        private void DoUnderscore()
        {
            if (!string.IsNullOrEmpty(EntryText))
            {
                EntryText = DownloadSupport.CleanText(EntryText);

                EntryText = EntryText.Replace(" ", "_");
            }
        }

        private void DoShorten()
        {
            if (!string.IsNullOrEmpty(EntryText)  && EntryText.Length > 100)
            {
                EntryText = DownloadSupport.CleanText(EntryText);
                string extn = Path.GetExtension(EntryText);

                string shortened = EntryText.Substring(0, EntryText.Length - extn.Length);

                string fileName = Path.GetFileNameWithoutExtension(EntryText);

                int lentoRemove = fileName.Length * 2 + extn.Length;

                EntryText = EntryText.Substring(0, 100 - extn.Length) + extn;
            }
        }

        private void DoHyphenate()
        {
            if (!string.IsNullOrEmpty(EntryText))
            {
                EntryText = DownloadSupport.CleanText(EntryText);

                EntryText = EntryText.Replace(" ", "-");
            }
        }

        #endregion

        #region Enums

        public enum EntryType
        {
            Text,
            Time,
            Date
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the EntryDate.
        /// </summary>
        public DateTime? EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the EntryText.
        /// </summary>
        public string? EntryText { get => entryText; set => this.RaiseAndSetIfChanged(ref entryText, value); }

        /// <summary>
        /// Gets or sets the EntryTime.
        /// </summary>
        public TimeSpan EntryTime { get => entryTime; set => this.RaiseAndSetIfChanged(ref entryTime, value); }

        /// <summary>
        /// Gets or sets the EntryTypeValue.
        /// </summary>
        public EntryType EntryTypeValue { get => entryTypeValue; set => entryTypeValue = value; }

        /// <summary>
        /// Gets or sets the MaxStringLength.
        /// </summary>
        public int? MaxStringLength { get; set; }
        public ReactiveCommand<Unit,Unit> Hyphenate { get; }
        public ReactiveCommand<Unit, Unit> Underscore { get; }

        public ReactiveCommand<Unit, Unit> Shorten { get; }

        #endregion
    }
}
