//-----------------------------------------------------------------------
// <copyright file="EntryDialog.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>20/05/2022 17:49:53 20/05/2022 17:49:53 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Dialogs
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using TaymadeEntities.Models;

    /// <summary>
    /// Defines the <see cref="EntryDialog" />.
    /// </summary>
    public partial class EntryDialog : Window
    {
        #region Fields

        /// <summary>
        /// Defines the EntryDate.
        /// </summary>
        private DatePicker? EntryDate;

        /// <summary>
        /// Defines the EntryText.
        /// </summary>
        private TextBox? EntryText;

        /// <summary>
        /// Defines the EntryTime.
        /// </summary>
        private TextBox? EntryTime;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryDialog"/> class.
        /// </summary>
        public EntryDialog()
        {
            InitializeComponent();



        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryDialog"/> class.
        /// </summary>
        /// <param name="model">The model<see cref="ViewModels.EntryDialogModel"/>.</param>
        public EntryDialog(ViewModels.EntryDialogModel model)
        {

            InitializeComponent();


            DataContext = model;

            // get controls
            EntryText = this.FindControl<TextBox>("entryText");
            EntryTime = this.FindControl<TextBox>("entryTime");
            EntryDate = this.FindControl<DatePicker>("entryDate");

            // set max string length property;
            if (model.MaxStringLength != null)
                EntryText.MaxLength = model.MaxStringLength.Value;

            switch (model.EntryTypeValue)
            {
                case ViewModels.EntryDialogModel.EntryType.Text:
                    EntryDate.IsVisible = false;
                    EntryText.IsVisible = true;
                    EntryTime.IsVisible = false;
                    break;
                case ViewModels.EntryDialogModel.EntryType.Time:
                    EntryDate.IsVisible = false;
                    EntryText.IsVisible = false;
                    EntryTime.IsVisible = true;
                    break;
                case ViewModels.EntryDialogModel.EntryType.Date:
                    EntryDate.IsVisible = true;
                    EntryText.IsVisible = false;
                    EntryTime.IsVisible = false;
                    break;
            }
        }

        private void Accept_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton result = new DialogResultButton()
            {
                Result = DialogResultButton.ResultType.Ok,
                Paramater = EntryText?.Text
            };

            this.Close(result);
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton result = new DialogResultButton()
            {
                Result = DialogResultButton.ResultType.Cancel
            };
            this.Close(result);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}

        #endregion
    }
}
