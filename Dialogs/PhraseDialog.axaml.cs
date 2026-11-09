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
        //this.OKCancelPanel?.OkButton?.Click += OkButton_Click;
        //this.OKCancelPanel?.CancelButton?.Click += CancelButton_Click;
            
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            this.Close(false);
        });
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            this.Close(true);
        });
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