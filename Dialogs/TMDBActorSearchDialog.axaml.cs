using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using TaymadeEntities.Support;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using TaymadeEntities.ViewModels;
using System;

namespace TaymadeEntities.Dialogs
{
    public partial class TMDBActorSearchDialog : Window,IDisposable
    {


        private List<Actor> actorList;

        private ActorSearchModel viewModel;
        private bool disposedValue;

        public TMDBActorSearchDialog()
        {
            InitializeComponent();

        }


        public TMDBActorSearchDialog(ViewModels.ActorSearchModel amodel)
        {
            InitializeComponent();


            this.Closed += TMDBActorSearchDialog_Closed;

            DataContext = amodel;
            ViewModel = amodel;

            TextBox searchFor = this.Find<TextBox>("SearchFor");

            Button searchTMDP = this.Find<Button>("TMDBSearch");

            if (searchFor != null && amodel.CurrentActor != null)
            {
                searchFor.Text = amodel.CurrentActor.Name;
            }


            SearchTMDP = ReactiveCommand.Create(SearchDatabase);

            if (searchTMDP != null)
            {
                searchTMDP.Command = SearchTMDP;
            }

            Opened += TMDBActorSearchDialog_Opened;
        }

        private void TMDBActorSearchDialog_Closed(object? sender, EventArgs e)
        {
          //  this.Dispose();
        }

        private void TMDBActorSearchDialog_Opened(object? sender, System.EventArgs e)
        {
            //if (Screens.ScreenCount > 1 && DataController.ShowOnAlternateScreen())
            //{
            //    int screenWidth = (int)this.Width;
            //    this.Position = new PixelPoint(-screenWidth, 50);
            //}
            //this.WindowState = WindowState.Maximized;
            // Support.Support.SetScreen(this);
        }

        public ActorSearchModel ViewModel { get => viewModel; set => viewModel = value; }


        private async void SearchDatabase()
        {
            TextBox searchFor = this.Find<TextBox>("SearchFor");
            if (searchFor != null)
            {
                DataGrid? dataGridA;

                string searchText = searchFor.Text;

                if (!string.IsNullOrEmpty(searchText))
                {
                    ViewModel.ActorList = new System.Collections.ObjectModel.ObservableCollection<Actor>(
                        DataController.ActorController.GetActorsByName(searchText.ToLower())
                        //DataController.SandboxEntities.Actors.AsNoTracking().Where(a => a.Name.ToLower().Contains(searchText.ToLower())).ToList()
                        );

                    List<Person>? peopleList = await TmdbSupport.GetPeopleListAsync(searchText);

                    ViewModel.FoundPeople = new System.Collections.ObjectModel.ObservableCollection<Person>(
                        peopleList
                        );


                    peopleList.Clear();

                    DataGrid dataGrid = this.Find<DataGrid>("dgFoundMovies");
                    dataGrid.SelectionChanged += MovieSelected;
                    dataGrid.ItemsSource = ViewModel.FoundPeople;

                    dataGridA = this.Find<DataGrid>("dgFoundActors");

                    if (dataGridA != null)
                    {
                        dataGridA.SelectionChanged += ActorSelected;
                        dataGridA.ItemsSource = ViewModel.ActorList;
                    }



                    else
                    {
                        dataGridA = this.Find<DataGrid>("dgFoundActors");


                        if (dataGridA != null)
                        {
                            dataGridA.SelectionChanged += ActorSelected;
                            dataGridA.ItemsSource = ViewModel.ActorList;
                        }


                    }
                }


            }
        }

        private void ActorSelected(object? sender, SelectionChangedEventArgs e)
        {
            DataGrid? actor = sender as DataGrid;

            if (actor != null)
            {

                Actor? selected = actor.SelectedItem as Actor;

                ViewModels.ActorSearchModel? viewModel = DataContext as ViewModels.ActorSearchModel;

                if (viewModel != null)
                {
                    viewModel.SelectedActor = selected;
                    viewModel.CurrentActor = selected;
                }
            }
        }

        private void MovieSelected(object? sender, SelectionChangedEventArgs e)
        {
            DataGrid? movies = sender as DataGrid;

            if (movies != null)
            {

                Person? selected = movies.SelectedItem as Person;

                ViewModels.ActorSearchModel? viewModel = DataContext as ViewModels.ActorSearchModel;

                if (viewModel != null) viewModel.FoundPerson = selected;
            }
        }

        

        public ReactiveCommand<Unit, Unit>? SearchTMDP { get; set; }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close(true);
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in actorList)
                    {
                        item.Dispose();
                    }
                    this.actorList?.Clear();
                    this.actorList = null;
                    this.DataContext = null;
                    this.viewModel.Dispose();
                    this.viewModel = null;

                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TMDBActorSearchDialog()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
