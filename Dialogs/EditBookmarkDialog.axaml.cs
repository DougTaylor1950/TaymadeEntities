using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using System;

namespace TaymadeEntities.Dialogs;

public partial class EditBookmarkDialog : Window
{
    public EditBookmarkDialog()
    {
        InitializeComponent();
    }


    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        try
        {
            this.DataContext = null;
            this.Close(true);
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur during the close operation
            Console.WriteLine($"An error occurred while closing the dialog: {ex.Message}");
        }
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(false);
    }
}