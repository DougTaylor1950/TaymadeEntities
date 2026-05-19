//-----------------------------------------------------------------------
// <copyright file="MovieDetailDialog.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>26/04/2022 12:44:00 26/04/2022 12:44:00 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Dialogs
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Interactivity;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Platform.Storage;
    using TaymadeEntities.Controls;
    using TaymadeEntities.Models;
    //using TaymadeEntities.Support;
    using TaymadeEntities.ViewModels;
    using TaymadeEntities.Views;

    //using CSharpFunctionalExtensions;
    // using DocumentFormat.OpenXml.Wordprocessing;
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reactive;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using TaymadeControls;
    using TaymadeControls.Builders;
    using TaymadeControls.Buttons;

    /// <summary>
    /// Defines the <see cref="MovieEditDialog" />.
    /// </summary>
    public partial class MovieEditDialog : Window
    {

        #region Private Fields

        private Movies? currentMovie;

        /// <summary>
        /// Defines the currentMovieModel.
        /// </summary>
        private ViewModels.MovieEditViewModel? currentMovieModel;

        /// <summary>
        /// Defines the initialising.
        /// </summary>
        private bool initialising = true;



        private ImagedButtonNoText _MissingImages;
        private ImagedButton _playFromLast;
        private ImagedButton _ReloadBookmarks;
        private ImagedButton _repeatLast;
        private ImagedButton AddBookmarks;
        private ImagedButton AddPoster;
        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieEditDialog"/> class.
        /// </summary>
        public MovieEditDialog()
        {
            initialising = true;

            InitializeComponent();

            Initialized += this.MovieEditDialog_Initialized;

            SizeChanged += this.MovieEditDialog_SizeChanged;

            DataContextChanged += this.MovieEditDialog_DataContextChanged;
            //SetupControls();


            SetupActions();
        }

        private void MovieEditDialog_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            this.BookmarksTab.Width = e.NewSize.Width - 5;
        }

        public MovieEditDialog(ViewModels.MovieEditViewModel model)
        {
            InitializeComponent();
            Initialising = true;

            SetupControls(model);

            SetupActions();

            SetupToolbar();

            model.NewPhrase = null;
            model.NewSubPhrase = null;

            Initialized += this.MovieEditDialog_Initialized;

            //if (this.MovieBookmarks != null && this.BookmarkDetails != null)
            //{
            //    //this.MovieBookmarks.BookmarkUserControl = this.BookmarkDetails;
            //}

            DataContextChanged += this.MovieEditDialog_DataContextChanged;
        }

        private void SetupToolbar()
        {
            if (this.ToolbarBookmarks != null)
            {
                this.ToolbarBookmarks.Height = 48;
                this.ToolbarBookmarks.Background = new SolidColorBrush(Colors.LightGray);

                AddBookmarks = new ImagedButton()
                {
                    LabelText = "New Bookmark",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/NewBookmark.png")),
                    // Command = viewModel.NewBookmark
                };
                this.ToolbarBookmarks.Children.Add(AddBookmarks);

                AddPoster = new ImagedButton()
                {
                    LabelText = "New Poster",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/bookmark.png"))
                };
                this.ToolbarBookmarks.Children.Add(AddPoster);

                _playFromLast = new ImagedButton()
                {
                    LabelText = "Play Last",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/playLast.png"))
                };
                this.ToolbarBookmarks.Children.Add(_playFromLast);

                _repeatLast = new ImagedButton()
                {
                    LabelText = "Repeat Last",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/sync.png"))
                };
                this.ToolbarBookmarks.Children.Add(_repeatLast);

                _ReloadBookmarks = new ImagedButton()
                {
                    LabelText = "Reload Bookmarks",
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/sync.png"))
                };
                this.ToolbarBookmarks.Children.Add(_ReloadBookmarks);

                _MissingImages = new ImagedButtonNoText()
                {
                    ImageSource = ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/missing_icon.png"))
                };
                ToolTip.SetTip(_MissingImages, "Look for images not built");
                this.ToolbarBookmarks.Children.Add(_MissingImages);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public Movies? CurrentMovie { get => currentMovie; set => currentMovie = value; }

        //}
        /// <summary>
        /// Gets the formErrorsVal.
        /// </summary>
        public TextBlock formErrorsVal => this.FindControl<TextBlock>("FormErrors");

        //    DataContextChanged += this.MovieEditDialog_DataContextChanged;
        /// <summary>
        /// Gets or sets the CurrentMovieModel.
        /// </summary>
        //public ViewModels.MovieViewModelBase CurrentMovieModel { get => currentMovieModel; set => currentMovieModel = value; }
        /// <summary>
        /// Gets or sets a value indicating whether Initialising.
        /// </summary>
        public bool Initialising { get => initialising; set => initialising = value; }

        //    model.NewPhrase = null;
        //    model.NewSubPhrase = null;
        //    Initialized += this.MovieEditDialog_Initialized;
        /// <summary>
        /// Gets the movieDurationVal.
        /// </summary>
        public TextBox movieDurationVal => this.FindControl<TextBox>("movieDurationSecs");

        //    SetupActions();
        //public MainWindowViewModel? mainWindowViewModel { get; private set; }
        //public MovieViewModel? movieViewModel { get; private set; }
        //public MovieViewModelBase? movieViewModelBase { get; private set; }
        public MovieEditViewModel? movieEditViewModel { get; internal set; }

        //    SetupControls(model);
        public StackPanel OkEditMovie { get; set; }

        #endregion Public Properties

        #region Private Methods

        //    InitializeComponent();
        /// <summary>
        /// The AddPhrase.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void AddPhrase(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            MainWindow? main = TaymadeEntities.Support.Support.GetMainWindow() as MainWindow;

            if (button != null && main != null)
            {
                MovieEditViewModel? model = button.DataContext as MovieEditViewModel;

                if (model != null && model?.CurrentMovie != null)
                {
                    //ComboBox cbPhrase = this.FindControl<ComboBox>("cbGroup");

                    if (model?.NewPhrase != null)
                    {
                        string? group = model?.CurrentMovie.FilmGroup;
                        PhraseEntry? id = model?.NewPhrase;
                        if (model?.NewSubPhrase != null)
                        {
                            id = model?.NewSubPhrase;
                        }

                        if (id != null && group != null)
                        {
                            if (string.IsNullOrEmpty(group) && !group.Contains(id.Id))
                                group += id.Id;
                            else if (!group.Contains(id.Id))
                                group += "," + id.Id;
                            model?.CurrentMovie.FilmGroup = group;

                            if (string.IsNullOrEmpty(model?.CurrentMovie.PrimaryFilmGroup)) model.CurrentMovie.PrimaryFilmGroup = id.Id;
                        }

                        // generate MovieGenre
                        MovieGenre movieGenre = new MovieGenre()
                        {
                            MovieId = model.CurrentMovie.Id,
                            Genre = model?.NewPhrase?.COMPKEY
                        };
                        if (model?.NewSubPhrase != null)
                        {
                            movieGenre.SubGenre = model.NewSubPhrase.COMPKEY;
                            //   movieGenre.SubGenreEntity = model.NewSubPhrase;
                        }
                        movieGenre.Insert();
                        model?.CurrentGenre = movieGenre;
                        model?.CurrentMovie?.MovieGenres?.Add(movieGenre);
                    }
                }
            }
        }

        private async void EditBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                await WaitOnBookmarkEditor(button);
            }
        }

        private async Task WaitOnBookmarkEditor(Button button)
        {
            if (button.DataContext is MovieEditViewModel movieEditViewModel &&
                movieEditViewModel.CurrentBookmark is Bookmark bookmark)
            {
                var editBookmarkDialog = new EditBookmarkDialog
                {
                    DataContext = movieEditViewModel.CurrentBookmark
                };

                bool result = await editBookmarkDialog.ShowDialog<bool>(this);

                if (result)
                {
                    bookmark.Save();
                }

                editBookmarkDialog.DataContext = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieEditDialog"/> class.
        /// </summary>
        /// <param name="model">The model<see cref="ViewModels.MovieViewModel"/>.</param>
        //public MovieEditDialog(ViewModels.MovieViewModelBase model)
        //{
        //    Initialising = true;
        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close(false);
        }

        /// <summary>
        /// The EpisodeChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        private void EpisodeChanged(object? sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            //ViewModels.MovieViewModel? model = DataContext as ViewModels.MovieViewModel;

            //if (comboBox != null && CurrentMovie != null)
            //{
            //    TVEpisode? newEpisode = comboBox.SelectedItem as TVEpisode;

            //    if (newEpisode != null)
            //    {
            //        CurrentMovie.Episode = newEpisode.Id;

            //        if (newEpisode.MovieId == null)
            //        {
            //            newEpisode.MovieId = CurrentMovie.Id;
            //            newEpisode.Save();
            //        }
            //    }
            //}
        }

        /// <summary>
        /// The GetBookmarks.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private async void GetBookmarks(object sender, RoutedEventArgs e)
        {
            Views.MainWindow? main = GetWindow() as Views.MainWindow;
            if (main != null)
            {
                MovieEditViewModel? vm = main.DataContext as MovieEditViewModel;
                Button? button = sender as Button;
                if (button != null)
                {
                    //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;
                    if (CurrentMovie != null)
                    {
                        if (Support.FFMpegSupport.FfMpegProc != null && !Support.FFMpegSupport.FfMpegProc.HasExited && Support.FFMpegSupport.FfMpegProc.ProcessName.ToLower() == "vlc")
                        {
                            Support.FFMpegSupport.FfMpegProc.Kill();
                        }

                        Support.FFMpegSupport mpegSupport = new Support.FFMpegSupport();

                        //bool success = await mpegSupport.GetChapterFileAsync(CurrentMovie);

                        //if (success)
                        //{
                        //    string chapterFile = string.Empty;
                        //    string fixedPath = Support.Support.FixImagePath((CurrentMovie.MoviePath));

                        //    string metafilePath = Support.FFMpegSupport.GetFFMetaDataPath(CurrentMovie.MoviePath);
                        //    if (!string.IsNullOrEmpty(metafilePath))
                        //    {
                        //        // read in file

                        //        using (StreamReader stream = new(metafilePath))
                        //        {
                        //            string chapterfile = stream.ReadToEnd();

                        //            stream.Close();

                        //            Chapter chapter = null;
                        //            do
                        //            {
                        //                chapter = GetChapter(ref chapterfile);

                        //                // have we got a chapter file?
                        //                if (chapter.Found)
                        //                {
                        //                    double time = chapter.Start / 1000;
                        //                    time = Math.Truncate(time);
                        //                    Bookmark? bookmark = CurrentMovie.Bookmarks.Where(x => x.TruncTime == time).FirstOrDefault();

                        //                    if (bookmark == null)
                        //                    {
                        //                        bookmark = new Bookmark()
                        //                        {
                        //                            Name = chapter.Title,
                        //                            Time = time,
                        //                            MovieID = CurrentMovie.Id,
                        //                            Type = "BOOKMARK"
                        //                        };

                        //                        await vm.AddActualBookmark(main, bookmark);

                        //                        CurrentMovie.Bookmarks.Add(bookmark);
                        //                    }
                        //                }
                        //            } while (chapter.Found);
                        //        }
                        //    }
                        //}

                        // look for bmp and jpgs in base directory

                        string? tempFilename = Support.Support.FixImagePath(CurrentMovie.MoviePath);

                        string? imagename = Path.GetFileNameWithoutExtension(tempFilename);

                        string? path = Path.GetDirectoryName(tempFilename);

                        string[] images = Directory.GetFiles(path, imagename + "*.bmp");

                        await GetExistingImage(main, CurrentMovie, null, images, imagename);

                        images = Directory.GetFiles(path, imagename + "*.jpg");

                        await GetExistingImage(main, CurrentMovie, null, images, imagename);
                    }
                }
            }
        }

        /// <summary>
        /// The GetChapter.
        /// </summary>
        /// <param name="chapterfile">The chapterfile<see cref="string"/>.</param>
        /// <returns>The <see cref="Chapter"/>.</returns>
        private Chapter GetChapter(ref string chapterfile)
        {
            Chapter returnValue = new Chapter();

            int pos = chapterfile.IndexOf("[CHAPTER]");

            if (pos >= 0)
            {
                returnValue.Found = true;

                chapterfile = chapterfile.Substring(pos + 9);

                pos = chapterfile.IndexOf("[CHAPTER]");

                string contents = string.Empty;
                if (pos >= 0)
                {
                    contents = chapterfile.Substring(0, pos);
                    chapterfile = chapterfile.Substring(pos);

                    returnValue.GetTimeBase(contents);
                    returnValue.GetTitle(contents);
                    returnValue.GetStart(contents);
                    returnValue.GetEnd(contents);
                }
                else contents = chapterfile;
            }
            else returnValue.Found = false;

            return returnValue;
        }

        private ReactiveCommand<Unit, Unit> GetCommand(ICommand command)
        {
            ReactiveCommand<Unit, Unit> myCommand = ReactiveCommand.Create(() =>
            {
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            });

            return myCommand;
        }

        private Movies? GetCurrentMovie(object? datacontext)
        {
            Movies? returnItem = null;
            if (datacontext != null)
            {
                if (datacontext is MovieEditViewModel)
                {
                    MovieEditViewModel? movieEditViewModel = datacontext as MovieEditViewModel;
                    returnItem = movieEditViewModel.CurrentMovie;
                }

                if (datacontext is MovieEditViewModel)
                {
                    MovieEditViewModel? movieEditViewModel = datacontext as MovieEditViewModel;
                    returnItem = movieEditViewModel.CurrentMovie;
                }
            }

            return returnItem;
        }

        /// <summary>
        /// The GetDirector.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void GetDirector(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;

                if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.DirectorsName))
                {
                    string newName = CurrentMovie.DirectorsName;

                    if (movieEditViewModel?.DirectorList != null)
                    {
                        Director? director = movieEditViewModel.DirectorList.Where(x => x.Name.ToLower() == newName.ToLower()).FirstOrDefault();
                        if (director == null)
                        {
                            director = new Director()
                            {
                                Name = newName
                            };

                            DataController.SandboxEntities.Directors.Add(director);
                            DataController.SandboxEntities.SaveChanges();

                            movieEditViewModel.DirectorList.Add(director);
                        }
                    }
                }
            }
        }

        private async void GetDuration(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                ViewModels.MovieEditViewModel? model = button.DataContext as ViewModels.MovieEditViewModel;

                if (model != null && CurrentMovie != null)
                {
                    await CurrentMovie.GetDuration();
                    //int time = await Support.VideoSupport.GetDurationSeconds(CurrentMovie.MoviePath, CurrentMovie);

                    //Movies temp = CurrentMovie;
                    //CurrentMovie.DurationSeconds = time;
                    //System.TimeSpan ts = temp.MovieDuration;
                    //CurrentMovie = temp;
                    //CurrentMovie.Save();

                    //TaymadeEntities.Support.Support.PlayMovie(Support.FixImagePath(CurrentMoviePath), null);
                }
            }
        }

        /// <summary>
        /// The GetDuration.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        ///
        /// <summary>
        /// The getEpisodesCombo.
        /// </summary>
        /// <returns>The <see cref="ComboBox"/>.</returns>
        //private ComboBox getEpisodesCombo()
        //{
        //    return seriesUserControl.Find<ComboBox>("EpisodeList");
        //}

        /// <summary>
        /// Gets the existing image.
        /// </summary>
        /// <param name="main">The main.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="model">The model.</param>
        /// <param name="images">The images.</param>
        /// <param name="baseFilename">The base filename.</param>
        /// <autogeneratedoc />
        private async Task GetExistingImage(MainWindow? main, Movies? currentMovie, MovieEditViewModel? model, string[] images, string baseFilename)
        {
            foreach (string image in images)
            {
                string filename = Path.GetFileNameWithoutExtension(image).Replace(baseFilename, "");

                int i = filename.Length - 1;

                string digit = string.Empty;

                while (i >= 0 && char.IsDigit(filename[i]))
                {
                    digit = filename[i] + digit;
                    i -= 1;
                }

                double duration = 0;

                if (double.TryParse(digit, out duration))
                {
                    Bookmark? bookmark = currentMovie.Bookmarks.Where(x => x.TruncTime == duration).FirstOrDefault();

                    if (bookmark == null)
                    {
                        bookmark = new Bookmark()
                        {
                            Name = duration.ToString(),
                            Time = duration,
                            MovieID = currentMovie.Id,
                            ImagePath = image,
                            Type = "BOOKMARK"
                        };

                        if (model == null)
                        {
                            return;
                        }

                        // await model.AddActualBookmark(main, bookmark);

                        currentMovie.Bookmarks.Add(bookmark);
                    }
                }
            }
        }

        /// <summary>
        /// The GetGroup.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private async void GetGroup(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;

                if (CurrentMovie != null)
                {
                    PhraseSelectDialog phraseSelectDialog = new PhraseSelectDialog();
                    PhraseViewModel viewModel = new PhraseViewModel();

                    if (viewModel != null)
                    {
                        phraseSelectDialog.DataContext = viewModel;
                        //viewModel.Caller = phraseSelectDialog;

                        DialogResultButton result = await phraseSelectDialog.ShowDialog<DialogResultButton>(this);

                        if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                        {
                            if (viewModel.CurrentSubPhrase != null)
                                CurrentMovie.FilmGroup = viewModel.CurrentSubPhrase.Id;
                            else if (viewModel.CurrentPhrase != null)
                            {
                                CurrentMovie.FilmGroup = viewModel.CurrentPhrase.Id;
                                CurrentMovie.PrimaryFilmGroup = viewModel.CurrentPhrase.Id;
                            }

                            CurrentMovie.CreateXSPFDirectory(viewModel.CurrentPhrase, CurrentMovie.MovieName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The GetSeasonCombo.
        /// </summary>
        /// <returns>The <see cref="ComboBox"/>.</returns>
        //private ComboBox GetSeasonCombo()
        //{
        //    return seriesUserControl.Find<ComboBox>("SeasonList");
        //}

        /// <summary>
        /// The GetSeriesCombo.
        /// </summary>
        /// <returns>The <see cref="ComboBox"/>.</returns>
        //private ComboBox GetSeriesCombo()
        //{
        //    return seriesUserControl.Find<ComboBox>("SeriesList");
        // }

        /// <summary>
        /// The GetWindow.
        /// </summary>
        /// <returns>The <see cref="Window"/>.</returns>
        private Window GetWindow()
        {
            if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        /// <summary>
        /// The MoveFile.
        /// </summary>
        /// <param name="newFilename">The newFilename<see cref="string"/>.</param>
        /// <param name="existingPath">The existingPath<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool MoveFile(string newFilename, string existingPath)
        {
            if (!File.Exists(newFilename) && File.Exists(existingPath) && newFilename != existingPath)
            {
                File.Move(existingPath, newFilename);

                return true;
            }
            else return false;
        }

        /// <summary>
        /// The MoveFolder.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private async void MoveFolder(object sender, RoutedEventArgs e)
        {
            //MovieViewModel? viewModel = DataContext as MovieViewModel;

            Window? window = Support.Support.GetMainWindow() as Window;

            if (window != null)
            {
                {
                    if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MoviePath))
                    {
                        string existingPath = TaymadeEntities.Support.Support.FixImagePath(CurrentMovie.MoviePath);

                        if (File.Exists(existingPath))
                        {
                            string? directory = Path.GetDirectoryName(existingPath);
                            var topLevel = TopLevel.GetTopLevel(this);

                            // Start async operation to open the dialog.
                            var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                            {
                                Title = "Open File",
                                AllowMultiple = false
                            });
                            //OpenFolderDialog? dialog = new();
                            //dialog.Directory = directory;
                            //// see which folder to open from
                            //var result = await dialog.ShowAsync(window);
                            if (files != null && files.Count > 0)
                            {
                                string result = files[0].Path.LocalPath;
                                // this will be the new folder.
                                string newFilename = files[0].Path.LocalPath + @"\" + Path.GetFileName(existingPath);
                                MoveOrRenameMovie(CurrentMovie, existingPath, newFilename);

                                newFilename = MoveOrRenameNfoFiles(directory, result, newFilename);

                                newFilename = MoveOrRenameBookmarkFolder(result, newFilename);
                            }
                        }
                        //else
                        //{
                        //    CurrentMovie.PathWrong = true;
                        //    CurrentMovie.Save();
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// Moves the or rename bookmark files.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="result">The result.</param>
        /// <param name="newMoviename">The new moviename.</param>
        /// <param name="oldMovieName">Old name of the movie.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        private string MoveOrRenameBookmarkFiles(Movies? currentMovie, string? result, string newMoviename, string oldMovieName)
        {
            string newFilename = string.Empty;
            if (currentMovie != null && currentMovie.Bookmarks != null)
            {
                foreach (Bookmark bkm in currentMovie.Bookmarks)
                {
                    if (!string.IsNullOrEmpty(bkm.ImagePath))
                    {
                        newFilename = result + @"\" + Path.GetFileName(bkm.ImagePath).Replace(oldMovieName, newMoviename);
                        File.Move(bkm.ImagePath, newFilename);
                        bkm.ImagePath = newFilename;
                        bkm.Save();
                    }
                }

                if (currentMovie.Bookmarks.Count > 0)
                {
                    if (!string.IsNullOrEmpty(currentMovie.Bookmarks.FirstOrDefault().ImagePath))
                    {
                        currentMovie.ImagePath = currentMovie.Bookmarks.FirstOrDefault().ImagePath;
                    }
                }
            }

            return newFilename;
        }

        /// <summary>
        /// Moves the or rename bookmark folder.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="result">The result.</param>
        /// <param name="newFilename">The new filename.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        private string MoveOrRenameBookmarkFolder(string? result, string newFilename)
        {
            if (CurrentMovie != null && CurrentMovie.Bookmarks != null)
            {
                foreach (Bookmark bkm in CurrentMovie.Bookmarks)
                {
                    if (!string.IsNullOrEmpty(bkm.ImagePath))
                    {
                        newFilename = result + @"\" + Path.GetFileName(bkm.ImagePath);
                        MoveFile(newFilename, bkm.ImagePath);
                        bkm.ImagePath = newFilename;
                        bkm.Save();
                    }
                }

                if (CurrentMovie.Bookmarks.Count > 0)
                {
                    if (!string.IsNullOrEmpty(CurrentMovie.Bookmarks.FirstOrDefault().ImagePath))
                    {
                        CurrentMovie.ImagePath = CurrentMovie.Bookmarks.FirstOrDefault().ImagePath;
                    }
                }
            }

            return newFilename;
        }

        /// <summary>
        /// Moves the or rename movie.
        /// </summary>
        /// <param name="currentMovie">The view model.</param>
        /// <param name="existingPath">The existing path.</param>
        /// <param name="newFilename">The new filename.</param>
        /// <autogeneratedoc />
        private void MoveOrRenameMovie(Movies? currentMovie, string existingPath, string newFilename)
        {
            if (MoveFile(newFilename, existingPath))
            {
                currentMovie.MoviePath = TaymadeEntities.Support.Support.FixPathBack(newFilename);
                //CurrentMovie.PathWrong = false;
                currentMovie.Save();
            }
        }

        /// <summary>
        /// The InitializeComponent.
        /// </summary>
        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
        /// <summary>
        /// Moves the or rename nfo files.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="result">The result.</param>
        /// <param name="newFilename">The new filename.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        private string MoveOrRenameNfoFiles(string? directory, string? result, string newFilename)
        {
            IEnumerable<string> matchingFiles = Directory.EnumerateFiles(directory, "*.NFO", SearchOption.TopDirectoryOnly);

            foreach (string item in matchingFiles)
            {
                newFilename = directory + @"\" + Path.GetFileName(item);
                MoveFile(newFilename, item);
            }

            return newFilename;
        }

        /// <summary>
        /// The MovieEditDialog_Closed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void MovieEditDialog_Closed(object? sender, EventArgs e)
        {
            // this.MovieBookmarks.BookmarkUserControl.DataContext = null;
            // this.MovieBookmarks.BookmarkUserControl = null;
            // this.Series = null;
            // this.MovieBookmarks.DataContext = null;
            //  this.MovieBookmarks = null;
            //this.BookmarkDetails.DataContext = null;
            // this.BookmarkDetails = null;
            this.DataContext = null;
        }

        /// <summary>
        /// Set local properties for the possible view models
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 23/02/2026 23/02/2026 </created>
        /// </remarks>
        private void MovieEditDialog_DataContextChanged(object? sender, EventArgs e)
        {
            if (this.DataContext != null)
            {
                //if (this.DataContext is MainWindowViewModel)
                //{
                //    mainWindowViewModel = this.DataContext as MainWindowViewModel;
                //    CurrentMovie = mainWindowViewModel.CurrentMovie;
                //}

                //if (this.DataContext is MovieViewModel)
                //{
                //    movieViewModel = this.DataContext as MovieViewModel;
                //    CurrentMovie = movieViewModel.CurrentMovie;
                //}

                //if (this.DataContext is MovieViewModelBase)
                //{
                //    movieViewModelBase = this.DataContext as MovieViewModelBase;
                //    CurrentMovie = movieViewModelBase.CurrentMovie;
                //}

                if (this.DataContext is MovieEditViewModel)
                {
                    movieEditViewModel = this.DataContext as MovieEditViewModel;
                    CurrentMovie = movieEditViewModel.CurrentMovie;
                    //this.MovieBookmarks.DataContext = movieEditViewModel;
                    //this.MovieBookmarks.BookmarkUserControl = this.BookmarkDetails;
                }

                this.SetButtonCommands();
            }
        }

        private void MovieEditDialog_Initialized(object? sender, EventArgs e)
        {
        }

        /// <summary>
        /// The MovieEditDialog_Opened.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void MovieEditDialog_Opened(object? sender, EventArgs e)
        {
            Initialising = false;
            //if (Screens.ScreenCount > 1 && DataController.ShowOnAlternateScreen())
            //{
            //    if (Support.Support.GetScreenId() != null)
            //    {
            //        if (Support.Support.GetScreenId() > 0)
            //            this.Position = new PixelPoint(-800, 50);
            //    }
            //}
            this.WindowState = WindowState.Maximized;
            this.BookmarksTab.Width = this.Width - 5;
            this.SetButtonCommands();
        }

        private void MovieEditDialog_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            PixelPoint point = e.Point;

            Debug.WriteLine("x = : " + point.X + ", " + point.Y);
        }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //this.movieEditViewModel.CurrentMovie.Save();
            this.Close(true);
        }

        /// <summary>
        /// Renames the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        private async void RenameFile(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                Movies? CurrentMovie = GetCurrentMovie(button.DataContext);
                //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;
                //if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MoviePath))
                //{
                //    string fileName = Path.GetFileNameWithoutExtension(CurrentMovie.MoviePath);
                //    var result = await TextInputDialog.Prompt(
                //        initialValue: fileName,
                //        parentWindow: this,
                //        title: "Text Input Dialog Title",
                //        caption: "Caption",
                //        isRequired: true
                //     );

                //    if (result != null)
                //    {
                //        string existingPath = CurrentMovie.MoviePath;
                //        string directory = Path.GetDirectoryName(existingPath);

                //        string newFilename = directory + @"\" + result + Path.GetExtension(existingPath);
                //        MoveOrRenameMovie(CurrentMovie, existingPath, newFilename);

                //        newFilename = MoveOrRenameNfoFiles(directory, result, newFilename);

                //        newFilename = MoveOrRenameBookmarkFiles(CurrentMovie, directory, result, Path.GetFileNameWithoutExtension(existingPath));
                //        // create new file name and rename old file to new, for completeness all similar file name need renaming
                //        //txtTextInputResult.Text = result;
                //    }
                // }
            }
        }

        /// <summary>
        /// Renames the folder.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        private async void RenameFolder(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;

                if (CurrentMovie != null && !string.IsNullOrEmpty(CurrentMovie.MoviePath))
                {
                    string? originalFolder = Path.GetDirectoryName(CurrentMovie.MoviePath);
                    string? folder = Support.PathExtensions.GetLastPathSegment(originalFolder);

                    string? stub = Path.GetDirectoryName(CurrentMovie.MoviePath).Replace(folder, "");

                    //string fileName = Path.GetDirectoryName(CurrentMovie.MoviePath);
                    //var result = await TextInputDialog.Prompt(
                    //    initialValue: folder,
                    //    parentWindow: this,
                    //    title: "Folder Rename",
                    //    caption: "Rename Folder",
                    //    isRequired: true
                    // );

                    //string newFolder = stub + result.Trim();

                    //if (!Directory.Exists(newFolder))
                    //{
                    //    Directory.Move(originalFolder, newFolder);

                    //    // then rename MoviePath, ImagePath and Bookmark ImagePaths; can be done with string replace

                    //    CurrentMovie.MoviePath = CurrentMovie.MoviePath.Replace(originalFolder, newFolder);
                    //    CurrentMovie.ImagePath = CurrentMovie.ImagePath.Replace(originalFolder, newFolder);

                    //    foreach (var bookmark in CurrentMovie.Bookmarks)
                    //    {
                    //        bookmark.ImagePath = bookmark.ImagePath.Replace(originalFolder, newFolder);
                    //        bookmark.Save();
                    //    }

                    //    CurrentMovie.Save();
                    //}
                }
            }
        }

        private void SetupActions()
        {
            Opened += MovieEditDialog_Opened;
            Closed += MovieEditDialog_Closed;
            PositionChanged += MovieEditDialog_PositionChanged;

            // add content to okbuttoncontrol content
            //ContentControl? contentControl = this.OkButtonPanelEditMovie.ExtraButtons;

            this.OkEditMovie = this.OkButtonPanelEditMovie;

            if (this.OkButtonPanelEditMovie.Children != null)

            {
                TaymadeControls.Buttons.ImagedButton searchButton =
                    new()
                    {
                        LabelText = "Search",
                        ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://TaymadeControls/Assets/search_icon.png"))),
                        HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.F, Avalonia.Input.KeyModifiers.Control)
                    };

                ToolTip.SetTip(searchButton, "Search for movie in TMDB (Ctrl F)");

                this.OkButtonPanelEditMovie.Children.Add(searchButton);

                TaymadeControls.Buttons.ImagedButton contentButton =
                    new()
                    {
                        LabelText = "Chapters",
                        ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://TaymadeControls/Assets/build.png"))),
                        HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.Insert, Avalonia.Input.KeyModifiers.Alt)
                    };

                ToolTip.SetTip(contentButton, "Build Chapter details in Movie file using the Bookmarks (Alt Ins)");
                this.OkButtonPanelEditMovie.Children.Add(contentButton);

                // MovieViewModelBase? mvmb = this.DataContext as MovieViewModelBase;
                //if (movieViewModelBase != null) // get commands from view model and set to buttons
                //{
                //    // Ok and Cancel can be generated directly from the view model,
                //    // as they are standard across all dialogs,
                //    // so can be added to the base view model and base dialog
                //    // local controls need generating in the dialog and then commands set from the view model
                //    OkButtonPanelEditMovie.OkButton.Command = movieViewModelBase.AddOKCommand();
                //    OkButtonPanelEditMovie.CancelButton.Command = movieViewModelBase.AddCancelCommand();

                //    searchButton.Command = movieViewModelBase.GetSearchCommand();
                //    MovieViewModel? movieViewModel = this.DataContext as MovieViewModel;
                //    contentButton.Command = movieViewModelBase.GetChaptersCommand();
                //}
            }
        }
        /// <summary>
        /// The SeasonChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        //private void SeasonChanged(object? sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox? comboBox = sender as ComboBox;

        //    ViewModels.MovieViewModelBase? model = DataContext as ViewModels.MovieViewModelBase;
        //    if (comboBox != null && !initialising)
        //    {
        //        Season? season = comboBox.SelectedItem as Season;
        //        if (season != null && model != null && model.CurrentMovie != null)
        //        {
        //            model.CurrentMovie.Season = season.Id;
        //            model.CurrentMovie.SeasonEntity = season;

        //            comboBox.SelectedItem = season;

        //            ComboBox? episodesCombo = getEpisodesCombo();
        //            if (episodesCombo != null)
        //            {
        //                episodesCombo.ItemsSource = season.TVEpisodes;
        //            }
        //            //update episodes;
        //        }
        //    }
        //}

        /// <summary>
        /// The SeriesChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object?"/>.</param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/>.</param>
        //private void SeriesChanged(object? sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox? series = sender as ComboBox;
        //    if (series != null && !this.initialising)
        //    {
        //        Series? newSeries = series.SelectedItem as Series;

        //        ViewModels.MovieViewModelBase? model = DataContext as ViewModels.MovieViewModelBase;

        //        if (model != null && model.CurrentMovie != null && newSeries != null && newSeries.Id != 22)
        //        {
        //            model.CurrentMovie.SeriesEntity = newSeries;
        //            if (newSeries != null)
        //            {
        //                model.CurrentMovie.Series = newSeries.Id;
        //                model.CurrentMovie.SeriesEntity = newSeries;
        //                if (model.CurrentMovie.Series != 2 && model.CurrentMovie.Series != 22)
        //                {
        //                    seriesUserControl.IsVisible = true;
        //                    ComboBox? seasonCombo = GetSeasonCombo();
        //                    if (seasonCombo != null)
        //                    {
        //                        seasonCombo.ItemsSource = newSeries.Seasons;
        //                    }
        //                }
        //                else seriesUserControl.IsVisible = false;
        //            }
        //        }

        //        seriesUserControl.IsVisible = true;
        //    }
        //}

        private void SetupControls(MovieEditViewModel model)
        {
            movieEditViewModel = model;
            CurrentMovie = model.CurrentMovie;

            //MainWindowViewModel? mainWindowViewModel = Support.Support.GetMainWindowViewModel();
            DataContext = model;

            //if (movieEditViewModel != null && CurrentMovie != null)
            //{
            //    if (CurrentMovie.MovieGenres == null || CurrentMovie.MovieGenres.Count == 0)
            //    {
            //        CurrentMovie.BuildGenreList();
            //    }
            //}

            //seriesUserControl = this.Series;
            //// change series control visibility
            //if (CurrentMovie.Series == null || CurrentMovie.Series == 2)
            //{
            //    this.Series.IsVisible = false;
            //}
            //else this.Series.IsVisible = true;

            model.DirectorList = new ObservableCollection<Director>(DataController.DirectorList);
            if (model != null && CurrentMovie != null)
            {
                model.ByMovie = true;

                // fix up movie.

                CurrentMovie.FixMovieData();

                if (!File.Exists(CurrentMovie.MoviePath))
                {
                    CurrentMovie.ErrorText = "Can't find film";
                }

                if (CurrentMovie.DirectorID != null && CurrentMovie.Director == null)
                {
                    CurrentMovie.Director = DataController.DirectorList.Where(x => x.Id == CurrentMovie.DirectorID).FirstOrDefault();
                }

                //if (seriesUserControl != null)
                //{
                //    //seriesUserControl.IsVisible = false;

                //    seriesUserControl.DataContext = model;

                //if (CurrentMovie.Series != null)
                //{
                //    CurrentMovie.SeriesEntity = DataController.SeriesList.Find(s => s.Id == CurrentMovie.Series);
                //    if (CurrentMovie.Series != 2) seriesUserControl.IsVisible = true;
                //}

                //ComboBox serList1 = GetSeriesCombo();
                //if (serList1 != null)
                //{
                //    serList1.ItemsSource = model.SeriesList;
                //    serList1.SelectedItem = CurrentMovie.SeriesEntity;
                //    serList1.SelectionChanged += SeriesChanged;
                //}

                //ComboBox seasonList = GetSeasonCombo();

                //if (seasonList != null)
                //{
                //    seasonList.SelectionChanged += SeasonChanged;

                //    if (CurrentMovie.SeasonEntity != null && CurrentMovie.SeasonEntity.TVEpisodes != null)
                //    {
                //        seasonList.ItemsSource = CurrentMovie.SeriesEntity.Seasons;
                //    }
                //}

                //ComboBox episodeList = getEpisodesCombo();
                //if (episodeList != null)
                //{
                //    episodeList.SelectionChanged += EpisodeChanged;
                //    ;
                //}
                //  }

                //ComboBox serList = this.SeriesList;
                //if (serList != null)
                //{
                //    serList.SelectedItem = CurrentMovie.SeriesEntity;
                //    serList.SelectionChanged += SeriesChanged;
                //  }
            }

            // set up cast control
            //if (this.MovieCast != null)
            //{
            //    this.MovieCast.DataContext = model;
            //    this.MovieCast.ActorDetail = this.ActorDetail;

            //    if (CurrentMovie != null &&
            //            CurrentMovie.Casts != null &&
            //            CurrentMovie.Casts.Count < 1)
            //    {
            //        CurrentMovie.Casts = new ObservableCollection<Cast>
            //            (
            //            DataController.SandboxEntities.Casts.Where(m => m.MovieID == CurrentMovie.Id).ToList()
            //            );
            //    }
            //}

            //if (this.BookmarkDetails != null)
            //{
            //    if (movieViewModelBase== null)
            //    {
            //        if (movieEditViewModel != null)
            //        {
            //            movieViewModelBase = movieEditViewModel;
            //        }

            //    }

            //    this.BookmarkDetails.EditBookmark.Command = movieViewModelBase.EditBookmark;
            //    this.BookmarkDetails.DelBookmark.Command = movieViewModelBase.DelBookmark;
            //    this.BookmarkDetails.PlayBookmark.Command = movieViewModelBase.PlayBookmark;
            //}
        }

        /// <summary>
        /// The SetupControls.
        /// </summary>
        /// <param name="model">The model<see cref="MovieViewModel"/>.</param>
        private void SetupControlsBaseModel(MovieEditViewModel model)
        {
            movieEditViewModel = model;

            //MainWindowViewModel? mainWindowViewModel = Support.Support.GetMainWindowViewModel();
            DataContext = model;

            CurrentMovie = model.CurrentMovie;

            //if (movieViewModelBase != null && CurrentMovie != null)
            //{
            //    if (CurrentMovie.MovieGenres == null || CurrentMovie.MovieGenres.Count == 0)
            //    {
            //        CurrentMovie.BuildGenreList();
            //    }
            //}

            //seriesUserControl = this.Series;
            //// change series control visibility
            //if (CurrentMovie.Series == null || CurrentMovie.Series == 2)
            //{
            //    this.Series.IsVisible = false;
            //}
            //else this.Series.IsVisible = true;

            model.DirectorList = new ObservableCollection<Director>(DataController.DirectorList);
            if (model != null && CurrentMovie != null)
            {
                model.ByMovie = true;

                // fix up movie.

                CurrentMovie.FixMovieData();

                if (!File.Exists(CurrentMovie.MoviePath))
                {
                    CurrentMovie.ErrorText = "Can't find film";
                }

                if (CurrentMovie.DirectorID != null && CurrentMovie.Director == null)
                {
                    CurrentMovie.Director = DataController.DirectorList.Where(x => x.Id == CurrentMovie.DirectorID).FirstOrDefault();
                }

                //if (seriesUserControl != null)
                //{
                //    //seriesUserControl.IsVisible = false;

                //    seriesUserControl.DataContext = model;

                //if (CurrentMovie.Series != null)
                //{
                //    CurrentMovie.SeriesEntity = DataController.SeriesList.Find(s => s.Id == CurrentMovie.Series);
                //    if (CurrentMovie.Series != 2) seriesUserControl.IsVisible = true;

                //}

                //ComboBox serList1 = GetSeriesCombo();
                //if (serList1 != null)
                //{
                //    serList1.ItemsSource = model.SeriesList;
                //    serList1.SelectedItem = CurrentMovie.SeriesEntity;
                //    serList1.SelectionChanged += SeriesChanged;
                //}

                //ComboBox seasonList = GetSeasonCombo();

                //if (seasonList != null)
                //{
                //    seasonList.SelectionChanged += SeasonChanged;

                //    //if (CurrentMovie.SeasonEntity != null && CurrentMovie.SeasonEntity.TVEpisodes != null)
                //    //{
                //    //    seasonList.ItemsSource = CurrentMovie.SeriesEntity.Seasons;
                //    //}
                //}

                //ComboBox episodeList = getEpisodesCombo();
                //if (episodeList != null)
                //{
                //    episodeList.SelectionChanged += EpisodeChanged;
                //    ;
                //}
                // }

                //ComboBox serList = this.SeriesList;
                //if (serList != null)
                //{
                //    serList.SelectedItem = CurrentMovie.SeriesEntity;
                //    serList.SelectionChanged += SeriesChanged;
                //}

                //if (this.MovieCast != null)
                //{
                //    this.MovieCast.DataContext = model;
                //    this.MovieCast.ActorDetail = this.ActorDetail;

                //    if (CurrentMovie != null &&
                //        CurrentMovie.Casts != null &&
                //        CurrentMovie.Casts.Count < 1)
                //    {
                //        CurrentMovie.Casts = new ObservableCollection<Cast>
                //            (
                //            DataController.SandboxEntities.Casts.Where(m => m.MovieID == CurrentMovie.Id).ToList()
                //            );
                //    }
                //}


            }


        }

        private void SetButtonCommands()
        {
            MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
            if (viewModel != null)
            {
                AddBookmarks.Command = viewModel.NewBookmark;
                AddPoster.Command = viewModel.NewPoster;
                _playFromLast.Command = viewModel.PlayFromLast;
                _repeatLast.Command = viewModel.RepeatLast;
                _ReloadBookmarks.Command = viewModel.ReloadBookmarks;
                _MissingImages.Command = viewModel.GetMissingImages;
            }
        }

        private async void GrabImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
                if (viewModel != null)
                {
                    if (viewModel.CurrentMovie != null && viewModel.CurrentBookmark != null)
                    {
                     //   await Support.VideoSupport.GrabBookmarkImage(viewModel.CurrentMovie, viewModel.CurrentBookmark, 0);
                       // System.Threading.Thread.Sleep(1000);
                        var bmp = viewModel.CurrentBookmark.ImageBMP;
                        viewModel.CurrentBookmark.SetImageBMP();
                    }
                }
            }
        }

        #endregion Private Methods

    }
}