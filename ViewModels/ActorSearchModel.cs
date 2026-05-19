//-----------------------------------------------------------------------
// <copyright file="ActorSearchModel.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>16/10/2022 12:18:29 16/10/2022 12:18:29 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.ViewModels
{
    using Avalonia.Controls;
    using TaymadeEntities.Models;
    using TaymadeEntities.Support;
    using TaymadeEntities.Views;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using System.Linq;
    using System.Reactive;

    /// <summary>
    /// Defines the <see cref="ActorSearchModel" />.
    /// </summary>
    public class ActorSearchModel : ViewModelBase, IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the actorList.
        /// </summary>
        private ObservableCollection<Actor>? actorList;

        /// <summary>
        /// Defines the oldActorList.
        /// </summary>
        private  List<Actor>? oldActorList;

        /// <summary>
        /// Defines the currentActor.
        /// </summary>
        private Actor? currentActor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSearchModel"/> class.
        /// </summary>
        public ActorSearchModel()
        {
            SetupCommands();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSearchModel"/> class.
        /// </summary>
        /// <param name="currentActor">The currentActor<see cref="Actor?"/>.</param>
        public ActorSearchModel(Actor? currentActor)
        {
            CurrentActor = currentActor;

            SetupCommands();
        }

        #endregion

        #region Properties

        private ObservableCollection<Person>? foundPeople;
        private bool disposedValue;

        public ObservableCollection<Person>? FoundPeople
        {
            get => foundPeople;
            set => this.RaiseAndSetIfChanged(ref foundPeople, value);
        }

        /// <summary>
        /// Gets or sets the ActorList.
        /// </summary>
        public ObservableCollection<Actor>? ActorList { get => actorList; set => actorList = value; }

        /// <summary>
        /// Gets the AddActor.
        /// </summary>
        public ReactiveCommand<string?, Unit>? AddActor { get; private set; }

        public ReactiveCommand<Unit, Unit>? MergeActors { get; private set; }

        /// <summary>
        /// Gets or sets the CurrentActor.
        /// </summary>
        public Actor? CurrentActor { get => currentActor; set => this.RaiseAndSetIfChanged(ref currentActor, value); }

        /// <summary>
        /// Gets the EndFindActor.
        /// </summary>
        public ReactiveCommand<Unit, Unit>? EndFindActor { get; private set; }

        /// <summary>
        /// Gets the FindActor.
        /// </summary>
        public ReactiveCommand<string?, Unit>? FindActor { get; private set; }

        /// <summary>
        /// Gets or sets the FoundPerson.
        /// </summary>
        public Person? FoundPerson { get; set; }
        public Actor? SelectedActor { get; internal set; }
        public string? FindText { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// The DoFindActorActual.
        /// </summary>
        /// <param name="findText">The findText<see cref="string"/>.</param>
        /// <param name="caller">The caller<see cref="Window"/>.</param>
        public void DoFindActorActual(string findText)
        {
            if (!string.IsNullOrEmpty(findText))
            {

                //if (ActorList != null) oldActorList = ActorList.ToList();
                //List<Actor> tempList = DataController.SandboxEntities.Actors.AsNoTracking().Where(a => a.Name.ToLower().Contains(findText.ToLower())).ToList();
                //ActorList = new ObservableCollection<Actor>(tempList);
                //findText = string.Empty;

                //if (caller != null && caller is Views.MainWindow)
                //{
                //    MainWindow main = caller as MainWindow;
                   
                //        DataGrid dataGrid = main.ACListControl.dgActors;

                //        if (dataGrid != null)
                //        {
                //            dataGrid.ItemsSource = ActorList;
                //        }
                   

                //}
            }
            else
                if (oldActorList != null) ActorList = new ObservableCollection<Actor>(oldActorList);
        }

        /// <summary>
        /// The DoFindActor.
        /// </summary>
        internal void DoFindActor(string? findText)
        {
            if (!string.IsNullOrEmpty(findText) )
            DoFindActorActual(findText);
        }

        /// <summary>
        /// The DoAddActor.
        /// </summary>
        private void DoAddActor(string? findText)
        {
            if (!string.IsNullOrEmpty(findText))
            {
                List<Actor> tempList = DataController.SandboxEntities.Actors.AsNoTracking().Where(a => a.Name.ToLower().Contains(findText.ToLower())).ToList();
                if (tempList == null || tempList.Count == 0)
                {
                    Actor actor = new Actor();
                    actor.Name = findText;
                    actor.Insert();
                    CurrentActor = actor;
                }
            }
        }

        /// <summary>
        /// The DoEndFind.
        /// </summary>
        private void DoEndFind()
        {

            
                //UserControl actorControl = Caller.ACListControl;
                //if (actorControl != null)
                //{
                //DataGrid dataGrid = main.ACListControl.dgActors;

                //if (dataGrid != null)
                //{
                //    dataGrid.ItemsSource = oldActorList;
                //}
                //}
        }

        /// <summary>
        /// The SetupCommands.
        /// </summary>
        private void SetupCommands()
        {
            //Accept = ReactiveCommand.Create(DoAccept);
            //Cancel = ReactiveCommand.Create(DoCancel);
            FindActor = ReactiveCommand.Create<string?>(DoFindActor);
            EndFindActor = ReactiveCommand.Create(DoEndFind);
            AddActor = ReactiveCommand.Create<string?>(DoAddActor);
            MergeActors = ReactiveCommand.Create(DoMergeActor);
        }

        private void DoMergeActor()
        {
            //Actor? sourceActor = ActorList.Where(x => x.MergeSource == false && x.MergeItem == true).FirstOrDefault();

            //Actor? destActor = ActorList.Where(x => x.MergeItem == true && x.MergeSource == true).FirstOrDefault();

            //if (sourceActor != null && destActor != null)
            //{
            //    DataController.SandboxEntities.MergeActors(sourceActor.Id, destActor.Id);
            //}
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.ActorList?.Clear();
                    this.ActorList = null;
                    //this.CurrentActor?.Dispose();
                    this.CurrentActor = null;
                    this.FoundPeople?.Clear();
                    this.FoundPeople = null;
                    this.FoundPerson = null;
                    this.oldActorList?.Clear();
                    this.oldActorList = null;
                    //this.SelectedActor?.Dispose();
                    this.SelectedActor = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ActorSearchModel()
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

        internal string? GetTMIDB()
        {
            string? returnValue = null;
            if (FoundPerson != null)
                returnValue = FoundPerson.PersonId;

            return returnValue;

        }



        #endregion
    }
}
