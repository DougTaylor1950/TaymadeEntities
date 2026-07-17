using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace TaymadeEntities.Dialogs;

public partial class MP3U8Selector : WindowBase
{
    public MP3U8Selector()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => { this.Close(true); });
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => { this.Close(false); });
    }
}