using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AvalonMVVM.Models
{
    public class StoryCollection : List<Story>
    {
        public StoryCollection(IEnumerable<Story> collection) : base(collection)
        {
        }

        public static ObservableCollection<Story> GetAndSortObservableCollection(List<Story> temp)
        {
            StoryCollection movies = new StoryCollection(temp);
            movies.SortCollection(DataController.StoryProperties.StorySortColumn, DataController.StoryProperties.StorySortDirection);
            return movies.GetObservableCollection();
        }

        public static ObservableCollection<Story> GetAndSortStoriesById(List<Story> temp)
        {
            StoryCollection movies = new StoryCollection(temp);
            movies.SortCollection("Id",ListSortDirection.Descending);
            return movies.GetObservableCollection();
        }

        public ObservableCollection<Story> GetObservableCollection()
        {
            return new ObservableCollection<Story>(this);
        }

        public void SortCollection(string? field, ListSortDirection direction)
        {
            if (!string.IsNullOrEmpty(field))
            {
                try
                {


                    switch (field)
                    {
                        case "Title":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Title!.CompareTo(y.Title));
                            }
                            else
                            {
                                Sort((y, x) => x.Title!.CompareTo(y.Title));
                            }
                            break;

                        case "Codes":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Codes!.CompareTo(y.Codes));
                            }
                            else
                            {
                                Sort((y, x) => x.Codes!.CompareTo(y.Codes));
                            }
                            break;
                        case "Author":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Author!.CompareTo(y.Author));
                            }
                            else
                            {
                                Sort((y, x) => x.Author!.CompareTo(y.Author));
                            }
                            break;
                        case "Age":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Age!.CompareTo(y.Age));
                            }
                            else
                            {
                                Sort((y, x) => x.Age!.CompareTo(y.Age));
                            }
                            break;
                        case "LowestAgeInt":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.LowestAgeInt.CompareTo(y.LowestAgeInt));
                            }
                            else
                            {
                                Sort((y, x) => x.LowestAgeInt.CompareTo(y.LowestAgeInt));
                            }
                            break;
                        case "Created":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Creation!.Value.CompareTo(y.Creation));
                            }
                            else
                            {
                                Sort((y, x) => x.Creation!.Value.CompareTo(y.Creation));
                            }
                            break;
                        case "Added":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Added!.Value.CompareTo(y.Added));
                            }
                            else
                            {
                                Sort((y, x) => x.Added!.Value.CompareTo(y.Added));
                            }
                            break;

                        case "Modified":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.LastModified!.Value.CompareTo(y.LastModified));
                            }
                            else
                            {
                                Sort((y, x) => x.LastModified!.Value.CompareTo(y.LastModified));
                            }
                            break;
                        default:
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Id.CompareTo(y.Id));
                            }
                            else
                            {
                                Sort((y, x) => x.Id!.CompareTo(y.Id));
                            }
                            break;
                        case "Id":
                            if (direction == ListSortDirection.Ascending)
                            {
                                Sort((x, y) => x.Id.CompareTo(y.Id));
                            }
                            else
                            {
                                Sort((y, x) => x.Id!.CompareTo(y.Id));
                            }
                            break;
                    }
                }
                catch (System.Exception)
                {

                    //throw;
                }
            }
            //SortCollection(Index, direction);
        }


    }

}

