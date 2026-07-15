using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Layout;
using System.ComponentModel;
using ReactiveUI;
using Avalonia.Data;
using DynamicData.Tests;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Azure;

namespace TaymadeEntities.Controls
{ 
    public partial class SpeedPicker : UserControl
    {
        private float selectedItem = 1;
        private int buttonWidth = 30;

        public SpeedPicker()
        {
            InitializeComponent();
            buttonWidth = 30;
            ClearBackgrounds();

        }

        public static readonly StyledProperty<IBrush> BackgroundProperty =
           Panel.BackgroundProperty.AddOwner<SpeedPicker>();

        public new IBrush Background
        {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public sealed override void Render(DrawingContext context)
        {
            if (Background != null)
            {
                var renderSize = Bounds.Size;
                context.FillRectangle(Background, new Rect(renderSize));
            }
            base.Render(context);
        }

        public int ButtonWidth {
            get => buttonWidth;
            set
            { 
                buttonWidth = value;
                ClearBackgrounds();
            }
        }
        public static readonly DirectProperty<SpeedPicker, float> SelectedSpeedProperty =
            AvaloniaProperty.RegisterDirect<SpeedPicker, float>(
            nameof(SelectedSpeed),
            o => o.SelectedSpeed,
            (o, v) => o.SelectedSpeed = v,
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly DirectProperty<SpeedPicker, int> ButtonWidthProperty =
            AvaloniaProperty.RegisterDirect<SpeedPicker, int>(
            nameof(ButtonWidth),
            o => o.ButtonWidth,
            (o, v) => o.buttonWidth = v,
            defaultBindingMode: BindingMode.TwoWay);

        public float SelectedSpeed
        {
            get => selectedItem; 
            set
            {
                //float oldValue = selectedItem;
                this.SetAndRaise(SelectedSpeedProperty, ref selectedItem, value);
                //OnPropertyChanged(nameof(SelectedSpeed));

            }
        }
        private void B01(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 0.1F;
            SetPicked(sender);
        }

        private void B02(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 0.2F;
            SetPicked(sender);
        }

        private void ClearBackgrounds()
        {
            foreach (var child in this.GetVisualDescendants())
            {
                if (child is Button )
                {
                    
                    Button button = (Button) child;
                    button.Background = Brushes.LightGray;
                    button.Foreground = Brushes.Black;
                    if (button != B1p5) button.Width = ButtonWidth; else button.Width = 35;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }

            //this.B0p1.Background = Brushes.LightGray;
            //this.B0p2.Background = Brushes.LightGray;
            //this.B0p5.Background = Brushes.LightGray;
            //this.B1p0.Background = Brushes.LightGray;
            //this.B1p5.Background = Brushes.LightGray;
            //this.B2p0.Background = Brushes.LightGray;
            //this.B3p0.Background = Brushes.LightGray;
            //this.B4p0.Background = Brushes.LightGray;

            //this.B0p1.Foreground = Brushes.Black;
            //this.B0p2.Foreground = Brushes.Black;
            //this.B0p5.Foreground = Brushes.Black;
            //this.B1p0.Foreground = Brushes.Black;
            //this.B1p5.Foreground = Brushes.Black;
            //this.B2p0.Foreground = Brushes.Black;
            //this.B3p0.Foreground = Brushes.Black;
            //this.B4p0.Foreground = Brushes.Black;


        }

        private void B05(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 0.5F;
            SetPicked(sender);
        }

        private void SetPicked(object sender)
        {
            ClearBackgrounds();
            Button button = sender as Button;
            button.Background = Brushes.Blue;
            button.Foreground = Brushes.White;
        }

        private void B1(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 1F;
            SetPicked(sender);
        }

        private void B15(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 1.5F;
            SetPicked(sender);
        }

        private void B2(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 2F;
            SetPicked(sender);
        }

        private void B3(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 3F;
            SetPicked(sender);
        }

        private void B4(object? sender, RoutedEventArgs e)
        {
            SelectedSpeed = 4F;
            SetPicked(sender);
        }
    }
}