using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TaymadeEntities.Models;

using System;
using TaymadeControls.Builders;
using TaymadeControls.Buttons;

namespace MovieDBViewer.Dialogs
{
    public partial class ActorEditDialog : Window
    {
        public ActorEditDialog()
        {
            InitializeComponent();
            SetButtons();
        }

        private void SetButtons()
        {
           
            //if (OkButtonPanelActorDialog != null)
            //{

            //    OkButtonPanelActorDialog.OkButton.Command = ((ActorViewModel)this.DataContext).AddOKCommand();
            //    OkButtonPanelActorDialog.CancelButton.Command = ((ActorViewModel)this.DataContext).AddCancelCommand();

            //    ImagedButton searchButton = new ImagedButton()
            //    {
            //        LabelText = "Search TMDB",
            //        ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://TaymadeControls/Assets/search_icon.png")))

            //    };
            //        searchButton.Command = ((ActorViewModel)this.DataContext).AddSearchCommand();
            //    OkButtonPanelActorDialog.Children.Add(searchButton);


            //}
        }

        //public ActorEditDialog(ActorViewModel actorView)
        //{
        //    InitializeComponent();


        //    this.DataContext = actorView;

        //    ComboBox genderlist = this.Genders;

        //    if (genderlist != null)
        //    {
        //        genderlist.ItemsSource = Models.DataController.GenderList;
        //        genderlist.SelectionChanged += GenderChanged;
        //    }
        //    Opened += ActorEditDialog_Opened;
        //    SetButtons();
        //}

        private void ActorEditDialog_Opened(object? sender, System.EventArgs e)
        {
            //if (Screens.ScreenCount > 1)
            //{
            //    int screenWidth = (int)this.Width;
            //    this.Position = new PixelPoint(-screenWidth, 50);
            //}
           

        }

        private void GenderChanged(object? sender, SelectionChangedEventArgs e)
        {
            //ComboBox? genderList = sender as ComboBox;

            //ActorViewModel? castViewModel = this.DataContext as MovieDBViewer.Vi

            //if (genderList != null && castViewModel != null && castViewModel.CurrentActor != null)
            //{
            //    PhraseEntry? gender = genderList.SelectedItem as PhraseEntry;
            //    if (gender != null)
            //    {
            //        castViewModel.CurrentActor.Gender = gender.Order;
            //        castViewModel.CurrentActor.GenderValue = gender;
            //        string t = castViewModel.CurrentActor.GenderDisplay;
            //    }
            //}
        }


        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
    }
}
