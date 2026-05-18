using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.LayoutRenderers.Wrappers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{

    public class StoryCast : ModelBase
    {
        #region Private Fields

        private string? age;
        private string? character;
        private string? codes;
        private int? castId;

        #endregion Private Fields

        #region Public Properties

        [NotMapped]
        public new int Id { get; set; }

        [JsonProperty]
        public string? Age { get => age; set => this.RaiseAndSetIfChanged(ref age, value); }

        [JsonProperty]
        public string? Character { get => character; set => this.RaiseAndSetIfChanged(ref character, value); }

        [JsonIgnore]
        public string? Codes { get => codes; set => this.RaiseAndSetIfChanged(ref codes, value); }

        [JsonProperty]
        public int? CastId { get => castId; set => this.RaiseAndSetIfChanged(ref castId, value); }

        [JsonIgnore]
        public int Pk { get; set; }

        [JsonIgnore]
        public int StoryId { get; set; }

        #endregion Public Properties

        internal void Update()
        {
            try
            {
                bool success = DataController.SandboxEntities.UpdateStoryCast(this);
                //int rowschanged = DataController.SandboxEntities.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                string error = ex.ToString();
            }

            DataController.SandboxEntities.Entry(this).State = EntityState.Unchanged;

        }
        public void Insert()
        {
            try
            {
                if (string.IsNullOrEmpty(Codes)) Codes = "";
                if (CastId == null) CastId = 0;

                StoryCast? tempCast = DataController.SandboxEntities.CreateStoryCast(StoryId, CastId.Value, Codes, Character, Age);
                if (tempCast != null) this.Pk = tempCast.Pk;
                //int rowschanged = DataController.SandboxEntities.SaveChanges();
            }
            catch (Exception ex)
            {

                string error = ex.Message;
            }
        }

        public void FindCodesForItem(Story currentStory)
        {
            if (CastId != null && CastId > 0 && currentStory != null && !string.IsNullOrEmpty(currentStory.Codes))
            {
                StoryId = currentStory.Id;
                string searchCode = "id:" + CastId.ToString().Trim() + ";";
                if (currentStory.Codes.Contains(searchCode, StringComparison.OrdinalIgnoreCase))
                {
                    int index1 = currentStory.Codes.IndexOf(searchCode, StringComparison.OrdinalIgnoreCase);
                    int index2 = currentStory.Codes.IndexOf("Id:", index1 + 1, StringComparison.OrdinalIgnoreCase);
                    // check we have found something 
                    if (index1 >= 0 && index2 > 0)
                    {
                        string code = currentStory.Codes.Substring(index1 + searchCode.Length, index2 - index1 - searchCode.Length);
                        Codes = code;
                    }
                }
                // update or insert
                if (Pk > 0)
                {
                    Update();
                }
                else
                {
                    Insert();
                    if (Pk > 0) Update();
                }
            }
        }

        internal void Delete()
        {
            DataController.SandboxEntities.StoryCast.Remove(this);
            int result = DataController.SandboxEntities.SaveChanges();
        }
    }

    public class StoryCastList : ObservableCollection<StoryCast>
    {
        #region Public Constructors

        public StoryCastList()
        {
        }

        public StoryCastList(string json, int storyId)
        {
            // clear and fill from database
            this.Clear();

            List<StoryCast> temp = DataController.SandboxEntities.StoryCast.Where(s => s.StoryId == storyId).ToList();
            foreach (var cast in temp)
            {
                this.Add(cast);
            }

            if (string.IsNullOrEmpty(json))
            // throw new ArgumentNullException(nameof(json));
            { }
            else
            {
                if (json.Contains("Cast" + '"'))
                {
                    json = json.Substring(json.IndexOf("["));
                    int ind = json.IndexOf("]");
                    json = json.Substring(0, ind + 1);

                }
                StoryCastList? temp1 = FromJSON(json);
                if (temp1 != null)
                {
                    // copy list to base
                    foreach (var item in temp1)
                    {  
                        //check to see if already present
                       
                        item.StoryId = storyId;
                        StoryCast? castTemp = this.Where(s => s.Character == item.Character).FirstOrDefault();
                        if (castTemp == null)
                        {
                            this.Add(item);
                        }
                    }
                }
            }
        }


        public StoryCastList(IEnumerable<StoryCast> collection) : base(collection)
        {
        }

        public StoryCastList(IEnumerable<StoryCast> collection, Story story) : base(collection)
        {
            foreach (var item in this)
            {
                item.StoryId = story.Id;
                item.FindCodesForItem(story);
                //StoryCast castTemp = this.Where(s => s.CastId == item.CastId && s.Character == item.Character).FirstOrDefault();

                //if (castTemp == null)
                //{
                //    this.Add(item);
                //}
            }
        }


        #endregion Public Constructors

        #region Methods

        public static StoryCastList? FromJSON(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            else
                return JsonConvert.DeserializeObject<StoryCastList>(json);
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        #endregion

    }

    public class StoryInfo : ModelBase
    {
        #region Private Fields

        private ObservableCollection<StoryCast>? cast;

        #endregion Private Fields

        #region Public Constructors

        public StoryInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryInfo"/> class.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <autogeneratedoc />
        public StoryInfo(string json, Story parent)
        {
            StoryInfo? storyInfo = FromJSON(json);

            Parent = parent;

            if (storyInfo != null)
            {
                CurrentChapter = storyInfo.CurrentChapter;
                CurrentPage = storyInfo.CurrentPage;
                CurrentSection = storyInfo.CurrentSection;
                TotalChapters = storyInfo.TotalChapters;
                TotalPages = storyInfo.TotalPages;
                TotalSections = storyInfo.TotalSections;
                Paragraphs = storyInfo.Paragraphs;
                WordCount = storyInfo.WordCount;

                // if Cast is not null create new 
                //if (storyInfo.Cast != null)
                //{
                //    if (parent == null)
                //        Cast = new ObservableCollection<StoryCast>(new StoryCastList(storyInfo.Cast));
                //    else
                //        Cast = new ObservableCollection<StoryCast>(new StoryCastList(parent.Json, parent.Id));

                //}
                //else Cast = new();
            }
            else
            {
                CurrentChapter = 0;
                CurrentPage = 0;
                CurrentSection = 0;
                TotalChapters = 0;
                TotalPages = 0;
                TotalSections = 0;
                Paragraphs = 0;
                WordCount = 0;
                //Cast = new();
            }
        }

        #endregion Public Constructors

        #region Public Properties

        // add a list of StoryCharacters
        //[JsonProperty]
        //public ObservableCollection<StoryCast> Cast
        //{
        //    get => cast;
        //    set
        //    {
        //        this.RaiseAndSetIfChanged(ref cast, value);
        //    }
        //}

        [JsonProperty]
        public int? CurrentChapter { get; set; }

        [JsonProperty]
        public int? CurrentPage { get; set; }

        [JsonProperty]
        public int? CurrentSection { get; set; }

        [JsonProperty]
        public int? Paragraphs { get; internal set; }

        [JsonIgnore]
        public Story Parent { get; set; }
        [JsonProperty]
        public int? TotalChapters { get; set; }
        [JsonProperty]
        public int? TotalPages { get; set; }
        [JsonProperty]
        public int? TotalSections { get; set; }
        [JsonProperty]
        public int? WordCount { get; internal set; }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        public static StoryInfo? FromJSON(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            else
                return JsonConvert.DeserializeObject<StoryInfo>(json);
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal string ToInfo()
        {
            string info = $"Chapter: {CurrentChapter}/{TotalChapters}, Page: {CurrentPage}/{TotalPages}, Section: {CurrentSection}/{TotalSections}";
            return info;
        }

        #endregion Public Methods
    }
}
