
using DocumentFormat.OpenXml.Wordprocessing;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaymadeEntities.Models;
using TaymadeEntities.MusicBrainzSupport;
using static System.Net.WebRequestMethods;
using Query = MetaBrainz.MusicBrainz.Query;

namespace TaymadeEntities.Support
{
    public class MusicBrainzSupport
    {
        private const string oauthClientId = "HhZIWIQLdE3N58GkKFk3tw";
        private const string pauthClientSecret = "L-NQiw3hsJMNLbfk_cl1RA";
        // username Doug1850

        private const string baseUrl = "https://musicbrainz.org/ws/2/";
        private const string recording = "recording";
        private const string artist = "artist";
        private const string UserAgentString = "MusicBox/1.1.0 (Doug.Taylor@taymade.co.uk )";
        private static string albumInclude = "?inc=artists+labels+recordings+media+artist-rels+genres";

        private static string artistInclude = "?inc=annotation+artist-rels+genres+url-rels+instrument-rels+artist-rels";

        private static string artistRecordingsInclude = "?inc=annotation+artist-rels+releases";

        private static string artistReleaseGroupsInclude = "?inc=annotation+artist-rels+release-groups+releases";

        private static string baseAddress = "https://musicbrainz.org/ws/2/";

        private static string jsonFormat = "&fmt=json";

        private static string trackInclude = "?inc=annotation+artist-credits+artists+isrcs+artist-rels+genres";

        private static string userAgent = "MusicBox/1.0 (doug.taylor@taymade.co.uk)";

        private static string wikiBaseAddress = "https://en.wikipedia.org/w/api.php";
        private Query? query { get; set; }
        public MusicBrainzSupport()
        {
            //test();
        }

        public async void test()
        {
            var q = new Query("MusicBox", "1.00", "mailto:Doug.Taylor@taymade.Co.Uk");
            var result = await q.FindArtistsAsync("Artist:\"Bert Jansch\"");
            var person  = await q.LookupArtistAsync(new Guid("d7f95537-2b48-403d-9ac2-f1fc7aad0960"));

        }

        public  static MBArtist GetArtist(string mbid)
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "artist/" + mbid + artistInclude + jsonFormat;
            empty = CallWebClient(searchUrl);
            MBArtist mBArtist = JsonConvert.DeserializeObject<MBArtist>(empty);
            if (mBArtist != null)
            {
                mBArtist.JSON = empty;
            }

            return mBArtist;
        }

        public async static Task<MBArtist?> GetArtistAsync(string mbid)
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "artist/" + mbid + artistInclude + jsonFormat;
            empty = await CallWebClientAsync(searchUrl);
            MBArtist? mBArtist = JsonConvert.DeserializeObject<MBArtist>(empty);
            if (mBArtist != null)
            {
                mBArtist.JSON = empty;
            }

