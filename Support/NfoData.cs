//-----------------------------------------------------------------------
// <copyright file="imdb.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>21/08/2019 12:43:27 21/08/2019 12:43:27 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="NfoData" />
    /// </summary>
    public class NfoData
    {
        #region Fields

        /// <summary>
        /// Defines the adult
        /// </summary>
        private bool adult;

        /// <summary>
        /// Defines the atvShow
        /// </summary>
        private bool atvShow = false;

        /// <summary>
        /// Defines the cast
        /// </summary>
        private CastList? cast = null;

        /// <summary>
        /// Defines the companies
        /// </summary>
        //private ProductionCompanyList? companies;

        private List<ProductionCompany>? companies;

        /// <summary>
        /// Defines the countries
        /// </summary>
        private CountryList? countries;

        /// <summary>
        /// Defines the dBid
        /// </summary>
        private int dBid;

        /// <summary>
        /// Defines the director
        /// </summary>
        private string? director;

        /// <summary>
        /// Defines the durationInSeconds
        /// </summary>
        private int? durationInSeconds = 0;

        /// <summary>
        /// Defines the episodeDetails
        /// </summary>
        private bool? episodeDetails = false;

        /// <summary>
        /// Defines the episodeDetails1
        /// </summary>
        private EpisodeDetails episodeDetails1;

        /// <summary>
        /// Defines the fileInfo
        /// </summary>
        private FileInfo? fileInfo;

        /// <summary>
        /// Defines the genre
        /// </summary>
        private string genre;

        /// <summary>
        /// Defines the genres
        /// </summary>
        private GenreList genres;

        /// <summary>
        /// Defines the imdbid
        /// </summary>
        private string imdbid;

        /// <summary>
        /// Defines the languages
        /// </summary>
        private LanguageList languages;

        /// <summary>
        /// Defines the originalLanguage
        /// </summary>
        private string originalLanguage;

        /// <summary>
        /// Defines the originalTitle
        /// </summary>
        private string originalTitle;

        /// <summary>
        /// Defines the plot
        /// </summary>
        private string plot;

        /// <summary>
        /// Defines the poster_Path
        /// </summary>
        private string poster_Path;

        /// <summary>
        /// Defines the rated
        /// </summary>
        private string rated;

        /// <summary>
        /// Defines the released
        /// </summary>
        private string released;

        /// <summary>
        /// Defines the runtime
        /// </summary>
        private string runtime;

        /// <summary>
        /// Defines the similarMovies
        /// </summary>
        private MovieList similarMovies;

        /// <summary>
        /// Defines the subtitle
        /// </summary>
        private string subtitle;

        /// <summary>
        /// Defines the tags
        /// </summary>
        private TagList tags;

        /// <summary>
        /// Defines the title
        /// </summary>
        private string title;

        /// <summary>
        /// Defines the type
        /// </summary>
        private string type;

        /// <summary>
        /// Defines the xML
        /// </summary>
        private XElement? xML = null;

        /// <summary>
        /// Defines the year
        /// </summary>
        private string year;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NfoData"/> class.
        /// </summary>
        public NfoData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NfoData"/> class.
        /// </summary>
        /// <param name="iMovie">The iMovie<see cref="Support.iMovie"/></param>
        public NfoData(iMovie iMovie)
        {
            if (iMovie != null)
            {
                Languages = iMovie.Languages;
                Title = iMovie.Title;
                Adult = iMovie.Adult;
                Cast = iMovie.CastList;
                Companies = iMovie.ProductionCompanies;
                Countries = iMovie.Countries;
                Genres = iMovie.GenreList;
                Released = iMovie.ReleaseDate.ToShortDateString();
                Plot = iMovie.Overview;
                TMDBid = iMovie.ID;
                Year = iMovie.ReleaseDate.ToString("yyyy-MM-dd");
                OriginalLanguage = iMovie.Language;
                OriginalTitle = iMovie.Title;
                Director = iMovie.DirectorName;
                Imdbid = iMovie.IMDBID;
                Rated = iMovie.Rating;
                //Tags = iMovie.ta
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        public bool Adult { get => adult; set => adult = value; }

        /// <summary>
        /// Gets or sets a value indicating whether AtvShow
        /// </summary>
        public bool AtvShow { get => atvShow; set => atvShow = value; }

        /// <summary>
        /// Gets or sets the Cast
        /// </summary>
        public CastList? Cast { get => cast; set => cast = value; }

        /// <summary>
        /// Gets or sets the Companies
        /// </summary>
        public List<ProductionCompany>? Companies { get => companies; set => companies = value; }

        /// <summary>
        /// Gets or sets the Countries
        /// </summary>
        public CountryList? Countries { get => countries; set => countries = value; }

        /// <summary>
        /// Gets or sets the Director
        /// </summary>
        public string? Director { get => director; set => director = value; }

        /// <summary>
        /// Gets or sets a value indicating whether EpisodeDetails
        /// </summary>
        public bool? EpisodeDetails { get => episodeDetails; set => episodeDetails = value; }

        /// <summary>
        /// Gets or sets the Episodes
        /// </summary>
        public EpisodeDetails Episodes
        {
            get
            {
                if (episodeDetails1 == null)
                {
                    episodeDetails1 = new EpisodeDetails();
                }
                return episodeDetails1;
            }
            set => episodeDetails1 = value;
        }

        /// <summary>
        /// Gets or sets the FileInfo
        /// </summary>
        public FileInfo FileInfo { get => fileInfo; set => fileInfo = value; }

        /// <summary>
        /// Gets or sets the Genre
        /// </summary>
        public string Genre { get => genre; set => genre = value; }

        /// <summary>
        /// Gets or sets the Genres
        /// </summary>
        public GenreList Genres
        {
            get
            {
                if (genres == null)
                {
                    genres = new GenreList();
                }
                return genres;
            }
            set => genres = value;
        }

        /// <summary>
        /// Gets or sets the Imdbid
        /// </summary>
        public string Imdbid { get => imdbid; set => imdbid = value; }

        /// <summary>
        /// Gets or sets the Languages
        /// </summary>
        public LanguageList Languages { get => languages; set => languages = value; }

        /// <summary>
        /// Gets or sets the OriginalLanguage
        /// </summary>
        public string OriginalLanguage { get => originalLanguage; set => originalLanguage = value; }

        /// <summary>
        /// Gets or sets the OriginalTitle
        /// </summary>
        public string OriginalTitle { get => originalTitle; set => originalTitle = value; }

        /// <summary>
        /// Gets or sets the Plot
        /// </summary>
        public string Plot { get => plot; set => plot = value; }

        /// <summary>
        /// Gets or sets the Poster_Path
        /// </summary>
        public string Poster_Path { get => poster_Path; set => poster_Path = value; }

        /// <summary>
        /// Gets or sets the Rated
        /// </summary>
        public string Rated { get => rated; set => rated = value; }

        /// <summary>
        /// Gets or sets the Released
        /// </summary>
        public string Released { get => released; set => released = value; }

        /// <summary>
        /// Gets or sets the Runtime
        /// </summary>
        public string Runtime { get => runtime; set => runtime = value; }

        /// <summary>
        /// Gets or sets the SimilarMovies
        /// </summary>
        public MovieList SimilarMovies
        {
            get
            {
                if (similarMovies == null)
                {
                    similarMovies = new MovieList();
                }
                return similarMovies;
            }

            set => similarMovies = value;
        }

        /// <summary>
        /// Gets or sets the Subtitle
        /// </summary>
        public string Subtitle { get => subtitle; set => subtitle = value; }

        /// <summary>
        /// Gets or sets the Tags
        /// </summary>
        public TagList Tags
        {
            get
            {
                if (tags == null)
                {
                    tags = new TagList();
                }

                return tags;
            }
            set => tags = value;
        }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title { get => title; set => title = value; }

        /// <summary>
        /// Gets or sets the TMDBid
        /// </summary>
        public int TMDBid { get => dBid; set => dBid = value; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public string Type { get => type; set => type = value; }

        /// <summary>
        /// Gets or sets the XML
        /// </summary>
        public XElement XML { get => xML; set => xML = value; }

        /// <summary>
        /// Gets or sets the Year
        /// </summary>
        public string Year { get => year; set => year = value; }

        #endregion

        #region Methods

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="filename">The filename<see cref="string"/></param>
        public void Save(string filename)
        {
            ToXML();

            XML.Save(filename);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            string returnValue = string.Empty;

            returnValue += "Title : " + Title + Environment.NewLine;
            returnValue += " Genre : " + genre + Environment.NewLine;
            returnValue += " Plot : " + Plot + Environment.NewLine;

            return returnValue;
        }

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML()
        {
            XElement root = null;
            if (episodeDetails!= null && episodeDetails.Value)
            {
                root = Episodes.ToXML();
                XML = Episodes.ToXML();
                //this.XML.Add(new XElement("episode", this.Episode));
                //this.XML.Add(new XElement("season", this.Season));
                //this.XML.Add(new XElement("showtitle", this.ShowTitle));
            }
            else if (atvShow)
            {
                root = new XElement("tvshow");
                XML = new XElement("tvshow");
                //foreach (XElement element in this.Episodes)
                //{
                //    this.XML.Add(element);
                //}
            }
            else
            {
                root = new XElement("movie");
                XML = new XElement("movie");
            }

            XML.Add(new XElement("title", Title));
            //this.myTitle = string.Empty;
            XML.Add(new XElement("originaltitle", OriginalTitle));
            XML.Add(new XElement("originallanguage", OriginalLanguage));
            XML.Add(new XElement("year", Year));
            //this.myYear = string.Empty;
            XML.Add(new XElement("plot", Plot));
            //this.myPlot = string.Empty;
            XML.Add(new XElement("id", Imdbid));
            //this.myIMDBid = string.Empty;
            XML.Add(new XElement("tmdbid", TMDBid));
            //this.TMDBID = 0;
            this.XML.Add(new XElement("director", this.Director));
            //this.myDirector = string.Empty;

            XML.Add(new XElement("adult", Adult));
            XML.Add(new XElement("vote_average", Rated));

            if (Genres != null)
            {
                XML = Genres.ToXML(XML);
            }

            if (Countries != null)
            {
                foreach (Country genre in Countries)
                {
                    XML.Add(new XElement("country", genre.Name));
                }
            }


            // do companies
            if (Companies != null)
            {
                string studio = string.Empty;
                foreach (ProductionCompany company in Companies)
                {
                    // check to see if already in list
                    if (!studio.Contains(company.Name))
                    {
                        if (studio != string.Empty)
                        {
                            studio += ",";
                        }
                        studio += company.Name;
                        XML.Add(company.ToXML());
                    }
                }
                XML.Add(new XElement("studio", studio));
            }

            if (Languages != null)
            {
                XElement xlanguage = new XElement("languages");
                foreach (Language genre in Languages)
                {
                    xlanguage.Add(new XElement("language", genre.Name));
                }
                XML.Add(xlanguage);
                XML.Add(new XElement("language", Languages.ToString()));
            }

            if (Runtime != null && Runtime != string.Empty)
            {
                durationInSeconds = (int)(double.Parse(Runtime) * 60D);
            }

            // add actors
            if (cast != null && cast.Count > 0)
            {
                XML = cast.ToXML(XML);
            }

            if (fileInfo != null)
            {
                XML.Add(fileInfo.ToXML());
            }

            if (Tags != null && Tags.Count > 0)
            {
                XML = Tags.ToXml(XML);
            }

            if (SimilarMovies != null && SimilarMovies.Count > 0)
            {
                XML.Add(SimilarMovies.ToXML("similarmovies"));
            }

            return XML;
        }

        #endregion
    }
}
