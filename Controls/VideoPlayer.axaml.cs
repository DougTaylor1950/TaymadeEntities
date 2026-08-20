using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Controls
{
    public partial class VideoPlayer : UserControl
    {
        public VideoPlayer()
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

        private void NumericUpDown_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {

            if (DataContext is PlayerViewModel vm)
            {
                double volume = (double)e.NewValue.Value;
                vm.SetVolume(volume);
            }
        }

        

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    vm.CurrentBookmark = e.AddedItems[0] as TaymadeEntities.Models.Bookmark;
                    vm.SeekCurrentBookmark();
                }

                vm.AutoCompleteBox = this.AutoComplete;
            }
        }

        private void Used_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    AutoComplete.Text = e.AddedItems[0].ToString();

                }

                vm.AutoCompleteBox = this.AutoComplete;
            }
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is PlayerViewModel vm)
            {
                // vm.Play();
            }
        }

        //private void VideoViewOnPointerEntered(object sender, PointerEventArgs e)
        //{
        //    ControlsPanel.IsVisible = true;
        //    System.Threading.Thread.Sleep(100);
        //}

        //private void VideoViewOnPointerExited(object sender, PointerEventArgs e)
        //{
        //    ControlsPanel.IsVisible = false;
        //}

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}