            return mBArtist;
        }

        public static MBArtist GetArtistReleases(string mbid)
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "artist/" + mbid + artistRecordingsInclude + jsonFormat;
            empty = CallWebClient(searchUrl);
            MBArtist mBArtist = JsonConvert.DeserializeObject<MBArtist>(empty);
            if (mBArtist != null)
            {
                mBArtist.JSON = empty;
            }

            return mBArtist;
        }

        private async static Task<string> CallWebClientAsync(string searchUrl)
        {
            string returnJSON = string.Empty;

            // Create and dispose HttpClient. (For long-lived apps prefer a shared HttpClient instance.)

            using var client = new HttpClient()
            {
                DefaultRequestHeaders = { { "user-agent", userAgent } }
            };

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

        public static async Task<MBAlbum> GetAlbumAsync(string mbid)
        {
            _ = string.Empty;
            string query = baseAddress + "release/" + mbid + albumInclude + jsonFormat;
            string json = await CallWebClientAsync(query);
            MBAlbum mbAlbum = JsonConvert.DeserializeObject<MBAlbum>(json);
            if (mbAlbum != null)
            {
                mbAlbum.JSON = json;
            }

            return mbAlbum;
        }

        //public  async Task<IRelease?> GetAlbumAsync(string musicBrainzID)
        //{
        //    if (query == null) query = new Query("MusicBox", "1.00", "mailto:Doug.Taylor@taymade.Co.Uk");
        //    MBAlbum? result = null;
        //    Guid guid = new Guid(musicBrainzID);
        //    IRelease person = await query.LookupReleaseAsync(guid);
        //    return person;
        //}

        public static SearchRelease GetReleaseInfo(string mbid)
        { 
            SearchRelease searchRelease = null;
            string empty = string.Empty;
            string searchUrl = baseAddress + "release/" + mbid + "?inc=labels+recordings+media+artists+artist-rels" + jsonFormat;
            empty = CallWebClient(searchUrl);
            return JsonConvert.DeserializeObject<SearchRelease>(empty);
        }

        public static async Task<SearchRelease> GetReleaseInfoAsync(string mbid, string? additionrels ="")
        {
            _ = string.Empty;
            string query = baseAddress + "release/" + mbid 
                + "?inc=labels+recordings+media+artists+artist-rels" 
                + additionrels
                + jsonFormat;

            string? resp = await CallWebClientAsync(query);
            return JsonConvert.DeserializeObject<SearchRelease>(resp);
        }

        public static MBReleaseGroup GetReleases(string musicBrainzReleaseGroup)
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "release-group/" + musicBrainzReleaseGroup + "?inc=releases" + jsonFormat;
            empty = CallWebClient(searchUrl);
            MBReleaseGroup mBReleaseGroup = JsonConvert.DeserializeObject<MBReleaseGroup>(empty);
            if (mBReleaseGroup != null && mBReleaseGroup.Releases != null)
            {
                foreach (SearchRelease release in mBReleaseGroup.Releases)
                {
                    SearchRelease releaseInfo = GetReleaseInfo(release.Id);
                    if (releaseInfo != null)
                    {
                        release.LabelInfo = releaseInfo.LabelInfo;
                        release.Media = releaseInfo.Media;
                        release.ArtistCredits = releaseInfo.ArtistCredits;
                    }
                }
            }

            return mBReleaseGroup;
        }

        public static async Task<bool> GetTrackAsync(AlbumTrack albumTrack)
        {
            bool success = false;

            if (string.IsNullOrEmpty(albumTrack.MusicBrainzTrackID))
                if (!string.IsNullOrEmpty(albumTrack.MusicBrainzID))
                {
                    albumTrack.MusicBrainzTrackID = albumTrack.MusicBrainzID;
                }
                else return success;

            string query = baseAddress + "recording/" + albumTrack.MusicBrainzTrackID
                + "?inc=releases+artist-credits+artist-rels"
                + jsonFormat;

            string? resp = await CallWebClientAsync(query);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var trackInfo = JsonConvert.DeserializeObject<MBTrackInfo>(resp,settings);

            MBTrack mBTrack = null;
            if (trackInfo != null)
            {
                success = true;
                albumTrack.Comment = trackInfo.Performers;
                albumTrack.Duration = trackInfo.Length;
                albumTrack.TrackName = trackInfo.Title;

            }

            return success;
        }

        public static AlbumSearch FindAlbum(string query, string? artist = "")
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "release?query=" + query.Replace(" ", "%20");
            // check for artist 
            if (!string.IsNullOrEmpty(artist))
            {
                searchUrl += "%20AND%20artist:" + artist.Replace(" ", "%20");
            }
            searchUrl  += jsonFormat;
            empty = CallWebClient(searchUrl);
            return JsonConvert.DeserializeObject<AlbumSearch>(empty);
        }

        //public static async Task<MBAlbum> FindAlbumFromArtistAsync(string artistMBID, string query)
        //{
        //MBArtist artist = GetArtistReleases(artistMBID);
        //SearchRelease release = artist.Releases.Find((SearchRelease x) => x.Title.ToLower() == query.ToLower());
        //MBAlbum mbAlbum = null;
        //if (release != null)
        //{
        //    mbAlbum = await GetAlbumAsync(release.Id);
        //}

        //return mbAlbum;
        //}

        public static ArtistSearch FindArtist(string query)
        {
            string empty = string.Empty;
            string searchUrl = baseAddress + "artist?query=" + query.Replace(" ", "%20") + jsonFormat;
            empty = CallWebClient(searchUrl);
            return JsonConvert.DeserializeObject<ArtistSearch>(empty);
        }

        public static WikiSearch GetWikiInfo(string queryInfo)
        {
            string empty = string.Empty;
            if (queryInfo == null)
            {
                queryInfo = string.Empty;
            }

            string searchUrl = wikiBaseAddress + "?action=query&format=json&list=search&srsearch=" + queryInfo.Replace(" ", "%20");
            empty = CallWebClient(searchUrl);
            string oldValue = "<span class=";
            string oldValue2 = "</span>";
            string oldValue3 = "\\\"searchmatch\\\">";
            empty = empty.Replace(oldValue2, "").Replace(oldValue, "").Replace(oldValue3, "");
            WikiSearch wikiSearch = JsonConvert.DeserializeObject<WikiSearch>(empty);
            if (wikiSearch != null)
            {
                wikiSearch.JSON = empty;
            }

            return wikiSearch;
        }

        public static string CallWebClient(string searchUrl, string userAgent = "MovieViewer/1.0 (doug.taylor@taymade.co.uk)")
        {
            string result = string.Empty;
            HttpClient webClient = new HttpClient();
            webClient.DefaultRequestHeaders.Add("user-agent", userAgent);
            try
            {
               result = webClient.GetStringAsync(searchUrl).GetAwaiter().GetResult();
            
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                string text = ex.ToString();
            }

            return result;
        }

        public static async Task<string> CallWebClientAsync(string searchUrl, string userAgent = "MovieViewer/1.0 (doug.taylor@taymade.co.uk)")
        {
            string returnJSON = string.Empty;
            try
            {
                HttpClient webClient = new HttpClient
                {
                    DefaultRequestHeaders = { { "user-agent", userAgent } }
                };
                Uri uri = new Uri(searchUrl);
                var response = await webClient.GetAsync(searchUrl);
                returnJSON =  await response.Content.ReadAsStringAsync(); 
                webClient.Dispose();
            }
            catch (Exception)
            {
            }

            return returnJSON;
        }

        private static string myReadWebclientResponse(Stream stream)
        {
            string empty = string.Empty;
            StreamReader streamReader = new StreamReader(stream);
            empty = streamReader.ReadToEnd();
            stream.Flush();
            stream.Close();
            return empty;
        }
    }
}

