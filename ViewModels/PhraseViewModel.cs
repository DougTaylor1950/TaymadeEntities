using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using TaymadeEntities.Models;

namespace TaymadeEntities.ViewModels
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TaymadeEntities.ViewModels.DialogModelBase" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 27/04/2026 11:31 </created>
    /// </remarks>
    public class PhraseViewModel : ViewModelBase
    {

        #region Private Fields

        /// <summary>
        /// The has sub phrases
        /// </summary>
        private bool hasSubPhrases;
        /// <summary>
        /// The phrase entries
        /// </summary>
        private ObservableCollection<Models.PhraseEntry>? phraseEntries;
        /// <summary>
        /// The phrase header list
        /// </summary>
        private List<Models.PhraseHeader>? phraseHeaderList;
        /// <summary>
        /// The selected phrase
        /// </summary>
        private Models.PhraseEntry? selectedPhrase;
        /// <summary>
        /// The selected phrase header
        /// </summary>
        private Models.PhraseHeader? selectedPhraseHeader;
        /// <summary>
        /// The selected sub phrase
        /// </summary>
        private Models.PhraseEntry? selectedSubPhrase;
        /// <summary>
        /// The subphrase entries
        /// </summary>
        private ObservableCollection<Models.PhraseEntry>? subphraseEntries;
        private Models.PhraseEntry currentPhrase;
        private Models.PhraseEntry currentSubPhrase;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseViewModel"/> class.
        /// </summary>
        public PhraseViewModel()
        {
            // setup the commands
            SavePhraseChangesCommand = ReactiveCommand.Create(SavePhraseChanges);
            SaveSubPhraseChangesCommand = ReactiveCommand.Create(SaveSubPhraseChanges);
            AddPhraseCommand = ReactiveCommand.Create(AddPhrase);
            AddSubPhraseCommand = ReactiveCommand.Create(AddSubPhrase);

            // initialise phraseheader list
            int count = PhraseHeaderList.Count;
            count = PhraseEntries.Count;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the add phrase command.
        /// </summary>
        /// <value>
        /// The add phrase command.
        /// </value>
        public ReactiveCommand<Unit, Unit> AddPhraseCommand { get; private set; }

        /// <summary>
        /// Gets the add sub phrase command.
        /// </summary>
        /// <value>
        /// The add sub phrase command.
        /// </value>
        public ReactiveCommand<Unit, Unit> AddSubPhraseCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has sub phrases.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has sub phrases; otherwise, <c>false</c>.
        /// </value>
        public bool HasSubPhrases
        {
            get => hasSubPhrases;
            set => this.RaiseAndSetIfChanged(ref hasSubPhrases, value);
        }

        /// <summary>
        /// Gets or sets the PhraseEntries value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public ObservableCollection<Models.PhraseEntry>? PhraseEntries
        {
            get
            {
                if (phraseEntries== null || phraseEntries.Count==0)
                {
                    phraseEntries = new ObservableCollection<Models.PhraseEntry>(DataController.PhraseEntries); 
                }
                return phraseEntries;
            }

            set => this.RaiseAndSetIfChanged(ref phraseEntries, value);
        }

        /// <summary>
        /// Gets or sets the PhraseHeaderList value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public List<Models.PhraseHeader> PhraseHeaderList
        {
            get
            {
                if (phraseHeaderList == null || phraseHeaderList.Count == 0)
                {
                    phraseHeaderList = DataController.SandboxEntities.PhraseHeader.ToList();
                }
                return phraseHeaderList;
            }

            set => this.RaiseAndSetIfChanged(ref phraseHeaderList, value);
        }

        /// <summary>
        /// Gets the save phrase changes command.
        /// </summary>
        /// <value>
        /// The save phrase changes command.
        /// </value>
        public ReactiveCommand<Unit, Unit> SavePhraseChangesCommand { get; private set; }

        /// <summary>
        /// Gets the save sub phrase changes command.
        /// </summary>
        /// <value>
        /// The save sub phrase changes command.
        /// </value>
        public ReactiveCommand<Unit, Unit> SaveSubPhraseChangesCommand { get; private set; }

        /// <summary>
        /// Gets or sets the SelectedPhrase value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public Models.PhraseEntry? SelectedPhrase
        {
            get => selectedPhrase;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedPhrase, value);
                HasSubPhrases = false;
                // if phraseId = 1 then load the subphrases of phraseheader id = 9 into a list of subphrases
                if (selectedPhrase != null && selectedPhrase.PhraseID == 1)
                {
                    var subPhrases = DataController.PhrasesController.GetSubPhraseEntries(selectedPhrase.Id);
                    //var subPhrases = DataController.SandboxEntities.PhraseEntry.Where(pe => pe.PhraseID == 9 && pe.Id.Contains(selectedPhrase.Id)).ToList();
                    // Do something with the subPhrases, e.g., display them in the UI
                    // You can create a new ObservableCollection for subphrases if needed
                    SubPhraseEntries = new ObservableCollection<Models.PhraseEntry>(subPhrases);
                    HasSubPhrases = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the SelectedPhraseHeader value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public Models.PhraseHeader? SelectedPhraseHeader
        {
            get => selectedPhraseHeader;
            set => this.RaiseAndSetIfChanged(ref selectedPhraseHeader, value);
        }

        /// <summary>
        /// Gets or sets the SelectedSubPhrase value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public Models.PhraseEntry? SelectedSubPhrase
        {
            get => selectedSubPhrase;
            set => this.RaiseAndSetIfChanged(ref selectedSubPhrase, value);
        }

        /// <summary>
        /// Gets or sets the SubPhraseEntries value
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 - 11:31 </created>
        /// </remarks>
        public ObservableCollection<Models.PhraseEntry>? SubPhraseEntries
        {
            get => subphraseEntries;
            set => this.RaiseAndSetIfChanged(ref subphraseEntries, value);
        }
        public Models.PhraseEntry CurrentPhrase 
        { 
            get => currentPhrase; 
            set => this.RaiseAndSetIfChanged(ref currentPhrase, value); 
        }
        public Models.PhraseEntry CurrentSubPhrase 
        { 
            get => currentSubPhrase; 
            set => this.RaiseAndSetIfChanged(ref currentSubPhrase, value); 
        }

        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 27/04/2026 </created>
        /// </remarks>
        internal void LoadPhraseEntriesForHeader()
        {
            if (SelectedPhraseHeader == null)
            {
                PhraseEntries = null;
                return;
            }
            // Load the PhraseEntries for the selected header into a list phrases
            var phrases = DataController.PhrasesController.GetPhrasesByPhraseHeaderId(SelectedPhraseHeader.Id);
            // Do something with the phrases, e.g., display them in the UI
            PhraseEntries = new ObservableCollection<Models.PhraseEntry>(phrases);
            // if you want to set the first phrase as selected, you can do so here
            if (PhraseEntries.Count > 0)
            {
                SelectedPhrase = PhraseEntries[0];
            }

            // if the selected header is id 1 then there are subphrases of prhaseheader id =9 that should be loaded into a list of subphrases when the selected phrase changes


        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 27/04/2026 </created>
        /// </remarks>
        private void AddPhrase()
        {
            // using the selected phrase header,create a new phrase with PhraseId = header.Id and add it to the database and the PhraseEntries collection
            if (SelectedPhraseHeader != null)
            {
                Models.PhraseEntry newPhrase = new Models.PhraseEntry()
                {
                    Id = "<Id>",
                    PhraseID = SelectedPhraseHeader.Id,
                    Description = "<New Phrase>",
                    Order = PhraseEntries != null ? PhraseEntries.Count + 1 : 1,
                    Sortable = true,
                    Searchable = true
                };
                //newPhrase.Save(); can't save as it requires the id to be obtained
                PhraseEntries?.Add(newPhrase);
                // Optionally, set the new phrase as the selected phrase
                SelectedPhrase = newPhrase;
            }
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 27/04/2026 </created>
        /// </remarks>
        private void AddSubPhrase()
        {

            // generate a new subphrase with the same phraseid as the selected phrase and an id that is the selected phrase id + a suffix and add it to the database and the SubPhraseEntries collection
            if (SelectedPhrase != null)
            {
                string selectedphraseId = SelectedPhrase.Id;
                Models.PhraseEntry newPhrase = new Models.PhraseEntry()
                {
                    Id = selectedphraseId = ".",
                    PhraseID = 9,
                    Description = "<New Phrase>",
                    Order = PhraseEntries != null ? PhraseEntries.Count + 1 : 1,
                    Sortable = true,
                    Searchable = true
                };
                newPhrase.Id = SelectedPhrase.COMPKEY.Replace("-1", ".");
                //newPhrase.Save(); can't save as it requires the id to be obtained
                SubPhraseEntries?.Add(newPhrase);
                // repopulate the subphrase entries for the selected phrase to ensure the new subphrase is included
                var subPhrases = DataController.PhrasesController.GetSubPhraseEntries(SelectedPhrase.Id);
                // var subPhrases = DataController.SandboxEntities.PhraseEntry.Where(pe => pe.PhraseID == 9 && pe.Id.Contains(SelectedPhrase.Id)).ToList();
                SubPhraseEntries = new ObservableCollection<Models.PhraseEntry>(subPhrases);
                // Optionally, set the new phrase as the selected phrase
                SelectedSubPhrase = newPhrase;
            }
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 27/04/2026 </created>
        /// </remarks>
        private void SavePhraseChanges()
        {
            SelectedPhrase?.Save();
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 27/04/2026 27/04/2026 </created>
        /// </remarks>
        private void SaveSubPhraseChanges()
        {
            if (SelectedSubPhrase != null)
            {
                SelectedSubPhrase.Save();
            }
        }

        #endregion Private Methods
    }
}