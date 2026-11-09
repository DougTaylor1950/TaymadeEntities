//-----------------------------------------------------------------------
// <copyright file="MovieDetailControl.axaml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>25/04/2022 11:56:16 25/04/2022 11:56:16 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Controls
{
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Avalonia.VisualTree;
    using TaymadeEntities.ViewModels;
    using System.Threading.Tasks;
    using TaymadeControls.Buttons;
    using TaymadeEntities.Models;
    using TaymadeEntities.Support;

    public partial class MovieDetailControl : UserControl
    {
        #region Constructors

        public MovieDetailControl()
        {
            InitializeComponent();
            DetachedFromVisualTree += OnDetachedFromVisualTree;
        }

        #endregion

        #region Methods 

        private void OnDetachedFromVisualTree(object? sender,
        Avalonia.VisualTreeAttachmentEventArgs e)
        {
            //DisposeButtonImages(this);
        }
        private static async Task EditMovie(Window main, Movies movie)
        {
            ViewModels.MovieViewModelBase? mvm = new MovieViewModelBase();

        }

        private void AddPhrase(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Window? main = Support.GetMainWindow() as Window;

            if (button != null && main != null)
            {
                MovieListViewModel? model = button.DataContext as MovieListViewModel;

                if (model != null && model.CurrentMovie != null)
                {
                    ComboBox cbPhrase = this.FindControl<ComboBox>("cbGroup");

                    if (cbPhrase != null)
                    {
                        string group = model.CurrentMovie.FilmGroup;
                        PhraseEntry? id = cbPhrase.SelectedItem as PhraseEntry;

                        if (id != null)
                        {
                            if (string.IsNullOrEmpty(group) && !group.Contains(id.Id))
                                group += id.Id;
                            else if (!group.Contains(id.Id))
                                group += "," + id.Id;
                            model.CurrentMovie.FilmGroup = group;
                        }
                    }
                }
            }
        }

        private async void Edit(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Window? main = Support.GetWindow() as Window;

            if (button != null && main != null)
            {
                MovieListViewModel? model = button.DataContext as MovieListViewModel;

                MovieListViewModel? viewModel = main.DataContext as MovieListViewModel;

                if (model != null)
                {
                    await EditMovie(main, model.CurrentMovie);

                    if (viewModel != null)
                    {
                        viewModel.CurrentMovie = model.CurrentMovie;

                        if (DataContext is MovieListViewModel)
                        {
                            DataContext = viewModel;

                            Refresh(sender, e);
                        }
                    }
                }
            }
        }

        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Window? main = Support.GetMainWindow() as Window;

            if (button != null && main != null)
            {
                MovieListViewModel? model = button.DataContext as MovieListViewModel;

                if (model != null && model.CurrentMovie != null)
                {
                    model.CurrentMovie.SetPercentUnmarked();
                }
            }
        }

        private static void DisposeButtonImages(Control root)
        {
            foreach (var button in root.GetVisualDescendants())
                if (button is ImagedButton)
                {
                    ImagedButton temp = button as ImagedButton;
                    temp.Dispose();
                }
                else if (button is Image image &&
                    image.Source is Bitmap bitmap)
                {
                    bitmap.Dispose();

                }
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is MovieViewModelBase viewModel)
            {
               await viewModel.CurrentMovie.SaveAsync();
            }
        }

        private void RefreshMovie_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is MovieViewModelBase viewModel) 
            {
                viewModel.CurrentMovie.Refresh();
            }
        }

        #endregion
    }
}
