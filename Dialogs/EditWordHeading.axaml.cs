using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Dialogs;

public partial class EditWordHeading : Window
{
    public EditWordHeading()
    {
        InitializeComponent();
    }

    public EditWordHeading(StoryViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();

        if (vm != null)
        {

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