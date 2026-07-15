using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Controls
{
    public partial class MusicPlayer : UserControl
    {
        public MusicPlayer()
        {
            InitializeComponent();

            //this.BookmarksCombo.SelectionChanged += this.BookmarksCombo_SelectionChanged;

            // SpeedControl.ItemsSource = new List<float>() { 0.1F, 0.2F, 0.5F, 1, 1.5F, 2, 3, 4 };


        }

        private void SpeedControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    float speed = (float)e.AddedItems[0];
                    vm.SpeedChanged(speed);
                }
            }
        }

        private void ScreenControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    int size = (int)e.AddedItems[0];
                    vm.ScreenWidth = size;
                    Window? main = Support.Support.GetWindow();

                    vm.PaneWidth = (int)(1920 - size);

                }
            }
        }


        private void VideoViewOnPointerEntered(object sender, PointerEventArgs e)
        {
            // ControlsPanel.IsVisible = true;
            // System.Threading.Thread.Sleep(100);
        }

        private void VideoViewOnPointerExited(object sender, PointerEventArgs e)
        {
            //ControlsPanel.IsVisible = false;
        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void SpeedPicker_DoubleTapped(object? sender, TappedEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                if (sender is SpeedPicker speedPicker)
                {
                    vm.SetSpeed(speedPicker.SelectedSpeed);
                }
            }
        }

        //private void StackPanel_KeyDown(object? sender, KeyEventArgs e)
        //{
        //    if (DataContext is PlayerViewModel vm)
        //    {
        //        if (e.Key == Key.Space)
        //        {
        //            vm.Pause();
        //        }
        //        else if (e.Key == Key.PageDown)
        //        {
        //            vm.Plus20();
        //        }
        //        else if (e.Key == Key.PageUp)
        //        {
        //            vm.MoveBy(-20);
        //        }
        //        else if (e.Key == Key.Home)
        //        {
        //            vm.MoveToStart();
        //        }
        //    }
        //}
    }
}