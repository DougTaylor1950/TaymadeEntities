using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using System;
using System.Linq;

namespace TaymadeEntities.Dialogs
{
    public partial class PhraseSelectDialog : Window , IDisposable
    {
        private PhraseViewModel phraseViewModel;
        private bool disposedValue;
        #region Public Constructors

        public PhraseSelectDialog()
        {
            InitializeComponent();

            ViewModels.PhraseViewModel phraseViewModel = new ViewModels.PhraseViewModel();
            SetUpDialogue(phraseViewModel);
        }

        private void SetUpDialogue(PhraseViewModel phraseViewModel)
        {
            DataContext = phraseViewModel;
            this.Closed += PhraseSelectDialog_Closed;

            ComboBox phrases = this.Phrases; // FindControl<ComboBox>("Phrases");

            if (phrases != null)
            {
                phrases.SelectionChanged += OnSelectionChanged;
            }

            ComboBox subPhrases = this.SubPhrases; // FindControl<ComboBox>("SubPhrases");
            if (subPhrases != null)
            {
                subPhrases.SelectionChanged += OnSubSelectionChanged;
            }
        }

        public PhraseSelectDialog(PhraseViewModel phraseViewModel)
        {
            this.phraseViewModel = phraseViewModel;
            SetUpDialogue(phraseViewModel);
        }

        #endregion Public Constructors

        #region Public Methods

        public void SetupSubPhraseList(PhraseEntry? phraseEntry, PhraseViewModel? viewModel)
        {
            viewModel.SubPhraseEntries = new System.Collections.ObjectModel.ObservableCollection<PhraseEntry>(
                DataController.SubPhraseEntries.Where(x => x.Id.Contains(phraseEntry.Id)).ToList());

            ComboBox comboBox = this.FindControl<ComboBox>("SubPhrases");

            if (comboBox != null)
            {
                comboBox.IsVisible = (viewModel.SubPhraseEntries.Count > 0);
                comboBox.ItemsSource = viewModel.SubPhraseEntries;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Accept_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton resultButton = new DialogResultButton();
            resultButton.Result = DialogResultButton.ResultType.Ok;
            resultButton.PhraseEntry = (DataContext as PhraseViewModel)?.CurrentPhrase;
            resultButton.SubPhraseEntry = (DataContext as PhraseViewModel)?.CurrentSubPhrase;
            this.Close(resultButton);
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogResultButton resultButton = new DialogResultButton();
            resultButton.Result = DialogResultButton.ResultType.Cancel;
            this.Close(resultButton);
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? phrases = sender as ComboBox;

            if (phrases != null && phrases.SelectedItem != null)
            {
                PhraseEntry? phraseEntry = phrases.SelectedItem as PhraseEntry;
                PhraseViewModel? viewModel = DataContext as PhraseViewModel;

                if (viewModel != null && phraseEntry != null)
                {
                    viewModel.CurrentPhrase = phraseEntry;

                    viewModel.CurrentSubPhrase = null;

                    // set the group to this value but not save it.
                    DataController.MovieProperties.Group = phraseEntry.Id;
                    SetupSubPhraseList(phraseEntry, viewModel);
                }
            }
        }

        private void OnSubSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? phrases = sender as ComboBox;

            if (phrases != null && phrases.SelectedItem != null)
            {
                PhraseEntry? phraseEntry = phrases.SelectedItem as PhraseEntry;
                PhraseViewModel? viewModel = DataContext as PhraseViewModel;

                if (viewModel != null)
                    viewModel.CurrentSubPhrase = phraseEntry;
            }
        }

        private void PhraseSelectDialog_Closed(object? sender, EventArgs e)
        {
            this.DataContext = null;
            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PhraseSelectDialog()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Private Methods

        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
    }
}