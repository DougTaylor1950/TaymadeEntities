//-----------------------------------------------------------------------
// <copyright file="Story.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>07/07/2022 15:42:48 07/07/2022 15:42:48 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    // using TaymadeEntities.Support;
    using SupportCore;
    using SupportCore.Word;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using IronPdf;
    
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using OpenXmlPowerTools;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using TaymadeEntities.ViewModels;

    // using global::Support.Core.Word;

    // using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Defines the <see cref="Story" />.
    /// </summary>
    [Table("Stories")]

    //    The Story class in the provided code is a model class that represents a story entity in the application.
    //    It inherits from ModelBase and includes various properties, fields, constants, and methods to manage the story's data and behavior.
    //    Here's a breakdown of its components:

    //Constants
    //•	divEnd, LiStyle, UlStyle: Constants for HTML styling.

    //Fields

    //•	Various fields to store story-related data such as added, age, author, characters, codes, creation, documentExtn, group, language,
    // lastModified, lines, lowestAge, lowestAgeInt, myBreaks, myopenBookmarks,
    // myOpenXMLHeadingList1, myOpenXMLHeadingList2, myOpenXMLHeadingList3, myWmlDocument, originalLanguage, pages, path, pathWrong,
    // published, score, sectionBreaks, seriesId, storySeries, title, translation, wordHeadingList, wordHeadings2, wordHeadings3, wpDocument.

    //Properties
    //•	Various properties to get and set story-related data, including:
    //•	Added, AddedString, Age, Author, AuthorItem, Breaks, Characters, Codes, CreatedString, Creation, Dirty, DocumentExtn, FixedPath,
    //  Group, Heading1List, Heading2List, Heading3List, HeadingCount, Id, IDAuthor, Language, LastModified, Lines, LowestAge, LowestAgeInt,
    //  ModifiedString, OpenBookmarks, OriginalLanguage, Pages, Path, PathWrong, Published, Score, SectionBreaks, SeriesId, StorySeries, Title,
    //  Translation, WmlDocument, WordHeadingList, WordHeadings1, WordHeadings2, WordHeadings3.

    //    Methods
    //•	Story() : Constructor that subscribes to the PropertyChanged event.
    //•	Story_PropertyChanged(): Event handler that sets the Dirty flag to true when a property changes.
    //•	Create(): Static method to create a new Story instance and save it to the database.
    //•	Create(string path): Static method to create a new Story instance from a file path and extract metadata.
    //•	GetProperties(string path): Static method to get properties from a document.
    //•	CheckExists(): Method to check if the story's file exists.
    //•	GetBreaksAndHeadings(): Method to extract breaks and headings from the document.
    //•	GetPropertiesFromDocument(): Method to get properties from the document and update the story.
    //•	OpenDocument(): Method to open the document and extract bookmarks, breaks, and headings.
    //•	SetProperties(WordProperties currentProperties): Method to set properties from a WordProperties object.
    //•	ToHtml(): Method to convert the story to HTML format.
    //•	Delete(): Method to delete the story from the database.
    //•	Insert(): Method to insert the story into the database.
    //•	Save(): Method to save the story to the database.
    //•	ParagraphText(Paragraph para): Helper method to get text from a paragraph.
    //•	GetBookmarks(WordprocessingDocument document): Helper method to get bookmarks from a document.
    //•	GetBreaks(int sectID, Paragraph para): Helper method to get breaks from a paragraph.
    //•	GetHeadings(ref int heading1, ref int heading2, ref int heading3, Paragraph para): Helper method to get headings from a paragraph.
    //•	GetHeadingsFromList(BreakList headingList): Helper method to get headings from a break list.
    //•	GetHeadingsFromList(OpenXMLHeadingList headingList): Helper method to get headings from a heading list.

    //The class uses various libraries and frameworks, including ReactiveUI, OpenXML, IronPdf, EntityFramework, and others, to manage the story's data
    // and behavior. The class also interacts with other classes such as Author, BookmarkList, BreakList, PhraseEntry, StorySeries, WordHeadings,
    // WordProperties, and OpenXmlBookmark.

    public partial class Story : ModelBase
    {
        #region Constants

        /// <summary>
        /// Defines the divEnd.
        /// </summary>
        public const string divEnd = "</div>";

        /// <summary>
        /// Defines the LiStyle.
        /// </summary>
        public const string LiStyle = "li {   display: inline-block;   display: inline;   float: left; } ";

        /// <summary>
        /// Defines the UlStyle.
        /// </summary>
        public const string UlStyle = "ul {   background-color: #F2C777; }  li a {   display: block;   padding: 10px;   color: #7C785B; } li a:hover {   background-color: #EC8C65; } ";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the div.
        /// </summary>
        public string div = "<div style=" + '"' + "display: flex; justify-content: flex-start" + '"' + ">";

        /// <summary>
        /// Defines the w.
        /// </summary>
        internal static XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private DateTime? added;

        /// <summary>
        /// Defines the age.
        /// </summary>
        private string? age;

        /// <summary>
        /// Defines the author.
        /// </summary>
        private string? author;

        private Author? authorItem;

        private ObservableCollection<StoryAuthor>? authors;

        /// <summary>
        /// Defines the characters.
        /// </summary>
        private int? characters;

        /// <summary>
        /// Defines the codes.
        /// </summary>
        private string? codes;

        private DateTime? creation = DateTime.MinValue;

        /// <summary>
        /// Defines the documentExtn.
        /// </summary>
        private string? documentExtn;

        /// <summary>
        /// Defines the group.
        /// </summary>
        private string? group;

        private int? iDAuthor;

        /// <summary>
        /// Defines the language.
        /// </summary>
        private PhraseEntry? language;

        /// <summary>
        /// Defines the lastModified.
        /// </summary>
        private DateTime? lastModified;

        /// <summary>
        /// Defines the lines.
        /// </summary>
        private int? lines;

        /// <summary>
        /// Defines the lowestAge.
        /// </summary>
        private string? lowestAge;

        /// <summary>
        /// Defines the lowestAgeInt.
        /// </summary>
        private int lowestAgeInt;

        /// <summary>
        /// Defines the myBreaks.
        /// </summary>
        private BreakList? myBreaks;

        /// <summary>
        /// Defines the myopenBookmarks.
        /// </summary>
        private BookmarkList? myopenBookmarks;

        /// <summary>
        /// Defines the myOpenXMLHeadingList1.
        /// </summary>
        private OpenXMLHeadingList? myOpenXMLHeadingList1;

        /// <summary>
        /// Defines the myOpenXMLHeadingList2.
        /// </summary>
        private OpenXMLHeadingList? myOpenXMLHeadingList2;

        /// <summary>
        /// Defines the myOpenXMLHeadingList3.
        /// </summary>
        private OpenXMLHeadingList? myOpenXMLHeadingList3;

        /// <summary>
        /// Defines the myWmlDocument.
        /// </summary>
        private WmlDocument? myWmlDocument;

        /// <summary>
        /// Defines the originalLanguage.
        /// </summary>
        private string? originalLanguage;

        /// <summary>
        /// Defines the pages.
        /// </summary>
        private int? pages;

        /// <summary>
        /// Defines the path.
        /// </summary>
        private string? path;

        /// <summary>
        /// Defines the pathWrong.
        /// </summary>
        private bool? pathWrong;

        private string? percent;

        /// <summary>
        /// Defines the published.
        /// </summary>
        private string? published;

        private int? score = 1;

        /// <summary>
        /// Defines the sectionBreaks.
        /// </summary>
        private ObservableCollection<WordHeadings>? sectionBreaks;

        private int? seriesId;

        /// <summary>
        /// Defines the storySeries.
        /// </summary>
        private StorySeries? storySeries;

        /// <summary>
        /// Defines the title.
        /// </summary>
        private string? title;

        /// <summary>
        /// Defines the translation.
        /// </summary>
        private bool? translation;

        /// <summary>
        /// Defines the wordHeadingList.
        /// </summary>
        private List<WordHeadings>? wordHeadingList;

        /// <summary>
        /// Defines the wordHeadings1.
        /// </summary>
        private WordheadingsCollection? wordHeadings1;

        /// <summary>
        /// Defines the wordHeadings2.
        /// </summary>
        private ObservableCollection<WordHeadings>? wordHeadings2;

        /// <summary>
        /// Defines the wordHeadings3.
        /// </summary>
        private ObservableCollection<WordHeadings>? wordHeadings3;

        /// <summary>
        /// Defines the wpDocument.
        /// </summary>
        private WordprocessingDocument? wpDocument = null;
        private string? json = string.Empty;

        public Story()
        {
            this.PropertyChanged += Story_PropertyChanged;
        }

        private void Story_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dirty = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Added.
        /// </summary>
        public Nullable<System.DateTime> Added
        {
            get => added;
            set
            {
                this.RaiseAndSetIfChanged(ref added, value);
                this.RaisePropertyChanged("AddedString");
            }
        }

        /// <summary>
        /// Gets or sets the AddedString
        /// Gets the AddedString..
        /// </summary>
        [NotMapped]
        public string? AddedString
        {
            get
            {
                if (Added != null)
                    return Added.Value.ToString("yyyy-MM-dd");
                else return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (DateTime.TryParse(value, out DateTime newDate))
                    {
                        Added = newDate;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Age.
        /// </summary>
        public string? Age
        {
            get
            {
                if (!string.IsNullOrEmpty(age))
                    return age;
                else return string.Empty;
            }

            set => this.RaiseAndSetIfChanged(ref age, value);
        }

        /// <summary>
        /// Gets or sets the Primary Author.
        /// </summary>
        public string? Author
        {
            get
            {
                if (!string.IsNullOrEmpty(author))
                    return author;
                else return string.Empty;
            }
            set => this.RaiseAndSetIfChanged(ref author, value);
        }

        /// <summary>
        /// Gets or sets the author item.
        /// </summary>
        /// <value>
        /// The author item.
        /// </value>
        /// <autogeneratedoc />
        [ForeignKey("IDAuthor")]
        public Author? AuthorItem
        {
            get => authorItem;
            set
            {
                this.RaiseAndSetIfChanged(ref authorItem, value);

                if (authorItem != null && authorItem.Id != 1)
                {
                    StoryAuthor? storyAuthor = DataController.SandboxEntities.StoryAuthor.FirstOrDefault(x => x.StoryId == this.Id && x.AuthorId == authorItem.Id);
                    if (storyAuthor == null)
                    {
                        storyAuthor = DataController.SandboxEntities.InsertStoryAuthor(this.Id, authorItem.Id);
                        if (storyAuthor != null)
                        {
                            storyAuthor.Save();
                            if (this.Authors != null) this.Authors.Add(storyAuthor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>
        /// The authors.
        /// </value>
        /// <autogeneratedoc />
        [NotMapped]
        public ObservableCollection<StoryAuthor>? Authors
        {
            get
            {
                if (authors == null)
                {
                    authors = new ObservableCollection<StoryAuthor>(DataController.SandboxEntities.StoryAuthor.Where(x => x.StoryId == this.Id));
                }

                return authors;
            }

            set => this.RaiseAndSetIfChanged(ref authors, value);
        }

        /// <summary>
        /// Gets or sets the Breaks.
        /// </summary>
        [NotMapped]
        public BreakList Breaks
        {
            get
            {
                if (myBreaks == null)
                {
                    myBreaks = new BreakList();
                    if (WordHeadingList != null)
                    {
                        foreach (WordHeadings heading in WordHeadingList)
                        {
                            if (heading.HeadingLevel == 0)
                            {
                                string sec = heading.StringId.Substring(5);
                                int sectID = int.Parse(sec);
                                OpenXMLBreak newBreak = heading.ToBreak(sectID);

                                myBreaks.Add(newBreak);
                            }
                        }
                    }
                }
                return myBreaks;
            }
            set => myBreaks = value;
        }

        /// <summary>
        /// Gets or sets the Characters.
        /// </summary>
        public int? Characters { get => characters; set => this.RaiseAndSetIfChanged(ref characters, value); }

        /// <summary>
        /// Gets or sets the Codes.
        /// </summary>
        public string? Codes
        {
            get
            {
                if (!string.IsNullOrEmpty(codes))
                {
                    codes = codes.Replace("_x000d_", Environment.NewLine);
                    return codes;
                }

                else return string.Empty;
            }
            set => this.RaiseAndSetIfChanged(ref codes, value);
        }

        /// <summary>
        /// Gets or sets the CreatedString
        /// Gets the CreatedString..
        /// </summary>
        [NotMapped]
        public string? CreatedString
        {
            get
            {
                if (Creation != null)
                    return Creation.Value.ToString("yyyy-MM-dd");
                else return string.Empty;
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets or sets the Creation.
        /// </summary>
        public Nullable<System.DateTime> Creation
        {
            get => creation;
            set
            {
                this.RaiseAndSetIfChanged(ref creation, value);
                this.RaisePropertyChanged("CreatedString");
            }
        }

        /// <summary>
        /// Gets a value indicating whether Dirty.
        /// </summary>
        [NotMapped]
        public bool Dirty { get; private set; }

        /// <summary>
        /// Gets the DocumentExtn.
        /// </summary>
        [NotMapped]
        public string DocumentExtn
        {
            get
            {
                if (string.IsNullOrEmpty(documentExtn) && !string.IsNullOrEmpty(Path))
                {
                    documentExtn = System.IO.Path.GetExtension(Path).ToLower();
                }
                return documentExtn;
            }

            set { documentExtn = value; }
        }

        [NotMapped]
        public string? FixedPath
        {
            get
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    string? temp = SupportCore.MiscSupport.FixImagePath(Path);
                    return temp;
                }
                else return null;
            }
        }

        /// <summary>
        /// Gets or sets the Group.
        /// </summary>
        public string? Group { get => group; set => this.RaiseAndSetIfChanged(ref group, value); }

        /// <summary>
        /// Gets or sets the Heading1List.
        /// </summary>
        //[NotMapped]
        //public OpenXMLHeadingList Heading1List { get => myOpenXMLHeadingList1; set => this.RaiseAndSetIfChanged(ref myOpenXMLHeadingList1, value); }

        /// <summary>
        /// Gets or sets the Heading2List.
        /// </summary>
        //[NotMapped]
        //public OpenXMLHeadingList Heading2List { get => myOpenXMLHeadingList2; set => this.RaiseAndSetIfChanged(ref myOpenXMLHeadingList2, value); }

        /// <summary>
        /// Gets or sets the Heading3List.
        /// </summary>
        //[NotMapped]
        //public OpenXMLHeadingList Heading3List { get => myOpenXMLHeadingList3; set => this.RaiseAndSetIfChanged(ref myOpenXMLHeadingList3, value); }

        /// <summary>
        /// Gets the HeadingCount.
        /// </summary>
        public int? HeadingCount => WordHeadingList?.Count;

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public new int Id { get; set; }

        public int? IDAuthor
        {
            get => iDAuthor;

            set
            {
                if (value != null && value > 1 && value != IDAuthor)
                {
                    AuthorItem = DataController.AuthorList.FirstOrDefault(x => x.Id == value);

                    StoryAuthor? present = this.Authors.Where(a => a.Id == value).FirstOrDefault();

                    if (present == null)
                    {
                        StoryAuthor? newAuthor = DataController.SandboxEntities.InsertStoryAuthor(this.Id, value.Value);
                        if (newAuthor != null)
                        {
                            newAuthor.Save();
                            this.Authors.Add(newAuthor);
                            this.Save();
                        }
                    }
                }

                this.RaiseAndSetIfChanged(ref iDAuthor, value);

                iDAuthor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Language.
        /// </summary>

        public string? Json
        {
            get
            {
                return json;
            }

            set => json = value;
        }

        [NotMapped]
        public string Info { get => info; set => this.RaiseAndSetIfChanged(ref info, value); }

        private StoryInfo? storyInfo = null;
        private string info;
        private ObservableCollection<StoryCast> cast;

        [NotMapped]
        public ObservableCollection<StoryCast> Cast
        {
            get
            {
                if (this.cast == null)
                {
                    this.cast = new ObservableCollection<StoryCast>(new StoryCastList("", this.Id));
                }

                return this.cast;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref cast, value);
            }
        }

        [NotMapped]
        public StoryInfo? StoryInfo
        {
            get
            {
                if (storyInfo == null)
                {

                    storyInfo = new StoryInfo(Json, this);

                }
                return storyInfo;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref storyInfo, value);

                if (value != null)
                {
                    Json = value.ToJSON();
                }
            }
        }

        [NotMapped]
        public PhraseEntry? Language
        {
            get
            {
                if (language == null)
                {
                    if (string.IsNullOrEmpty(OriginalLanguage))
                    {
                        language = DataController.LanguageList.Find(x => x.Id == "UNKNOWN");
                    }
                    else
                        language = DataController.LanguageList.Find(x => x.Description == OriginalLanguage.Trim());
                }

                return language;
            }

            set
            {
                language = value;
                if (value != null)
                {
                    OriginalLanguage = value.Description;
                }
            }
        }

        /// <summary>
        /// Gets or sets the LastModified.
        /// </summary>
        public Nullable<System.DateTime> LastModified { get => lastModified; set => this.RaiseAndSetIfChanged(ref lastModified, value); }

        /// <summary>
        /// Gets or sets the Lines.
        /// </summary>
        public int? Lines { get => lines; set => this.RaiseAndSetIfChanged(ref lines, value); }

        /// <summary>
        /// Gets or sets the LowestAge.
        /// </summary>
        public string? LowestAge
        {
            get => lowestAge;
            set
            {
                this.RaiseAndSetIfChanged(ref lowestAge, value);
                if (!string.IsNullOrEmpty(value) && SupportCore.MiscSupport.IsNumeric(value)) LowestAgeInt = int.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets the LowestAgeInt
        /// Gets the LowestAgeInt......
        /// </summary>
        [NotMapped]
        public int LowestAgeInt
        {
            get
            {
                if (!string.IsNullOrEmpty(LowestAge) && SupportCore.MiscSupport.IsNumeric(LowestAge))
                {
                    lowestAgeInt = int.Parse(LowestAge);
                }
                else if (string.IsNullOrEmpty(LowestAge) && !string.IsNullOrEmpty(Age))
                {
                    List<int> intList = SupportCore.MiscSupport.StringIntListToIntList(age);

                    if (intList.Count > 0) LowestAge = intList.FirstOrDefault().ToString();
                    Save();
                }
                return lowestAgeInt;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref lowestAgeInt, value);
                lowestAge = value.ToString();
            }
        }

        /// <summary>
        /// Gets the ModifiedString.
        /// </summary>
        [NotMapped]
        public string? ModifiedString
        {
            get
            {
                if (LastModified != null)
                    return LastModified.Value.ToString("yyyy-MM-dd");
                else return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the OpenBookmarks.
        /// </summary>
        [NotMapped]
        public BookmarkList? OpenBookmarks { get => myopenBookmarks; set => myopenBookmarks = value; }

        /// <summary>
        /// Gets or sets the OriginalLanguage.
        /// </summary>
        public string? OriginalLanguage
        {
            get => originalLanguage;

            set
            {
                if (value == "English" || value == "Not Defined")
                    Translation = false;
                else Translation = true;

                this.RaiseAndSetIfChanged(ref originalLanguage, value);
            }
        }

        /// <summary>
        /// Gets or sets the Pages.
        /// </summary>
        public int? Pages { get => pages; set => this.RaiseAndSetIfChanged(ref pages, value); }

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>
        public string? Path { get => path; set => this.RaiseAndSetIfChanged(ref path, value); }

        /// <summary>
        /// Gets or sets the PathWrong.
        /// </summary>
        public bool? PathWrong { get => pathWrong; set => this.RaiseAndSetIfChanged(ref pathWrong, value); }

        public string? Percent { get => percent;  set => this.RaiseAndSetIfChanged(ref percent, value); }

        /// <summary>
        /// Gets or sets the Published.
        /// </summary>
        public string? Published { get => published; set => this.RaiseAndSetIfChanged(ref published, value); }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        /// <autogeneratedoc />
        public int? Score { get => score; set => this.RaiseAndSetIfChanged(ref score, value); }
        /// <summary>
        /// Gets or sets the SectionBreaks
        /// Gets the SectionBreaks..
        /// </summary>
        [NotMapped]
        public ObservableCollection<WordHeadings>? SectionBreaks
        {
            get
            {
                if (sectionBreaks == null) sectionBreaks = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 0).ToList());
                return sectionBreaks;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref sectionBreaks, value);
                sectionBreaks = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 0).ToList());
            }
        }

        /// <summary>
        /// Gets or sets the SeriesId.
        /// </summary>
        public int? SeriesId
        {
            get => seriesId;

            set
            {
                if (value != null)
                {
                    StorySeries? sSeries = DataController.StorySeriesList.Find(x => x.Id == value);
                    if (sSeries != null)
                    {
                        StorySeries = sSeries;
                    }

                }
                this.RaiseAndSetIfChanged(ref seriesId, value);
            }
        }

        /// <summary>
        /// Gets or sets the StorySeries.
        /// </summary>
        [ForeignKey("SeriesId")]
        public StorySeries? StorySeries { get => storySeries; set => this.RaiseAndSetIfChanged(ref storySeries, value); }
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string? Title
        {
            get
            {
                if (!string.IsNullOrEmpty(title))
                    return title;
                else return string.Empty;
            }
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        /// <summary>
        /// Gets or sets the Translation.
        /// </summary>
        public bool? Translation { get => translation; set => this.RaiseAndSetIfChanged(ref translation, value); }

        /// <summary>
        /// Gets or sets the WmlDocument.
        /// </summary>
        [NotMapped]
        public OpenXmlPowerTools.WmlDocument WmlDocument { get => myWmlDocument; set => myWmlDocument = value; }

        /// <summary>
        /// Gets or sets the WordHeadingList.
        /// </summary>
        public List<WordHeadings>? WordHeadingList
        {
            get
            {
                if (wordHeadingList == null || wordHeadingList.Count == 0)
                {
                    wordHeadingList = DataController.SandboxEntities.WordHeadings.Where(x => x.StoryId == this.Id).ToList();
                }

                return wordHeadingList;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref wordHeadingList, value);
                WordHeadings1 = new WordheadingsCollection(WordHeadingList?.Where(h => h.HeadingLevel == 1).Distinct(new WordHeadingsComparer()).ToList());
                WordHeadings2 = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 2).Distinct(new WordHeadingsComparer()).ToList());
                WordHeadings3 = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 3).Distinct(new WordHeadingsComparer()).ToList());
                SectionBreaks = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 0).OrderBy(x => x.StringId).Distinct(new WordHeadingsComparer()).ToList());
            }
        }

        /// <summary>
        /// Gets or sets the WordHeadings1
        /// Gets the WordHeadings1..
        /// </summary>
        [NotMapped]
        public WordheadingsCollection? WordHeadings1
        {
            get
            {
                if (wordHeadings1 == null || wordHeadings1.Count == 0)
                    wordHeadings1 = new WordheadingsCollection((WordHeadingList?.Where(h => h.HeadingLevel == 1).Distinct(new WordHeadingsComparer()).ToList()));
                //this.RaisePropertyChanged("WordHeadings1");
                return wordHeadings1;
            }
            set => this.RaiseAndSetIfChanged(ref wordHeadings1, value);
        }

        /// <summary>
        /// Gets or sets the WordHeadings2
        /// Gets the WordHeadings2..
        /// </summary>
        [NotMapped]
        public ObservableCollection<WordHeadings>? WordHeadings2
        {
            get
            {
                if (wordHeadings2 == null || wordHeadings2.Count == 0)
                    wordHeadings2 = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 2).Distinct(new WordHeadingsComparer()).ToList());
                return wordHeadings2;
            }
            set => this.RaiseAndSetIfChanged(ref wordHeadings2, value);
        }

        /// <summary>
        /// Gets or sets the WordHeadings3
        /// Gets the WordHeadings3..
        /// </summary>
        [NotMapped]
        public ObservableCollection<WordHeadings>? WordHeadings3
        {
            get
            {
                if (wordHeadings3 == null || wordHeadings3.Count == 0)
                    wordHeadings3 = new ObservableCollection<WordHeadings>(WordHeadingList?.Where(h => h.HeadingLevel == 3).Distinct(new WordHeadingsComparer()).ToList());
                return wordHeadings3;
            }
            set => this.RaiseAndSetIfChanged(ref wordHeadings3, value);
        }
        #endregion

        #region Methods

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns>The <see cref="Story"/>.</returns>
        public static Story Create()
        {
            Story newStory = new();

            try
            {
                newStory.Creation = DateTime.Now;
                newStory.Added = DateTime.Now;
                newStory.LastModified = DateTime.Now;
                newStory.IDAuthor = 1;
                newStory.SeriesId = 1;


                DataController.SandboxEntities.Story.Add(newStory);
                DataController.SandboxEntities.SaveChanges();
                newStory.Dirty = false;
            }
            catch (Exception ex)
            {

                string error = ex.ToString();
            }
            return newStory;
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="Story"/>.</returns>
        public static Story Create(string path)
        {
            Story newStory = Story.Create();

            // ensure we are storing the Window spath not the Linux virtual path
            newStory.Path = SupportCore.MiscSupport.FixPathBack(path).ToLower();

            string extn = System.IO.Path.GetExtension(path);


            if (extn == ".pdf")
            {

                var Pdf = PdfDocument.FromFile(path);
                newStory.Author = Pdf.MetaData.Author;
                newStory.Title = Pdf.MetaData.Title;
                newStory.Creation = Pdf.MetaData.CreationDate;
                newStory.Added = DateTime.Now;

            }
            else
            {
                WordProperties wordProperties = Story.GetProperties(path);

                if (wordProperties != null && wordProperties.Created != null)
                {
                    newStory.Added = DateTime.Now;
                    newStory.Title = wordProperties.Title;
                    newStory.Age = wordProperties.Age;
                    newStory.Author = wordProperties.Author;
                    //newStory.Creation = wordProperties.Created;
                    newStory.Published = wordProperties.Created.Value.Year.ToString();
                    newStory.LowestAge = wordProperties.LowestAge;
                    newStory.Codes = wordProperties.Keywords;
                }

            }
            newStory.Save();
            return newStory;
        }



        /// <summary>
        /// The GetProperties.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="WordProperties"/>.</returns>
        public static WordProperties GetProperties(string path)
        {
            WordProperties properties = OpenXML.GetProperties(path);
            return properties;
        }

        /// <summary>
        /// The CheckExists.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool CheckExists()
        {
            bool check = false;

            if (!string.IsNullOrEmpty(Path))
            {
                check = File.Exists(SupportCore.MiscSupport.FixImagePath(Path));

                if (!check)
                {
                    Path = SupportCore.MiscSupport.FixImagePath(Path);

                    if (AuthorItem != null && !string.IsNullOrEmpty(AuthorItem.StoryPath))
                    {
                        // change directory of path to match authoritem.storypath
                        string directory = Path.Substring(0, Path.LastIndexOf("\\"));
                        string filename = Path.Substring(Path.LastIndexOf("\\") + 1);
                        Path = AuthorItem.StoryPath + "\\" + filename;

                        check = File.Exists(SupportCore.MiscSupport.FixImagePath(Path));
                    }
                }
            }
            PathWrong = !check;

            return check;
        }

        /// <summary>
        /// The GetBreaksAndHeadings.
        /// </summary>
        public void GetBreaksAndHeadings()
        {
            // breaks

            WordprocessingDocument? document = wpDocument;

            if (document != null)
            {
                int sectID = 1;

                string sectId = "Sect_" + sectID.ToString("00");

                WordHeadings? currentSection = this.WordHeadingList.Where(h => h.StringId == sectId && h.HeadingLevel == 0).FirstOrDefault();

                WordHeadings? currentHeading1 = null;
                WordHeadings? currentHeading2 = null;
                WordHeadings? currentHeading3 = null;


                int heading1 = 0;
                int heading2 = 0;
                int heading3 = 0;

                var body = document?.MainDocumentPart?.Document.Body;



                int nPara = 0;

                if (body != null)
                {
                    IEnumerable<Paragraph> paras = body.Descendants<Paragraph>();

                    //  go through each paragraph in the body
                    foreach (Paragraph para in paras)
                    {
                        nPara++;

                        // look to see if we have a section break
                        IEnumerable<SectionProperties> elems = para.Descendants<SectionProperties>();
                        SectionProperties? paraelement = elems.FirstOrDefault<SectionProperties>();

                        if (paraelement != null)
                        {
                            // we have a section break, so we need to create a new section heading
                            sectID++;  // The first one found will mark the second section so increment first

                            // we have a section break
                            sectId = "Sect_" + sectID.ToString("00");
                            currentSection = this.WordHeadingList.Where(h => h.StringId == sectId && h.HeadingLevel == 0).FirstOrDefault();

                            if (currentSection == null)
                            {
                                currentSection = new WordHeadings()
                                {
                                    StoryId = this.Id,
                                    StringId = sectId,
                                    HeadingLevel = 0,
                                    HeadingText = sectId,
                                    PageNumber = nPara
                                };
                                currentSection.Insert();
                            }
                            // else we need to update paragraph number
                            else
                            {
                                currentSection.PageNumber = nPara;
                                currentSection.Update();
                            }
                        }

                        // look for headings they are indicated by a ParagraphStyleId with a value of Heading1, Heading2 or Heading3
                        ParagraphStyleId? paraStyle = para.Descendants<ParagraphStyleId>().FirstOrDefault<ParagraphStyleId>() as ParagraphStyleId;

                        if (paraStyle != null)
                        {
                            // need to look for heading
                            OpenXmlAttribute? attrib = paraStyle.GetAttribute("val", w.NamespaceName);
                            if (attrib != null)
                            {
                                if (attrib.Value.Value == "Heading1")
                                {
                                    // we have a heading 1; first null out headings 2 and 3 and set counters to 1
                                    currentHeading2 = null;
                                    currentHeading3 = null;
                                    heading2 = 0;
                                    heading3 = 0;
                                    heading1++;


                                    string text = ParagraphText(para);
                                    string string_id = "Heading1_" + heading1.ToString("00");

                                    currentHeading1 = this.WordHeadingList.Where(h => h.StringId.ToLower() == string_id.ToLower()).FirstOrDefault();

                                    if (currentHeading1 == null)
                                    {
                                        currentHeading1 = new WordHeadings()
                                        {
                                            StoryId = this.Id,
                                            HeadingLevel = 1,
                                            StringId = string_id,
                                            HeadingText = text,
                                            PageNumber = nPara
                                        };
                                        WordHeadingList.Add(currentHeading1);
                                    }


                                    else if (currentHeading1.PageNumber != nPara)
                                    {
                                        currentHeading1.HeadingText = text;
                                        currentHeading1.PageNumber = nPara;
                                        currentHeading1.Update();
                                    }

                                    // look for section
                                    //currentSection = this.WordHeadingList.Where(h => h.StringId.ToLower() == sectId.ToLower() && h.HeadingLevel == 0).FirstOrDefault();

                                    if (currentSection != null)
                                    {
                                        if (currentHeading1.ParentId is 0 )
                                        {
                                            // this is a new heading so set the parent to the current section

                                            currentHeading1.ParentId = currentSection.Id;
                                            currentHeading1.Update();
                                        }

                                        // see if this heading is in section.children list
                                        WordHeadings? temp = currentSection.Children.FirstOrDefault(h => h.StringId.ToLower() == currentHeading1.StringId.ToLower() && h.HeadingLevel == 1);

                                        if (temp == null) currentSection.Children.Add(currentHeading1);
                                    }

                                }  // heading 1 complete
                                else if (attrib.Value.Value == "Heading2")
                                {
                                    currentHeading3 = null; // clear out possible heading 3s
                                    heading3 = 0; // reset heading 3 counter
                                    heading2++;
                                    // we have a heading 2 increment heading 2 counter

                                    string text = ParagraphText(para);
                                    string string_id = "Heading2_" + heading1.ToString("00") + "_" + heading2.ToString("00");

                                    // see if already found
                                    currentHeading2 = this.WordHeadingList.Where(h => h.StringId.ToLower() == string_id.ToLower()).FirstOrDefault();
                                    if (currentHeading2 == null)
                                    {
                                        currentHeading2 = new WordHeadings()
                                        {
                                            StoryId = this.Id,
                                            HeadingLevel = 2,
                                            StringId = string_id,
                                            HeadingText = text,
                                            PageNumber = nPara
                                        };
                                        WordHeadingList.Add(currentHeading2);
                                    }


                                    else if (currentHeading2.PageNumber != nPara || currentHeading2.HeadingText != text)
                                    {
                                        currentHeading2.HeadingText = text;
                                        currentHeading2.PageNumber = nPara;
                                        currentHeading2.Update();
                                    }
                                    // look for section
                                    //currentSection = this.WordHeadingList.Where(h => h.StringId.ToLower() == sectId.ToLower() && h.HeadingLevel == 0).FirstOrDefault();
                                    if (currentHeading1 != null)
                                    {
                                        if (currentHeading2.ParentId == 0 || currentHeading2.ParentId == null)
                                        {
                                            // this is a new heading so set the parent to the current heading 1

                                            currentHeading2.ParentId = currentHeading1.Id;
                                            currentHeading2.Update();
                                        }

                                        // see if this heading is in section.children list
                                        WordHeadings? temp = currentHeading1.Children.FirstOrDefault(h => h.StringId.ToLower() == currentHeading2.StringId.ToLower() && h.HeadingLevel == 2);
                                        // then add it if null
                                        if (temp == null) currentHeading1.Children.Add(currentHeading2);

                                    }

                                } // heading 2 complete
                                else if (attrib.Value.Value == "Heading3")
                                {
                                    heading3++;

                                    string text = ParagraphText(para);
                                    string string_id = "Heading3_" + heading1.ToString("00") + "_" + heading2.ToString("00") + "_" + heading3.ToString("00");
                                    currentHeading3 = this.WordHeadingList.Where(h => h.StringId.ToLower() == string_id.ToLower()).FirstOrDefault();
                                    if (currentHeading3 == null)
                                    {
                                        currentHeading3 = new WordHeadings()
                                        {
                                            StoryId = this.Id,
                                            HeadingLevel = 3,
                                            StringId = string_id,
                                            HeadingText = text,
                                            PageNumber = nPara
                                        };
                                        WordHeadingList.Add(currentHeading3);
                                    }
                                    else
                                    {
                                        currentHeading3.HeadingText = text;
                                        currentHeading3.PageNumber = nPara;
                                        currentHeading3.Update();
                                    }
                                    // look for section

                                    if (currentHeading2 != null)
                                    {
                                        currentHeading3.ParentId = currentHeading2.Id;
                                        currentHeading3.Update();
                                        // see if this heading is in section.children list
                                        WordHeadings? temp = currentHeading2.Children.FirstOrDefault(h => h.StringId.ToLower() == currentHeading3.StringId.ToLower() && h.HeadingLevel == 2);
                                        // then add it if null
                                        if (temp == null) currentHeading2.Children.Add(currentHeading3);

                                    }
                                }
                            }

                        }
                        // GetHeadings(ref heading1, ref heading2, ref heading3, para, nPara);

                        //sectID = GetBreaks(sectID, para);
                    }
                }
            }
        }

        /// <summary>
        /// The GetPropertiesFromDocument.
        /// </summary>
        /// <returns>The <see cref="WordProperties"/>.</returns>
        public WordProperties GetPropertiesFromDocument()
        {
            WordProperties? properties = null;
            if (!string.IsNullOrEmpty(FixedPath) && File.Exists(FixedPath))
            {
                properties = OpenXML.GetProperties(SupportCore.MiscSupport.FixImagePath(FixedPath));

                if (properties != null)
                {
                    if (!string.IsNullOrEmpty(properties.LowestAge)) LowestAge = properties.LowestAge;
                    if (!string.IsNullOrEmpty(properties.Age)) Age = properties.Age;
                    if (!string.IsNullOrEmpty(properties.Lines)) Lines = int.Parse(properties.Lines);
                    if (!string.IsNullOrEmpty(properties.Pages)) Pages = int.Parse(properties.Pages);
                    if (!string.IsNullOrEmpty(properties.Percent)) Percent = properties.Percent;
                    if (!string.IsNullOrEmpty(properties.Characters)) Characters = int.Parse(properties.Characters);

                    if (!string.IsNullOrEmpty(properties.Keywords)) Codes = properties.Keywords;
                    if (!string.IsNullOrEmpty(properties.Author)) Author = properties.Author.TrimEnd('\r', '\n').Replace("_x000d_", "");
                    if (!string.IsNullOrEmpty(properties.Title)) Title = properties.Title.TrimEnd('\r', '\n').Replace("_x000d_", "");
                    //  replace ,1st with :1st
                    if (!string.IsNullOrEmpty(Codes) && Codes.Contains(",1st")) Codes = Codes.Replace(",1st", ":1st");

                    if (Pages != null) this.StoryInfo.TotalPages = Pages.Value;
                    this.Info = this.StoryInfo.ToInfo();
                }
            }

            return properties;
        }

        /// <summary>
        /// The OpenDocument.
        /// </summary>
        public void OpenDocument()
        {
            IEnumerable<BookmarkStart>? bookmarks = null;

            if (!string.IsNullOrEmpty(Path))
            {
                string tempPath = SupportCore.MiscSupport.FixImagePath(Path);
                if (WmlDocument == null && File.Exists(tempPath))
                {
                    OpenBookmarks = new BookmarkList();
                    //Breaks = new BreakList();
                    WmlDocument = new OpenXmlPowerTools.WmlDocument(tempPath);
                    using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(WmlDocument))
                    using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                    {
                        wpDocument = document;
                        bookmarks = GetBookmarks(document);

                        GetBreaksAndHeadings();
                    }

                    //if (Heading1List != null)
                    //{
                    //    GetHeadingsFromList(Heading1List);

                    //    this.RaiseAndSetIfChanged(ref myOpenXMLHeadingList1, Heading1List, "Heading1List");
                    //}

                    //if (Heading2List != null)
                    //{
                    //    GetHeadingsFromList(Heading2List);
                    //}

                    //if (Heading3List != null)
                    //{
                    //    GetHeadingsFromList(Heading3List);
                    //}

                    //if (Breaks != null)
                    //{
                    //    GetHeadingsFromList(Breaks);
                    //}
                }
            }
        }

        /// <summary>
        /// The SetProperties.
        /// </summary>
        /// <param name="currentProperties">The currentProperties<see cref="WordProperties"/>.</param>
        public void SetProperties(WordProperties currentProperties)
        {
            if (currentProperties != null)
            {
                if (currentProperties.Age != null && currentProperties.Age.Trim().Length > 0 && Age != currentProperties.Age)
                {
                    Age = currentProperties.Age;
                    Dirty = true;
                }

                if (!string.IsNullOrEmpty(currentProperties.Characters))
                {
                    if (int.TryParse(currentProperties.Characters, out int temp))
                    {
                        Characters = temp;
                    }
                }

                if (!string.IsNullOrEmpty(currentProperties.Lines))
                {
                    if (int.TryParse(currentProperties.Lines, out int temp))
                    {
                        Lines = temp;
                    }
                }

                if (!string.IsNullOrEmpty(currentProperties.Pages))
                {
                    if (int.TryParse(currentProperties.Pages, out int temp))
                    {
                        Pages = temp;
                    }
                }

                if (currentProperties.Keywords != null && currentProperties.Keywords.Trim().Length > 0 && Codes != currentProperties.Keywords)
                {
                    Codes = currentProperties.Keywords;
                    Dirty = true;
                }

                if (!string.IsNullOrEmpty(currentProperties.Title) && Title != currentProperties.Title)
                {
                    Title = SupportCore.MiscSupport.Capitalise(currentProperties.Title);
                    Dirty = true;
                }

                if (!string.IsNullOrEmpty(currentProperties.Author) && Author != currentProperties.Author)
                {
                    Author = currentProperties.Author;
                    Dirty = true;
                }

                // check modified on
                if (LastModified == DateTime.MinValue && currentProperties.Modified != null)
                {
                    LastModified = (DateTime)currentProperties.Modified;
                    Dirty = true;
                }

                // check creation
                if (Creation == DateTime.MinValue && currentProperties.Created != null)
                {
                    Creation = (DateTime)currentProperties.Created;
                    Dirty = true;
                }

                if (String.IsNullOrEmpty(LowestAge) && !string.IsNullOrEmpty(currentProperties.LowestAge))
                {
                    LowestAge = currentProperties.LowestAge;
                    Dirty = true;
                }

                if (Dirty) Save();
                Dirty = false;
            }
        }

        /// <summary>
        /// The ToHtml.
        /// </summary>
        /// <returns>The <see cref="XElement"/>.</returns>
        public XElement ToHtml()
        {

            OpenDocument();
            XElement? structuredHTML = null;
            if (WmlDocument != null)
            {
                OpenXmlPowerTools.WmlToHtmlConverterSettings converterSettings = new OpenXmlPowerTools.WmlToHtmlConverterSettings()
                {
                    FabricateCssClasses = true,
                    CssClassPrefix = "pt-",
                    PageTitle = this.Title,
                    RestrictToSupportedLanguages = false,
                    RestrictToSupportedNumberingFormats = false,
                };
                structuredHTML = WmlDocument.ConvertToHtml(converterSettings);

                string navigation = string.Empty;

                if (SectionBreaks != null && SectionBreaks.Count > 0)
                {
                    navigation += div + "<ul>" + Environment.NewLine + "<li>";
                    foreach (var item in SectionBreaks)
                    {
                        navigation += "<a href=" + '"' + "#" + item.StringId + '"' + ">" + item.HeadingText + "</a></li>" + Environment.NewLine + "<li>";
                    }
                    navigation = navigation.Substring(0, navigation.Length - 4);
                    navigation += "</ul>" + divEnd + "<br/>" + Environment.NewLine;

                }


                if (WordHeadings1 != null && WordHeadings1.Count > 0)
                {
                    navigation += div + "<ul>" + Environment.NewLine + "<li>";

                    foreach (var item in WordHeadings1)
                    {
                        navigation += "<a href=" + '"' + "#" + item.StringId.Replace("_0", "_").ToLower() + '"' + ">" + item.HeadingText + "</a></li>" + Environment.NewLine + "<li>";
                    }

                    navigation = navigation.Substring(0, navigation.Length - 4);

                    navigation += "</ul>" + divEnd + "<br/>" + Environment.NewLine;
                }

                //if (Heading2List != null && Heading2List.Count > 0)
                //{
                //    navigation += div + "<ul>" + Environment.NewLine + "<li>";

                //    foreach (var item in Heading2List)
                //    {
                //        navigation += "<a href=" + '"' + "#" + item.StringID + '"' + "> Section " + item.Index.ToString() + "</a></li>" + Environment.NewLine + "<li>";
                //    }

                //    navigation = navigation.Substring(0, navigation.Length - 4);

                //    navigation += "</ul>" + divEnd + "<br/>" + Environment.NewLine;
                //}

                structuredHTML = XElement.Parse(structuredHTML.ToString().Replace("<body>", "<body>" + navigation));
                structuredHTML = XElement.Parse(structuredHTML.ToString().Replace("</style>", LiStyle + Environment.NewLine + UlStyle + Environment.NewLine + "</style>"));
            }
            else
            {
                PathWrong = true;
                Save();
                structuredHTML = XElement.Parse("<html><body>Document not found</body></html>");
            }
            return structuredHTML;
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        public void Delete()
        {
            var local = DataController.SandboxEntities.Set<Story>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
               // DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            // set Modified flag in your entry


            DataController.SandboxEntities.DeleteStory(Id);
            // var rowsDeleted = DataController.SandboxEntities.Database.ExecuteSqlRaw("DELETE FROM [WordHeadings] WHERE ([StoryId] = @Original_Id)", Original_Id);

            //var rowsReturned = DataController.SandboxEntities.Database.ExecuteSqlRaw("DELETE FROM [Stories] WHERE ([Id] = @Original_Id)", Original_Id);

            //DataController.SandboxEntities.Story.Remove(this);
            //DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        public void Insert()
        {
            DataController.SandboxEntities.Story.Add(this);
            int rowschanged = DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            if (Added == null)
            {
                Added = DateTime.Today;
            }

            if (this.Id < 1) this.Insert();

            Json = StoryInfo?.ToJSON();

            if (Score == null) Score = 1;

            // find file creation time from directory
            if (Creation == null && Path != null)
            {
                System.IO.FileInfo info = new System.IO.FileInfo(this.Path);

                Creation = info.CreationTime;
            }

            //if (DataController.SandboxEntities.Entry(this).State == EntityState.Unchanged)
            //{
            LastModified = DateTime.Now;

            var local = DataController.SandboxEntities.Set<Story>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            // set Modified flag in your entry
            try
            {
                DataController.SandboxEntities.Entry(this).State = EntityState.Modified;
                int rowschanged = DataController.SandboxEntities.SaveChanges();
                if (rowschanged >= 1)
                {
                    Debug.WriteLine("saved ok : " + Id.ToString() + " rows changed " + rowschanged.ToString());
                }

                else if (rowschanged == 0)
                {
                    Debug.WriteLine("saved failed : " + Id.ToString());
                }
                else
                {
                    Debug.WriteLine("saved ok : " + Id.ToString());
                }
            }
            catch (Exception e)
            {

                string error = e.ToString();
            }
            //}
            Dirty = false;
        }

        public async Task<bool> SaveAsync()
        {
            bool success = false;
            if (Added == null)
            {
                Added = DateTime.Today;
            }

            if (this.Id < 1) this.Insert();

            Json = StoryInfo?.ToJSON();

            if (Score == null) Score = 1;

            // find file creation time from directory
            if (Creation == null && Path != null)
            {
                System.IO.FileInfo info = new System.IO.FileInfo(this.Path);

                Creation = info.CreationTime;
            }

            //if (DataController.SandboxEntities.Entry(this).State == EntityState.Unchanged)
            //{
            LastModified = DateTime.Now;

            var local = DataController.SandboxEntities.Set<Story>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            // set Modified flag in your entry
            try
            {
                 DataController.SandboxEntities.Entry(this).State = EntityState.Modified;
                int rowschanged = await DataController.SandboxEntities.SaveChangesAsync();
                if (rowschanged >= 1)
                {
                    success = true;
                    Debug.WriteLine("saved ok : " + Id.ToString() + " rows changed " + rowschanged.ToString());
                }

                else if (rowschanged == 0)
                {
                    Debug.WriteLine("saved failed : " + Id.ToString());
                }
                else
                {
                    Debug.WriteLine("saved ok : " + Id.ToString());
                }
            }
            catch (Exception e)
            {

                string error = e.ToString();
            }
            //}
            Dirty = false;

            return success;
        }

        /// <summary>
        /// The ParagraphText.
        /// </summary>
        /// <param name="para">The para<see cref="Paragraph"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string ParagraphText(Paragraph para)
        {
            IEnumerable<Text> Texts = para.Descendants<Text>();
            string text = "";
            foreach (Text txt in Texts)
            {
                text = text + txt.Text;
            }

            return text;
        }

        /// <summary>
        /// The GetBookmarks.
        /// </summary>
        /// <param name="document">The document<see cref="WordprocessingDocument"/>.</param>
        /// <returns>The <see cref="IEnumerable{BookmarkStart}"/>.</returns>
        private IEnumerable<BookmarkStart> GetBookmarks(WordprocessingDocument document)
        {
            // XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            IEnumerable<BookmarkStart>? bookmarks = document?.MainDocumentPart?.Document?.Body?.Descendants<BookmarkStart>();
            if (bookmarks != null && OpenBookmarks != null)
            {
                // now process bookmarks
                foreach (OpenXmlElement bmStart in bookmarks)
                {
                    BookmarkEnd? bmEnd = null;
                    BookmarkStart? bookmarkStart = null;

                    bookmarkStart = bmStart as BookmarkStart;

                    if (bookmarkStart != null && OpenBookmarks != null)
                    {
                        OpenXmlBookmark? newBookmark = OpenBookmarks?.Find(x => x?.Start?.Name?.ToString()?.ToUpper() == bookmarkStart?.Name?.ToString()?.ToUpper());


                        ////If the bookmark name is not in our list. Just continue with the loop


                        if (newBookmark == null && OpenBookmarks != null)
                        {
                            newBookmark = new OpenXmlBookmark();
                            newBookmark.Start = bookmarkStart;
                            newBookmark.SectionType = OpenXML.DocumentSection.Main;
                            newBookmark.BookmarkIndex = OpenBookmarks.Count + 1;
                            OpenBookmarks.Add(newBookmark);

                            MainDocumentPart? mainDocumentPart = document?.MainDocumentPart;
                            //object section = mainDocumentPart;

                            if (mainDocumentPart != null)
                            {
                                bmEnd = mainDocumentPart?.Document?.Body?.Descendants<BookmarkEnd>().Where(b => b.Id == bookmarkStart?.Id?.ToString()).FirstOrDefault();
                                if (bmEnd != null)
                                {
                                    newBookmark.End = bmEnd;
                                }
                            }
                        }
                    }
                }
            }

            return bookmarks;
        }

        ///// <summary>
        ///// The GetBreaks.
        ///// </summary>
        ///// <param name="sectID">The sectID<see cref="int"/>.</param>
        ///// <param name="para">The para<see cref="Paragraph"/>.</param>
        ///// <returns>The <see cref="int"/>.</returns>
        //private int GetBreaks(int sectID, Paragraph para)
        //{
        //    IEnumerable<SectionProperties> elems = para.Descendants<SectionProperties>();
        //    foreach (SectionProperties elem in elems)
        //    {
        //        SectionProperties paraelement = elem;
        //        OpenXMLBreak openXMLBreak = new OpenXMLBreak();
        //        openXMLBreak.BreakID = sectID;
        //        openXMLBreak.StringId = "Sect_" + sectID.ToString("00").Trim();
        //        Breaks.Add(openXMLBreak);

        //        WordHeadings? newHeading = this.wordHeadingList?.Find(h => h.StringId.ToLower() == openXMLBreak.StringId.ToLower());

        //        OpenXmlElement openXml = elem.NextSibling();

        //        // add new heading and section break if not already in list
        //        if (newHeading == null)
        //        {
        //            newHeading = new WordHeadings
        //            {
        //                StoryId = Id,
        //                StringId = openXMLBreak.StringId,
        //                HeadingLevel = 0,
        //                HeadingText = openXMLBreak.StringId
        //            };
        //            newHeading.Insert();
        //            if (WordHeadingList != null) WordHeadingList.Add(newHeading);
        //            if (SectionBreaks != null)
        //            {
        //                SectionBreaks.Add(newHeading);
        //            }
        //        }

        //        sectID += 1;

        //    }

        //    return sectID;
        //}

        /// <summary>
        /// The GetHeadings.
        /// </summary>
        /// <param name="heading1">The heading1<see cref="int"/>.</param>
        /// <param name="heading2">The heading2<see cref="int"/>.</param>
        /// <param name="heading3">The heading3<see cref="int"/>.</param>
        /// <param name="para">The para<see cref="Paragraph"/>.</param>
        private void GetHeadings(ref int heading1, ref int heading2, ref int heading3, Paragraph para, int npara)
        {
            if (para != null)
            {
                ParagraphStyleId? paraStyle = para.Descendants<ParagraphStyleId>().FirstOrDefault<ParagraphStyleId>() as ParagraphStyleId;

                if (paraStyle != null)
                {
                    // need to look for heading
                    OpenXmlAttribute? attrib = paraStyle.GetAttribute("val", w.NamespaceName);
                    if (attrib != null)
                    {


                        if (attrib.Value.Value == "Heading1")
                        {
                            string text = ParagraphText(para);
                            if (!string.IsNullOrEmpty(text))
                            {
                                // add a new heading to list
                                OpenXMLHeading newHeading = new OpenXMLHeading();
                                newHeading.Index = heading1;
                                newHeading.HeadingLevel = 1;
                                newHeading.StringID = "heading1_" + heading1.ToString().Trim();
                                newHeading.PageNumber = npara;

                                //if (Heading1List.Where(x => x.Text == text) == null)
                                //{
                                //    newHeading.Text = text;
                                //}

                                heading1 += 1;
                                // Heading1List.Add(newHeading);
                            }

                        }
                        else if (attrib.Value.Value == "Heading2")
                        {

                            string text = ParagraphText(para);
                            if (!string.IsNullOrEmpty(text))
                            {
                                // add a new heading to list
                                OpenXMLHeading newHeading = new OpenXMLHeading();
                                newHeading.Index = heading2;
                                newHeading.HeadingLevel = 2;
                                newHeading.StringID = "heading2_" + heading2.ToString().Trim();
                                newHeading.PageNumber = npara;

                                //if (Heading2List.Where(x => x.Text == text) == null)
                                //{
                                //    newHeading.Text = text;
                                //    heading2 += 1;
                                //    Heading2List.Add(newHeading);
                                //}
                            }
                        }
                        else if (attrib.Value.Value == "Heading3")
                        {
                            string text = ParagraphText(para);
                            if (!string.IsNullOrEmpty(text))
                            {
                                // add a new heading to list
                                OpenXMLHeading newHeading = new OpenXMLHeading();
                                newHeading.Index = heading3;
                                newHeading.HeadingLevel = 3;
                                newHeading.StringID = "heading3_" + heading3.ToString().Trim();
                                newHeading.PageNumber = npara;

                                //if (Heading3List.Where(x => x.Text == text) == null)
                                //{
                                //    newHeading.Text = text;
                                //    heading3 += 1;
                                //    Heading3List.Add(newHeading);
                                //}
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The GetHeadingsFromList.
        /// </summary>
        /// <param name="headingList">The headingList<see cref="BreakList"/>.</param>
        private void GetHeadingsFromList(BreakList headingList)
        {
            foreach (var item in headingList)
            {
                WordHeadings? headings = WordHeadingList?.Find(h => h.StringId == item.StringId);

                if (headings == null)
                {
                    headings = new()
                    {
                        StoryId = Id,
                        StringId = item.StringId,
                        HeadingLevel = 0,
                        HeadingText = item.StringId
                    };

                    headings.Insert();
                    if (WordHeadingList != null) WordHeadingList.Add(headings);
                }
            }
        }

        /// <summary>
        /// /// The GetHeadingsFromList.
        /// </summary>
        /// <param name="headingList">The headingList<see cref="OpenXMLHeadingList"/>.</param>
        private void GetHeadingsFromList(OpenXMLHeadingList headingList)
        {
            foreach (var item in headingList)
            {
                WordHeadings? headings = WordHeadingList?.Find(h => h.StringId == item.StringID);

                if (headings == null)
                {
                    headings = new()
                    {
                        StoryId = Id,
                        StringId = item.StringID,
                        HeadingLevel = item.HeadingLevel,
                        HeadingText = item.Text
                    };

                    headings.Insert();
                    if (WordHeadingList != null) WordHeadingList.Add(headings);
                }
            }
        }

        internal void OpenDocumentDoc()
        {
            // create an instance of Word application
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            // open the document
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(SupportCore.MiscSupport.FixImagePath(Path));
            // read built in properties
            object builtInProps = doc.BuiltInDocumentProperties;
            // get categories
            var builtInPropsType = builtInProps.GetType();
            var categoryProp = builtInPropsType.InvokeMember("Item", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, builtInProps, new object[] { Microsoft.Office.Interop.Word.WdBuiltInProperty.wdPropertyCategory });
            var categoryValue = categoryProp.GetType().InvokeMember("Value", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, categoryProp, null);
            var categories = categoryValue;
            // close the document
            // set codes = to categories
            Codes = categories.ToString();
            doc.Close();
            // quit word application
            wordApp.Quit();


        }

        /// <summary>
        /// Build Code List from Cast Members
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 18/01/2026 18/01/2026 </created>
        /// </remarks>
        public void BuildCodesFromCast()
        {
            if (cast != null && cast.Count > 0)
            {
                string tempCodes = string.Empty;
                foreach (var member in cast)
                {
                    // add id: + CastId + ; to tempCodes
                    tempCodes += "id:" + member.CastId + "; ";
                    // add Codes  to tempCodes
                    if (!string.IsNullOrEmpty(member.Codes))
                    {
                        tempCodes += member.Codes + "; ";
                    }
                    // add a space
                    tempCodes += " ";
                }
                // add closeing 'Id:;' to tempCodes
                tempCodes += "Id:;";
                // set codes to tempCodes
                Codes = string.Join(", ", tempCodes);
            }

        }

        public void UpdateCastMember(StoryCast currentCastMember)
        {
            if (currentCastMember != null)
            {
                // see if already in list
                StoryCast? existingCastMember = this.cast.Where(c => c.Id == currentCastMember.Id).FirstOrDefault();
                if (existingCastMember == null)
                {
                    // add new
                    this.cast.Add(currentCastMember);
                }
                else
                {
                    // update existing
                    existingCastMember.Character = currentCastMember.Character;
                    existingCastMember.Age = currentCastMember.Age;
                    existingCastMember.CastId = currentCastMember.CastId;
                    existingCastMember.Codes = currentCastMember.Codes;
                    existingCastMember.Update();
                }
            }
        }

        //internal async void EditCastMember(StoryCast currentCastMember, StoryViewModel viewModel)
        //{
        //    Dialogs.EditCastMemberDialog? EditCastMemberDialog = new();

        //    if (EditCastMemberDialog != null)
        //    {
        //        StoryCast oldCastMember = viewModel.CurrentCastMember;
        //        // give dialog access to StoryViewModel
        //        EditCastMemberDialog.DataContext = viewModel;

        //        // Set the Accept and Cancel buttons to the ViewModel (actually DialogModelBase)
        //        EditCastMemberDialog.OkButtonPanelEditMovie.OkButton.Command = viewModel.Accept;
        //        EditCastMemberDialog.OkButtonPanelEditMovie.CancelButton.Command = viewModel.Cancel;

        //        // find the Mian Window and use that to host the dialogue
        //        Views.MainWindow? mainWindow = Support.Support.GetMainWindow();
        //        if (mainWindow != null)
        //        {
        //            viewModel.Caller = EditCastMemberDialog;
        //            await EditCastMemberDialog.ShowDialog(mainWindow);

        //            // The view Model will contain the result button, if ok save the changes
        //            if (viewModel.resultButton != null && viewModel.resultButton.Result == Models.DialogResultButton.ResultType.Ok)
        //            {
        //                viewModel.CurrentCastMember.Update();
        //            }
        //            else
        //                viewModel.CurrentCastMember = oldCastMember;
        //        }
        //    }
        //}

        public void DeleteCastMember(StoryCast currentCastMember)
        {
            //throw new NotImplementedException();
            if (currentCastMember != null)
            {
                currentCastMember.Delete();
                // set Modified flag in your entry
                this.cast.Remove(currentCastMember);
            }
        }

        /// <summary>
        /// Create a new Cast Member and save in database, set CastId to cast.Count
        /// </summary>
        /// <param name="currentCastMember">The current cast member.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 18/01/2026 18/01/2026 </created>
        /// </remarks>
        public void AddCastMember(StoryCast currentCastMember)
        {
            // create new cast member
            if (currentCastMember != null)
            {
                currentCastMember.StoryId = this.Id;
                currentCastMember.Insert();
                this.cast.Add(currentCastMember);
                // set cast id to cast.Count
                currentCastMember.CastId = this.cast.Count;
                currentCastMember.Update();
            }
        }

        #endregion
    }
}
