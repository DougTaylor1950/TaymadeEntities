namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Filter
    {
        private bool notHasChapter = false;

        private bool hasChapter = false;

        private bool notHasEpisode = false;

        private bool hasEpisode = false;

        private bool filterByName = false;

        private bool useCurrentPhrase = false;

        private bool useDuration = false;
        private string? jSON;
        private string? filterName;
        private string secondaryFilterId;
        private PhraseEntry secondaryPhrase;
        private PhraseEntry tertiaryPhrase;
        private string tertiaryFilterId;
        private bool useDirector;

        public Filter()
        {
            Populate();
        }

        public int Id { get; set; }


        [JsonIgnore]
        public string? FilterName
        {
            get
            {
                Populate();
                return filterName;
            }

            set => filterName = value;
        }

        [JsonIgnore]
        public string? JSON
        {
            get => jSON;
            set
            {
                jSON = value;
                Populate();
            }
        }

        private void Populate()
        {

            if (!string.IsNullOrEmpty(JSON))
            {
                FromJson(JSON);
            }
            else
            {
                NotHasChapter = false;
                NotHasEpisode = false;
                HasChapter = false;
                HasEpisode = false;
                UseAdded = false;
                UseCurrentPhrase = false;
                UseDuration = false;
                UseModified = false;
                UseSecondaryFilter = false;
                UseSeries = false;
                UseBookmark = false;
                UseDirector = false;
                FilterName = "<New>";
                BookmarkText = string.Empty;
                UseActor = false;
            }
        }

        [NotMapped]
        public bool NotHasChapter { get => notHasChapter; set => notHasChapter = value; }

        [NotMapped]
        public bool NotHasEpisode { get => notHasEpisode; set => notHasEpisode = value; }

        [NotMapped]
        public bool HasChapter { get => hasChapter; set => hasChapter = value; }

        [NotMapped]
        public bool HasEpisode { get => hasEpisode; set => hasEpisode = value; }

        [NotMapped]
        public bool UseCurrentPhrase { get => useCurrentPhrase; set => useCurrentPhrase = value; }

        [NotMapped]
        public bool UseDirector { get => useDirector; set => useDirector = value; }

        [NotMapped]
        public bool UseDuration { get => useDuration; set => useDuration = value; }

        [NotMapped]
        public bool FilterByName { get => filterByName; set => filterByName = value; }

        [NotMapped]
        public string DurationFilter { get; set; } = string.Empty;

        [NotMapped]
        public string AddedFilter { get; set; } = string.Empty;

        [NotMapped]
        public string ModifiedFilter { get; set; } = string.Empty;

        [NotMapped]
        public bool UseAdded { get; set; }

        [NotMapped]
        public bool UseActor { get; set; }

        [NotMapped]
        public bool UseSeries { get; set; }


        [NotMapped]
        public bool UseModified { get; set; }

        [NotMapped]
        public bool UseSecondaryFilter { get; set; }

        [NotMapped]
        public bool UseTertiaryFilter { get; set; }

        [NotMapped]
        public bool UseBookmark { get; set; }


        [NotMapped]
        public string BookmarkText { get; set; }

        [NotMapped]
        [JsonIgnore]
        public PhraseEntry SecondaryPhrase
        {
            get => secondaryPhrase;
            set
            {
                secondaryPhrase = value;
                if (value != null && string.IsNullOrEmpty(secondaryFilterId))
                {
                    SecondaryFilterId = value.Id;
                }
            }
        }

        [NotMapped]
        [JsonIgnore]
        public Director CurrentDirector { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<Actor> CurrentActorList { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string ActorName { get; set; }

        [NotMapped]
        [JsonIgnore]
        public PhraseEntry TertiaryPhrase
        {
            get => tertiaryPhrase;
            set
            {
                tertiaryPhrase = value;
                if (value != null && string.IsNullOrEmpty(tertiaryFilterId))
                {
                    tertiaryFilterId = value.Id;
                }
            }
        }

        [NotMapped]
        public string SecondaryFilterId
        {
            get => secondaryFilterId;

            set
            {
                secondaryFilterId = value;

                if (!string.IsNullOrEmpty(value))
                {
                    secondaryPhrase = DataController.PhrasesController.GetByPhraseId(value);
                    //secondaryPhrase = DataController.SandboxEntities.PhraseEntry.Where(x => x.Id == value).FirstOrDefault();
                }
            }
        }

        [NotMapped]
        public string TertiaryFilterId
        {
            get => tertiaryFilterId;

            set
            {
                tertiaryFilterId = value;

                if (!string.IsNullOrEmpty(value))
                {
                    tertiaryPhrase = DataController.PhrasesController.GetByPhraseId(value);
                    //tertiaryPhrase = DataController.SandboxEntities.PhraseEntry.Where(x => x.Id == value).FirstOrDefault();
                }
            }
        }

        public void ToJson()


        {
            string json = JsonConvert.SerializeObject(this);

            this.JSON = json;
        }

        public void FromJson(string json)

        {
            if (!string.IsNullOrEmpty(json))
            {
                JObject tempFilter = (JObject)JsonConvert.DeserializeObject(json);

                if (tempFilter != null)
                {
                    if (tempFilter.TryGetValue("HasChapter", out JToken? hChapter))
                    {
                        HasChapter = (bool)hChapter;
                    }

                    if (tempFilter.TryGetValue("NotHasChapter", out JToken? hnChapter))
                    {
                        NotHasChapter = (bool)hnChapter;
                    }

                    if (tempFilter.TryGetValue("HasEpisode", out JToken? hepisode))
                    {
                        HasEpisode = (bool)hepisode;
                    }

                    if (tempFilter.TryGetValue("NotHasEpisode", out JToken? hnepisode))
                    {
                        NotHasEpisode = (bool)hnepisode;
                    }

                    if (tempFilter.TryGetValue("UseCurrentPhrase", out JToken? useCurrentPhrase))
                    {
                        UseCurrentPhrase = (bool)useCurrentPhrase;
                    }

                    if (tempFilter.TryGetValue("FilterByName", out JToken? filterByName))
                    {
                        FilterByName = (bool)filterByName;
                    }

                    if (tempFilter.TryGetValue("UseDuration", out JToken? useDuration))
                    {
                        UseDuration = (bool)useDuration;
                    }

                    if (tempFilter.TryGetValue("DurationFilter", out JToken? durationFilter))
                    {
                        DurationFilter = durationFilter.ToString();
                    }

                    if (tempFilter.TryGetValue("UseAdded", out JToken? useAdded))
                    {
                        UseAdded = (bool)useAdded;
                    }

                    if (tempFilter.TryGetValue("AddedFilter", out JToken? addedFilter))
                    {
                        AddedFilter = addedFilter.ToString();
                    }

                    if (tempFilter.TryGetValue("UseModified", out JToken? useModified))
                    {
                        UseModified = (bool)useModified;
                    }

                    if (tempFilter.TryGetValue("ModifiedFilter", out JToken? modifiedFilter))
                    {
                        ModifiedFilter = modifiedFilter.ToString();
                    }

                    if (tempFilter.TryGetValue("UseSeries", out JToken? useSeries))
                    {
                        UseSeries = (bool)useSeries;
                    }

                    if (tempFilter.TryGetValue("UseSecondaryFilter", out JToken? useSecondaryFilter))
                    {
                        UseSecondaryFilter = (bool)useSecondaryFilter;
                    }

                    if (tempFilter.TryGetValue("SecondaryFilterId", out JToken? secondaryFilterId))
                    {
                        SecondaryFilterId = secondaryFilterId.ToString();
                    }
                }

                //this.HasChapter = ;
            }

        }

        public override string ToString()
        {
            return FilterName;
        }

        public void Update()
        {
            if (Id == 0)
            {
            }
            else
            {
                EntityState state = DataController.SandboxEntities.Entry(this).State;

                var local = DataController.SandboxEntities.Set<Filter>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                // check if local is not null
                if (local != null)
                {
                    // detach
                    DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                }
                // set Modified flag in your entry
                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();
            }
        }

        public void Insert()
        {
            DataController.SandboxEntities.Filter.Add(this);
            DataController.SandboxEntities.SaveChanges();
            Update();
        }
    }
}
