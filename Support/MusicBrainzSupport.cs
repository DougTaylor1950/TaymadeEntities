using DocumentFormat.OpenXml.Bibliography;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaymadeEntities.MusicBrainzSupport
{
    public class MBTrackHeader
    {
        private string display = string.Empty;

        private string trackId;

        public string Display
        {
            get
            {
                if (Track != null)
                {
                    if (!string.IsNullOrEmpty(Number))
                    {
                        display = Number + " : ";
                    }

                    display += Track.Title;
                }

                return display;
            }
            set
            {
                display = value;
            }
        }

        [JsonProperty("id")]
        public string? ID { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        [JsonProperty("number")]
        public string? Number { get; set; }

        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("recording")]
        public MBTrack? Track { get; set; }

        public string TrackId
        {
            get
            {
                if (Track != null)
                {
                    trackId = Track.ID;
                }

                return trackId;
            }
            set
            {
                trackId = value;
            }
        }

        [JsonProperty("genres")]
        private List<MBGenre> Genres { get; set; }

        public string[] GenreNames
        {
            get
            {
                string text = string.Empty;
                foreach (MBGenre genre in Genres)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ",";
                    }

                    text += genre.Name;
                }

                return text.Split(new char[1] { ',' });
            }
        }
    }

    public class Continuation
    {
        [JsonProperty("continue")]
        public string? Continue { get; set; }

        [JsonProperty("sroffset")]
        public int? SROffset { get; set; }
    }


    public class WikiSearch
    {
        [JsonProperty("batchcomplete")]
        public string BatchComplete { get; set; }

        [JsonProperty("continue")]
        public Continuation Continuation { get; set; }

        public string JSON { get; set; }

        [JsonProperty("query")]
        public WikiQuery Query { get; set; }
    }

    public class WikiQueryResult
    {
        [JsonProperty("ns")]
        public int? NS { get; set; }

        [JsonProperty("pageid")]
        public string? PageId { get; set; }

        [JsonProperty("size")]
        public int? PageSize { get; set; }

        [JsonProperty("snippet")]
        public string? Snippet { get; set; }

        [JsonProperty("timestamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("wordcount")]
        public int? WordCount { get; set; }
    }

    public class SearchInfo
    {
        [JsonProperty("totalhits")]
        public int? TotalHits { get; set; }
    }

    public class WikiQuery
    {
        [JsonProperty("search")]
        public List<WikiQueryResult> Results { get; set; }

        [JsonProperty("searchinfo")]
        public SearchInfo SearchInfo { get; set; }
    }
    public class MBTrack : TrackBase
    {
        private string? performers;

        [JsonProperty("artist-credit")]
        public List<MBArtistCredit> ArtistCredits { get; set; }

        [JsonProperty("disambiguation")]
        public string? Disambiguation { get; set; }

        public string? JSON { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        public string? Performers
        {
            get
            {
                if (Relations != null && Relations.Count > 0)
                {
                    performers = string.Empty;
                    foreach (MBRelationship relation in Relations)
                    {
                        if (relation.Artist != null)
                        {
                            performers = performers + relation.Artist.Name + ":" + relation.Type;
                            if (relation.Attributes.Count > 0)
                            {
                                performers = performers + " " + string.Join(", ", relation.Attributes.Select((string x) => x).ToArray());
                            }
                        }

                        performers += "|";
                    }
                }

                return performers;
            }
        }

        [JsonProperty("relations")]
        public List<MBRelationship> Relations { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }
    }

    public class MBRelationship
    {
        [JsonProperty("artist")]
        public MBArtist? Artist { get; set; }

        [JsonProperty("attributes")]
        public List<string>? Attributes { get; set; }

        [JsonProperty("target-type")]
        public string? TargetType { get; set; }

        [JsonProperty("type-id")]
        public string? TargetTypeID { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }

    public class MBMedia
    {
        [JsonProperty("format")]
        public string? Format { get; set; }

        [JsonProperty("format-id")]
        public string? FormatID { get; set; }

        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("track-count")]
        public int? TrackCount { get; set; }

        [JsonProperty("track-offset")]
        public int? TrackOffset { get; set; }

        [JsonProperty("tracks")]
        public List<MBTrackHeader>? Tracks { get; set; }
    }

    public class MBAlbum 
    {
         
        [JsonProperty("barcode")]
        public string? Barcode { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("date")]
        public string? Date { get; set; }

        [JsonProperty("id")]
        public string? ID { get; set; }

        public string? JSON { get; set; }

        [JsonProperty("media")]
        public List<MBMedia>? Media { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("genres")]
        private List<MBGenre>? Genres { get; set; }

        public string[]? GenreNames
        {
            get
            {
                string text = string.Empty;
                foreach (MBGenre genre in Genres)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ",";
                    }

                    text += genre.Name;
                }

                return text.Split(new char[1] { ',' });
            }
        }

        public string GenreList
        {
            get
            {
                string text = string.Empty;
                foreach (MBGenre genre in Genres)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ",";
                    }

                    text += genre.Name;
                }

                return text;
            }
        }

        
    }

    public class MBGenre
    {
        [JsonProperty("count")]
        public int? Count { get; set; }

        [JsonProperty("disambiguation")]
        public string? Disambiguation { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class MBArtistRelationship
    {
        [JsonProperty("artist")]
        public MBArtist? Artist { get; set; }

        [JsonProperty("url")]
        public MBUrl Urls { get; set; }

        [JsonProperty("direction")]
        public string? Direction { get; set; }

        public string? Id => Artist.ID;

        public string? Name => Artist.Name;

        [JsonProperty("target")]
        public string? Target { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("target-type")]
        public string? TargetType { get; set; }
    }

    public class MBUrl
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("resource")]
        public string? Resource { get; set; }
    }

    public class ArtistSearch
    {
        [JsonProperty("artists")]
        public List<MBArtist>? Artists { get; set; }

        

        [JsonProperty("count")]
        public int? Count { get; set; }

        [JsonProperty("created")]
        public DateTime? Created { get; set; }
    }

    public class AlbumSearch
    {
        [JsonProperty("count")]
        public int? Count { get; set; }

        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [JsonProperty("releases")]
        public List<SearchRelease> Releases { get; set; }
    }
    public class MBArtist
    {
        [JsonProperty("type")]
        public string ArtistType { get; set; }

        [JsonProperty("relations")]
        public List<MBArtistRelationship>? Relations { get; set; }

        public List<MBArtistRelationship>? BandMembers => Relations?.Where((MBArtistRelationship x) => x.TargetType.ToLower() == "artist").ToList();

        public List<MBArtistRelationship>? Urls => Relations?.Where((MBArtistRelationship x) => x.TargetType.ToLower() == "url").ToList();

        [JsonProperty("begin_area")]
        public MBArea BeginArea { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("disambiguation")]
        public string Disambiguation { get; set; }

        public string Display => Name + ": " + ArtistType;

        [JsonProperty("end_area")]
        public MBArea EndArea { get; set; }

        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [JsonProperty("id")]
        public string? ID { get; set; }

        public string? JSON { get; set; }

        [JsonProperty("life-span")]
        public ArtistLifeSpan LifeSpan { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sort-name")]
        public string SortName { get; set; }

        [JsonProperty("type-id")]
        public string TypeId { get; set; }

        [JsonProperty("genres")]
        private List<MBGenre> Genres { get; set; }

        public string[] GenreNames
        {
            get
            {
                string text = string.Empty;
                if (Genres != null)
                foreach (MBGenre genre in Genres)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ",";
                    }

                    text += genre.Name;
                }

                return text?.Split(new char[1] { ',' });
            }
        }

        [JsonProperty("releases")]
        public List<SearchRelease>? Releases { get; set; }

        [JsonProperty("release-groups")]
        public List<MBReleaseGroup>? ReleaseGroups { get; set; }
    }

    public class ArtistLifeSpan
    {
        [JsonProperty("begin")]
        public string? Begin { get; set; }

        [JsonProperty("end")]
        public string? DeathDate { get; set; }

        [JsonProperty("ended")]
        public bool? Ended { get; set; }
    }

    public class MBArea
    {
        [JsonProperty("disambiguation")]
        public string? Disambiguation { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("sort-name")]
        public string? SortName { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("type-id")]
        public string? TypeId { get; set; }
    }

    public class MBLabelInfo
    {
        [JsonProperty("catalog-number")]
        public string? CatalogueNumber { get; set; }

        [JsonProperty("label")]
        public MBLabel Label { get; set; }
    }

    public class MBLabel
    {
        [JsonProperty("id")]
        public string? ID { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }

    public class MBArtistCredit
    {
        [JsonProperty("artist")]
        public MBArtist? Artist { get; set; }

        [JsonProperty("joinphrase")]
        public string? JoinPhrase { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("genres")]
        private List<MBGenre>? Genres { get; set; }

        public string[]? GenreNames
        {
            get
            {
                string text = string.Empty;
                foreach (MBGenre genre in Genres)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ",";
                    }

                    text += genre.Name;
                }

                return text.Split(new char[1] { ',' });
            }
        }
    }

    public class MBTrackInfo
    {
        [JsonProperty("artist-credit")]
        public List<MBArtistCredit>? ArtistCredits { get; set; }

        //[JsonProperty("first-release-date",)]
        //public DateTime? FirstReleaseDate { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        private string performers = string.Empty;
        public string? Performers
        {
            get
            {
                if (Relations != null && Relations.Count > 0)
                {
                    performers = string.Empty;
                    foreach (MBRelationship relation in Relations)
                    {
                        if (relation.Artist != null)
                        {
                            performers = performers + relation.Artist.Name + ":" + relation.Type;
                            if (relation.Attributes?.Count > 0)
                            {
                                performers = performers + " " + string.Join(", ", relation.Attributes.Select((string x) => x).ToArray());
                            }
                        }

                        performers += "|";
                    }
                }

                return performers;
            }
        }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("releases")]
        public List<SearchRelease>? Releases { get; set; }

        [JsonProperty("relations")]
        public List<MBRelationship>? Relations { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

    }

    public class SearchRelease
    {
        private string? display;

        [JsonProperty("artist-credit")]
        public List<MBArtistCredit> ArtistCredits { get; set; }

        [JsonProperty("count")]
        public int? Count { get; set; }

        public string? Display
        {
            get
            {
                display = Title;
                if (ArtistCredits != null)
                {
                    string text = string.Empty;
                    foreach (MBArtistCredit artistCredit in ArtistCredits)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            text += " - ";
                        }

                        text += artistCredit.Name;
                    }

                    if (!string.IsNullOrEmpty(display))
                    {
                        display = display + " Artists:" + text;
                    }

                    if (Media != null)
                    {
                        display = display + " Discs: " + Media.Count;
                        foreach (MBMedia medium in Media)
                        {
                            display = display + medium.Title + " tracks :" + medium.TrackCount;
                        }
                    }
                    else
                    {
                        display = display + " Tracks: " + TrackCount + " Score: " + Score;
                    }
                }

                if (LabelInfo != null)
                {
                    string text2 = string.Empty;
                    foreach (MBLabelInfo item in LabelInfo)
                    {
                        if (!string.IsNullOrEmpty(text2))
                        {
                            text2 += " -";
                        }

                        text2 += item.CatalogueNumber;
                        if (item.Label != null)
                        {
                            text2 = text2 + " " + item.Label.Name;
                        }
                    }

                    if (!string.IsNullOrEmpty(display))
                    {
                        display = display + " Labels:" + text2;
                    }
                }

                return display;
            }
            set
            {
                display = value;
            }
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label-info")]
        public List<MBLabelInfo> LabelInfo { get; set; }

        [JsonProperty("media")]
        public List<MBMedia> Media { get; set; }

        [JsonProperty("release-group")]
        public MBReleaseGroup ReleaseGroup { get; set; }

        [JsonProperty("score")]
        public int? Score { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("track-count")]
        public int? TrackCount { get; set; }
    }

    public class TrackBase
    {
        [JsonProperty("id")]
        public string? ID { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }
    }

    public class MBReleaseGroup : TrackBase
    {
        [JsonProperty("primary-type")]
        public string? PrimaryType { get; set; }

        public string? Display
        {
            get
            {
                string empty = string.Empty;
                return empty + base.Title + " " + PrimaryType;
            }
        }

        [JsonProperty("releases")]
        public List<SearchRelease>? Releases { get; set; }
    }

}
