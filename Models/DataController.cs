//-----------------------------------------------------------------------
// <copyright file="DataController.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>27/04/2022 17:37:51 27/04/2022 17:37:51 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using TaymadeEntities.Controllers;
    using TaymadeEntities.DAL;
    using TaymadeEntities.DAL.Classes;
    using TaymadeEntities.DAL.Interfaces;
    using TaymadeEntities.DBContext;

    /// <summary>
    /// Defines the <see cref="DataController" />.
    /// </summary>
    public class DataController
    {

        #region Public Fields

        public static ActorController? actorController = null;

        public static BookmarkController? bookmarkController = null;

        public static CastController? castController = null;

        public static DirectorController directorController = null;

        public static MovieController? movieController = null;
        public static MovieGenreController? movieGenreController = null;

        public static MoviePropertiesController? moviePropertiesController;

        public static PhrasesController? phrasesController = null;
        public static UnboundController? unboundController = null;

        /// <summary>
        /// Defines the MusicEntitiesContext.
        /// </summary>
        public static MusicEntitiesContext MusicEntitiesContext = new MusicEntitiesContext();

        #endregion Public Fields

        #region Private Fields

        private static List<Actor> actorList = new List<Actor>();
        /// <summary>
        /// Defines the autoCompleteList.
        /// </summary>
        private static List<string>? autoCompleteList;

        /// <summary>
        /// Defines the directorList.
        /// </summary>
        private static List<Models.Director> directorList = new List<Director>();

        /// <summary>
        /// Defines the genders.
        /// </summary>
        private static List<Models.PhraseEntry> genders = new List<PhraseEntry>();

        /// <summary>
        /// Defines the languages.
        /// </summary>
        private static List<Models.PhraseEntry> languages = new List<PhraseEntry>();

        private static MovieImageEntity movieImageEntity = null;

        /// <summary>
        /// Defines the movieList.
        /// </summary>
        private static IEnumerable<Models.Movies>? movieList;

        /// <summary>
        /// Defines the movieProperties.
        /// </summary>
        private static MovieProperties? movieProperties;

        /// <summary>
        /// Defines the phraseEntries.
        /// </summary>
        private static List<Models.PhraseEntry>? phraseEntries = new List<PhraseEntry>();

        private static List<ProductionCompany> productionCompanies;

        /// <summary>
        /// Defines the SandboxEntities.
        /// </summary>
        // keep a field for backwards compatibility with the setter, but do not use
        private static DBContext.SandboxEntities sandboxEntities;

        /// <summary>
        /// Defines the seriesEntries.
        /// </summary>
        private static List<Models.Series> seriesEntries = new List<Series>();

        /// <summary>
        /// Defines the storyFields.
        /// </summary>
        private static List<Models.PhraseEntry> storyFields = new List<PhraseEntry>();

        /// <summary>
        /// Defines the storyProperties.
        /// </summary>
        private static StoryProperties? storyProperties = null;

        private static List<StorySeries> storySeriesList = new List<StorySeries>();

        /// <summary>
        /// Defines the subPhraseEntries.
        /// </summary>
        private static List<Models.PhraseEntry> subPhraseEntries = new List<PhraseEntry>();
        private static StoryController? storyController;

        #endregion Private Fields

        #region Public Properties

        public static ActorController ActorController
        {
            get
            {
                if (actorController == null)
                {
                    actorController = new ActorController(new ActorRepository(SandboxEntities));
                }
                return actorController;
            }
        }

        public static List<Actor> ActorList
        {
            get
            {
                if (actorList.Count == 0)
                {
                    using var ctx = SandboxEntities;
                    actorList = ctx.Actors.AsNoTracking().OrderBy(d => d.Name).ToList();
                }
                return actorList;
            }
        }

        /// <summary>
        /// Gets the AlbumSelectionList.
        /// </summary>
        public static List<SelectEntry> AlbumSelectionList
        {
            get
            {
                var temp = MusicEntitiesContext.Albums.Select(a => new SelectEntry { Description = a.AlbumName, Id = a.Id.ToString() }).Distinct().ToList();

                return new List<SelectEntry>((IEnumerable<SelectEntry>)temp);
            }
        }

        /// <summary>
        /// Gets the ArtistList.
        /// </summary>
        public static List<Artist> ArtistList => MusicEntitiesContext.Artists.ToList();

        /// <summary>
        /// Gets the ArtistSelectionList.
        /// </summary>
        public static List<SelectEntry> ArtistSelectionList
        {
            get
            {
                var temp = MusicEntitiesContext.Artists.Where(a => a.ArtistType == "Person").Select(a => new SelectEntry { Description = a.Name, Id = a.Id.ToString() }).Distinct().ToList();

                return new List<SelectEntry>((IEnumerable<SelectEntry>)temp);
            }
        }

        /// <summary>
        /// Gets or sets the author list.
        /// </summary>
        /// <value>
        /// The author list.
        /// </value>
        /// <autogeneratedoc />
        public static ObservableCollection<Author> AuthorList
        {
            get
            {
                using var ctx = SandboxEntities;
                var list = ctx.Author.AsNoTracking().OrderBy(d => d.Name).ToList();
                return new ObservableCollection<Author>(list);
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the AutoCompleteList.
        /// </summary>
        public static List<string> AutoCompleteList
        {
            get
            {
                if (autoCompleteList == null)
                {
                    string? aComplete = DataController.SandboxEntities.MovieProperties.FirstOrDefault()?.AutoComplete;

                    if (!string.IsNullOrEmpty(aComplete))
                        autoCompleteList = aComplete.Split(',').ToList();
                }

                return autoCompleteList;
            }
            set => autoCompleteList = value;
        }

        public static BookmarkController BookmarkController
        {
            get
            {
                if (bookmarkController == null)
                {
                    bookmarkController = new BookmarkController(new BookmarkRepository(SandboxEntities));
                }
                return bookmarkController;
            }
        }

        public static CastController CastController
        {
            get
            {
                if (castController == null)
                {
                    castController = new CastController(new CastRepository(SandboxEntities));
                }
                return castController;
            }
        }

        public static DirectorController DirectorController
        {
            get
            {
                if (directorController == null)
                {
                    directorController = new DirectorController(new DirectorRepository(SandboxEntities));
                }
                return directorController;
            }
        }

        /// <summary>
        /// Gets the DirectorList.
        /// </summary>
        public static List<Director> DirectorList
        {
            get
            {
                if (directorList.Count == 0)
                {
                    //  using var ctx = SandboxEntities;
                    List<Director>? tempList = DirectorController.GetDirectorList();
                    if (tempList != null) directorList = tempList.OrderBy(d => d.Name).ToList();
                }
                return directorList;
            }
        }

        /// <summary>
        /// Gets the GenderList.
        /// </summary>
        public static List<Models.PhraseEntry> GenderList
        {
            get
            {
                if (genders.Count == 0)
                {
                    using var ctx = SandboxEntities;
                    genders = ctx.PhraseEntry.AsNoTracking().Where(x => x.PhraseID == 2).OrderBy(x => x.Description).ToList();
                }

                return genders;
            }
        }

        /// <summary>
        /// Gets the GroupSelectionList.
        /// </summary>
        public static List<SelectEntry> GroupSelectionList
        {
            get
            {
                var temp = MusicEntitiesContext.Artists.Where(a => a.ArtistType == "Group").Select(a => new SelectEntry { Description = a.Name, Id = a.Id.ToString() }).Distinct().ToList();

                return new List<SelectEntry>((IEnumerable<SelectEntry>)temp);
            }
        }

        /// <summary>
        /// Gets the LanguageList.
        /// </summary>
        public static List<Models.PhraseEntry> LanguageList
        {
            get
            {
                if (languages.Count == 0)
                {
                    using var ctx = SandboxEntities;
                    languages = ctx.PhraseEntry.AsNoTracking().Where(x => x.PhraseID == 10).OrderBy(x => x.Description).ToList();
                }

                return languages;
            }
        }

        public static List<MovieAppProperties> MovieAppPropertiesList
        {
            get
            {
                string computerName = Support.Support.GetComputerName();
                using var ctx = SandboxEntities;
                var temp = ctx.MovieAppProperties.Where(p => p.Computer == computerName).ToList();
                return temp;
            }
        }

        public static MovieGenreController MovieGenreController
        {
            get
            {
                if (movieGenreController == null)
                {
                    movieGenreController = new MovieGenreController(new MovieGenreRepository(SandboxEntities));
                }
                return movieGenreController;
            }
        }

        public static MovieController MovieController
        {
            get
            {
                if (movieController == null)
                {
                    movieController = new MovieController(new MovieRepository(SandboxEntities));
                }
                return movieController;
            }
        }

        public static MoviePropertiesController MoviePropertiesController
        {
            get
            {
                if (moviePropertiesController == null)
                {
                    moviePropertiesController = new MoviePropertiesController(new MoviePropertiesRepository(SandboxEntities));
                }
                return moviePropertiesController;
            }
        }
        public static PhrasesController PhrasesController
        {
            get
            {
                if (phrasesController == null)
                {
                    phrasesController = new PhrasesController(new PhrasesRepository(SandboxEntities));
                }
                return phrasesController;
            }
        }

        public static StoryController StoryController
        {
            get
            {
                if (storyController == null)
                {
                    storyController = new StoryController(new StoryRepository(SandboxEntities));
                }
                return storyController;
            }
        }

        public static UnboundController UnboundController
        {
            get
            {
                if (unboundController == null)
                {
                    unboundController = new UnboundController(new UnboundRepository(SandboxEntities));
                }
                return unboundController;
            }
        }


        public static MovieImageEntity MovieImageEntity
        {
            get
            {
                if (movieImageEntity == null) movieImageEntity = new MovieImageEntity();
                if (movieImageEntity != null)
                {
                    movieImageEntity.ChangeTracker.AutoDetectChangesEnabled = false;
                }
                return movieImageEntity;
            }
            set => movieImageEntity = value;
        }

        /// <summary>
        /// Gets or sets the MovieList.
        /// </summary>
        public static IEnumerable<Movies> MovieList
        {
            get
            {
                //if (movieList == null)
                //{
                //    movieList = SandboxEntities.Movies.AsNoTracking();
                //}

                return movieList;
            }
            set => movieList = value;
        }

        /// <summary>
        /// Gets or sets the MovieProperties.
        /// </summary>
        public static MovieProperties MovieProperties
        {
            get
            {
                if (movieProperties == null)
                {
                    movieProperties = MoviePropertiesController.GetById(1);
                    //SandboxEntities.MovieProperties.Attach(movieProperties);
                }

                return movieProperties;
            }

            set => movieProperties = value;
        }

        /// <summary>
        /// Gets the PhraseEntries.
        /// </summary>
        public static List<Models.PhraseEntry>? PhraseEntries
        {
            get
            {
                if (phraseEntries?.Count == 0)
                {
                    phraseEntries = PhrasesController.GetPhrasesByPhraseHeaderId(1);
                    //phraseEntries = SandboxEntities.PhraseEntry.Where(x => x.PhraseID == 1).OrderBy(x => x.Description).ToList();
                }

                return phraseEntries;
            }
        }

        public static List<ProductionCompany> ProductionCompanies
        {
            get
            {
                if (productionCompanies == null)
                {
                    using var ctx = SandboxEntities;
                    productionCompanies = ctx.ProductionCompany.AsNoTracking().ToList();
                }
                return productionCompanies;
            }

            set => productionCompanies = value;
        }

        public static DBContext.SandboxEntities SandboxEntities
        {
            get
            {
                // Return a new short-lived DbContext for each call to avoid shared context across threads
                var ctx = new DBContext.SandboxEntities();
                ctx.ChangeTracker.AutoDetectChangesEnabled = false;
                return ctx;
            }
            // keep setter for compatibility (sets the unused backing field)
            set => sandboxEntities = value;
        }

        /// <summary>
        /// Gets the SeriesList.
        /// </summary>
        public static List<Series> SeriesList
        {
            get
            {
                if (seriesEntries.Count == 0)
                {
                    seriesEntries = SandboxEntities.Series.OrderBy(s => s.Name).ToList();
                }
                return seriesEntries;
            }
        }

        /// <summary>
        /// Gets the StoryFields.
        /// </summary>
        public static List<Models.PhraseEntry> StoryFields
        {
            get
            {
                if (storyFields.Count == 0)
                {
                    storyFields = SandboxEntities.PhraseEntry.Where(x => x.PhraseID == 11).OrderBy(x => x.Description).ToList();
                }

                return storyFields;
            }
        }

        /// <summary>
        /// Gets the StoryList.
        /// </summary>
        public static List<Story> StoryList => StoryController.GetStories().ToList();
            //SandboxEntities.Story.Include(s => s.WordHeadingList).Include(s => s.StorySeries).ToList();

        /// <summary>
        /// Gets or sets the StoryProperties.
        /// </summary>
        public static StoryProperties StoryProperties
        {
            get
            {
                if (storyProperties == null)
                {
                    storyProperties = StoryController.GetStoryProperties();
                }

                return storyProperties;
            }

            set => storyProperties = value;
        }

        public static List<StorySeries> StorySeriesList
        {
            get
            {
                using var ctx = SandboxEntities;
                storySeriesList = ctx.StorySeries.AsNoTracking().OrderBy(d => d.Name).ToList();

                return storySeriesList;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the SubPhraseEntries.
        /// </summary>
        public static List<Models.PhraseEntry> SubPhraseEntries
        {
            get
            {
                if (subPhraseEntries.Count == 0)
                {
                    subPhraseEntries = SandboxEntities.PhraseEntry.Where(x => x.PhraseID == 9).OrderBy(x => x.Id).OrderBy(x => x.Description).ToList();
                }

                return subPhraseEntries;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static DownloadProperties GetDownloadProperties()
        {
            DownloadProperties downloadProperties = new DownloadProperties();
            var temp = SandboxEntities.DownloadProperties.FirstOrDefault();
            if (temp != null)
            {
                downloadProperties.Id = temp.Id;
                downloadProperties.SortDirection = temp.SortDirection;
                downloadProperties.SortedColumn = temp.SortedColumn;
                downloadProperties.LastUnboundIndex = temp.LastUnboundIndex;
            }
            else
            {
                downloadProperties.Id = 1;
                downloadProperties.SortDirection = 0;
                downloadProperties.SortedColumn = 0;
                downloadProperties.LastUnboundIndex = 0;
            }
            return downloadProperties;
        }

        /// <summary>
        /// Gets the phrase entries.
        /// </summary>
        /// <returns></returns>
        /// <autogeneratedoc />
        public static ObservableCollection<PhraseEntry> GetPhraseEntries()
        {
            // get a list of phraseentries where phraseId = 1
            List<Models.PhraseEntry> temp = SandboxEntities.PhraseEntry.Where(x => x.PhraseID == 1).OrderBy(x => x.Description).ToList();
            return new ObservableCollection<PhraseEntry>(temp);
        }

        /// <summary>
        /// The GetSubPhraseEntries.
        /// </summary>
        /// <param name="phrase">The phrase<see cref="PhraseEntry"/>.</param>
        /// <returns>The <see cref="List{Models.PhraseEntry}"/>.</returns>
        public static List<Models.PhraseEntry> GetSubPhraseEntries(PhraseEntry phrase)
        {
            // get a temporary variable = the phraseId + '.' the subphrase identifier
            string tempPhraseId = phrase.Id + ".";
            List<Models.PhraseEntry> tempList = PhrasesController.GetSubPhraseEntries(phrase.Id);
            //List<Models.PhraseEntry> temp = SandboxEntities.PhraseEntry.Where(p => p.Id.Contains(tempPhraseId) && p.PhraseID == 9).OrderBy(x => x.Description).ToList();
            if (tempList == null) tempList = new List<Models.PhraseEntry>();
            if (tempList.Count == 0)
            {
                PhraseEntry tempPhrase = GetTempPhrase(phrase);

                PhrasesController.Add(tempPhrase);
                PhrasesController.Update(tempPhrase);
                tempList.Add(phrase);
            }
            else
            {
                tempPhraseId += phrase.Id;
                Models.PhraseEntry tempPhrase = tempList.Where(p => p.Id == tempPhraseId).FirstOrDefault();

                if (tempPhrase == null)
                {
                    // add phrase
                    tempPhrase = GetTempPhrase(phrase);
                    PhrasesController.Add(tempPhrase);
                    PhrasesController.Update(tempPhrase);
                    tempList.Add(phrase);
                }
            }
            
            return tempList;
        }

        private static PhraseEntry GetTempPhrase(PhraseEntry phrase)
        {
            return new PhraseEntry()
            {
                PhraseID = 9,
                Description = phrase.Description,
                Id = phrase.Id + "." + phrase.Id,
                Order = subPhraseEntries.Count + 1
            };
        }

        public static void ReloadBookMarks(Movies movie)
        {
            if (movie == null) return;
            using var ctx = SandboxEntities;
            var dbMovie = ctx.Movies.Find(movie.Id);
            if (dbMovie != null)
            {
                ctx.Entry(dbMovie).Collection(b => b.Bookmarks).Reload();
            }
        }

        public static void ReloadMovie(Movies movie)
        {
            if (movie == null) return;
            using var ctx = SandboxEntities;
            var dbMovie = ctx.Movies.Find(movie.Id);
            if (dbMovie != null)
            {
                ctx.Entry(dbMovie).Reload();
            }
        }

        public static bool ShowOnAlternateScreen()
        {
            bool returnValue = false;

            MovieAppProperties movieAppProperties = MovieAppPropertiesList.FirstOrDefault(p => string.IsNullOrEmpty(p.PropertyName) || p.PropertyName == "AltScreen" || p.PropertyName == "All");

            if (movieAppProperties != null)
            {
                returnValue = movieAppProperties.BoolValue;
            }

            return returnValue;
        }

        #endregion Public Methods

    }
}