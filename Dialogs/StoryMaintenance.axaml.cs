using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using TaymadeEntities.Models;

namespace TaymadeEntities.Dialogs;

public partial class StoryMaintenance : Window, IDisposable
{
    private bool disposedValue;

    public StoryMaintenance()
    {
        InitializeComponent();

        if (NewAuthorName != null)
        {
            //NewAuthorName.Text = "Enter New Author";
        }
        DataContextChanged += StoryMaintenance_DataContextChanged;


    }

    public StoryMaintenance(StoryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        if (NewAuthorName != null)
        {
            //NewAuthorName.Text = "Enter New Author";
        }
        DataContextChanged += StoryMaintenance_DataContextChanged;

    }
    private void StoryMaintenance_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext != null)
        {
            StoryViewModel = DataContext as StoryViewModel;

            StoryViewModel?.Caller = this;
        }
    }

    private void AddSeries_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (NewSeriesName != null && !string.IsNullOrWhiteSpace(NewSeriesName.Text))
        {
            NewSeries = NewSeriesName.Text;
            TaymadeEntities.Models.StorySeries newSeries = new TaymadeEntities.Models.StorySeries()
            {
                Name = NewSeries
            };
            newSeries.Save();
        }
    }

    private void AddAuthor_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (NewAuthorName != null && !string.IsNullOrWhiteSpace(NewAuthorName.Text))
        {
            NewAuthor = NewAuthorName.Text;

            Author newAuthor = new Author()
            {
                Name = NewAuthor
            };



            // build the path to the new author's story directory
            string authorStoryPath = System.IO.Path.Combine(StoryViewModel.DefaultStoryDirectory + @"\pd\done\", newAuthor.Name);
            if (!Directory.Exists(authorStoryPath))
            {
                // create directory
                Directory.CreateDirectory(authorStoryPath);
            }
            newAuthor.StoryPath = authorStoryPath;
            newAuthor.Save();

            StoryViewModel?.Authors = new ObservableCollection<Author>(
                DataController.StoryController.GetAuthors());
            StoryViewModel?.Authors.Add(newAuthor);

        }

    }

    string? NewAuthor { get; set; }
    public StoryViewModel? StoryViewModel { get; private set; }
    public string? NewSeries { get; private set; }

    private void ImagedButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TaymadeEntities.Dialogs.PhraseDialog phraseDialog = new TaymadeEntities.Dialogs.PhraseDialog();

        phraseDialog.ShowDialog(this);
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
    // ~StoryMaintenance()
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

    private void SaveAuthor_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (StoryViewModel != null && StoryViewModel.CurrentAuthor != null)
        {
            // properties are bound to the CurrentAuthor, so no need to set the name explicitly
            StoryViewModel.CurrentAuthor.Save();
        }
    }
    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(true);
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }
}