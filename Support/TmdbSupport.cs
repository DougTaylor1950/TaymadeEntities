//-----------------------------------------------------------------------
// <copyright file="TmdbSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>11/04/2019 12:37:48 11/04/2019 12:37:48 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TmdbSupport" />
    /// </summary>
    public class TmdbSupport
    {
        #region Constants

        /// <summary>
        /// Defines the ApiKey
        /// </summary>
        public const string ApiKey = "b2ec1f95f2b5fc3da1721329af71e35e";

        public const string OmdbApiKey = "a4b62404";

        public const string OmdbApiUrl = "http://www.omdbapi.com/";

        /// <summary>
        /// Defines the ImageURL
        /// </summary>
        public const string ImageURL = "https://image.tmdb.org/t/p/w500";

        public static JsonSerializerSettings StandardSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static JsonLoadSettings StandardLoadSettings = new JsonLoadSettings();
       
        #endregion

        #region Enumeration

        public enum Database
        {
            IMDB,
            Omdb,
            MovieDb
        }
        #endregion

        #region Methods

        public static string GetCredits(string creditId)
        {
            string searchUrl = "https://api.themoviedb.org/3/credit/" + creditId.Trim() + "?api_key=" + ApiKey + "&language=en-US";

            string returnJSON = CallWebClient(searchUrl);
            return returnJSON;
        }

        public async static Task<List<Person>>? GetPeopleListAsync(string name)
        {
            List<Person>? people = null;
            string searchUrl = "https://api.themoviedb.org/3/search/person?api_key=" + ApiKey + "&language=en-US&query=" + name.Replace(" ", "%20") + "&page=1&include_adult=true";

            string? returnJSON = await CallWebClientAsync(searchUrl);

            if (returnJSON.Contains("results\":"))
            {
                JObject jObject = JObject.Parse(returnJSON);
                var results = jObject["results"];

                try
                {
                    if (results != null)
                        people = JsonConvert.DeserializeObject<List<Person>>(results.ToString());
                    //if (people != null && people.Count > 0) person = people[0];
                    if (people != null)
                    {
                        people = people.OrderByDescending(a => a.Adult).ThenBy(a => a.Name).ToList();
                    }

                }
                catch (System.Exception e)
                {
                    string error = e.ToString();
                    //throw;
                }
            }

            people ??= new List<Person>();

            Person newPerson = new()
            {
                Name = name,
                PersonId = ""

            };

            people.Add(newPerson);

            return people;

        }

        public static List<Person>? GetPeopleList(string name)
        {
            List<Person>? people = null;
            string searchUrl = "https://api.themoviedb.org/3/search/person?api_key=" + ApiKey + "&language=en-US&query=" + name.Replace(" ", "%20") + "&page=1&include_adult=true";

            string? returnJSON = CallWebClient(searchUrl);

            if (returnJSON.Contains("results\":"))
            {
                JObject jObject = JObject.Parse(returnJSON);
                var results = jObject["results"];

                try
                {
                    if (results != null)
                        people = JsonConvert.DeserializeObject<List<Person>>(results.ToString());
                    //if (people != null && people.Count > 0) person = people[0];


                }
                catch (System.Exception e)
                {
                    string error = e.ToString();
                    //throw;
                }
            }

            people ??= new List<Person>();

            Person newPerson = new()
            {
                Name = name,
                PersonId = ""

            };

            people.Add(newPerson);

            return people;

        }

        public static Person GetPerson(string name)
        {
            Person? person = null;
            string searchUrl = "https://api.themoviedb.org/3/search/person?api_key=" + ApiKey + "&language=en-US&query=" + name.Replace(" ", "%20") + "&page=1&include_adult=true";

            string returnJSON = CallWebClient(searchUrl);

            if (returnJSON.Contains("results\":"))
            {
                JObject jObject = JObject.Parse(returnJSON);
                var results = jObject["results"];

                try
                {
                    if (results != null)
                    {
                        List<Person>? people = JsonConvert.DeserializeObject<List<Person>>(results.ToString());
                        if (people != null && people.Count > 0) person = people[0];
                    }


                }
                catch (System.Exception e)
                {
                    string error = e.ToString();
                    //throw;
                }
            }

            return person;

        }

        public static Credit GetCredit(string creditId)
        {
            Credit? credit = null;

            string json = GetCredits(creditId);
            credit = JsonConvert.DeserializeObject<Credit>(json);

            return credit;
        }



        /// <summary>
        /// The GetActor
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetActor(int id)
        {
            string searchUrl = "https://api.themoviedb.org/3/person/" + id.ToString().Trim() + "?api_key=" + ApiKey + "&language=en-US";

            string returnJSON = CallWebClient(searchUrl);
            return returnJSON;
        }

        public async static Task<string?> GetActorAsync(int id)
        {
            string searchUrl = "https://api.themoviedb.org/3/person/" + id.ToString().Trim() + "?api_key=" + ApiKey + "&language=en-US";

            string? returnJSON = await CallWebClientAsync(searchUrl);
            return returnJSON;
        }

        public async static Task<string?> GetActorAsync(string id)
        {
            string searchUrl = "https://api.themoviedb.org/3/person/" + id.Trim() + "?api_key=" + ApiKey + "&language=en-US";

            string? returnJSON = await CallWebClientAsync(searchUrl);
            return returnJSON;
        }

        public async static Task<Person?> GetPersonDetailsAsync(string id)
        {
            string? returnJSON = await GetActorAsync(id);

            Person? person = null;
            if (!string.IsNullOrEmpty(returnJSON))
            {
                person = JsonConvert.DeserializeObject<Person>(returnJSON, TmdbSupport.StandardSettings);
            }
            return person;
        }

        public static string GetActorExternalIds(int id)
        {
            string searchUrl = "https://api.themoviedb.org/3/person/" + id.ToString().Trim() + "/external_ids?api_key=" + ApiKey + "&language=en-US";

            string returnJSON = CallWebClient(searchUrl);
            return returnJSON;
        }

        /// <summary>
        /// The GetCredits
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="CastList"/></returns>
        public static CastList GetMovieCredits(int id)
        {
            CastList? castMembers = [];

            string searchUrl = "https://api.themoviedb.org/3/movie/" + id.ToString().Trim() + "/credits?api_key=" + ApiKey + "&language=en-US";

            string returnJSON = CallWebClient(searchUrl);

            if (returnJSON != string.Empty)
            {
                var jObject = JsonConvert.DeserializeObject<List<Credit>>(returnJSON,StandardSettings);
                //var castList = jObject["cast"];

                //if (castList != null)
                //    castMembers = JsonConvert.DeserializeObject<CastList>(castList.ToString());

                //var crewlist = jObject["crew"];

                //if (crewlist != null)
                //{
                //    CastList? crewMembers = JsonConvert.DeserializeObject<CastList>(crewlist.ToString());

                //    if (crewMembers != null && castMembers != null)
                //    {
                //        CastMember? director = crewMembers.Find(x => x.Department == "Directing");
                //        if (director != null)
                //        {
                //            director.IsDirector = true;
                //            castMembers.Add(director);
                //        }
                //    }
                //}
            }
            return castMembers;
        }

        public static async Task<CastList> GetMovieCreditsAsync(int id)
        {
            CastList? castMembers = [];

            string searchUrl = "https://api.themoviedb.org/3/movie/" + id.ToString().Trim() + "/credits?api_key=" + ApiKey + "&language=en-US";

            string returnJSON = await CallWebClientAsync(searchUrl);

            if (returnJSON != string.Empty)
            {
                var jObject = JObject.Parse(returnJSON);

                //var jObject = JsonConvert.DeserializeObject<Credit>(returnJSON, StandardSettings);
                var castList = jObject["cast"];

                if (castList != null)
                    castMembers = JsonConvert.DeserializeObject<CastList>(castList.ToString());

                var crewlist = jObject["crew"];

                if (crewlist != null)
                {
                    CastList? crewMembers = JsonConvert.DeserializeObject<CastList>(crewlist.ToString());

                    if (crewMembers != null && castMembers != null)
                    {
                        CastMember? director = crewMembers.Find(x => x.Department == "Directing");
                        if (director != null)
                        {
                            director.IsDirector = true;
                            castMembers.Add(director);
                        }
                    }
                }
            }
            return castMembers;
        }

        private static GenreList? genres = null;

        /// <summary>
        /// The GetGenres
        /// </summary>
        /// <returns>The <see cref="GenreList"/></returns>
        public static GenreList GetGenres()
        {
            if (genres == null)
            {
                genres = [];

                string searchUrl = "https://api.themoviedb.org/3/genre/movie/list?api_key=" + ApiKey + "&language=en-US";

                string returnJSON = CallWebClient(searchUrl);

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(returnJSON);
                var genrelist = jObject["genres"];

                if (genrelist != null)
                    genres = JsonConvert.DeserializeObject<GenreList>(genrelist.ToString());
            }

            return genres;
        }

        /// <summary>
        /// The GetJsonValue
        /// </summary>
        /// <param name="jObject">The jObject<see cref="Newtonsoft.Json.Linq.JObject"/></param>
        /// <param name="field">The field<see cref="string"/></param>
        /// <param name="defaultValue">The defaultValue<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetJsonValue(Newtonsoft.Json.Linq.JObject jObject, string field, string defaultValue = "")
        {
            string? returnValue = string.Empty;
            object? temp = jObject.GetValue(field);
            if (temp != null)
            {
                returnValue = temp.ToString();
            }
            else
            {
                returnValue = defaultValue;
            }

            return returnValue;
        }

        /// <summary>
        /// The GetMovieDBJson
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static async Task<string> GetMovieDBJsonAsync(int id, string ImdbId = "", Database database = Database.MovieDb)
        {
            string returnJSON = string.Empty;

            string searchUrl = "https://api.themoviedb.org/3/movie/";

            if (database == Database.MovieDb)
            {
                searchUrl += id.ToString().Trim() + "?api_key=" + ApiKey + "&language=en-US";
            }
            else
            {
                searchUrl = OmdbApiUrl + "/?i=" + ImdbId + "&apikey=" + OmdbApiKey;
            }

            HttpClient client = new();
            Stream stream = await client.GetStreamAsync(searchUrl);

            StreamReader reader = new(stream);
            returnJSON = reader.ReadToEnd();

            stream.Flush();
            stream.Close();
            client.Dispose();

            return returnJSON;
        }

        public static Person GetPerson(int id)
        {
            string json = GetActor(id);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            Person? person = JsonConvert.DeserializeObject<Person>(json, settings);

            return person;
        }

        public async static Task<Person?> GetPersonAsync(int id)
        {
            string? json = await GetActorAsync(id);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            Person? person = JsonConvert.DeserializeObject<Person>(json, settings);

            return person;
        }

        public static iMovie GetMovieFromIMDB(string imdb)
        {
            iMovie? movie = null;

            string searchUrl = "https://api.themoviedb.org/3/find/" + imdb + "?api_key=" + ApiKey + "&language=en-US&external_source=imdb_id";

            string returnJSON = CallWebClient(searchUrl);

            if (returnJSON.Contains("movie_results\":"))
            {
                JObject jObject = JObject.Parse(returnJSON);
                var results = jObject["movie_results"];

                if (results != null)
                {
                    MovieList? movies = JsonConvert.DeserializeObject<MovieList>(results.ToString());

                    if (movies != null && movies.Count > 0)
                    {
                        movie = movies[0];
                    }
                }
            }

            return movie;
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ImdbId">The imdb identifier.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        /// <author>
        /// doug
        /// </author>
        /// <remarks>
        ///   <created>21/06/2019 21:55</created>
        /// </remarks>
        /// <autogeneratedoc />
        public async static Task<iMovie> GetMovieData(int id, string ImdbId = "", Database database = Database.MovieDb)
        {
            iMovie? movie = null;

            jObject = await MovieItem.GetJObject(id);

            string json = await GetMovieDBJsonAsync(id, ImdbId, database);

            if (database == Database.MovieDb)
            {
                movie = JsonConvert.DeserializeObject<MovieItem>(json);
            }
            else if (database == Database.Omdb)
            {
                // movie = JsonConvert.DeserializeObject<OMDBMovie>(json);
            }

            return movie;
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <author>
        /// doug
        /// </author>
        /// <remarks>
        ///   <created>22/06/2019 11:22</created>
        /// </remarks>
        /// <autogeneratedoc />
        public async static Task<GenreList> GetGenres(int id)
        {
            GenreList? genres = null;

            string nfoData = await GetMovieDBJsonAsync(id);
            if (nfoData != string.Empty)
            {
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(nfoData);
                genres = GetGenres(jObject);

            }

            return genres;
        }

        public static GenreList GetGenres(JObject jObject)
        {
            GenreList? genres = null;
            var genrelist = jObject["genres"];
            if (genrelist != null)
                genres = JsonConvert.DeserializeObject<GenreList>(genrelist.ToString());
            return genres;
        }

        //public static NfoData GetMovieDBNFOData(string id)
        //{
        //    NfoData returnValue = null;

        //    iMovie iMovie = GetMovieData(0, id, Database.Omdb);

        //    returnValue = new NfoData(iMovie);

        //    return returnValue;
        //}

        /// <summary>
        /// The GetMovieDBNFOData
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="NfoData"/></returns>
        public async static Task<NfoData> GetMovieDBNFOData(int id)
        {
            NfoData returnValue = null;

            string nfoData = await GetMovieDBJsonAsync(id);

            if (nfoData != string.Empty)
            {
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(nfoData);
                if (jObject != null)
                {
                    returnValue = new NfoData();
                    returnValue.TMDBid = id;
                    returnValue.Title = GetJsonValue(jObject, "title");
                    returnValue.OriginalTitle = GetJsonValue(jObject, "original_title");
                    returnValue.OriginalLanguage = GetJsonValue(jObject, "original_language");
                    returnValue.Year = GetJsonValue(jObject, "release_date");

                    string temp = GetJsonValue(jObject, "video");
                    if (temp == "true")
                    {
                        returnValue.Type = "Video";
                    }
                    else
                    {
                        returnValue.Type = "Movie";
                    }

                    returnValue.Imdbid = GetJsonValue(jObject, "imdb_id");
                    returnValue.Rated = GetJsonValue(jObject, "popularity");
                    returnValue.Released = GetJsonValue(jObject, "release_date");
                    returnValue.Runtime = GetJsonValue(jObject, "runtime");
                    returnValue.Plot = GetJsonValue(jObject, "overview");
                    returnValue.Poster_Path = GetJsonValue(jObject, "poster_path");

                    bool tempBool = false;
                    bool.TryParse(GetJsonValue(jObject, "adult"), out tempBool);

                    returnValue.Adult = tempBool;

                    //var genrelist = jObject["genres"];
                    returnValue.Genres = GetGenres(jObject);

                    List<ProductionCompany> companies = GetProductionCompanies();

                    returnValue.Companies = companies;
                    CountryList countries = GetProductionCountries();

                    returnValue.Countries = countries;
                    LanguageList spokenLanguages = GetLanguages();

                    returnValue.Languages = spokenLanguages;

                }
            }

            return returnValue;
        }

        public static LanguageList? GetLanguages()
        {
            LanguageList? spokenLanguages = null;
            if (jObject != null)
            {
                var languages = jObject["spoken_languages"];
                if (languages != null)
                {
                    spokenLanguages = JsonConvert.DeserializeObject<LanguageList>(languages.ToString());

                }
            }
            return spokenLanguages;

        }

        public static CountryList GetProductionCountries()
        {
            var productionCountries = jObject["production_countries"];
            if (productionCountries != null)
            {
                CountryList? countries = JsonConvert.DeserializeObject<CountryList>(productionCountries.ToString());
                return countries;
            }
            else return null;
        }

        public static JObject? jObject { get; set; }

        public static List<ProductionCompany> GetProductionCompanies()
        {
            List<ProductionCompany>? companies = null;
            if (jObject != null)
            {
                var productionCompanies = jObject["production_companies"];
                companies = JsonConvert.DeserializeObject<List<ProductionCompany>>(productionCompanies.ToString());
            }
            return companies;
        }

        /// <summary>
        /// </summary>
        /// <param name="company">The company.</param>
        /// <returns></returns>
        /// <author>
        /// doug
        /// </author>
        /// <remarks>
        ///   <created>19/08/2019 15:13</created>
        /// </remarks>
        /// <autogeneratedoc />
        //public static ProductionCompanyList SearchCompany(string company)
        //{
        //    ProductionCompanyList companies = new ProductionCompanyList();

        //    string searchUrl = "https://api.themoviedb.org/3/search/company?api_key=" + ApiKey + "&query=" + company.Replace(" ", "%20");

        //    string returnJSON = CallWebClient(searchUrl);

        //    if (returnJSON.Contains("results\":"))
        //    {
        //        JObject jObject = JObject.Parse(returnJSON);
        //        var results = jObject["results"];


        //        companies = JsonConvert.DeserializeObject<ProductionCompanyList>(results.ToString());
        //    }
        //    return companies;
        //}


        /// <summary>
        /// The SearchActor
        /// </summary>
        /// <param name="name">The name<see cref="string"/></param>
        /// <returns>The <see cref="CastList"/></returns>
        public static CastList SearchActor(string name)
        {
            CastList? castMembers = null;
            string searchUrl = "https://api.themoviedb.org/3/search/person/?api_key=" + ApiKey + "&language=en-US&query="
                + name.Replace(" ", "%20") + "&page=1&include_adult=true";

            string returnJSON = CallWebClient(searchUrl);

            if (returnJSON.Contains("results\":"))
            {
                JObject jObject = JObject.Parse(returnJSON);
                var results = jObject["results"];

                if (results != null)
                    castMembers = JsonConvert.DeserializeObject<CastList>(results.ToString());
            }


            return castMembers;
        }

        public static string GetTVSeason(int tvID, int season)
        {
            string returnJSON = string.Empty;

            string searchUrl = string.Empty;

            searchUrl = "https://api.themoviedb.org/3/tv/" + tvID.ToString().Trim()
                + "/season/" + season.ToString().Trim() +
                "?api_key=" + ApiKey + "&language=en-US";


            //searchUrl += "&query=" + title.Replace(" ", "+");
            returnJSON = CallWebClient(searchUrl);

            return returnJSON;
        }

        public static string GetTVEpisode(int tvID, int season, int episode)
        {
            string returnJSON = string.Empty;

            string searchUrl = string.Empty;

            searchUrl = "https://api.themoviedb.org/3/tv/" + tvID.ToString().Trim()
                + "/season/" + season.ToString().Trim() + "/episode/" + episode.ToString().Trim() +
                "?api_key=" + ApiKey + "&language=en-US";
            returnJSON = CallWebClient(searchUrl);

            return returnJSON;
        }

        /// <summary>
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns></returns>
        /// <author>
        /// doug
        /// </author>
        /// <remarks>
        ///   <created>21/09/2019 12:41</created>
        /// </remarks>
        /// <autogeneratedoc />
        public static string GetTVShow(int showId)
        {
            string json = string.Empty;

            string searchUrl = string.Empty;

            searchUrl = "https://api.themoviedb.org/3/tv/" + showId.ToString().Trim()
                + "?api_key=" + ApiKey + "&language=en-US";

            //searchUrl += "&query=" + title.Replace(" ", "+");
            json = CallWebClient(searchUrl);

            return json;
        }

        public static TVShow GetTVShowDetails(int showID)
        {
            TVShow? show = null;

            string json = GetTVShow(showID);

            if (!string.IsNullOrEmpty(json))
            {
                show = JsonConvert.DeserializeObject<TVShow>(json);
            }
            return show;
        }

        public static EpisodeDetailsList GetTVEpisodes(int tvID, int season)
        {
            EpisodeDetailsList? episodeDetails = null;

            string json = GetTVSeason(tvID, season);

            JObject root = JObject.Parse(json);

            object? episodes = root["episodes"];

            if (episodes != null)
            {
                string? episodelist = episodes.ToString();
                if (!string.IsNullOrEmpty(episodelist))
                {
                    var mdetails = JsonConvert.DeserializeObject<List<EpisodeDetails>>(episodelist);

                    episodeDetails = [];

                    if (mdetails != null)
                        foreach (EpisodeDetails item in mdetails)
                        {
                            episodeDetails.Add(item);
                        }
                }
            }
            return episodeDetails;
        }

        public static EpisodeDetails GetTVEpisodeDetails(int tvId, int season, int episode)
        {
            EpisodeDetails? episodeDetails = null;

            string json = GetTVEpisode(tvId, season, episode);

            if (!string.IsNullOrEmpty(json))
            {

                JObject root = JObject.Parse(json);

                var mdetails = JsonConvert.DeserializeObject<EpisodeDetails>(root.ToString());

                episodeDetails = mdetails;
            }

            return episodeDetails;
        }

        /// <summary>
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        /// <author>
        /// doug
        /// </author>
        /// <remarks>
        ///   <created>20/09/2019 19:50</created>
        /// </remarks>
        /// <autogeneratedoc />
        public static EpisodeDetailsList GetTVEpisodes(string json)
        {
            EpisodeDetailsList? episodeDetails = null;

            if (json != string.Empty)
            {
                JObject root = JObject.Parse(json);

                object? episodes = root["episodes"];

                if (episodes != null && !string.IsNullOrEmpty(episodes.ToString()))
                {
                    string? episodeDetailslist = episodes.ToString();

                    if (!string.IsNullOrEmpty(episodeDetailslist))
                    {
                        List<EpisodeDetails>? mdetails = JsonConvert.DeserializeObject<List<EpisodeDetails>>(episodeDetailslist);

                        episodeDetails = [];

                        if (mdetails != null)
                            foreach (EpisodeDetails item in mdetails)
                            {
                                episodeDetails.Add(item);
                            }
                    }
                }
            }

            return episodeDetails;
        }

        public static string SearchTV(string title, int page = 1)
        {
            string returnJSON = string.Empty;

            string searchUrl = string.Empty;

            searchUrl = "https://api.themoviedb.org/3/search/tv?api_key=" + ApiKey + "&language=en-US&include_adult=true&page=" +
                   page.ToString().Trim();


            searchUrl += "&query=" + title.Replace(" ", "+");
            returnJSON = CallWebClient(searchUrl);

            return returnJSON;
        }

        /// <summary>
        /// The SearchMovieDatabase
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <param name="page">The page<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string SearchMovieDatabase(string title, int page = 1, Database database = Database.MovieDb)
        {
            string returnJSON = string.Empty;


            string searchUrl = string.Empty;
            if (database == Database.MovieDb)
            {
                searchUrl = "https://api.themoviedb.org/3/search/movie?api_key=" + ApiKey + "&language=en-US&page=" +
                    page.ToString().Trim() +
                    "&include_adult=true";


                searchUrl += "&query=" + title.Replace(" ", "+");
            }
            else if (database == Database.Omdb)
            {
                searchUrl = OmdbApiUrl + "?apikey=" + OmdbApiKey + "&s=" + title.Replace(" ", "+") + "&Page=" + page.ToString().Trim();
            }

            returnJSON = CallWebClient(searchUrl);

            return returnJSON;
        }

        public static List<TVShow> SearchTVList(string title, int pagein = 1)
        {
            List<TVShow>? nfoDataList = null;


            int page = pagein;
            string json = TmdbSupport.SearchTV(title, page);

            JObject root = JObject.Parse(json);
            int? totalresults = 0;
            object? results = null;
            object? totalResults = null;


            totalResults = root["total_results"];
            results = root["results"];


            if (totalResults != null)
            {
                string? total = totalResults.ToString();
                if (!string.IsNullOrEmpty(total))
                    totalresults = int.Parse(total);

            }

            string? resultList = results.ToString();

            if (!string.IsNullOrEmpty(resultList))
            {
                var mList = JsonConvert.DeserializeObject<List<TVShow>>(resultList);

                if (mList != null)
                    foreach (TVShow item in mList)
                    {
                        nfoDataList.Add(item);
                    }
            }

            return nfoDataList;

        }

        /// <summary>
        /// The SearchMovieDatabaseList
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <returns>The <see cref="List{NfoData}"/></returns>
        public static List<MovieBase> SearchMovieDatabaseList(string title, int pagein = 1, Database database = Database.MovieDb)
        {
            List<MovieBase>? nfoDataList = null;

            int page = pagein;

            string json = TmdbSupport.SearchMovieDatabase(title, page, database);
            JObject root = JObject.Parse(json);
            int? totalresults = 0;
            object? results = null;
            object? totalResults = null;

            if (database == Database.MovieDb)
            {
                totalResults = root["total_results"];
                results = root["results"];
            }
            else if (database == Database.Omdb)
            {
                results = root["Search"];
                totalResults = root["totalResults"];

            }

            if (totalResults != null)
            {
                string? total = totalResults.ToString();
                if (!string.IsNullOrEmpty(total))
                    totalresults = int.Parse(total);

            }


            nfoDataList = [];
            MoveToSearchList(database, nfoDataList, results);

            //nfoDataList = BuildSearchList(nfoDataList, list);

            while (nfoDataList.Count < totalresults)
            {
                page += 1;
                json = TmdbSupport.SearchMovieDatabase(title, page, database);
                if (json != string.Empty)
                {
                    root = JObject.Parse(json);
                    if (database == Database.MovieDb)
                    {
                        results = root["results"];
                    }
                    else if (database == Database.Omdb)
                    {
                        results = root["Search"];
                    }
                    //results = root["results"];
                }

                MoveToSearchList(database, nfoDataList, results);
                // nfoDataList = BuildSearchList(nfoDataList, list);
            }

            nfoDataList = [.. nfoDataList.OrderByDescending(a => a.Adult).ThenBy(a => a.Title)];

            return nfoDataList;
        }

        private static void MoveToSearchList(Database database, List<MovieBase> nfoDataList, object? results)
        {
            if (results != null)
            {
                if (database == Database.MovieDb)
                {
                    string? resultList = results.ToString();

                    if (!string.IsNullOrEmpty(resultList))
                    {
                        var mList = JsonConvert.DeserializeObject<List<MovieItem>>(resultList);

                        if (mList != null)
                            foreach (MovieItem item in mList)
                            {
                                nfoDataList.Add(item);
                            }
                    }
                }
                else if (database == Database.Omdb)
                {
                    //    var oList = JsonConvert.DeserializeObject<List<OMDBMovie>>(results.ToString());
                    //    foreach (OMDBMovie item in oList)
                    //    {
                    //        nfoDataList.Add(item);
                    //    }

                }
            }
        }

        /// <summary>
        /// The SimilarMovies
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="MovieList"/></returns>
        public static MovieList SimilarMovies(int id)
        {
            string returnJSON = string.Empty;

            string searchUrl = "https://api.themoviedb.org/3/movie/" + id.ToString().Trim() + "/similar?api_key=" + ApiKey + "&language=en-US&page=1&include_adult=true";

            returnJSON = CallWebClient(searchUrl);
            JObject jObject = JObject.Parse(returnJSON);
            var movies = jObject["results"];

            MovieList? movieItems = null;
            if (movies != null)
            {
                movieItems = JsonConvert.DeserializeObject<MovieList>(movies.ToString());

                if (movieItems != null)
                    movieItems.Sort((x, y) => y.Rating.CompareTo(x.Rating));
            }
            return movieItems;
        }

        // PSEUDOCODE PLAN (detailed):
        // 1. Replace the incorrect use of HttpClient.Open(...) which does not exist.
        // 2. Use HttpClient.GetStringAsync(...) to retrieve the response body as a string.
        // 3. Keep the method synchronous to match existing callers by blocking on the Task
        //    (use GetAwaiter().GetResult()). This avoids changing the method signature.
        // 4. Ensure HttpClient is disposed properly (use a 'using' or local using declaration).
        // 5. Preserve the existing error handling behavior: catch exceptions, record the
        //    exception string to a local variable, and return an empty string on error.
        //
        // Implementation notes:
        // - This change touches only the CallWebClient method to fix CS1061.
        // - No other methods or signatures are modified to avoid unrelated changes.

        private static string CallWebClient(string searchUrl)
        {
            string returnJSON = string.Empty;

            // Create and dispose HttpClient. (For long-lived apps prefer a shared HttpClient instance.)
            using var client = new HttpClient();

            try
            {
                // Use GetStringAsync to fetch the response body as a string.
                // Block synchronously to maintain the original synchronous method signature.
                returnJSON = client.GetStringAsync(searchUrl).GetAwaiter().GetResult();
            }
            catch (System.Exception e)
            {
                string err = e.ToString();
                // preserve original behavior of swallowing the exception and returning empty string
            }

            return returnJSON;
        }

        private async static Task<string> CallWebClientAsync(string searchUrl)
        {
            string returnJSON = string.Empty;

            // Create and dispose HttpClient. (For long-lived apps prefer a shared HttpClient instance.)

            using var client = new HttpClient();

            var response = await client.GetAsync(searchUrl);

            Debug.WriteLine(response.Content.Headers.ContentType);

            string text = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(text);
            return text;
            //using var client = new HttpClient();

            //try
            //{
            //    // Use GetStringAsync to fetch the response body as a string.
            //    // Block synchronously to maintain the original synchronous method signature.
            //    returnJSON = await client.GetStringAsync(searchUrl);
            //}
            //catch (System.Exception e)
            //{
            //    string err = e.ToString();
            //    // preserve original behavior of swallowing the exception and returning empty string
            //}

            return returnJSON;
        }

        /// <summary>
        /// The GetImage.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <remarks> Support.TmdbSupport.ImageURL + mynfoData.Poster_Path</remarks>
        /// <returns>.</returns>
        public async static Task<Avalonia.Media.Imaging.Bitmap?> GetImage(string imageUrl)
        {

            Avalonia.Media.Imaging.Bitmap? returnValue = null;
            try
            {
                returnValue = await TaymadeControls.ImageHelper.LoadFromWeb(new Uri(imageUrl));
            }
            catch (Exception)
            {
                // throw;
            }
            finally
            {

            }




            return returnValue;
        }

        public async static Task<Avalonia.Media.Imaging.Bitmap>? GetImageFromProfileAsync(string profile)
        {
            string url = ImageURL + profile;
            Avalonia.Media.Imaging.Bitmap? result;
            result = await GetImage(url);
            return result;
        }
        public static Avalonia.Media.Imaging.Bitmap? GetImageFromProfile(string profile)
        {
            string url = ImageURL + profile;
            Avalonia.Media.Imaging.Bitmap? result;
            result = GetImage(url).GetAwaiter().GetResult();
            return result;
        }

        #endregion
    }
}
