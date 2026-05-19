using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Dialogs;

public partial class PhraseDialog : Window
{
    public PhraseDialog()
    {
        InitializeComponent();

        this.DataContext = new PhraseViewModel();
        ViewModel = this.DataContext as PhraseViewModel;

        this.DataContextChanged += PhraseDialog_DataContextChanged;
    }

    public PhraseViewModel? ViewModel { get; internal set; }

    private void PhraseDialog_DataContextChanged(object? sender, System.EventArgs e)
    {
        ViewModel = this.DataContext as PhraseViewModel;
    }

    

    private void PhraseHeader_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ComboPhraseHeader != null && ComboPhraseHeader.IsInitialized)
        {
            PhraseHeader? selectedHeader = ComboPhraseHeader.SelectedItem as PhraseHeader;
            ViewModel?.LoadPhraseEntriesForHeader();
        }
    }
}