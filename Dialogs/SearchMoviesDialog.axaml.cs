using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace TaymadeEntities;

public partial class SearchMoviesDialog : Window, IDisposable
{
    private bool disposedValue;

    public SearchMoviesDialog()
    {
        InitializeComponent();

        DataContextChanged += this.SearchMoviesDialog_DataContextChanged;

        this.Closed += SearchMoviesDialog_Closed;
    }

    private void SearchMoviesDialog_Closed(object? sender, EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        DataContext = null;
    }

    private void SearchMoviesDialog_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext != null)
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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.DataContext = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SearchMoviesDialog()
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
}