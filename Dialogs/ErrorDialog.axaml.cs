using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using System;
using System.Linq;
using TaymadeControls.Buttons;

namespace TaymadeEntities.Dialogs;

public partial class ErrorDialog : Window
{
    public ErrorDialog()
    {
        InitializeComponent();
        AddButtons();

        this.Initialized += this.ErrorDialog_Initialized;
        this.DataContextChanged += ErrorDialog_DataContextChanged;
    }

    private void ErrorDialog_DataContextChanged(object? sender, EventArgs e)
    {
        
    }

    private void ErrorDialog_Initialized(object? sender, System.EventArgs e)
    {
        if (this.ErrorViewModel != null)
        {
            this.ErrorLogsControl.DataContext = ErrorViewModel;
            OkCancel.OkButton.Click += CloseThis;
            OkCancel.CancelButton.Click += CloseThis;

        }

    }

    private void AddButtons()
    {
        ImagedButton clearAll = new ImagedButton()
        {
            LabelText = "Clear All",
            ImageSource= TaymadeControls.ImageHelper.LoadFromResource(new Uri("avares://TaymadeControls/Assets/cleaning.png"))
        };
        clearAll.Click += this.ClearAll_Click;
        OkCancel.Children.Add(clearAll);
        OkCancel.OkButton.Click += CloseThis;
        OkCancel.CancelButton.Click += CloseThis;

        //if (this.ErrorViewModel != null)
        //{
        //    this.ErrorLogsControl.DataContext = ErrorViewModel;
        //    this.ErrorViewModel.SetOkCancelPanelCommands(OkCancel);

        //}
        this.WindowState = WindowState.Maximized;
    }

    public ErrorDialog(ErrorViewModel errorViewModel)
    {
        InitializeComponent();
        this.ErrorViewModel = errorViewModel;
        AddButtons();

        this.Initialized += this.ErrorDialog_Initialized;
    }

    public ErrorViewModel? ErrorViewModel { get; }

    private void CloseThis(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close(true);
    }

    private void ClearAll_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // get Viewmodel check is not null
        if (this.ErrorViewModel != null)
        {
            for (int i = 0; i < ErrorViewModel.ErrorLogs.Count; i++)
            {
                MVMLogs mVMLogs = ErrorViewModel.ErrorLogs[i];
                mVMLogs.Delete();
            }
            ErrorViewModel.ErrorLogs.Clear();
            ErrorViewModel.ErrorLogs = new System.Collections.ObjectModel.ObservableCollection<MVMLogs>(
                DataController.MaintenaceController.GetLogs().ToList());
        }
    }
}