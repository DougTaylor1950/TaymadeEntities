using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities;

/// <summary>
/// Used to show movies in an Avalonia environment, 
/// Calling application must have LibVLCSharp, LibVLCSharp.Avalonia and VideoLan.Windows installed.
/// There are two basic version Full Screen Player or a full featured control window, that allows stepping and bookmark grapping
/// PlayerViewModel is the viewModel used with the dialog - It has an autoplay property, a PlayFromBookmark property 
/// that will attempt to start from the current set bookmark - the FullScreen property switches between the two dialog modes.
/// </summary>
/// <seealso cref="Avalonia.Controls.Window" />
/// <seealso cref="System.IDisposable" />
/// <author>
/// Doug Taylor - Taymade Software Services
/// </author>
/// <remarks>
///   <created> 10/07/2026 23:20 </created>
/// </remarks>
public partial class PlayerDialog : Window, IDisposable
{
    

    public PlayerDialog()
    {
        InitializeComponent();
        Opened += PlayerDialog_Opened;
        SizeChanged += PlayerDialog_SizeChanged;
        KeyDown += PlayerDialog_KeyDown;
    }

    

    public PlayerDialog(PlayerViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
        Opened += PlayerDialog_Opened;
        SizeChanged += PlayerDialog_SizeChanged;
        KeyDown += PlayerDialog_KeyDown;
    }

    private void PlayerDialog_KeyDown(object? sender, KeyEventArgs e)
    {
        if (this.DataContext is PlayerViewModel vm)
        {
            if (e.Key == Key.Space)
            {
                vm.Pause();
                e.Handled = true;

            }
            else if (e.Key == Key.PageDown)
            {
                vm.Plus20();
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                vm.MoveBy(-20);
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                vm.MoveToStart();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                vm.Stop();
                Avalonia.Threading.Dispatcher.UIThread.Post(() => Close());
                e.Handled = true;
            }
            else if (e.Key == Key.NumPad6 || e.Key == Key.Right)
            {
                vm.MoveBy(5);
                e.Handled = true;

            }
            else if (e.Key == Key.NumPad4 || e.Key == Key.Left)
            {
                vm.MoveBy(-5);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                vm.VolumeUp();
                e.Handled = true;   
            }
            else if (e.Key == Key.Down)
            {
                vm.VolumeDown();
                e.Handled = true;
            }
        }
    }

    private void CloseDialog()
    {
        if (DataContext is PlayerViewModel vm)
        {
            if (vm.MediaPlayer != null)
            {
                vm.MediaPlayer.Stop();
                vm.MediaPlayer?.Media?.Dispose();
                // detach the native window handle
                vm.MediaPlayer.Hwnd = IntPtr.Zero;
                vm.MediaPlayer.Dispose();
            }

            
            Dispatcher.UIThread.Post(()=> { this.Close(); });
        }
    }

    private void PlayerDialog_Opened(object? sender, EventArgs e)
    {
        if (this.DataContext != null && this.DataContext is PlayerViewModel vm)
        {
            vm.OnDialogOpened();

            if (this.MusicPlayer != null)
            {
                vm.TrackGrid = this.MusicPlayer.TrackGrid;
            }
        }
    }

    private void PlayerDialog_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (ControlledPlayer != null && ControlledPlayer.DataContext is PlayerViewModel vm && this.Width > 0)
        {
            ControlledPlayer.Width = this.Width - 20; // Adjust for padding
            ControlledPlayer.Height = this.Height - 80;
            // Adjust for padding and title bar
            vm.ScreenWidth = 1200;
            vm.ScreenHeight = 780;
        }

        if (FullScreenPlayer != null && 
            FullScreenPlayer.DataContext is PlayerViewModel vmFull 
            && this.IsInitialized && this.Width > 0)
        {
            if (vmFull.FullScreen)
            {
                vmFull.ScreenWidth = (int)this.Width - 20; // Adjust for padding

                vmFull.ScreenHeight = (int)this.Height - 120; // Adjust for padding and title bar
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources
        }
    }
}