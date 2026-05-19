using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using TaymadeEntities.ViewModels;
using TaymadeEntities.Views;
using ExCSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaymadeControls;
using TaymadeControls.Buttons;
using Colors = Avalonia.Media.Colors;

namespace TaymadeEntities.Dialogs;

public partial class EditMovie : Window
{
    private ImagedButtonNoText _MissingImages;
    private ImagedButton _playFromLast;
    private ImagedButton _ReloadBookmarks;
    private ImagedButton _repeatLast;
    private ImagedButton AddBookmarks;
    private ImagedButton AddPoster;

    #region Public Constructors

    public EditMovie()
    {
        InitializeComponent();

        Initialized += EditMovie_Initialized;

        DataContextChanged += EditMovie_DataContextChanged;

        SizeChanged += EditMovie_SizeChanged;

        this.WindowState = WindowState.Maximized;
        this.BookmarksTab.Width = this.Width - 4;

        SetupToolbar();

        Closed += (_, _) =>
        {
            (DataContext as IDisposable)?.Dispose();
        };
    }

    #endregion Public Constructors

    #region Private Properties

    private MovieEditViewModel? ViewModel
    {
        get
        {
            return this.DataContext as MovieEditViewModel;
        }
    }

    #endregion Private Properties

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
            if (ViewModel != null && ViewModel?.CurrentMovie != null)
            {
                //ComboBox cbPhrase = this.FindControl<ComboBox>("cbGroup");

                if (ViewModel?.NewPhrase != null)
                {
                    string? group = ViewModel?.CurrentMovie.FilmGroup;
                    PhraseEntry? id = ViewModel?.NewPhrase;
                    if (ViewModel?.NewSubPhrase != null)
                    {
                        id = ViewModel?.NewSubPhrase;
                    }

                    if (id != null && group != null)
                    {
                        if (string.IsNullOrEmpty(group) && !group.Contains(id.Id))
                            group += id.Id;
                        else if (!group.Contains(id.Id))
                            group += "," + id.Id;
                        ViewModel?.CurrentMovie.FilmGroup = group;

                        if (string.IsNullOrEmpty(ViewModel?.CurrentMovie.PrimaryFilmGroup)) ViewModel.CurrentMovie.PrimaryFilmGroup = id.Id;
                    }

                    // generate MovieGenre
                    MovieGenre movieGenre = new MovieGenre()
                    {
                        MovieId = ViewModel.CurrentMovie.Id,
                        Genre = ViewModel?.NewPhrase?.COMPKEY
                    };
                    if (ViewModel?.NewSubPhrase != null)
                    {
                        movieGenre.SubGenre = ViewModel.NewSubPhrase.COMPKEY;
                        //   movieGenre.SubGenreEntity = model.NewSubPhrase;
                    }
                    movieGenre.Insert();
                    ViewModel?.CurrentGenre = movieGenre;
                    ViewModel?.CurrentMovie?.MovieGenres?.Add(movieGenre);
                }
            }
        }
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }

    private void EditMovie_DataContextChanged(object? sender, EventArgs e)
    {
        this.SetButtonCommands();
    }

    private void EditMovie_Initialized(object? sender, EventArgs e)
    {
        this.BookmarksTab.Width = this.Width - 4;
    }

    private void EditMovie_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        //throw new NotImplementedException();
    }
    /// <summary>
    /// The GetBookmarks.
    /// </summary>
    /// <param name="sender">The sender<see cref="object"/>.</param>
    /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
    private async void GetBookmarks(object sender, RoutedEventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            //ViewModels.MovieViewModel? model = button.DataContext as ViewModels.MovieViewModel;
            if (ViewModel != null && ViewModel.CurrentMovie != null)
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

                string? tempFilename = Support.Support.FixImagePath(ViewModel.CurrentMovie.MoviePath);

                string? imagename = Path.GetFileNameWithoutExtension(tempFilename);

                string? path = Path.GetDirectoryName(tempFilename);

                string[] images = Directory.GetFiles(path, imagename + "*.bmp");

                await GetExistingImage(this, ViewModel.CurrentMovie, null, images, imagename);

                images = Directory.GetFiles(path, imagename + "*.jpg");

                await GetExistingImage(this, ViewModel.CurrentMovie, null, images, imagename);
            }
        }
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

            if (ViewModel != null && ViewModel.CurrentMovie != null &&
                !string.IsNullOrEmpty(ViewModel.CurrentMovie.DirectorsName))
            {
                string newName = ViewModel.CurrentMovie.DirectorsName;

                if (ViewModel?.DirectorList != null)
                {
                    Director? director = ViewModel.DirectorList.Where(x => x.Name.ToLower() == newName.ToLower()).FirstOrDefault();
                    if (director == null)
                    {
                        director = new Director()
                        {
                            Name = newName
                        };

                        DataController.SandboxEntities.Directors.Add(director);
                        DataController.SandboxEntities.SaveChanges();

                        ViewModel.DirectorList.Add(director);
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
            // ViewModels.MovieEditViewModel? model = button.DataContext as ViewModels.MovieEditViewModel;

            if (ViewModel != null && ViewModel.CurrentMovie != null)
            {
                await ViewModel.CurrentMovie.GetDuration(ViewModel.CurrentMovie.MoviePath);
                //int time = await Support.VideoSupport.GetDurationSeconds(ViewModel.CurrentMovie.MoviePath, ViewModel.CurrentMovie);

                //Movies temp = ViewModel.CurrentMovie;
                //ViewModel.CurrentMovie.DurationSeconds = time;
                //System.TimeSpan ts = temp.MovieDuration;
                //CurrentMovie = temp;
                //CurrentMovie.Save();

                //TaymadeEntities.Support.Support.PlayMovie(Support.FixImagePath(CurrentMoviePath), null);
            }
        }
    }

    /// <summary>
    /// Gets the existing image.
    /// </summary>
    /// <param name="main">The main.</param>
    /// <param name="vm">The vm.</param>
    /// <param name="model">The model.</param>
    /// <param name="images">The images.</param>
    /// <param name="baseFilename">The base filename.</param>
    /// <autogeneratedoc />
    private async Task GetExistingImage(Window? main, Movies? currentMovie, MovieEditViewModel? model, string[] images, string baseFilename)
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
            // ViewModels.MovieEditViewModel? viewModel = this.DataContext as ViewModels.MovieEditViewModel;

            if (ViewModel != null && ViewModel.CurrentMovie != null)
            {
                PhraseSelectDialog phraseSelectDialog = new PhraseSelectDialog();
                PhraseViewModel phraseViewModel = new PhraseViewModel();

                if (phraseViewModel != null)
                {
                    phraseSelectDialog.DataContext = phraseViewModel;
                    //viewModel.Caller = phraseSelectDialog;

                    DialogResultButton result = await phraseSelectDialog.ShowDialog<DialogResultButton>(this);

                    if (result != null && result.Result == DialogResultButton.ResultType.Ok)
                    {
                        if (phraseViewModel.CurrentSubPhrase != null)
                            ViewModel.CurrentMovie.FilmGroup = phraseViewModel.CurrentSubPhrase.Id;
                        else if (phraseViewModel.CurrentPhrase != null)
                        {
                            ViewModel.CurrentMovie.FilmGroup = phraseViewModel.CurrentPhrase.Id;
                            ViewModel.CurrentMovie.PrimaryFilmGroup = phraseViewModel.CurrentPhrase.Id;
                        }

                        ViewModel.CurrentMovie.CreateXSPFDirectory(phraseViewModel.CurrentPhrase, ViewModel.CurrentMovie.MovieName);
                    }
                }
            }
        }
    }

    private async void GetImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (ViewModel != null && ViewModel.CurrentActor != null)
            {
                if (!string.IsNullOrEmpty(ViewModel.CurrentActor.Thumb))
                {
                    Avalonia.Media.Imaging.Bitmap? temp = await TmdbSupport.GetImageFromProfileAsync(ViewModel.CurrentActor.Thumb);


                    if (temp != null && ViewModel.CurrentActor.Id > 0)
                    {
                        ViewModel.CurrentActor.ImagePath = @"k:\TD1\MovieImages\ActorImages\id-" + ViewModel.CurrentActor.Id.ToString().Trim() + ".jpg";

                        if (!System.IO.File.Exists(Support.Support.FixImagePath(ViewModel.CurrentActor.ImagePath)))
                        {
                            temp.Save(Support.Support.FixImagePath(ViewModel.CurrentActor.ImagePath));
                        }
                        ViewModel.CurrentActor.ImageBMP = temp;
                        //temp?.Dispose();
                    }
                }
            }
        }
    }

    private async void GrabImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            //MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
            if (ViewModel != null)
            {
                if (ViewModel.CurrentMovie != null && ViewModel.CurrentBookmark != null)
                {
                    double tempTime = ViewModel.CurrentBookmark.Time.Value;

                    string moviePath = ViewModel.CurrentMovie.MoviePath;
                    string bookmarkImagePath = Path.GetDirectoryName(ViewModel.CurrentMovie.MoviePath) + @"\" +
                        Path.GetFileNameWithoutExtension(ViewModel.CurrentMovie.MoviePath)
                        + tempTime.ToString().Trim() + ".BMP";

                    FFMpegSupport fFMpegSupport = new FFMpegSupport();
                    string winThumbnailpath = await fFMpegSupport.GrabImage(moviePath, bookmarkImagePath, ViewModel.CurrentBookmark.Time);
                    // System.Threading.Thread.Sleep(1000);
                    var bmp = ViewModel.CurrentBookmark.ImageBMP;
                    ViewModel.CurrentBookmark.ImagePath = winThumbnailpath;
                    ViewModel.CurrentBookmark.SetImageBMP();
                    // save changed bookmark
                    bool success = await ViewModel.CurrentBookmark.SaveAsync();
                    fFMpegSupport = null;
                }
            }
        }
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
                if (ViewModel != null && ViewModel.CurrentMovie != null && !string.IsNullOrEmpty(ViewModel.CurrentMovie.MoviePath))
                {
                    string existingPath = TaymadeEntities.Support.Support.FixImagePath(ViewModel.CurrentMovie.MoviePath);

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
                            MoveOrRenameMovie(ViewModel.CurrentMovie, existingPath, newFilename);

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
        if (ViewModel != null && ViewModel.CurrentMovie != null && ViewModel.CurrentMovie.Bookmarks != null)
        {
            foreach (Bookmark bkm in ViewModel.CurrentMovie.Bookmarks)
            {
                if (!string.IsNullOrEmpty(bkm.ImagePath))
                {
                    newFilename = result + @"\" + Path.GetFileName(bkm.ImagePath);
                    MoveFile(newFilename, bkm.ImagePath);
                    bkm.ImagePath = newFilename;
                    bkm.Save();
                }
            }

            if (ViewModel.CurrentMovie?.Bookmarks.Count > 0)
            {
                if (!string.IsNullOrEmpty(ViewModel.CurrentMovie.Bookmarks.FirstOrDefault().ImagePath))
                {
                    ViewModel.CurrentMovie.ImagePath = ViewModel.CurrentMovie.Bookmarks.FirstOrDefault().ImagePath;
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

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.CurrentMovie?.Save();
        }
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
            //   Movies? CurrentMovie = GetCurrentMovie(button.DataContext);
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

            if (ViewModel != null && ViewModel.CurrentMovie != null &&
                !string.IsNullOrEmpty(ViewModel.CurrentMovie.MoviePath))
            {
                string? originalFolder = Path.GetDirectoryName(ViewModel.CurrentMovie.MoviePath);
                string? folder = Support.PathExtensions.GetLastPathSegment(originalFolder);

                string? stub = Path.GetDirectoryName(ViewModel.CurrentMovie.MoviePath).Replace(folder, "");

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
    }

    private void SetButtonCommands()
    {
        // MovieEditViewModel? viewModel = this.DataContext as MovieEditViewModel;
        if (ViewModel != null)
        {
            AddBookmarks.Command = ViewModel.NewBookmark;
            AddPoster.Command = ViewModel.NewPoster;
            _playFromLast.Command = ViewModel.PlayFromLast;
            _repeatLast.Command = ViewModel.RepeatLast;
            _ReloadBookmarks.Command = ViewModel.ReloadBookmarks;
            _MissingImages.Command = ViewModel.GetMissingImages;
        }
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


    private void dgCast_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Cast? cast = dgCast?.SelectedItem as Cast;

        if (cast != null)
        {
            //   if (ViewModel?.CurrentActor != null) { ViewModel.CurrentActor.Dispose(); }

            if (cast.Actor == null && cast.ActorId > 0)
            {
                ViewModel?.CurrentActor = new Actor(cast.ActorId.Value);
                ViewModel?.CurrentActor?.Parent = cast;
            }
            else
            {
                ViewModel?.CurrentActor = cast.Actor;
                ViewModel?.CurrentActor?.Parent = cast;
            }
            if (ViewModel != null && ViewModel.CurrentActor != null)
            {
                var ImageSource = ImageHelper.LoadFromFile(ViewModel?.CurrentActor.ImagePath);

                if (ImageSource != null)
                {
                    ViewModel?.CurrentActor.ImageBMP = ImageSource;
                }
            }
        }
    }


    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (sender != null && sender is Button)
        {
            Button? button = sender as Button;

            if (button.DataContext is MovieEditViewModel)
            {
                MovieEditViewModel viewModel = button.DataContext as MovieEditViewModel;
                if (viewModel.CurrentActor != null)
                {
                    viewModel.CurrentActor.Save();
                }
            }
        }
    }

    private async void SearchTMDB_Click(object? sender, RoutedEventArgs e)
    {
        if (sender != null && sender is Button)
        {
            Button? button = sender as Button;

            if (button != null && button.DataContext is MovieEditViewModel)
            {
                MovieEditViewModel viewModel = button.DataContext as MovieEditViewModel;
                if (viewModel != null && viewModel.CurrentActor != null)
                {
                    ActorSearchModel? actorSearchModel = new ActorSearchModel(viewModel.CurrentActor);
                    Dialogs.TMDBActorSearchDialog? searchDialog = new Dialogs.TMDBActorSearchDialog(actorSearchModel);

                    bool result = await searchDialog.ShowDialog<bool>(this);
                    if (result)
                    {
                        string? tmidb = actorSearchModel.GetTMIDB();

                        if (!string.IsNullOrEmpty(tmidb))  // not  cancelled or none selected
                        {
                            // need to set the tmdbid

                            viewModel.CurrentActor.TMDBID = int.Parse(tmidb);
                            //viewModel.CurrentActor.SetDetailsFromPerson(found);
                            //viewModel.CurrentActor.Save();
                        }
                        else if (actorSearchModel.CurrentActor != null)
                        {
                            viewModel.CurrentActor.TMDBID = actorSearchModel.CurrentActor.TMDBID;
                        }
                        //viewModel.CurrentActor.Save();
                        //actorSearchModel?.Dispose();
                        //actorSearchModel = null;
                        //searchDialog?.Dispose();
                        //searchDialog = null;
                        //found = null;

                    }
                    searchDialog = null;
                    //actorSearchModel?.Dispose();
                    //actorSearchModel = null;
                }
            }
        }
    }

    private void GetActor_Click(object? sender, RoutedEventArgs e)
    {
        if (sender != null && sender is Button)
        {
            Button? button = sender as Button;

            if (button != null && button.DataContext is MovieEditViewModel)
            {
                MovieEditViewModel viewModel = button.DataContext as MovieEditViewModel;
                if (viewModel != null && viewModel.CurrentActor != null)
                {
                    viewModel.CurrentActor.GetDetailsFromTMDB();
                }
            }
        }
    }

    private async void NewMember_Click(object? sender, RoutedEventArgs e)
    {
        if (sender != null && sender is Button)
        {
            Button? button = sender as Button;

            if (button != null && button.DataContext is MovieEditViewModel)
            {

                MovieEditViewModel viewModel = button.DataContext as MovieEditViewModel;

                // create a new cast entity can get the movie Id from the viewModel.CurrentMovie.
                // need to search for an actor
                if (viewModel != null && viewModel.CurrentMovie != null)
                {
                    Cast tempCastMember = new Cast();
                    tempCastMember.MovieID = viewModel.CurrentMovie.Id;
                    ActorSearchModel? actorSearchModel = new ActorSearchModel();
                    actorSearchModel.CurrentActor = new Actor()
                    {
                        Name = "<enter>"
                    };
                    Dialogs.TMDBActorSearchDialog? searchDialog = new Dialogs.TMDBActorSearchDialog(actorSearchModel);

                    bool result = await searchDialog.ShowDialog<bool>(this);
                    searchDialog = null;

                    if (result)
                    {
                        Person? found = actorSearchModel.FoundPerson;

                        if (found != null)  // not  cancelled or none selected
                        {
                            Actor? actor = DataController.ActorList
                                .Where(x => x.Name.ToLower() == found.Name.ToLower())
                                .FirstOrDefault();

                            if (actor != null) // we already know about this actor
                            {
                                Cast? castmember =
                                    actor.Casts
                                        .Where(x => x.MovieID == viewModel.CurrentMovie.Id)
                                        .FirstOrDefault() as Cast;

                                if (castmember == null)
                                {
                                    actor.SetDetailsFromPerson(found);

                                    tempCastMember.ActorId = actor.Id;
                                    //tempCastMember.Actor = actor;

                                    tempCastMember.MovieID = viewModel.CurrentMovie.Id;
                                    tempCastMember.Insert();
                                    tempCastMember.Actor = actor;
                                    tempCastMember.Movies = viewModel.CurrentMovie;
                                    viewModel.CurrentMovie.Casts.Add(tempCastMember);
                                }
                            }
                            else
                            {
                                actor = new Actor();
                                actor.Name = found.Name;
                                if (actor.Id == 0) actor.Insert();

                                actor.SetDetailsFromPerson(found);
                                //tempCastMember.Actor = actor;
                                DataController.ActorList.Add(actor);
                                tempCastMember.ActorId = actor.Id;
                                tempCastMember.MovieID = viewModel.CurrentMovie.Id;
                                tempCastMember.Insert();
                                tempCastMember.Actor = actor;
                                tempCastMember.Movies = viewModel.CurrentMovie;
                                viewModel.CurrentMovie.Casts.Add(tempCastMember);
                            }
                        }


                        actorSearchModel.Dispose();
                        actorSearchModel = null;
                    }

                }
            }

        }


    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
    }


    #endregion Private Methods

    // end of class
}