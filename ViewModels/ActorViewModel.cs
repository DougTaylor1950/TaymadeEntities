//-----------------------------------------------------------------------
// <copyright file="CastViewModel.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>07/05/2022 16:57:57 07/05/2022 16:57:57 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.ViewModels
{
    using Avalonia.Controls;
    using TaymadeEntities.Models;
    
    using ReactiveUI;
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using TaymadeEntities.Support;

    /// <summary>
    /// Defines the <see cref="CastViewModel" />.
    /// </summary>
    public class ActorViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the currentActor.
        /// </summary>
        private Actor? currentActor;

        /// <summary>
        /// Defines the editingActor.
        /// </summary>
        private bool editingActor = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorViewModel"/> class.
        /// </summary>
        public ActorViewModel()
        {
            SetupCommands();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorViewModel"/> class.
        /// </summary>
        /// <param name="currentActor">The currentActor<see cref="Actor"/>.</param>
        public ActorViewModel(Actor currentActor)
        {
            CurrentActor = currentActor; //?? throw new ArgumentNullException(nameof(currentActor));

            SetupCommands();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the CurrentActor.
        /// </summary>
        public Actor? CurrentActor { get => currentActor; set => this.RaiseAndSetIfChanged(ref currentActor, value); }

        /// <summary>
        /// Gets or sets a value indicating whether EditingActor.
        /// </summary>
        public bool EditingActor { get => editingActor; set => editingActor = value; }

        /// <summary>
        /// Gets the FoundPerson.
        /// </summary>
        public Person? FoundPerson { get; private set; }

        /// <summary>
        /// Gets or sets the GetActor.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? GetActor { get; set; }

        /// <summary>
        /// Gets or sets the Save.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? Save { get; set; }

        /// <summary>
        /// Gets or sets the Search
        /// Defines the resultButton..
        /// </summary>
        public ReactiveCommand<Unit, Unit>? Search { get; set; }

        internal ICommand? AddSearchCommand()
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                
                    this.DoSearchTMDB();
               
            });

            return myCommand;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The DoGetActor.
        /// </summary>
        private void DoGetActor()
        {
            if (CurrentActor != null)
            {
                CurrentActor.GetDetailsFromTMDB();
            }
        }

        /// <summary>
        /// The DoSave.
        /// </summary>
        private void DoSave()
        {
            if (CurrentActor != null) CurrentActor.Save();
        }

        /// <summary>
        /// The DoSearchTMDB.
        /// </summary>
        private async void DoSearchTMDB()
        {
            ////CastList listFound = TmdbSupport.SearchActor(currentActor.Name);

            TaymadeEntities.ViewModels.ActorSearchModel actorSearchModel = 
                new TaymadeEntities.ViewModels.ActorSearchModel(currentActor);

            TaymadeEntities.Dialogs.TMDBActorSearchDialog searchDialog = 
                new TaymadeEntities.Dialogs.TMDBActorSearchDialog(actorSearchModel);
            

            //await searchDialog.ShowDialog(original);

            //ActorSearchModel? actorSearchView = searchDialog.DataContext as ActorSearchModel;

            //if (actorSearchView != null && actorSearchView.resultButton != null && actorSearchView.resultButton.Result == Dialogs.DialogResultButton.ResultType.Ok)
            //{
            //    Person? found = actorSearchView.FoundPerson;

            //    if (found != null && currentActor != null)
            //    {
            //        currentActor.SetDetailsFromPerson(found);

            //    }


            //}

            //Caller = original;

            //if (currentActor != null)
            //{
            //    Person person = TmdbSupport.GetPerson(currentActor.Name);
            //    currentActor.SetDetailsFromPerson(person);
            //}
        }

        /// <summary>
        /// The SetupCommands.
        /// </summary>
        private void SetupCommands()
        {
            
            Search = ReactiveCommand.Create(DoSearchTMDB);
            Save = ReactiveCommand.Create(DoSave);
            GetActor = ReactiveCommand.Create(DoGetActor);
        }

        #endregion
    }
}
