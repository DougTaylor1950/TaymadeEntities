//-----------------------------------------------------------------------
// <copyright file="MoviesPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>27/04/2022 17:16:40 27/04/2022 17:16:40 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace AvalonMVVM.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="MovieCollection" />.
    /// </summary>
    public class MovieCollection : List<Movies>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieCollection"/> class.
        /// </summary>
        public MovieCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection<see cref="IEnumerable{Movies}"/>.</param>
        public MovieCollection(IEnumerable<Movies> collection) : base(collection)
        {
        }

        #endregion

        #region Methods

        public static List<Movies> ListsIntersection(List<Movies> list1, List<Movies>? list2)
        {
            if (list2 == null) return list1;

            var ids = list1.Select(x => x.Id).Intersect(list2.Select(x => x.Id));
            List<Movies> result = list1.Where(x => ids.Contains(x.Id)).ToList();
            return result;
        }

        /// <summary>
        /// The GetAndSortObservableCollection.
        /// </summary>
        /// <param name="temp">The temp<see cref="List{Movies}"/>.</param>
        /// <returns>The <see cref="ObservableCollection{Movies}"/>.</returns>
        public static ObservableCollection<Movies>? GetAndSortObservableCollection(List<Movies>? temp, bool doSort = true)
        {
            if (temp != null)
            {
                MovieCollection movies = new MovieCollection(temp);
                if (doSort) movies.SortCollection(DataController.MovieProperties.SortColumnString, DataController.MovieProperties.MovieSortDirection);
                return movies.GetObservableCollection();
            }
            else return null;
        }

        /// <summary>
        /// The GetObservableCollection.
        /// </summary>
        /// <returns>The <see cref="ObservableCollection{Movies}"/>.</returns>
        public ObservableCollection<Movies> GetObservableCollection()
        {
            return new ObservableCollection<Movies>(this);
        }

        /// <summary>
        /// The SortCollection.
        /// </summary>
        /// <param name="field">The field<see cref="int?"/>.</param>
        /// <param name="direction">The direction<see cref="ListSortDirection"/>.</param>
        public void SortCollection(int? field, ListSortDirection direction = ListSortDirection.Ascending)
        {
            if (field != null)
            {
                switch (field)
                {
                    case 0:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.MovieName.CompareTo(y.MovieName));
                        }
                        else
                        {
                            Sort((y, x) => x.MovieName.CompareTo(y.MovieName));
                        }

                        break;
                    case 1:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.MoviePath.CompareTo(y.MoviePath));
                        }
                        else
                        {
                            Sort((y, x) => x.MoviePath.CompareTo(y.MoviePath));
                        }

                        break;
                    case 2:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.DurationSeconds!.Value.CompareTo(y.DurationSeconds!.Value));
                        }
                        else
                        {
                            Sort((y, x) => x.DurationSeconds!.Value.CompareTo(y.DurationSeconds!.Value));
                        }

                        break;
                    case 3:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Added!.Value.CompareTo(y.Added));
                        }
                        else
                        {
                            Sort((y, x) => x.Added!.Value.CompareTo(y.Added));
                        }

                        break;
                    case 4:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.ModifiedOn!.Value.CompareTo(y.ModifiedOn));
                        }
                        else
                        {
                            Sort((y, x) => x.ModifiedOn!.Value.CompareTo(y.ModifiedOn));
                        }

                        break;
                    case 5:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.FilmGroup.CompareTo(y.FilmGroup));
                        }
                        else
                        {
                            Sort((y, x) => x.FilmGroup.CompareTo(y.FilmGroup));
                        }

                        break;

                    case 6:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Id.CompareTo(y.Id));
                        }
                        else
                        {
                            Sort((y, x) => x.Id.CompareTo(y.Id));
                        }

                        break;
                    case 7:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Episode!.Value.CompareTo(y.Episode));
                        }
                        else
                        {
                            Sort((y, x) => x.Episode!.Value.CompareTo(y.Episode));
                        }

                        break;
                    case 8:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Season!.Value.CompareTo(y.Season));
                        }
                        else
                        {
                            Sort((y, x) => x.Season!.Value.CompareTo(y.Season));
                        }

                        break;
                    case 9:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Series!.Value.CompareTo(y.Series));
                        }
                        else
                        {
                            Sort((y, x) => x.Series!.Value.CompareTo(y.Series));
                        }

                        break;
                    case 10:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Year!.Value.CompareTo(y.Year));
                        }
                        else
                        {
                            Sort((y, x) => x.Year!.Value.CompareTo(y.Year));
                        }

                        break;
                    case 11:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.ImagesCount!.Value.CompareTo(y.ImagesCount));
                        }
                        else
                        {
                            Sort((y, x) => x.ImagesCount!.Value.CompareTo(y.ImagesCount));
                        }

                        break;
                    case 12:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.link!.Value.CompareTo(y.link));
                        }
                        else
                        {
                            Sort((y, x) => x.link!.Value.CompareTo(y.link));
                        }

                        break;

                    case 13:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.Info.CompareTo(y.Info));
                        }
                        else
                        {
                            Sort((y, x) => x.Info.CompareTo(y.Info));
                        }

                        break;
                    case 14:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.PercentUnBookmarked.CompareTo(y.PercentUnBookmarked));
                        }
                        else
                        {
                            Sort((y, x) => x.PercentUnBookmarked.CompareTo(y.PercentUnBookmarked));
                        }

                        break;
                    default:
                        if (direction == ListSortDirection.Ascending)
                        {
                            Sort((x, y) => x.MovieName.CompareTo(y.MovieName));
                        }
                        else
                        {
                            Sort((y, x) => x.MovieName.CompareTo(y.MovieName));
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// The SortCollection.
        /// </summary>
        /// <param name="field">The field<see cref="string"/>.</param>
        /// <param name="direction">The direction<see cref="ListSortDirection"/>.</param>
        public void SortCollection(string? field, ListSortDirection direction)
        {
            if (!string.IsNullOrEmpty(field))
            {
                int Index = 0;
                switch (field)
                {
                    case "Name":
                        Index = 0;
                        break;

                    case "Information":
                        Index = 13;
                        break;
                    case "Duration":
                        Index = 2;
                        break;
                    case "#Bookmarks":
                        Index = 11;
                        break;
                    case "Added":
                        Index = 3;
                        break;
                    case "PercentUnBookmarked":
                        Index = 14;
                        break;
                    case "Modified":
                        Index = 4;
                        break;
                    default:
                        Index = 0;
                        break;
                }

                SortCollection(Index, direction);
            }
        }

        #endregion
    }
}
