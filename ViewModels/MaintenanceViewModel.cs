using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
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
        private string? searchFolder;
        private ObservableCollection<UnboundGridData>? missingMovies;
        private PhraseEntry? selectedPhrase;
        private UnboundGridData currentMissing;
        private string maintenanceProgress = "Progress ...";

        public MaintenanceViewModel()
        {
            AuthorList = new ObservableCollection<Models.Author>(
                DataController.StoryController.GetAuthors()
                );

            StorySeriesList = DataController.StoryController.GetStorySeriesList();
            DoAddStorySeries = ReactiveCommand.Create(Do_AddStorySeries);

        }

        public delegate void ProgressEventHandler(object sender, MovieProgressEventargs e);
        public event ProgressEventHandler ProgressInformation;


        protected virtual void OnProgress(MovieProgressEventargs e)
        {
            ProgressEventHandler handler = ProgressInformation;
            handler?.Invoke(this, e);
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

        public string? SearchFolder
        {
            get => searchFolder;
            set
            {
                this.RaiseAndSetIfChanged(ref searchFolder, value);
                if (value != null)
                {
                    MaintenanceProgress = "Checking: " + searchFolder;
                }
            }
        }

        /// <summary>
        /// Gets the DoAddStorySeries.
        /// </summary>
        public ReactiveCommand<Unit, Unit> DoAddStorySeries { get; }

        public UnboundGridData CurrentMissing
        {
            get => currentMissing;
            set => this.RaiseAndSetIfChanged(ref currentMissing, value);
        }

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

        public PhraseEntry? SelectedPhrase
        {
            get => selectedPhrase;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedPhrase, value);
                if (value != null && value.PhraseInfo != null)
                {
                    SearchFolder = value.PhraseInfo.Path;
                }
            }
        }

        public ObservableCollection<UnboundGridData>? MissingMovies
        {
            get => missingMovies;
            set => this.RaiseAndSetIfChanged(ref missingMovies, value);
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
        public string MaintenanceProgress
        {
            get => maintenanceProgress;
            set => this.RaiseAndSetIfChanged(ref maintenanceProgress, value);
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

        public  void MissingMoviesChecker()
        {
            //this.ProgressInformation += MaintenanceViewModel_ProgressInformation;
            MissingMovies = new ObservableCollection<UnboundGridData>();
            MaintenanceProgress = "Processing Files";
            //string? folder = await GetFolder(SearchFolder);
           // if (folder == null) return;
            GetMissingFiles(SearchFolder);
        }

        public async Task<string?> GetFolder(string sourceFolder)
        {
            Window? main = Support.Support.GetMainWindow();
            var topLevel = TopLevel.GetTopLevel(main);

            var startLoc = await
                     topLevel.StorageProvider.TryGetFolderFromPathAsync(sourceFolder);

            if (startLoc == null || !Directory.Exists(startLoc.Path.LocalPath))
            {

            }

            FolderPickerOpenOptions pickerOpenOptions = new FolderPickerOpenOptions()
            {
                Title = "Search Folder",
                AllowMultiple = false,
                SuggestedStartLocation = startLoc
            };

            var result = await topLevel.StorageProvider.OpenFolderPickerAsync(pickerOpenOptions);
            if (result == null) return null;
            var folder = result[0];
            if (folder == null)
            {
                MaintenanceProgress = "No such folder or cancelled";
                return null;
            }
            string localPath = folder.Path.LocalPath;
            return localPath;
        }

        private void MaintenanceViewModel_ProgressInformation(object sender, MovieProgressEventargs e)
        {
            MaintenanceProgress = e.Info;
            Dispatcher.UIThread.Post(() =>
            {
                this.RaisePropertyChanged(nameof(MaintenanceProgress));
            });
            System.Threading.Thread.Sleep(100);
        }

        private void GetMissingFiles(string localPath)
        {


            string[] files = System.IO.Directory.GetFiles(localPath, "*.*", SearchOption.AllDirectories);


            int nfilesDone = 0;
            foreach (string file in files)
            {
                string extention = Path.GetExtension(file).ToLower();
                if (extention == ".wmv" || extention == ".mp4" || extention == ".avi" || extention == ".flv"
                    || extention == ".mpeg" || extention == ".mts" || extention == ".mov" || extention == ".mpg"
                    || extention == ".webm" || extention == ".rm" || extention == ".mkv")
                {
                    string filename = Path.GetFileNameWithoutExtension(file)
                        .Replace("\\", "/")

                        .ToLower();
                    nfilesDone += 1;
                    Movies? temp = DataController.SandboxEntities.Movies.Where(m => m.MoviePath.ToLower().Contains(filename)).FirstOrDefault();
                    if (temp == null)
                    {
                        UnboundGridData ubg = new UnboundGridData()
                        {
                            FilePath = file,
                            FileName = filename
                        };
                        MissingMovies.Add(ubg);

                        MaintenanceProgress = "Found file :" + ubg.FilePath;
                        
                        Thread.Sleep(100);
                    }
                    else
                    {
                        
                        MaintenanceProgress = "Processed File : " + nfilesDone.ToString() + " of " + files.Length.ToString();
                        Thread.Sleep(100);
                    }
                }

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    this.RaisePropertyChanged(nameof(MaintenanceProgress));
                }
                );
                //Thread.Sleep(100);

            }

            MaintenanceProgress = "Processing Files : Finished";
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