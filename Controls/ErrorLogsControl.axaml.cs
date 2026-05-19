using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Controls;

public partial class ErrorLogsControl : UserControl
{
    public ErrorLogsControl()
    {
        InitializeComponent();

        this.DataContext = new ErrorViewModel();
    }
}