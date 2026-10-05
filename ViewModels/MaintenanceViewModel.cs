using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using TaymadeEntities.Models;
using TaymadeEntities.Views;

namespace TaymadeEntities.ViewModels
{
    public class MaintenanceViewModel : MovieViewModelBase, IDisposable
    {
        private ObservableCollection<StorySeries>? storySeriesList;
        private StorySeries? currentStorySeries;
        private string? newStoryName;
        private ObservableCollection<Author>? authorList;
        private Story? currentStory;
        private Author? currentAuthor;
        private string? newAuthorName;

        public MaintenanceViewModel()
        {
            AuthorList = new ObservableCollection<Models.Author>(
                DataController.StoryController.GetAuthors()
                );

            StorySeriesList = DataController.StoryController.GetStorySeriesList();
            DoAddStorySeries = ReactiveCommand.Create(Do_AddStorySeries);

        }

        /// <summary>
        /// Gets or sets the AuthorList.
        /// </summary>
        public ObservableCollection<Author>?
            AuthorList
        {
            get => authorList;
            set => this.RaiseAndSetIfChanged(ref authorList, value);
        }


        /// <summary>
        /// Gets the DoAddStorySeries.
        /// </summary>
        public ReactiveCommand<Unit, Unit> DoAddStorySeries { get; }

        public string? NewAuthorName
        {
            get => newAuthorName;
            set => this.RaiseAndSetIfChanged(ref newAuthorName, value);
        }

        public string? NewStoryName
        {
            get => newStoryName;
            set => this.RaiseAndSetIfChanged(ref newStoryName, value);
        }
        public void Dispose()
        {

        }

        /// <summary>
        /// Gets or sets the StorySeriesList.
        /// </summary>
        public ObservableCollection<StorySeries>? StorySeriesList
        {
            get => storySeriesList;
            set => this.RaiseAndSetIfChanged(ref storySeriesList, value);
        }

        public Author? CurrentAuthor
        {
            get => currentAuthor;
            set
            {
                this.RaiseAndSetIfChanged(ref currentAuthor, value);
                
            }
        }



        public Story? CurrentStory
        {
            get => currentStory;
            set => this.RaiseAndSetIfChanged(ref currentStory, value);
        }

        public StorySeries? CurrentStorySeries
        {
            get => currentStorySeries;
            set => this.RaiseAndSetIfChanged(ref currentStorySeries, value);
        }

        private void Do_AddStorySeries()
        {
            Window? main = GetWindow() as Window;

            if (main != null)
            {
                StorySeries storySeries = new();


                storySeries.Name = NewStoryName;
                //DataController.StoryController.
                storySeries.Save();

                StorySeriesList = new ObservableCollection<StorySeries>(DataController.StorySeriesList);

            }
        }

        public void DoAddAuthor()
        {

            Author author = new();

            // remove '.' from NewAuthorName

            author.Name = NewAuthorName?.Replace(".", "") ?? string.Empty;
            DataController.SandboxEntities.Author.Add(author);
            DataController.SandboxEntities.SaveChanges();

            // create holding directory
            string dir = @"K:\Drive_I\Stories\PD\Done\" + author.Name;

            //check for existence
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // save into author 
            author.StoryPath = dir;
            author.Save();

            AuthorList = new ObservableCollection<Author>(DataController.AuthorList.ToList());

        }

        public async void EditStory()
        {
            // not implemented yet
        }

    }
}