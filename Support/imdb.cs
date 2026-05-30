//-----------------------------------------------------------------------
// <copyright file="imdb.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>21/08/2019 12:43:27 21/08/2019 12:43:27 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="iCastMember" />
    /// </summary>
    public interface iCastMember
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Adult
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the CastID
        /// </summary>
        int CastID { get; set; }

        /// <summary>
        /// Gets or sets the Character
        /// </summary>
        string Character { get; set; }

        /// <summary>
        /// Gets or sets the DBID
        /// </summary>
        string DBID { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        string Name { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="iMovie" />
    /// </summary>
    public interface iMovie
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Actors
        /// </summary>
        string Actors { get; set; }

        /// <summary>
        /// Gets or sets the Adult
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the CastList
        /// </summary>
        CastList CastList { get; set; }

        /// <summary>
        /// Gets or sets the Countries
        /// </summary>
        CountryList Countries { get; set; }

        /// <summary>
        /// Gets or sets the GenreList
        /// </summary>
        GenreList GenreList { get; set; }

        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the IMDBID
        /// </summary>
        string IMDBID { get; set; }

        /// <summary>
        /// Gets or sets the Language
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the Languages
        /// </summary>
        LanguageList Languages { get; set; }

        /// <summary>
        /// Gets or sets the OriginalTitle
        /// </summary>
        string OriginalTitle { get; set; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        string Overview { get; set; }

        /// <summary>
        /// Gets or sets the ProductionCompanies
        /// </summary>
        List<ProductionCompany> ProductionCompanies { get; set; }

        /// <summary>
        /// Gets or sets the ReleaseDate
        /// </summary>
        DateTime ReleaseDate { get; set; }

        string DirectorName { get; }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        string Title { get; set; }

        string Rating { get; set; }

        /// <summary>
        /// Gets or sets the Video
        /// Gets or sets a value indicating whether Video
        /// </summary>
        bool Video { get; set; }

        #endregion
    }

    #endregion

    /// <summary>
    /// Defines the <see cref="Audio" />
    /// </summary>
    public class Audio
    {
        #region Fields

        /// <summary>
        /// Defines the channels
        /// </summary>
        private int channels;

        /// <summary>
        /// Defines the codec
        /// </summary>
        private string? codec;

        /// <summary>
        /// Defines the language
        /// </summary>
        private string? language;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Channels
        /// </summary>
        public int Channels { get => channels; set => channels = value; }

        /// <summary>
        /// Gets or sets the Codec
        /// </summary>
        public string Codec { get => codec; set => codec = value; }

        /// <summary>
        /// Gets or sets the Language
        /// </summary>
        public string Language { get => language; set => language = value; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to xml.
        /// </summary>
        /// <returns></returns>
        public XElement ToXML()
        {
            XElement returnXML = new XElement("audio");
            returnXML.Add(new XElement("channels", channels));
            returnXML.Add(new XElement("codec", codec));
            returnXML.Add(new XElement("language", language));

            return returnXML;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="CastList" />
    /// </summary>
    public class CastList : List<CastMember>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CastList"/> class.
        /// </summary>
        public CastList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CastList"/> class.
        /// </summary>
        /// <param name="actors">The actors<see cref="List{XElement}"/></param>
        public CastList(List<XElement> actors)
        {
            //foreach (XElement item in actors)
            //{
            //    CastMember member = new CastMember()
            //    {
            //        Name = XMLSupport.GetValue(item, "Name", "name", "", ""),
            //        Character = XMLSupport.GetValue(item, "Role", "role", "", "")
            //    };
            //    Add(member);
            //}
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GetDirector
        /// </summary>
        /// <returns>The <see cref="CastMember"/></returns>
        public CastMember GetDirector()
        {
            CastMember? director = null;
            foreach (CastMember item in this)
            {
                if (item.IsDirector)
                {
                    director = item;
                }
            }

            return director;
        }

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <param name="xml">The xml<see cref="XElement"/></param>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML(XElement? xml)
        {
            if (xml == null) xml = new XElement("");
            foreach (CastMember actor in this)
            {
                if (actor.Department == "Directing")
                {
                    xml.Add(new XElement("director", actor.Name));
                }
                else
                {
                    XElement xActor = new XElement("actor");
                    xml.Add(xActor);
                    xActor.Add(new XElement("name", actor.Name));
                    xActor.Add(new XElement("role", actor.Character));
                    xActor.Add(new XElement("thumb", TmdbSupport.ImageURL + actor.Profile_path));
                }
            }

            return xml;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="CastMember" />
    /// </summary>
    public class CastMember : iCastMember
    {
        #region Fields

        /// <summary>
        /// Defines the adult
        /// </summary>
        private bool adult = false;

        /// <summary>
        /// Defines the castID
        /// </summary>
        private int castID = 0;

        /// <summary>
        /// Defines the character
        /// </summary>
        private string character = string.Empty;

        /// <summary>
        /// Defines the deathDate
        /// </summary>
        private DateTime deathDate = DateTime.MinValue;

        /// <summary>
        /// Defines the department
        /// </summary>
        private string department = string.Empty;

        /// <summary>
        /// Defines the gender
        /// </summary>
        private int gender = 0;

        /// <summary>
        /// Defines the id
        /// </summary>
        private int id = 0;

        /// <summary>
        /// Defines the iMDB
        /// </summary>
        private string iMDB = string.Empty;

        /// <summary>
        /// Defines the isDirector
        /// </summary>
        private bool isDirector = false;

        /// <summary>
        /// Defines the knownAs
        /// </summary>
        private string[]? knownAs;

        /// <summary>
        /// Defines the myBirthDate
        /// </summary>
        private DateTime myBirthDate = DateTime.MinValue;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Defines the placeOfBirth
        /// </summary>
        private string placeOfBirth = string.Empty;

        /// <summary>
        /// Defines the profile_path
        /// </summary>
        private string profile_path = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CastMember"/> class.
        /// </summary>
        public CastMember()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CastMember"/> class.
        /// </summary>
        /// <param name="json">The json<see cref="string"/></param>
        public CastMember(string json)
        {
            CastMember? member = JsonConvert.DeserializeObject<CastMember>(json);

            if (member != null)
            {
                adult = member.adult;
                castID = member.castID;
                Character = member.character;
                DeathDate = member.deathDate;
                Department = member.department;
                Gender = member.gender;
                id = member.ID;
                IMDB = member.IMDB;
                KnownAs = member.knownAs;
                Name = member.name;
                PlaceOfBirth = member.placeOfBirth;
                Profile_path = member.profile_path;
            }
        }

        #endregion

        #region Properties

        public string KnownForDescription
        {
            get
            {
                string knownForDescription = string.Empty;
                if (KnownFor != null)
                {
                    knownForDescription += KnownFor.DescriptionList;
                }
                return knownForDescription;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        [JsonProperty("adult")]
        public bool Adult { get => adult; set => adult = value; }

        /// <summary>
        /// Gets or sets the BirthDate
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                if (birthday != null && birthday != string.Empty)
                {
                    myBirthDate = DateTime.Parse(birthday);
                }
                else
                {
                    myBirthDate = DateTime.MinValue;
                }
                return myBirthDate;
            }

            set => myBirthDate = value;
        }

        /// <summary>
        /// Gets or sets the birthday
        /// </summary>
        [JsonProperty("birthday")]
        public string? birthday { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cast identifier.
        /// </summary>
        [JsonProperty("cast_id")]
        public int CastID { get => castID; set => castID = value; }

        /// <summary>
        /// Gets or sets the Character
        /// </summary>
        [JsonProperty("character")]
        public string Character { get => character; set => character = value; }

        /// <summary>
        /// Gets or sets the CreditId
        /// </summary>
        [JsonProperty("credit_id")]
        public string CreditId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the DBID
        /// </summary>
        public string DBID { get => IMDB; set => IMDB = value; }

        /// <summary>
        /// Gets or sets the DeathDate
        /// </summary>
        public DateTime DeathDate
        {
            get
            {
                if (!string.IsNullOrEmpty(DeathDay))
                {
                    deathDate = DateTime.Parse(DeathDay);
                }
                return deathDate;
            }

            set => deathDate = value;
        }

        /// <summary>
        /// Gets or sets the DeathDay
        /// </summary>
        [JsonProperty("deathday")]
        public string DeathDay { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Department
        /// </summary>
        [JsonProperty("department")]
        public string Department { get => department; set => department = value; }

        /// <summary>
        /// Gets or sets the Gender
        /// </summary>
        [JsonProperty("gender")]
        public int Gender { get => gender; set => gender = value; }

        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        [JsonProperty("id")]
        public int ID { get => id; set => id = value; }

        /// <summary>
        /// Gets or sets the IMDB
        /// </summary>
        [JsonProperty("imdb")]
        public string IMDB { get => iMDB; set => iMDB = value; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDirector
        /// </summary>
        public bool IsDirector { get => isDirector; set => isDirector = value; }

        /// <summary>
        /// Gets or sets the KnownAs
        /// </summary>
        [JsonProperty("also_known_as")]
        public string[]? KnownAs { get => knownAs; set => knownAs = value; }

        /// <summary>
        /// Gets or sets the KnownFor
        /// </summary>
        [JsonProperty("known_for")]
        public KnownForList? KnownFor { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets or sets the PlaceOfBirth
        /// </summary>
        [JsonProperty("placeofbirth")]
        public string PlaceOfBirth { get => placeOfBirth; set => placeOfBirth = value; }

        /// <summary>
        /// Gets or sets the Profile_path
        /// </summary>
        [JsonProperty("profile_path")]
        public string Profile_path { get => profile_path; set => profile_path = value; }

        #endregion

        #region Methods

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            string retValue = string.Empty;
            if (Adult)
            {
                retValue += "Adult Actor ";
            }

            if (knownAs != null && knownAs.Count() > 0)
            {
                retValue += " also known as : ";
                foreach (string item in knownAs)
                {
                    retValue += item + ";";
                }
            }


            return retValue;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Country" />
    /// </summary>
    public class Country
    {
        #region Fields

        /// <summary>
        /// Defines the iso_3166_1
        /// </summary>
        private string? iso_3166_1;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string? name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Iso_3166_1
        /// </summary>
        [JsonProperty("iso_3166_1")]
        public string Iso_3166_1 { get => iso_3166_1; set => iso_3166_1 = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="CountryList" />
    /// </summary>
    public class CountryList : List<Country>
    {
    }

    /// <summary>
    /// Defines the <see cref="Credit" />
    /// </summary>
    public class Credit
    {
        #region Fields

        /// <summary>
        /// Defines the creditType
        /// </summary>
        private string? creditType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the CreditType
        /// </summary>
        [JsonProperty("credit_type")]
        public string CreditType { get => creditType; set => creditType = value; }

        /// <summary>
        /// Gets or sets the Department
        /// </summary>
        [JsonProperty("department")]
        public string Department { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the Job
        /// </summary>
        [JsonProperty("job")]
        public string Job { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Media
        /// </summary>
        [JsonProperty("media")]
        public Media? Media { get; set; }

        /// <summary>
        /// Gets or sets the MediaType
        /// </summary>
        [JsonProperty("media_type")]
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Person
        /// </summary>
        [JsonProperty("person")]
        public Person? Person { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Episode" />
    /// </summary>
    public class Episode
    {
        #region Fields

        /// <summary>
        /// Defines the myEpisode
        /// </summary>
        private string myEpisode = string.Empty;

        /// <summary>
        /// Defines the myId
        /// </summary>
        private string myId = string.Empty;

        /// <summary>
        /// Defines the myName
        /// </summary>
        private string myName = string.Empty;

        /// <summary>
        /// Defines the myOutline
        /// </summary>
        private string myOutline = string.Empty;

        /// <summary>
        /// Defines the mySeason
        /// </summary>
        private string mySeason = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Episode"/> class.
        /// </summary>
        /// <param name="element">The element<see cref="XElement"/></param>
        public Episode(XElement element)
        {
            //EpisodeNo = XMLSupport.GetValue(element, "EpisodeNumber", "");
            //Id = XMLSupport.GetValue(element, "id", "");
            //Name = XMLSupport.GetValue(element, "EpisodeName", "");
            //Overview = XMLSupport.GetValue(element, "Overview", "");
            //SeasonNo = XMLSupport.GetValue(element, "Combined_season", "");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the EpisodeNo
        /// </summary>
        public string EpisodeNo { get => myEpisode; set => myEpisode = value; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public string Id { get => myId; set => myId = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get => myName; set => myName = value; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        public string Overview { get => myOutline; set => myOutline = value; }

        /// <summary>
        /// Gets or sets the SeasonNo
        /// </summary>
        public string SeasonNo { get => mySeason; set => mySeason = value; }

        #endregion

        #region Methods

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML()
        {
            XElement element = new XElement("Episode");
            element.Add(new XElement("EpisodeNumber", EpisodeNo));
            element.Add(new XElement("Combined_season", SeasonNo));
            element.Add(new XElement("id", Id));
            element.Add(new XElement("EpisodeName", Name));
            element.Add(new XElement("Overview", Overview));

            return element;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="EpisodeDetails" />
    /// </summary>
    public class EpisodeDetails
    {
        #region Fields

        /// <summary>
        /// Defines the airDate
        /// </summary>
        private DateTime airDate;

        /// <summary>
        /// Defines the episodeNumber
        /// </summary>
        private int episodeNumber;

        /// <summary>
        /// Defines the id
        /// </summary>
        private int id;

        /// <summary>
        /// Defines the myEpisode
        /// </summary>
        private string myEpisode = string.Empty;

        /// <summary>
        /// Defines the mySeason
        /// </summary>
        private string mySeason = string.Empty;

        /// <summary>
        /// Defines the myShowTitle
        /// </summary>
        private string myShowTitle = string.Empty;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Defines the overview
        /// </summary>
        private string overview = string.Empty;

        /// <summary>
        /// Defines the seasonNumber
        /// </summary>
        private int seasonNumber;

        /// <summary>
        /// Defines the showId
        /// </summary>
        private int showId;
        private CastList? castMembers;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AirDate
        /// </summary>
        [JsonProperty("air_date")]
        public DateTime AirDate { get => airDate; set => airDate = value; }

        /// <summary>
        /// Gets or sets the Episode
        /// </summary>
        public string Episode { get => myEpisode; set => myEpisode = value; }

        /// <summary>
        /// Gets or sets the EpisodeNumber
        /// </summary>
        [JsonProperty("episode_number")]
        public int EpisodeNumber { get => episodeNumber; set => episodeNumber = value; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get => id; set => id = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public string Overview
        {
            get
            {
                if (overview.Length > 200)
                {
                    overview = overview.Substring(0, 200);
                }
                return overview;
            }

            set
            {
                if (value.Length > 200)
                {
                    value = value.Substring(0, 200);
                }
                overview = value;
            }
        }

        /// <summary>
        /// Gets or sets the Season
        /// </summary>
        public string Season { get => mySeason; set => mySeason = value; }

        /// <summary>
        /// Gets or sets the SeasonNumber
        /// </summary>
        [JsonProperty("season_number")]
        public int SeasonNumber { get => seasonNumber; set => seasonNumber = value; }

        /// <summary>
        /// Gets or sets the ShowId
        /// </summary>
        [JsonProperty("show_id")]
        public int ShowId { get => showId; set => showId = value; }


        [JsonProperty("crew")]
        public CastList CastMembers { get => castMembers; set => castMembers = value; }

        /// <summary>
        /// Gets or sets the ShowTitle
        /// </summary>
        public string ShowTitle { get => myShowTitle; set => myShowTitle = value; }

        #endregion

        #region Methods

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML()
        {
            XElement XML = new XElement("episodedetails");
            XML.Add(new XElement("episode", EpisodeNumber));
            XML.Add(new XElement("season", SeasonNumber));
            XML.Add(new XElement("showtitle", ShowTitle));
            XML.Add(new XElement("id", Id));
            XML.Add(new XElement("aired", airDate.ToString()));
            XML.Add(new XElement("plot", overview));
            return XML;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="EpisodeDetailsList" />
    /// </summary>
    public class EpisodeDetailsList : List<EpisodeDetails>
    {
    }

    /// <summary>
    /// Defines the <see cref="EpisodeList" />
    /// </summary>
    public class EpisodeList : List<Episode>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeList"/> class.
        /// </summary>
        public EpisodeList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeList"/> class.
        /// </summary>
        /// <param name="collection">The collection<see cref="List{XElement}"/></param>
        public EpisodeList(List<XElement> collection)
        {
            foreach (XElement item in collection)
            {
                Episode episode = new Episode(item);
                Add(episode);
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="FileInfo" />
    /// </summary>
    public class FileInfo
    {
        #region Fields

        /// <summary>
        /// The stream details
        /// </summary>
        private StreamDetails? streamDetails = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the stream details.
        /// </summary>
        public StreamDetails StreamDetails
        {
            get
            {
                if (streamDetails == null)
                {
                    streamDetails = new StreamDetails();
                }
                return streamDetails;
            }
            set => streamDetails = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to xml.
        /// </summary>
        /// <returns></returns>
        public XElement ToXML()
        {
            XElement xElement = new XElement("fileinfo");
            xElement.Add(StreamDetails.ToXML());
            return xElement;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Genre" />
    /// </summary>
    public class Genre
    {
        #region Fields

        /// <summary>
        /// Defines the iD
        /// </summary>
        private int iD;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        public Genre()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        public Genre(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        [JsonProperty("id")]
        public int ID { get => iD; set => iD = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="GenreId" />
    /// </summary>
    public class GenreId
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="GenreList" />
    /// </summary>
    public class GenreList : List<Genre>
    {
        #region Fields

        /// <summary>
        /// Defines the instance
        /// </summary>
        private static GenreList? instance = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreList"/> class.
        /// </summary>
        public GenreList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreList"/> class.
        /// </summary>
        /// <param name="list">The list<see cref="List{XElement}"/></param>
        public GenreList(List<XElement> list)
        {
            Clear();
            foreach (XElement item in list)
            {
                Genre genre = new Genre();
                genre.Name = item.Value;
                Add(genre);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the genres.
        /// </summary>
        /// <returns></returns>
        public static GenreList GetGenres()
        {
            if (instance == null)
            {
                instance = TmdbSupport.GetGenres();
            }

            return instance;
        }

        /// <summary>
        /// Finds the genre by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Genre FindGenreById(int id)
        {
            return Find(x => x.ID == id);
        }

        /// <summary>
        /// The GenresFromIEnum
        /// </summary>
        /// <param name="list">The list<see cref="IEnumerable{string}"/></param>
        /// <returns>The <see cref="GenreList"/></returns>
        public GenreList GenresFromIEnum(IEnumerable<string> list)
        {
            Clear();
            foreach (string item in list)
            {
                Genre genre = new Genre();
                genre.Name = item;
            }

            return this;
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            string returnString = string.Empty;

            foreach (Genre item in this)
            {
                if (returnString != string.Empty)
                {
                    returnString += ",";
                }
                returnString += item.Name;
            }

            return returnString;
        }

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <param name="xml">The xml<see cref="XElement"/></param>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML(XElement xml)
        {
            foreach (Genre item in this)
            {
                xml.Add(new XElement("genre", item.Name));
            }

            return xml;
        }

        #endregion
    }

    public class ProductionCompany
    {
        private string? country;
        private string? logoPath;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the Country
        /// </summary>
        [JsonProperty("origin_country")]
        public string? Country { get => country; set => country = value; }

        /// <summary>
        /// Gets or sets the LogoPath
        /// </summary>
        [JsonProperty("logo_path")]
        public string? LogoPath { get => logoPath; set => logoPath = value; }

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML()
        {
            XElement xElement = new XElement("production_company");
            xElement.Add(new XElement("id", Id));
            xElement.Add(new XElement("name", Name));
            xElement.Add(new XElement("logo_path", LogoPath));
            xElement.Add(new XElement("origin_country", Country));
            return xElement;
        }

        //  "id": 40250,
        //"logo_path": null,
        //"name": "Production Group",
        //"origin_country": "IT"
    }

    /// <summary>
    /// Defines the <see cref="ISO3166Country" />
    /// </summary>
    public class ISO3166Country
    {
        #region Fields

        /// <summary>
        /// Defines the iso_3166_1
        /// </summary>
        private string iso_3166_1 = " ";

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Iso_3166_1
        /// </summary>
        [JsonProperty("Code")]
        public string Iso_3166_1 { get => iso_3166_1; set => iso_3166_1 = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get => name; set => name = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ISO3166CountryList" />
    /// </summary>
    public class ISO3166CountryList : List<ISO3166Country>
    {
        #region Fields

        /// <summary>
        /// Defines the instance
        /// </summary>
        private static ISO3166CountryList? instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ISO3166CountryList"/> class.
        /// </summary>
        public ISO3166CountryList()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GetCountries
        /// </summary>
        /// <returns>The <see cref="ISO3166CountryList"/></returns>
        public static ISO3166CountryList GetCountries()
        {
            if (instance == null)
            {
                string path = @"C:\Users\doug\Documents\Visual Studio 2015\Projects\Git\Support\Support\CountryCodes.json";
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string json = streamReader.ReadToEnd();

                    instance = JsonConvert.DeserializeObject<ISO3166CountryList>(json);
                }
            }

            return instance;
        }

        /// <summary>
        /// The FindByCountryCode
        /// </summary>
        /// <param name="code">The code<see cref="string"/></param>
        /// <returns>The <see cref="ISO3166Country"/></returns>
        public ISO3166Country FindByCountryCode(string code)
        {
            ISO3166Country? iso3166Country = null;

            iso3166Country = Find(x => x.Iso_3166_1.ToUpper() == code.ToUpper());
            return iso3166Country;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="KnownFor" />
    /// </summary>
    public class KnownFor
    {
        #region Fields

        /// <summary>
        /// Defines the adultValue
        /// </summary>
        private bool adultValue = false;

        /// <summary>
        /// Defines the video1
        /// </summary>
        private bool video1 = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether Adult
        /// </summary>
        public bool Adult
        {
            get
            {
                if (adult != string.Empty)
                {
                    adultValue = adult == "true";
                }
                return adultValue;
            }
        }

        /// <summary>
        /// Gets or sets the Genres
        /// </summary>
        [JsonProperty("genre_ids")]
        public double[]? Genres { get; set; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MediaType
        /// </summary>
        [JsonProperty("media_type")]
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ReleaseDate
        /// </summary>
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether Video
        /// </summary>
        public bool Video
        {
            get
            {
                if (video != string.Empty)
                {
                    video1 = video == "true";
                }

                return video1;
            }
        }

        /// <summary>
        /// Gets or sets the adult
        /// </summary>
        [JsonProperty("adult")]
        private string adult { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the video
        /// </summary>
        [JsonProperty("video")]
        private string video { get; set; } = string.Empty;

        public string Description
        {
            get
            {
                string description = string.Empty;

                if (Adult)
                {
                    description += "Adult :";
                }

                description += " Title : " + Title;

                return description;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="KnownForList" />
    /// </summary>
    public class KnownForList : List<KnownFor>
    {
        public string DescriptionList
        {
            get
            {
                string descriptionList = string.Empty;
                foreach (KnownFor item in this)
                {
                    if (!string.IsNullOrEmpty(descriptionList)) descriptionList += "|";
                    descriptionList += item.Description;
                }

                return descriptionList;
            }
        }
    }

    /// <summary>
    /// Defines the <see cref="Language" />
    /// </summary>
    public class Language
    {
        #region Fields

        /// <summary>
        /// Defines the iso_639_1
        /// </summary>
        private string iso_639_1 = string.Empty;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Iso_639_1
        /// </summary>
        [JsonProperty("iso_639_1")]
        public string Iso_639_1 { get => iso_639_1; set => iso_639_1 = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="LanguageList" />
    /// </summary>
    public class LanguageList : List<Language>
    {
        public override string ToString()
        {
            string returnVal = string.Join(",", this.Select(x => x.Name));
            return returnVal;
        }
    }

    /// <summary>
    /// Defines the <see cref="Media" />
    /// </summary>
    public class Media
    {
        #region Fields

        /// <summary>
        /// Defines the adultValue
        /// </summary>
        private bool adultValue = false;

        /// <summary>
        /// Defines the video1
        /// </summary>
        private bool video1 = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether Adult
        /// </summary>
        public bool Adult
        {
            get
            {
                if (adult != string.Empty)
                {
                    adultValue = adult == "true";
                }
                return adultValue;
            }
        }

        /// <summary>
        /// Gets or sets the Character
        /// </summary>
        [JsonProperty("character")]
        public string Character { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Genres
        /// </summary>
        [JsonProperty("genre_ids")]
        public int[]? Genres { get; set; } = null;

        /// <summary>
        /// Gets or sets the MediaId
        /// </summary>
        [JsonProperty("id")]
        public string MediaId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ReleaseDate
        /// </summary>
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether Video
        /// </summary>
        public bool Video
        {
            get
            {
                if (video != string.Empty)
                {
                    video1 = video == "true";
                }

                return video1;
            }
        }

        /// <summary>
        /// Gets or sets the adult
        /// </summary>
        [JsonProperty("adult")]
        private string adult { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the video
        /// </summary>
        [JsonProperty("video")]
        private string video { get; set; } = string.Empty;

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="MovieBase" />
    /// </summary>
    public class MovieBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the IMDBID
        /// </summary>
        public string IMDBID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Year
        /// </summary>
        public string Year { get; set; } = string.Empty;

        public string Rating { get; set; } = string.Empty;



        #endregion
    }

    /// <summary>
    /// Defines the <see cref="MovieItem" />
    /// </summary>
    public class MovieItem : MovieBase, iMovie
    {
        #region Fields

        /// <summary>
        /// Defines the adult
        /// </summary>
       // private bool adult;

        /// <summary>
        /// Defines the castMembers
        /// </summary>
        private CastList? castMembers;

        /// <summary>
        /// Defines the countries
        /// </summary>
        private CountryList? countries = null;

        /// <summary>
        /// Defines the genreList
        /// </summary>
        private GenreList? genreList = null;

        /// <summary>
        /// Defines the jObject1
        /// </summary>
        private Newtonsoft.Json.Linq.JObject? jObject1 = null;

        /// <summary>
        /// Defines the languages
        /// </summary>
        private LanguageList? languages = null;

        /// <summary>
        /// Defines the originalTitle
        /// </summary>
        private string originalTitle = string.Empty;

        /// <summary>
        /// Defines the overview
        /// </summary>
        private string overview = string.Empty;

        /// <summary>
        /// Defines the productionCompanies
        /// </summary>
        private List<ProductionCompany> productionCompanies = null;

        /// <summary>
        /// Defines the rating
        /// </summary>
        private string rating = string.Empty;

        /// <summary>
        /// Defines the releaseDate
        /// </summary>
        private DateTime releaseDate = DateTime.MinValue;

        /// <summary>
        /// Defines the title
        /// </summary>
        private string title = string.Empty;

        /// <summary>
        /// Defines the video
        /// </summary>
        private bool video = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieItem"/> class.
        /// </summary>
        public MovieItem()
        {
            // need to set Jobject value
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieItem"/> class.
        /// </summary>
        /// <param name="element">The element<see cref="XElement"/></param>
        public MovieItem(XElement element)
        {
            if (element != null)
            {
                //title = XMLSupport.GetValue(element, "title", "Title", "", "");
                //adult = bool.Parse(XMLSupport.GetValue(element, "adult", "Adult", "", "false"));
                //ID = int.Parse(XMLSupport.GetValue(element, "id", "ID", "", "0"));
                //overview = XMLSupport.GetValue(element, "overview", "Overview", "", "");
                //rating = XMLSupport.GetValue(element, "rating", "rating", "", "");
                //releaseDate = DateTime.Parse(XMLSupport.GetValue(element, "releaseDate", "ReleaseDate", "", "1900/1/1"));
            }
        }

        #endregion

        #region Properties

        public JObject jObject 
        { get
            {
                return jObject1;
            }
            set => jObject1 = value; 
        }

        /// <summary>
        /// Gets or sets the Actors
        /// </summary>
        public string Actors { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether Adult
        /// </summary>
        [JsonProperty("adult")]
        public new bool Adult { get => base.Adult; set => base.Adult = value; }

        /// <summary>
        /// Gets or sets the CastList
        /// </summary>
        public CastList CastList
        {
            get
            {
                if (castMembers == null)
                {
                    getCast();
                }

                return castMembers;
            }

            set => castMembers = value;
        }



        /// <summary>
        /// Gets or sets the Countries
        /// </summary>
        public CountryList Countries
        {
            get
            {
                if (countries == null)
                {
                    countries = TmdbSupport.GetProductionCountries();
                }

                return countries;
            }

            set => countries = value;
        }

        /// <summary>
        /// Gets or sets the GenreList
        /// </summary>
        public GenreList GenreList
        {
            get
            {
                if (genreList == null)
                {
                    getGenres();
                }
                return genreList;
            }

            set => genreList = value;
        }

        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        [JsonProperty("id")]
        public new int ID { get => base.ID; set => base.ID = value; }

        /// <summary>
        /// Gets or sets the IMDBID
        /// </summary>
        [JsonProperty("imdb_id")]
        public new string IMDBID { get => base.IMDBID; set => base.IMDBID = value; }

        /// <summary>
        /// Gets or sets the Language
        /// </summary>
        [JsonProperty("original_language")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Languages
        /// </summary>
        public LanguageList? Languages
        {
            get
            {
                if (languages == null)
                {
                    Languages = TmdbSupport.GetLanguages();
                }
                return languages;
            }

            set => languages = value;
        }

        /// <summary>
        /// Gets or sets the OriginalTitle
        /// </summary>
        [JsonProperty("original_title")]
        public string OriginalTitle { get => originalTitle; set => originalTitle = value; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public new string Overview { get => base.Overview; set => base.Overview = value; }

        /// <summary>
        /// Gets or sets the ProductionCompanies
        /// </summary>
        [JsonProperty("production_companies")]
        public List<ProductionCompany> ProductionCompanies
        {
            get
            {
                if (productionCompanies == null)
                {
                    productionCompanies = TmdbSupport.GetProductionCompanies();
                }
                return productionCompanies;
            }

            set => productionCompanies = value;
        }

        /// <summary>
        /// Gets or sets the Rating
        /// </summary>
        [JsonProperty("vote_average")]
        public new string Rating { get => base.Rating; set => base.Rating = value; }

        /// <summary>
        /// Gets or sets the ReleaseDate
        /// </summary>
        public DateTime ReleaseDate
        {
            get
            {
                if (ReleaseDateString != string.Empty)
                {
                    DateTime.TryParse(ReleaseDateString, out releaseDate);
                }

                return releaseDate;
            }

            set => releaseDate = value;
        }

        /// <summary>
        /// Gets or sets the ReleaseDateString
        /// </summary>
        [JsonProperty("release_date")]
        public string ReleaseDateString { get => base.Year; set => base.Year = value; }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [JsonProperty("title")]
        public new string Title { get => base.Title; set => base.Title = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Video
        /// </summary>
        [JsonProperty("video")]
        public bool Video { get => video; set => video = value; }

        /// <summary>
        /// Gets the name of the director.
        /// </summary>
        /// <value>
        /// The name of the director.
        /// </value>
        /// <autogeneratedoc />
        public string DirectorName
        {
            get
            {
                string directorName = string.Empty;

                CastMember? castMember = CastList.Find(x => x.IsDirector);

                if (castMember != null)
                {
                    directorName = castMember.Name;
                }

                return directorName;
            }
        }

        // Pseudocode / Plan (detailed):
        // 1. Locate the synchronous property getter that calls an async method:
        //      private Newtonsoft.Json.Linq.JObject jObject { get { ... } }
        // 2. The code currently does:
        //      string nfoData = TmdbSupport.GetMovieDBJsonAsync(ID);
        //    which produces CS0029 because GetMovieDBJsonAsync returns Task<string>.
        // 3. Fix options:
        //    - Make the caller async and await the call (requires broader API changes).
        //    - Or block on the Task to obtain the result synchronously.
        // 4. Choose blocking approach for minimal, local change: call .GetAwaiter().GetResult() to get string.
        // 5. Update the getter to use:
        //      string nfoData = TmdbSupport.GetMovieDBJsonAsync(ID).GetAwaiter().GetResult();
        //    and guard with string.IsNullOrEmpty before parsing.
        // 6. Keep rest of logic unchanged.
        // 7. This change is minimal and will resolve CS0029 at this location.
        //    If you prefer async propagation, convert this property into an async method instead.

        // Replacement: updated jObject property to synchronously obtain task result
        public static async Task<Newtonsoft.Json.Linq.JObject?> GetJObject(int ID)
        {

            JObject jObject1 = null;
            {
                // Synchronously wait for the async call result to satisfy the string assignment.
                // Using GetAwaiter().GetResult() avoids an extra AggregateException wrapper.
                string nfoData = await TmdbSupport.GetMovieDBJsonAsync(ID);

                if (!string.IsNullOrEmpty(nfoData))
                {
                    jObject1 = Newtonsoft.Json.Linq.JObject.Parse(nfoData);
                }
            }

            return jObject1;


        }

        #endregion

        #region Methods

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML()
        {
            XElement returnValue = new XElement("movie");
            returnValue.Add(new XElement("id", ID));
            returnValue.Add(new XElement("adult", Adult));
            returnValue.Add(new XElement("title", Title));
            returnValue.Add(new XElement("overview", Overview));
            returnValue.Add(new XElement("rating", Rating));
            returnValue.Add(new XElement("releaseDate", releaseDate));
            return returnValue;
        }

        /// <summary>
        /// The getCast
        /// </summary>
        private void getCast()
        {
            castMembers = TmdbSupport.GetMovieCredits(ID);
        }

        /// <summary>
        /// The getGenres
        /// </summary>
        private void getGenres()
        {
            genreList = TmdbSupport.GetGenres(jObject);
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="MovieList" />
    /// </summary>
    public class MovieList : List<MovieItem>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieList"/> class.
        /// </summary>
        public MovieList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieList"/> class.
        /// </summary>
        /// <param name="element">The element<see cref="XElement"/></param>
        public MovieList(XElement element)
        {
            IEnumerable<XElement> xElement = element.Elements("movie");

            foreach (XElement item in xElement)
            {
                Add(new MovieItem(item));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ToXML
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXML(string title)
        {
            XElement returnValue = new XElement(title);
            foreach (MovieItem item in this)
            {
                returnValue.Add(item.ToXML());
            }
            return returnValue;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Person" />
    /// </summary>
    public class Person
    {
        #region Fields

        /// <summary>
        /// Defines the adult1
        /// </summary>
        private bool adult1 = false;

        /// <summary>
        /// Defines the genderValue
        /// </summary>
        private string genderValue = string.Empty;
        private string knownForDescription = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether Adult
        /// </summary>

        public bool Adult
        {
            get
            {
                if (adult != string.Empty)
                {
                    adult1 = adult == "true";
                }

                return adult1;
            }
        }

        [JsonProperty("biography")]
        public string Biography { get; set; } = string.Empty;

        [JsonProperty("imdb_id")]
        public string IMDBID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Gender
        /// </summary>
        [JsonProperty("gender")]
        public int Gender { get; set; }

        /// <summary>
        /// Gets the GenderValue
        /// </summary>
        public string GenderValue
        {
            get
            {
                if (Gender == 0)
                {
                    genderValue = "Not speciifed";
                }
                else if (Gender == 1)
                {
                    genderValue = "Female";
                }
                else
                {
                    genderValue = "Male";
                }

                return genderValue;
            }
        }

        /// <summary>
        /// Gets or sets the KnownFor
        /// </summary>
        [JsonProperty("known_for")]
        public KnownForList? KnownFor { get; set; }

        public string KnownForDescription
        {
            get
            {
                if (string.IsNullOrEmpty(knownForDescription) && KnownFor != null)
                {
                    knownForDescription = KnownFor.DescriptionList;
                }

                return knownForDescription;
            }
            private set => knownForDescription = value;
        }

        /// <summary>
        /// Gets or sets the KnownForDepartment
        /// </summary>
        [JsonProperty("known_for_department")]
        public string KnownForDepartment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the PersonId
        /// </summary>
        [JsonProperty("id")]
        public string PersonId { get; set; } = string.Empty;

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; } = string.Empty;

        [JsonProperty("place_of_birth")]
        public string PlaceOfBirth { get; set; } = string.Empty;

        [JsonProperty("birthday")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("deathday")]
        public DateTime DateOfDeath { get; set; }

        [JsonProperty("also_known_as")]
        public string[]? AlsoKnownAs { get; set; }

        /// <summary>
        /// Gets or sets the adult
        /// </summary>
        [JsonProperty("adult")]
        private string adult { get; set; } = string.Empty;

        #endregion
    }



    /// <summary>
    /// Defines the <see cref="ProductionCompanyMovieList" />
    /// </summary>
    //public class ProductionCompanyMovieList : List<ProductionCompanyMovie>
    //{
    //    #region Fields

    //    /// <summary>
    //    /// Defines the companyMovieRows
    //    /// </summary>
    //    private SupportXSD.ProductionCompanyMovieDataTable companyMovieRows = null;

    //    #endregion

    //    #region Constructors

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ProductionCompanyMovieList"/> class.
    //    /// </summary>
    //    public ProductionCompanyMovieList()
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ProductionCompanyMovieList"/> class.
    //    /// </summary>
    //    /// <param name="id">The id<see cref="int"/></param>
    //    /// <param name="loadBy">The loadBy<see cref="ProductionCompanyList.LoadBy"/></param>
    //    public ProductionCompanyMovieList(int id, ProductionCompanyList.LoadBy loadBy)
    //    {
    //        if (loadBy == ProductionCompanyList.LoadBy.Company)
    //        {
    //            companyMovieRows = DataAdapters.ProductionCompanyMovieAdapter.GetDataByCompanyId(id);
    //        }
    //        else if (loadBy == ProductionCompanyList.LoadBy.Movie)
    //        {
    //            companyMovieRows = DataAdapters.ProductionCompanyMovieAdapter.GetDataByMovieId(id);
    //        }

    //        if (companyMovieRows != null)
    //        {
    //            foreach (SupportXSD.ProductionCompanyMovieRow item in companyMovieRows)
    //            {
    //                ProductionCompanyMovie productionCompanyMovie = new ProductionCompanyMovie(item);
    //                Add(productionCompanyMovie);
    //            }
    //        }
    //    }

    //    #endregion
    //}

    /// <summary>
    /// Defines the <see cref="Season" />
    /// </summary>
    public class Season
    {
        #region Fields

        /// <summary>
        /// The air date
        /// </summary>
        private DateTime airDate;

        /// <summary>
        /// The episode count
        /// </summary>
        private int episodeCount;

        /// <summary>
        /// The name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The overview
        /// </summary>
        private string overview = string.Empty;

        /// <summary>
        /// The poster path
        /// </summary>
        private string posterPath = string.Empty;

        /// <summary>
        /// The season no
        /// </summary>
        private int seasonNo;

        /// <summary>
        /// The t mid
        /// </summary>
        private int tMID;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the air date.
        /// </summary>
        [JsonProperty("air_date")]
        public DateTime AirDate { get => airDate; set => airDate = value; }

        /// <summary>
        /// Gets or sets the episode count.
        /// </summary>
        [JsonProperty("episode_count")]
        public int EpisodeCount { get => episodeCount; set => episodeCount = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public string Overview { get => overview; set => overview = value; }

        /// <summary>
        /// Gets or sets the poster path.
        /// </summary>
        [JsonProperty("poster_path")]
        public string PosterPath { get => posterPath; set => posterPath = value; }

        /// <summary>
        /// Gets or sets the season no.
        /// </summary>
        [JsonProperty("season_number")]
        public int SeasonNo { get => seasonNo; set => seasonNo = value; }

        /// <summary>
        /// Gets or sets the TMID
        /// </summary>
        [JsonProperty("id")]
        public int TMID { get => tMID; set => tMID = value; }


        #endregion

        public static implicit operator Models.Season(Season v)
        {
            if (v == null)
            {
                return null;
            }

            Models.Season result = new Models.Season
            {
                Name = v.Name,
                Description = v.Overview,
                Path = v.PosterPath,
                SeasonNo = v.SeasonNo,
                TMDBID = v.TMID
            };

            if (v.AirDate != DateTime.MinValue)
            {
                result.Year = v.AirDate.Year;
            }

            return result;
        }
    }

    /// <summary>
    /// Defines the <see cref="SeasonList" />
    /// </summary>
    public class SeasonList : List<Season>
    {
        public void Refresh()
        {

        }

        internal List<Models.Season>? GetSeasons()
        {
            List<Models.Season> returnList = new();
            foreach (var item in this)
            {
                returnList.Add(item);
            }

            return returnList;

        }


    }

    /// <summary>
    /// Defines the <see cref="StreamDetails" />
    /// </summary>
    public class StreamDetails
    {
        #region Fields

        /// <summary>
        /// Defines the audioDetails
        /// </summary>
        private Audio? audioDetails = null;

        /// <summary>
        /// Defines the videoDetails
        /// </summary>
        private Video? videoDetails = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the audio details.
        /// </summary>
        public Audio AudioDetails
        {
            get
            {
                if (audioDetails == null)
                {
                    audioDetails = new Audio();
                }

                return audioDetails;
            }
            set => audioDetails = value;
        }

        /// <summary>
        /// Gets or sets the video details.
        /// </summary>
        public Video VideoDetails
        {
            get
            {
                if (videoDetails == null)
                {
                    videoDetails = new Video();
                }

                return videoDetails;
            }
            set => videoDetails = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to xml.
        /// </summary>
        /// <returns></returns>
        public XElement ToXML()
        {
            XElement returnXML = new XElement("streamdetails");
            returnXML.Add(new XElement("video", VideoDetails));
            returnXML.Add(new XElement("audio", AudioDetails));
            return returnXML;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Tag" />
    /// </summary>
    public class Tag
    {
        #region Fields

        /// <summary>
        /// Defines the tagItem
        /// </summary>
        private string tagItem = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        /// <param name="tagItem">The tagItem<see cref="string"/></param>
        public Tag(string tagItem)
        {
            this.tagItem = tagItem;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the TagItem
        /// </summary>
        public string TagItem { get => tagItem; set => tagItem = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="TagList" />
    /// </summary>
    public class TagList : List<Tag>
    {
        #region Methods

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            string returnValue = string.Empty;

            foreach (Tag item in this)
            {
                if (returnValue != string.Empty)
                {
                    returnValue += ",";
                }

                returnValue += item.TagItem;
            }

            return returnValue;
        }

        /// <summary>
        /// The ToXml
        /// </summary>
        /// <param name="xml">The xml<see cref="XElement"/></param>
        /// <returns>The <see cref="XElement"/></returns>
        public XElement ToXml(XElement xml)
        {
            foreach (Tag item in this)
            {
                xml.Add(new XElement("tag", item.TagItem));
            }
            return xml;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="TVShow" />
    /// </summary>
    public class TVShow
    {
        #region Fields

        /// <summary>
        /// Defines the backdropPath
        /// </summary>
        private string backdropPath = string.Empty;

        /// <summary>
        /// Defines the credits
        /// </summary>
        private CastList? credits;

        /// <summary>
        /// Defines the firstAirDate
        /// </summary>
        private DateTime? firstAirDate;

        /// <summary>
        /// Defines the genres
        /// </summary>
        private GenreList? genres;

        /// <summary>
        /// Defines the homePage
        /// </summary>
        private string homePage = string.Empty;

        /// <summary>
        /// Defines the inProduction
        /// </summary>
        private bool inProduction;

        /// <summary>
        /// Defines the languages
        /// </summary>
        private string[]? languages;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Defines the noOfEpisodes
        /// </summary>
        private int noOfEpisodes;

        /// <summary>
        /// Defines the noOfSeasons
        /// </summary>
        private int noOfSeasons;

        /// <summary>
        /// Defines the originalLanguage
        /// </summary>
        private string originalLanguage = string.Empty;

        /// <summary>
        /// Defines the originalName
        /// </summary>
        private string originalName = string.Empty;

        /// <summary>
        /// Defines the originCountries
        /// </summary>
        private string[]? originCountries;

        /// <summary>
        /// Defines the overview
        /// </summary>
        private string overview = string.Empty;

        /// <summary>
        /// Defines the productionCompanies
        /// </summary>
        //private ProductionCompanyList productionCompanies;

        /// <summary>
        /// Defines the showID
        /// </summary>
        private int showID = -1;

        /// <summary>
        /// The seasons
        /// </summary>
        /// <autogeneratedoc />
        private SeasonList? seasons;

        /// <summary>
        /// The status
        /// </summary>
        /// <autogeneratedoc />
        private string status = string.Empty;

        /// <summary>
        /// The type
        /// </summary>
        /// <autogeneratedoc />
        private string type = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TVShow"/> class.
        /// </summary>
        public TVShow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TVShow"/> class.
        /// </summary>
        /// <param name="json">The json<see cref="string"/></param>
        public TVShow(string json)
        {
            try
            {

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                TVShow? tvShow = JsonConvert.DeserializeObject<TVShow>(json, settings);
                if (tvShow != null)
                {
                    backdropPath = tvShow.backdropPath;
                    Credits = tvShow.Credits;
                    FirstAirDate = tvShow.FirstAirDate;
                    Genres = tvShow.Genres;
                    HomePage = tvShow.HomePage;
                    ShowID = tvShow.ShowID;
                    inProduction = tvShow.InProduction;
                    //this.Languages = tvShow.Languages;
                    Name = tvShow.Name;
                    OriginalName = tvShow.OriginalName;
                    OriginalLanguage = tvShow.OriginalLanguage;
                    OriginCountries = tvShow.OriginCountries;
                    Overview = tvShow.Overview;
                    //ProductionCompanies = tvShow.ProductionCompanies;
                    ShowID = tvShow.ShowID;
                    noOfEpisodes = tvShow.NoOfEpisodes;
                    NoOfSeasons = tvShow.NoOfSeasons;
                    Seasons = tvShow.Seasons;
                    Status = tvShow.Status;
                    Type = tvShow.Type;
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BackdropPath
        /// </summary>
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get => backdropPath; set => backdropPath = value; }

        /// <summary>
        /// Gets or sets the Credits
        /// </summary>
        [JsonProperty("created_by")]
        public CastList? Credits { get => credits; set => credits = value; }

        /// <summary>
        /// Gets or sets the FirstAirDate
        /// </summary>
        [JsonProperty("first_air_date")]
        public DateTime? FirstAirDate { get => firstAirDate; set => firstAirDate = value; }

        /// <summary>
        /// Gets or sets the Genres
        /// </summary>
        [JsonProperty("genres")]
        public GenreList? Genres { get => genres; set => genres = value; }

        /// <summary>
        /// Gets or sets the HomePage
        /// </summary>
        [JsonProperty("homepage")]
        public string HomePage { get => homePage; set => homePage = value; }

        /// <summary>
        /// Gets or sets a value indicating whether InProduction
        /// </summary>
        [JsonProperty("in_production")]
        public bool InProduction { get => inProduction; set => inProduction = value; }

        /// <summary>
        /// Gets or sets the Languages
        /// </summary>
        [JsonProperty("languages")]
        public string[] Languages { get => languages; set => languages = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets or sets the NoOfEpisodes
        /// </summary>
        [JsonProperty("number_of_episodes")]
        public int NoOfEpisodes { get => noOfEpisodes; set => noOfEpisodes = value; }

        /// <summary>
        /// Gets or sets the NoOfSeasons
        /// </summary>
        [JsonProperty("number_of_seasons")]
        public int NoOfSeasons { get => noOfSeasons; set => noOfSeasons = value; }


        /// <summary>
        /// Gets or sets the OriginalLanguage
        /// </summary>
        [JsonProperty("original_language")]
        public string OriginalLanguage { get => originalLanguage; set => originalLanguage = value; }

        /// <summary>
        /// Gets or sets the OriginalName
        /// </summary>
        [JsonProperty("original_name")]
        public string OriginalName { get => originalName; set => originalName = value; }

        /// <summary>
        /// Gets or sets the OriginCountries
        /// </summary>
        [JsonProperty("origin_country")]
        public string[]? OriginCountries { get => originCountries; set => originCountries = value; }

        /// <summary>
        /// Gets or sets the Overview
        /// </summary>
        [JsonProperty("overview")]
        public string Overview { get => overview; set => overview = value; }

        /// <summary>
        /// Gets or sets the ProductionCompanies
        /// </summary>
        //[JsonProperty("production_companies")]
        //public ProductionCompanyList ProductionCompanies { get => productionCompanies; set => productionCompanies = value; }

        /// <summary>
        /// Gets or sets the ShowID
        /// </summary>
        [JsonProperty("id")]
        public int ShowID { get => showID; set => showID = value; }

        /// <summary>
        /// Gets or sets the seasons.
        /// </summary>
        /// <value>
        /// The seasons.
        /// </value>
        /// <autogeneratedoc />
        [JsonProperty("seasons")]
        public SeasonList Seasons { get => seasons; set => seasons = value; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        /// <autogeneratedoc />
        [JsonProperty("status")]
        public string Status { get => status; set => status = value; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        /// <autogeneratedoc />
        [JsonProperty("type")]
        public string Type { get => type; set => type = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="Video" />
    /// </summary>
    public class Video
    {
        #region Fields

        /// <summary>
        /// Defines the aspect
        /// </summary>
        private double aspect;

        /// <summary>
        /// Defines the codec
        /// </summary>
        private string codec = string.Empty;

        /// <summary>
        /// Defines the durationinseconds
        /// </summary>
        private int durationinseconds;

        /// <summary>
        /// Defines the height
        /// </summary>
        private int height;

        /// <summary>
        /// Defines the width
        /// </summary>
        private int width;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Aspect
        /// </summary>
        public double Aspect { get => aspect; set => aspect = value; }

        /// <summary>
        /// Gets or sets the Codec
        /// </summary>
        public string Codec { get => codec; set => codec = value; }

        /// <summary>
        /// Gets or sets the Durationinseconds
        /// </summary>
        public int Durationinseconds { get => durationinseconds; set => durationinseconds = value; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public int Height { get => height; set => height = value; }

        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        public int Width { get => width; set => width = value; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to xml.
        /// </summary>
        /// <returns></returns>
        public XElement ToXML()
        {
            XElement returnXML = new XElement("video");
            returnXML.Add(new XElement("aspect", aspect));
            returnXML.Add(new XElement("codec", codec));
            returnXML.Add(new XElement("durationinseconds", durationinseconds));
            returnXML.Add(new XElement("height", height));
            returnXML.Add(new XElement("width", width));

            return returnXML;
        }

        #endregion
    }
}
