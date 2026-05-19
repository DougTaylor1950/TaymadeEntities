using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Dialogs;

public partial class MovieListDialog : Window
{
    public MovieListDialog()
    {
        InitializeComponent();
    }

    public MovieListDialog(MovieViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        //this.MovieDetails.DataContext = viewModel;
        //this.MovieList.DataContext = viewModel;

        //this.MovieList.dgMovies.SelectionChanged += this.DgMovies_SelectionChanged;
    }

    private void DgMovies_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
    }
}