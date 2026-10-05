using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using Avalonia.Interactivity;

namespace TaymadeEntities.Dialogs;

public partial class Maintenance : Window, IDisposable
{
    private bool disposedValue;

    public Maintenance()
    {
        InitializeComponent();

        DataContextChanged += StoryMaintenance_DataContextChanged;


    }

    public Maintenance(MaintenanceViewModel viewModel)
    {
        InitializeComponent();

        DataContextChanged += StoryMaintenance_DataContextChanged;

        DataContext = viewModel;
        MaintenaceViewModel = viewModel;
    }

    private void StoryMaintenance_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext != null)
        {
            MaintenaceViewModel = DataContext as ViewModels.MaintenanceViewModel;

        }
    }






    public MaintenanceViewModel? MaintenaceViewModel { get; private set; }


    private void Phrases_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TaymadeEntities.Dialogs.PhraseDialog phraseDialog = new TaymadeEntities.Dialogs.PhraseDialog();

        phraseDialog.ShowDialog(this);
    }

    private void SeriesMaintenance_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using SeriesViewModel seriesViewModel = new SeriesViewModel();
        using TaymadeEntities.Dialogs.SeriesDialog seriesMaintenance = new TaymadeEntities.Dialogs.SeriesDialog(seriesViewModel);
        seriesMaintenance.ShowDialog(this);
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
    // ~Maintenance()
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

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(true);
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }

    /// <summary>
    /// The AddToken.
    /// </summary>
    /// <param name="sender">The sender<see cref="object?"/>.</param>
    /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
    private void AddToken(object? sender, RoutedEventArgs e)
    {
        TextBox? newToken = this.newToken;

        if (newToken != null)
        {
            if (this.DataContext != null && this.DataContext is MaintenanceViewModel mvm)
            {
                if (mvm != null && !string.IsNullOrEmpty(newToken.Text))
                {
                    string temp = mvm.AutoComplete + "," + newToken.Text;

                    List<string> tempList = temp.Split(',').ToList();

                    tempList.Sort();

                    mvm.AutoComplete = string.Join(',', tempList);

                    mvm.AutoCompleteTokens = tempList;

                    DataGrid? dgAutoComplete = this.dgAutoComplete;

                    if (dgAutoComplete != null) dgAutoComplete.ItemsSource = mvm.AutoCompleteTokens;
                }
            }
        }
    }

    /// <summary>
    /// The DeleteToken.
    /// </summary>
    /// <param name="sender">The sender<see cref="object?"/>.</param>
    /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
    private void DeleteToken(object? sender, RoutedEventArgs e)
    {
        TextBlock? selectedToken = this.selectedToken;

        if (selectedToken != null)
        {
            if (this.DataContext != null && this.DataContext is MaintenanceViewModel mvm)
            {
                if (mvm != null && !string.IsNullOrEmpty(selectedToken.Text))
                {
                    int pos = mvm.AutoComplete.IndexOf(selectedToken.Text);

                    string temp = string.Empty;

                    if (pos >= 0)  // found
                    {
                        if (pos == 0)
                        {
                            temp = mvm.AutoComplete.Remove(0, selectedToken.Text.Length + 1);
                        }
                        else temp = mvm.AutoComplete.Remove(pos - 1, selectedToken.Text.Length + 1);

                        List<string> tempList = temp.Split(',').ToList();

                        tempList.Sort();

                        mvm.AutoComplete = string.Join(',', tempList);

                        mvm.AutoCompleteTokens = tempList;

                        DataGrid? dgAutoComplete = this.dgAutoComplete;

                        if (dgAutoComplete != null) dgAutoComplete.ItemsSource = mvm.AutoCompleteTokens;
                    }
                }
            }
        }
    }

    private void EditStory(object? sender, RoutedEventArgs e)
    {
    }
}