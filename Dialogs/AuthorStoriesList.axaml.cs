using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;
using TaymadeEntities.Models;
using System;
//using System.Windows.Forms;
using TaymadeControls.Buttons;

namespace TaymadeEntities.Dialogs;

public partial class AuthorStoriesList : Window
{
    public AuthorStoriesList()
    {
        InitializeComponent();

        DataContextChanged += AuthorStoriesList_DataContextChanged;





        dgAuthorStories.SelectionChanged += (sender, e) =>
            {
                if (dgAuthorStories.SelectedItem is Story)
                {
                    StoryViewModel? vm = DataContext as StoryViewModel;
                    if (vm != null) vm.CurrentStory = dgAuthorStories.SelectedItem as Story;
                }
            };

        Closed += AuthorStoriesList_Closed;
    }

    private void AuthorStoriesList_Closed(object? sender, EventArgs e)
    {
        
    }

    private void AuthorStoriesList_DataContextChanged(object? sender, EventArgs e)
    {
      
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StoryViewModel? vm = DataContext as StoryViewModel;
        if (vm != null)
        {
            vm.EditStory();
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