//-----------------------------------------------------------------------
// <copyright file="WordXML.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>01/02/2018 14:58:08 01/02/2018 14:58:08 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace TaymadeEntities.Support.Word
{
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.CustomProperties;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.VariantTypes;
    using DocumentFormat.OpenXml.Wordprocessing;
    using OpenXmlPowerTools;
    using System;
    using System.Collections.Generic;
    using System.IO;
    //using System.IO.Abstractions;
    using System.IO.Packaging;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    
    using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;

    /// <summary>
    /// Defines the <see cref="WordProperties" />.
    /// </summary>
    public class WordProperties
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Added value...
        /// </summary>
        public DateTime Added { get; set; }

        /// <summary>
        /// Gets or sets the Age.
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// Gets or sets the Author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the Characters.
        /// </summary>
        public string Characters { get; set; }

        /// <summary>
        /// Gets or sets the Codes.
        /// </summary>
        public string Codes { get; set; }

        /// <summary>
        /// Gets or sets the Created value...
        /// </summary>
        public DateTime? Created { get; set; }

        public int? DocumentId { get; set; }

        public string Keywords
        {
            get { return Codes; }
            set { Codes = value; }
        }
        /// <summary>
        /// Gets or sets the Lines.
        /// </summary>
        public string Lines { get; set; }

        public string? LowestAge { get; set; }

        /// <summary>
        /// Gets or sets the Modified value...
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Gets or sets the Notes.
        /// </summary>
        public string? Notes { get; set; }

        public string? Subject { get; set; }

        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the Pages.
        /// </summary>
        public string? Pages { get; set; }

        public string? Percent { get; set; }

        public string? Published { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string? Title { get; set; }
        #endregion
    }

    /// <summary>
    /// Defines the <see cref="WordXML" />.
    /// </summary>
    public class WordXML
    {
        #region Fields

        /// <summary>
        /// Defines the wordDocument.
        /// </summary>
        public static WordprocessingDocument wordDocument = null;

        /// <summary>
        /// Defines the w.
        /// </summary>
        internal static XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        /// <summary>
        /// Defines the myBookmarks.
        /// </summary>
        private static BookmarkList myBookmarks;

        /// <summary>
        /// Defines the myBreaks.
        /// </summary>
        private static BreakList myBreaks;

        /// <summary>
        /// Defines the myOpenXMLHeadingList1.
        /// </summary>
        private static OpenXMLHeadingList myOpenXMLHeadingList1;

        /// <summary>
        /// Defines the myWmlDocument.
        /// </summary>
        private static OpenXmlPowerTools.WmlDocument myWmlDocument;

        /// <summary>
        /// Defines the wordPackage.
        /// </summary>
        private static Package wordPackage = null;

        #endregion

        #region Constants
        /// <summary>
        /// The styles path
        /// </summary>

        public const string stylesPath = "C:\\Users\\doug\\AppData\\Roaming\\Microsoft\\Templates\\styles.docx";

        /// <summary>
        /// The templates
        /// </summary>
        public const string Templates = @"C:\Users\doug\AppData\Roaming\Microsoft\Templates\";

        #endregion Constants

        #region Enums

        /// <summary>
        /// Property types
        /// </summary>
        public enum PropertyTypes : int
        {
            /// <summary>
            /// The yes no
            /// </summary>
            YesNo,
            /// <summary>
            /// The text
            /// </summary>
            Text,
            /// <summary>
            /// The date time
            /// </summary>
            DateTime,
            /// <summary>
            /// The number integer
            /// </summary>
            NumberInteger,
            /// <summary>
            /// The number double
            /// </summary>
            NumberDouble
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Bookmarks.
        /// </summary>
        public static BookmarkList Bookmarks { get => myBookmarks; set => myBookmarks = value; }

        /// <summary>
        /// Gets or sets the BreakList.
        /// </summary>
        public static BreakList BreakList { get => myBreaks; set => myBreaks = value; }

        /// <summary>
        /// Gets or sets the WmlDocument.
        /// </summary>
        public static OpenXmlPowerTools.WmlDocument WmlDocument { get => myWmlDocument; set => myWmlDocument = value; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="paragraphText">The paragraph text.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Body AppendTextToBody(Body body, string paragraphText)
        {
            // split the paragraph text into paragraphs based on new lines
            string[] paragraphs = paragraphText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string para in paragraphs)
            {
                body.Append(CreateParagraph(para));
            }
            return body;
        }

        /// <summary>
        /// </summary>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Body CreateBodyFromParagraphs(List<Paragraph> paragraphs)
        {
            Body body = new Body();
            foreach (Paragraph para in paragraphs)
            {
                body.Append(para);
            }
            return body;
        }

        /// <summary>
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Body AppendStringArrayToBody(Body body, string[] paragraphs)
        {
            foreach (string para in paragraphs)
            {
                body.Append(CreateParagraph(para));
            }
            return body;
        }

        /// <summary>
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="filePath">The file path.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 15/12/2025 15/12/2025 </created>
        /// </remarks>
        public static void AppendTextFileToBody(Body body, string filePath)
        {
            // read the text file
            string paragraphText = File.ReadAllText(filePath);
            // split the paragraph text into paragraphs based on new lines
            string[] paragraphs = paragraphText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string para in paragraphs)
            {
                body.Append(CreateParagraph(para));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="paragraphText">The paragraph text.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Body CreateBodyFromString(string paragraphText, WordProperties? wordProperties = null, bool formatted = false)
        {
            Body? body = null;
            // split the paragraph text into paragraphs based on new lines
            string[] paragraphs = paragraphText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            body = CreateBodyFromStringArray(paragraphs, wordProperties, formatted);
            return body;
        }

        /// <summary>
        /// </summary>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Body CreateBodyFromStringArray(string[] paragraphs, WordProperties? wordProperties = null, bool formatted = false)
        {
            Body body = new Body();
            int paraIndex = 0;
            bool authorSet = false;
            foreach (string para in paragraphs)
            {
                Paragraph? paragrah = null;
                if (formatted)
                {
                    // check each line of text for formating codes
                    // skip if text is empty
                    if (!string.IsNullOrEmpty(para.Trim()))
                    {
                        // if trimed text ="Perverts" and "Us" then skip this line
                        string paraTrimmed = para.Trim();
                        if (paraTrimmed.IndexOf("Perverts") == 0 && paraTrimmed.IndexOf("Us") > 0)
                        {

                        }
                        else if (paraIndex == 0)
                        {
                            paragrah = CreateTitle(para);
                            // should add standardproperty Title here
                            if (wordProperties != null)
                            {
                                wordProperties.Title = para;
                            }

                        }
                        else if (para.Contains("by ", StringComparison.CurrentCultureIgnoreCase) && !authorSet && paraIndex < 4)
                        {
                            paragrah = CreateAuthorParagraph(para);
                            authorSet = true;
                            // should add standardproperty Author here
                            if (wordProperties != null)
                            {
                                wordProperties.Author = para.Replace("by ", "", StringComparison.CurrentCultureIgnoreCase).Trim();
                            }
                        }
                        else if (para.StartsWith("Chapter ", StringComparison.CurrentCultureIgnoreCase))
                        {
                            paragrah = CreateHeading1(para);
                        }
                        // look for <BREAK> and if so insert sectionbreak
                        else if (para.Trim().ToUpper() == "<BREAK>")
                        {
                            // insert a section break
                            SectionProperties sectionProps = new SectionProperties();
                            SectionType sectionType = new SectionType() { Val = SectionMarkValues.NextPage };
                            sectionProps.Append(sectionType);
                            body.Append(sectionProps);
                        }
                        // add more options as necessary
                        else
                            paragrah = CreateParagraph(para);
                        if (paragrah != null)
                        {
                            body.Append(paragrah);
                            paraIndex += 1;
                        }
                    }
                }
                else
                    body.Append(CreateParagraph(para));
            }
            return body;
        }

        /// <summary>
        /// The CloneEmptyDocument.
        /// </summary>
        /// <param name="title">The title<see cref="string"/>.</param>
        /// <param name="template">The template<see cref="string"/>.</param>
        /// <returns>The <see cref="WordprocessingDocument"/>.</returns>
        public static WordprocessingDocument CloneEmptyDocument(string title, string template)
        {

            // wiil need to create a new story 
            // string template = @"C:\Users\doug\AppData\Roaming\Microsoft\Templates\Normal.dotm";
            string path = @"C:\Drive_I\Stories\";

            List<OpenXmlPowerTools.Source> sources = new List<Source>()
            {
                new Source(template,true)
            };


            string tempFileName = path + title.ToString() + ".docx";


            tempFileName = path + DateTime.Now.ToString("yyyyMMddHHmm") + ".docx";
            DocumentBuilder.BuildDocument(sources, tempFileName);


            System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(tempFileName, FileMode.Open, FileAccess.ReadWrite);
            OpenSettings openSettings = new OpenSettings
            {
                AutoSave = true
            };
            // Open a WordprocessingDocument based on a package.
            DocumentFormat.OpenXml.Packaging.WordprocessingDocument document = WordprocessingDocument.Open(package, openSettings);


            DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;
            body.RemoveAllChildren<Paragraph>();
            // add a title to the document
            body.Append(WordXML.CreateTitle(title.ToString()));
            // body.Append(WordXML.CreateParagraph("A long and winding road leads to your door."));


            System.IO.File.Delete(tempFileName);

            document.Dispose();
            //package.Close();

            return document;
        }

        public static MemoryStream Create(WordprocessingDocument document)
        {
            MemoryStream ms = new MemoryStream();

            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                mainPart.Document.Body = new Body();
            }

            return ms;
        }

        /// <summary>
        /// Creates the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static MemoryStream Create(Document document)
        {
            MemoryStream ms = new MemoryStream();
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = document;
            }
            return ms;

        }

        public static WordprocessingDocument CreateDocumentFromTemplate(string newStoryName, bool noSave = false)
        {
            // wiil need to create a new story 
            string template = @"C:\Users\doug\AppData\Roaming\Microsoft\Templates\Normal.dotm";
            // string path = @"K:\Drive_I\Stories\";
            DocumentFormat.OpenXml.Packaging.WordprocessingDocument document = WordprocessingDocument.Open(template, true);
            DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;
            // save the document to a new file
            //string tempFileName = path + newStoryName.ToString() + ".docx";

            if (document.CanSave)
            {
                document.Save();
            }


            return document;
        }

        /// <summary>
        /// The CreateHeading1.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <returns>The <see cref="Paragraph"/>.</returns>
        public static Paragraph CreateHeading1(string text)
        {
            return CreateParagraph(text, "Heading1");
        }

        /// <summary>
        /// The CreateNewDocument.
        /// </summary>
        /// <param name="newStoryName">The newStoryName<see cref="string"/>.</param>
        /// <param name="noSave">The noSave<see cref="bool"/>.</param>
        /// <returns>The <see cref="WordprocessingDocument"/>.</returns>
        public static WordprocessingDocument CreateNewDocument(string newStoryName, bool noSave = false)
        {

            // wiil need to create a new story 
            string template = @"C:\Users\doug\AppData\Roaming\Microsoft\Templates\Normal.dotm";
            string path = @"K:\Drive_I\Stories\";

            List<Source> sources = new List<Source>()
            {
                new Source(template,true)
            };


            string tempFileName = path + newStoryName.ToString() + ".docx";

            if (noSave)
            {
                tempFileName = path + DateTime.Now.ToString("yyyyMMddHHmm") + ".docx";
                DocumentBuilder.BuildDocument(sources, tempFileName);
            }
            else
            {
                DocumentBuilder.BuildDocument(sources, tempFileName);
            }


            System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(tempFileName, FileMode.Open, FileAccess.ReadWrite);
            OpenSettings openSettings = new OpenSettings
            {
                AutoSave = true
            };
            // Open a WordprocessingDocument based on a package.
            DocumentFormat.OpenXml.Packaging.WordprocessingDocument document = WordprocessingDocument.Open(package, openSettings);


            DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;
            body.RemoveAllChildren<Paragraph>();
            // add a title to the document
            body.Append(WordXML.CreateTitle(newStoryName.ToString()));
            body.Append(WordXML.CreateParagraph("A long and winding road leads to your door."));

            if (noSave)
            {
                System.IO.File.Delete(tempFileName);
            }
            else
                document.Save();
            //document.Close();
            //package.Close();

            return document;
        }

        /// <summary>
        /// </summary>
        /// <param name="tempPath">The temporary path.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void CreateNewDocument(string tempPath)
        {
            CreateNewDocument(tempPath, null, null, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="tempPath">The temporary path.</param>
        /// <param name="inputBody">The input body.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void CreateNewDocument(string tempPath, Body inputBody)
        {
            CreateNewDocument(tempPath, inputBody, null, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="tempPath">The temporary path.</param>
        /// <param name="inputBody">The input body.</param>
        /// <param name="wordProperties">The word properties.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void CreateNewDocument(string tempPath, Body inputBody, WordProperties wordProperties)
        {
            CreateNewDocument(tempPath, inputBody, wordProperties, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="tempPath">The temporary path.</param>
        /// <param name="inputBody">The input body.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void CreateNewDocument(string tempPath, Body? inputBody, WordProperties? wordProperties, WordProperties? customProps)
        {
            WordprocessingDocument document = WordXML.CreateDocumentFromTemplate("test", false);
            if (document != null)
            {
                // getstyle from template and add to this document
                //ReplaceStyles(stylesPath, document.MainDocumentPart.Document);
                MemoryStream memoryStream = WordXML.Create(document);
                OpenXmlPowerTools.WmlDocument doc = new WmlDocument(tempPath, memoryStream, false);
                using (OpenXmlMemoryStreamDocument streamDoc = OpenXmlMemoryStreamDocument.CreateWordprocessingDocument())
                // using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
                {
                    WordprocessingDocument wpddocument = streamDoc.GetWordprocessingDocument();

                    // wpddocument nees mainpart to be set up
                    WordprocessingDocument wpDocument = wpddocument;

                    MainDocumentPart? mainPart = wpDocument.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = document.MainDocumentPart;
                    }

                    Body? body = wpDocument.MainDocumentPart.Document?.Body;

                    if (body == null)
                    {

                        wpDocument.MainDocumentPart.Document.Append(document.MainDocumentPart.Document.Body);
                    }

                    if (body != null)
                    {
                        if (inputBody != null)
                        {
                            body.RemoveAllChildren<Paragraph>();
                            foreach (var para in inputBody.Elements<Paragraph>())
                            {
                                body.Append(para.CloneNode(true));
                            }
                        }

                    }

                    if (wordProperties != null)
                    {
                        SetStandardProperties(wordProperties, wpDocument);
                    }

                    // deal with custom properties
                    if (customProps != null)
                    {
                        SetCustomProperties(customProps, wpDocument);
                    }

                    wpDocument.Save();
                    streamDoc.GetModifiedDocument().SaveAs(tempPath);
                }
                document.Dispose();

            }
        }

        /// <summary>
        /// The CreateParagraph.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <param name="styleName">The styleName<see cref="string"/>.</param>
        /// <returns>The <see cref="Paragraph"/>.</returns>
        public static Paragraph CreateParagraph(string text, string styleName = "PlainText")
        {
            Text title = new Text(text);
            Run run = new Run(title);
            ParagraphProperties ppH1 = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId()
                {
                    Val = styleName
                }
            };


            Paragraph newPara = new Paragraph();
            newPara.Append(ppH1);
            newPara.AppendChild(run);

            return newPara;
        }

        /// <summary>
        /// The CreateTitle.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <returns>The <see cref="Paragraph"/>.</returns>
        public static Paragraph CreateTitle(string text)
        {

            return CreateParagraph(text, "Title");
        }

        /// <summary>
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 14/12/2025 14/12/2025 </created>
        /// </remarks>
        public static Paragraph CreateAuthorParagraph(string text)
        {
            return CreateParagraph(text, "Subtitle");
        }



        /// <summary>
        /// The FindText.
        /// </summary>
        /// <param name="text">The text<see cref="string"/>.</param>
        /// <returns>The <see cref="Run"/>.</returns>
        public static Run FindText(string text)
        {
            Body body = wordDocument.MainDocumentPart.Document.Body;
            var runs = body.Descendants<Run>();
            Run returnRun = null;


            foreach (Run txt in runs)
            {
                if (txt.InnerText.Contains(text))
                {
                    returnRun = txt;
                    break;
                }
            }

            return returnRun;
        }

        /// <summary>
        /// The GetDocument.
        /// </summary>
        /// <param name="filepath">The file path<see cref="string"/>.</param>
        /// <param name="readOnly">The readOnly<see cref="bool"/>.</param>
        /// <returns>The return value<see cref="string"/>.</returns>
        public static string GetDocument(string filepath, bool readOnly)
        {
            BreakList = new BreakList();
            string returnXML = string.Empty;

            FileAccess access = FileAccess.Read;
            if (!readOnly)
            {
                access = FileAccess.ReadWrite;
            }

            try
            {
                if (wordDocument != null)
                {
                    //wordDocument.Close();
                    wordDocument = null;
                }
            }
            catch (Exception)
            {

            }

            if (wordPackage != null)
            {
                wordPackage.Close();
                wordPackage = null;
            }
            wordPackage = Package.Open(filepath, FileMode.Open, access);

            BookmarkList values = new BookmarkList();
            OpenSettings openSettings = new OpenSettings
            {
                AutoSave = true
            };
            // Open a WordprocessingDocument based on a package.
            wordDocument = WordprocessingDocument.Open(wordPackage, openSettings);
            {
                returnXML = wordDocument.MainDocumentPart.Document.OuterXml;

                // process bookmarks
                if (wordDocument.MainDocumentPart.HeaderParts != null)
                {
                    foreach (var header in wordDocument.MainDocumentPart.HeaderParts)
                    {
                        OpenXML.ProcessBookmarksPart(out values, OpenXML.DocumentSection.Header, header);
                    }
                }

                OpenXML.ProcessBookmarksPart(out values, OpenXML.DocumentSection.Main, wordDocument.MainDocumentPart);

                if (wordDocument.MainDocumentPart.FooterParts != null)
                {
                    foreach (var footer in wordDocument.MainDocumentPart.FooterParts)
                    {
                        OpenXML.ProcessBookmarksPart(out values, OpenXML.DocumentSection.Footer, footer);
                    }
                }
            }

            Bookmarks = values;
            if (values.Count > 0)
            {
                string stop = values[0].BookmarkText()?.Text;
            }

            returnXML = returnXML.Replace("…", "...");
            returnXML = returnXML.Replace("‘", "'");
            returnXML = returnXML.Replace("’", "'");
            returnXML = returnXML.Replace("“", "\"");
            returnXML = returnXML.Replace("”", "\"");

            return returnXML;
        }

        /// <summary>
        /// The GetProperties.
        /// </summary>
        /// <param name="fileName">The <see cref="string"/>.</param>
        /// <returns>The <see cref="WordProperties"/>.</returns>
        public static WordProperties GetProperties(string fileName)
        {
            WordProperties returnProps = new WordProperties();
            try
            {


                WordprocessingDocument document = WordprocessingDocument.Open(fileName, false);
                {
                    DocumentFormat.OpenXml.ExtendedProperties.Properties props = document.ExtendedFilePropertiesPart.Properties;

                    var packageProps = document.PackageProperties;
                    NameTable nt = new NameTable();

                    if (packageProps != null)
                    {
                        // Author
                        if (packageProps.Creator != null)
                        {
                            if (packageProps.Creator.ToLower() == "doug taylor")
                            {
                                returnProps.Author = string.Empty;
                            }
                            else if (packageProps.Creator.ToLower() == "doug")
                            {
                                returnProps.Author = string.Empty;
                            }
                            else
                            {
                                returnProps.Author = packageProps.Creator;
                            }
                        }

                        // modified on 
                        if (packageProps.Modified != null)
                        {
                            returnProps.Modified = packageProps.Modified;
                        }

                        //created on
                        if (packageProps.Created != null)
                        {
                            returnProps.Created = packageProps.Created;
                        }

                        if (packageProps.Title != null)
                        {
                            returnProps.Title = packageProps.Title;
                            if (returnProps.Title.Length > 150)
                            {
                                returnProps.Title = returnProps.Title.Substring(1, 150);
                            }
                        }
                        // Store keywords in temporary variable
                        string tempKeywords = packageProps.Keywords ?? string.Empty;
                        if (tempKeywords == string.Empty)
                        {
                            tempKeywords = packageProps.Category ?? string.Empty;
                        }
                        TidyKeywords(returnProps, tempKeywords);
                    }

                    // deal with extended properties
                    if (props != null)
                    {
                        string app = props.Application.Text;
                        if (app.IndexOf("Word") >= 0)
                        {
                            // it's a word document
                        }

                        if (props.Lines != null)
                        {
                            returnProps.Lines = props.Lines.Text;
                        }

                        if (props.Characters != null)
                        {
                            returnProps.Characters = props.Characters.Text;
                        }

                        if (props.Notes != null)
                        {
                            returnProps.Notes = props.Notes.Text;
                        }

                        if (props.Pages != null)
                        {
                            returnProps.Pages = props.Pages.Text;
                        }
                    }

                    var customProps = document.CustomFilePropertiesPart;
                    if (customProps != null)
                    {
                        // No custom properties? Nothing to return, in that case.
                        var cprops = customProps.Properties;
                        if (cprops != null)
                        {
                            string codes = GetCustomProperty(cprops, "Codes");
                            if (codes != null)
                            {
                                returnProps.Codes = codes;
                            }

                            string age = GetCustomProperty(cprops, "Age");
                            if (age != null)
                            {
                                returnProps.Age = age;
                            }

                            string lowestAge = GetCustomProperty(cprops, "LowestAge");
                            if (age != null)
                            {
                                returnProps.LowestAge = lowestAge;
                            }

                        }
                    }
                }
                //document.Close();
                document.Dispose();
            }
            catch (Exception e)
            {
                string error = e.Message;
                //throw;
            }
            //document = null;
            return returnProps;
        }

        /// <summary>
        /// The Heading1List.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="OpenXMLHeadingList"/>.</returns>
        public static OpenXMLHeadingList Heading1List(string path)
        {
            WmlDocument = new OpenXmlPowerTools.WmlDocument(path);
            OpenXMLHeadingList openXMLHeadings = new OpenXMLHeadingList();

            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(WmlDocument))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                if (document != null)
                {
                    int heading1 = 1;
                    var body = document.MainDocumentPart.Document.Body;

                    IEnumerable<Paragraph> paras = body.Descendants<Paragraph>();

                    foreach (Paragraph para in paras)
                    {
                        ParagraphStyleId paraStyle = para.Descendants<ParagraphStyleId>().FirstOrDefault<ParagraphStyleId>() as ParagraphStyleId;

                        if (paraStyle != null)
                        {
                            // need to look for heading
                            OpenXmlAttribute attrib = paraStyle.GetAttribute("val", w.NamespaceName);
                            if (attrib != null)
                            {
                                if (attrib.Value == "Heading1")
                                {
                                    string text = ParagraphText(para);
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        // add a new heading to list
                                        OpenXMLHeading newHeading = new OpenXMLHeading();
                                        newHeading.Index = heading1;
                                        newHeading.HeadingLevel = 1;
                                        newHeading.StringID = "heading1_" + heading1.ToString().Trim();
                                        if (openXMLHeadings.Find(x => x.Text == text) == null)
                                        {
                                            newHeading.Text = text;
                                        }

                                        heading1 += 1;
                                        openXMLHeadings.Add(newHeading);
                                    }
                                    else
                                    {

                                    }
                                }

                            }
                        }
                    }

                }
            }


            return openXMLHeadings;
        }

        /// <summary>
        /// </summary>
        /// <param name="fromDoc">From document.</param>
        /// <param name="toDoc">To document.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void ReplaceStyles(string fromDoc, Document toDoc)
        {
            // Extract and replace the styles part.
            XDocument? node = ExtractStylesPart(fromDoc, false);
            if (node is not null)
            {
                ReplaceStylesPart(toDoc, node);
            }
            // Extract and replace the stylesWithEffects part. To fully support 
            // round-tripping from Word 2010 to Word 2007, you should 
            // replace this part, as well.
            //node = ExtractStylesPart(fromDoc);
            //if (node is not null)
            //{
            //    ReplaceStylesPart(toDoc, node);
            //}
            return;
        }

        /// <summary>
        /// </summary>
        /// <param name="fromDoc">From document.</param>
        /// <param name="toDoc">To document.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void ReplaceStyles(Document fromDoc, Document toDoc)
        {
            // Extract and replace the styles part.
            XDocument? node = ExtractStylesPart(fromDoc);
            if (node is not null)
            {
                ReplaceStylesPart(toDoc, node);
            }
            return;
        }

        // Replace the styles in the "to" document with the styles in
        // the "from" document.
        public static void ReplaceStyles(string fromDoc, string toDoc)
        {

            // Extract and replace the styles part.
            XDocument? node = ExtractStylesPart(fromDoc, false);

            if (node is not null)
            {
                ReplaceStylesPart(toDoc, node, false);
            }

            // Extract and replace the stylesWithEffects part. To fully support 
            // round-tripping from Word 2010 to Word 2007, you should 
            // replace this part, as well.
            node = ExtractStylesPart(fromDoc);

            if (node is not null)
            {
                ReplaceStylesPart(toDoc, node);
            }

            return;
        }

        /// <summary>
        /// The SaveDocument.
        /// </summary>
        public static void SaveDocument()
        {

            if (wordDocument != null)
            {
                wordDocument.MainDocumentPart.Document.Save();
                //wordDocument.Close();
                wordDocument.Dispose();
            }
        }

        public static List<WmlDocument> SectionsAsDocuments(string path)
        {
            IEnumerable<WmlDocument> wordprocessingDocuments;



            WmlDocument = new OpenXmlPowerTools.WmlDocument(path);
            OpenXMLHeadingList openXMLHeadings = new OpenXMLHeadingList();
            wordprocessingDocuments = WmlDocument.SplitOnSections();

            return wordprocessingDocuments.ToList();
        }

        public static List<WPSection> SectionsAsHTML(string path)
        {
            IEnumerable<WmlDocument> wordprocessingDocuments;

            List<WPSection> rawHTML = new List<WPSection>();

            WmlDocument = new OpenXmlPowerTools.WmlDocument(path);
            OpenXMLHeadingList openXMLHeadings = new OpenXMLHeadingList();
            wordprocessingDocuments = WmlDocument.SplitOnSections();

            List<WmlDocument> wmlDocuments = wordprocessingDocuments.ToList();

            int index = 1;

            foreach (WmlDocument item in wmlDocuments)
            {
                WPSection section = new WPSection(item, index);
                rawHTML.Add(section);
                index += 1;
                //XElement xElement = item.ConvertToHtml(new OpenXmlPowerTools.WmlToHtmlConverterSettings());
                //rawHTML.Add(xElement.ToString());
            }

            return rawHTML;
        }

        public static void SetCustomProperties(WordProperties props, string fileName)
        {
            if (!string.IsNullOrEmpty(props.Codes))
            {
                WDSetCustomProperty(fileName, "Codes", props.Codes, PropertyTypes.Text);
            }
            else if (props.Keywords != null)
            {
                WDSetCustomProperty(fileName, "Codes", props.Keywords, PropertyTypes.Text);
            }

            if (props.Age != null)
            {
                WDSetCustomProperty(fileName, "Age", props.Age, PropertyTypes.Text);
            }

            if (props.LowestAge != null)
            {
                WDSetCustomProperty(fileName, "LowestAge", props.LowestAge, PropertyTypes.Text);
            }

            if (props.Published != null)
            {
                WDSetCustomProperty(fileName, "Published", props.Published, PropertyTypes.Text);
            }

            if (props.Percent != null)
            {
                WDSetCustomProperty(fileName, "percent", props.Percent, PropertyTypes.Text);
            }

            if (props.DocumentId > 0)
            {
                WDSetCustomProperty(fileName, "DocumentId", props.DocumentId, PropertyTypes.NumberInteger);
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="props">The props.</param>
        /// <param name="document">The document.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void SetCustomProperties(WordProperties props, WordprocessingDocument document)
        {
            if (!string.IsNullOrEmpty(props.Codes))
            {
                WDSetCustomProperty(document, "Codes", props.Codes, PropertyTypes.Text);
            }
            else if (props.Keywords != null)
            {
                WDSetCustomProperty(document, "Codes", props.Keywords, PropertyTypes.Text);
            }

            if (props.Age != null)
            {
                WDSetCustomProperty(document, "Age", props.Age, PropertyTypes.Text);
            }

            if (props.LowestAge != null)
            {
                WDSetCustomProperty(document, "LowestAge", props.LowestAge, PropertyTypes.Text);
            }

            if (props.Published != null)
            {
                WDSetCustomProperty(document, "Published", props.Published, PropertyTypes.Text);
            }

            if (props.Percent != null)
            {
                WDSetCustomProperty(document, "percent", props.Percent, PropertyTypes.Text);
            }

            if (props.DocumentId > 0)
            {
                WDSetCustomProperty(document, "DocumentId", props.DocumentId, PropertyTypes.NumberInteger);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="props">The props.</param>
        /// <param name="document">The document.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        public static void SetStandardProperties(WordProperties props, WordprocessingDocument document)
        {
            if (document != null)
            {
                var packageProps = document.PackageProperties;
                if (props.Author != string.Empty)
                {
                    document.PackageProperties.Creator = props.Author;
                }
                if (props.Title != string.Empty)
                {
                    document.PackageProperties.Title = props.Title;
                }

                if (!String.IsNullOrEmpty(props.Subject))
                {
                    document.PackageProperties.Subject = props.Subject;
                }
                if (!string.IsNullOrEmpty(props.Language))
                {
                    document.PackageProperties.Language = props.Language;
                }

                if (!string.IsNullOrEmpty(props.Codes)) document.PackageProperties.Keywords = props.Codes;
                if (!string.IsNullOrEmpty(props.Codes)) document.PackageProperties.Category = props.Codes;
                document.PackageProperties.Modified = DateTime.Now;
                document.PackageProperties.Revision = "0";
            }
        }

        /// <summary>
        /// The SetStandardProperties.
        /// </summary>
        /// <param name="props">The <see cref="WordProperties"/>.</param>
        /// <param name="fileName">The <see cref="string"/>.</param>
        public static void SetStandardProperties(WordProperties props, string fileName)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Open(fileName, true))
            {
                var packageProps = document.PackageProperties;

                if (props.Author != string.Empty)
                {
                    document.PackageProperties.Creator = props.Author;
                }

                if (props.Title != string.Empty)
                {
                    document.PackageProperties.Title = props.Title;
                }

                if (!String.IsNullOrEmpty(props.Subject))
                {
                    document.PackageProperties.Subject = props.Subject;
                }
                if (!string.IsNullOrEmpty(props.Language))
                {
                    document.PackageProperties.Language = props.Language;
                }

                if (!string.IsNullOrEmpty(props.Codes)) document.PackageProperties.Keywords = props.Codes;

                document.PackageProperties.Modified = DateTime.Now;
                document.PackageProperties.Revision = document.PackageProperties.Revision + 1;
                // document.Close(); obsolete use dispose instead
                document.Dispose();
            }
        }

        /// <summary>
        /// The SortOutSections.
        /// </summary>
        /// <param name="xmlIn">The xmlIn<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SortOutSections(string xmlIn)
        {
            XElement raw = XElement.Parse(xmlIn);
            XElement body = raw.Element(w + "body");
            //XName xNamePara = "w:p";
            // get all paragraphs
            IEnumerable<XElement> paras = body.Elements(w + "p");
            int sectID = 1;
            foreach (XElement para in paras)
            {
                XElement word_pPr = para.Element(w + "pPr");
                if (word_pPr != null)
                {
                    XElement sectbr = word_pPr.Element(w + "sectPr");
                    if (sectbr != null)
                    {
                        XElement newSBR = XElement.Parse(sectbr.ToString());
                        XAttribute id = new XAttribute(w + "sectid", sectID);
                        newSBR.Add(id);
                        word_pPr.Element(w + "sectPr").ReplaceWith(newSBR);
                        OpenXMLBreak openXMLBreak = new OpenXMLBreak
                        {
                            BreakID = sectID,
                            StringId = "sect_" + sectID.ToString().Trim()
                        };
                        BreakList.Add(openXMLBreak);

                        sectID += 1;

                    }
                }
            }

            // see if there is a final one
            XElement finalBreak = body.Element(w + "sectPr");
            if (finalBreak != null)
            {
                XAttribute id = new XAttribute(w + "sectid", sectID);
                finalBreak.Add(id);
                body.Element(w + "sectPr").ReplaceWith(finalBreak);
                OpenXMLBreak openXMLBreak = new OpenXMLBreak
                {
                    BreakID = sectID,
                    StringId = "sect_" + sectID.ToString().Trim()
                };
                BreakList.Add(openXMLBreak);
            }

            return raw.ToString(); ;
        }

        /// <summary>
        /// The TidyKeywords.
        /// </summary>
        /// <param name="returnProps">The returnProps<see cref="WordProperties"/>.</param>
        /// <param name="tempKeywords">The tempKeywords<see cref="string"/>.</param>
        public static void TidyKeywords(WordProperties returnProps, string tempKeywords)
        {


            // check categories
            if (tempKeywords != string.Empty)
            {
                // change values to match current
                tempKeywords = tempKeywords.Replace("/", "");
                //tempKeywords = tempKeywords.Replace(" - ", ",");
                tempKeywords = tempKeywords.Replace("  ", " ").Replace(" ", ",");
                tempKeywords = tempKeywords.Replace(", ", ",");
                tempKeywords = tempKeywords.Replace(",inc,", ",inc:");
                tempKeywords = tempKeywords.Replace(",incest,", ",inc:");
                tempKeywords = tempKeywords.Replace(",fuck", ",fk");
                tempKeywords = tempKeywords.Replace(",fuck:", ",fk:");
                tempKeywords = tempKeywords.Replace("year old", "year_old");
                tempKeywords = tempKeywords.Replace("year_old,", "y,");
                tempKeywords = tempKeywords.Replace(",fl", ",oral");
                tempKeywords = tempKeywords.Replace(",bj", ",oral");
                tempKeywords = tempKeywords.Replace(",,", ",");

                returnProps.Codes = tempKeywords;

                // check on ages
                string agestring = string.Empty;
                string[] codes = tempKeywords.Split(new char[] { ',' });

                // go through codes lokking for year old
                foreach (string code in codes)
                {
                    if (code.Contains("year_old"))
                    {
                        int pos = code.IndexOf("year_old");
                        string tempAge = code.Substring(0, pos);
                        if (agestring != string.Empty)
                        {
                            agestring += ",";
                        }
                        agestring += tempAge;
                    }

                    if (IsNumeric(code) && code != "69")
                    {
                        int.TryParse(code, out int n);
                        if (agestring != string.Empty)
                        {
                            agestring += ",";
                        }
                        agestring += n.ToString().Trim();
                    }

                    Regex regex = new Regex("\\d+\\b");
                    Match match = regex.Match(code);
                    if (match.Success == true)
                    {
                        if (match.Value != "69")
                        {
                            if (agestring != string.Empty)
                            {
                                agestring += ",";
                            }
                            agestring += match.Value;
                        }
                    }
                }
                if (agestring != string.Empty)
                {
                    try
                    {
                        string output = String.Join(",", agestring.Split(',').Select(x => int.Parse(x)).OrderBy(x => x));
                        returnProps.Age = output;
                    }
                    catch (Exception)
                    {
                        returnProps.Age = agestring;

                    }

                }
            }
        }

        /// <summary>
        /// The WDGetCustomProperty.
        /// </summary>
        /// <param name="fileName">The File Name<see cref="string"/>.</param>
        /// <param name="propertyName">The Property name <see cref="string"/>.</param>
        /// <returns>The Return value<see cref="string"/>.</returns>
        public static string WDGetCustomProperty(string fileName, string propertyName)
        {
            string returnValue = null;

            using (var document = WordprocessingDocument.Open(fileName, false))
            {
                var customProps = document.CustomFilePropertiesPart;
                if (customProps != null)
                {
                    // No custom properties? Nothing to return, in that case.
                    var props = customProps.Properties;
                    if (props != null)
                    {
                        var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();

                        // Does the property exist? If so, get the return value.
                        if (prop != null)
                        {
                            returnValue = prop.InnerText;
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// The WDSetCustomProperty.
        /// </summary>
        /// <param name="fileName">The File Name<see cref="string"/>.</param>
        /// <param name="propertyName">The Name<see cref="string"/>.</param>
        /// <param name="propertyValue">The Value<see cref="object"/>.</param>
        /// <param name="propertyType">The Property Type<see cref="PropertyTypes"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string WDSetCustomProperty(string fileName, string propertyName, object propertyValue, PropertyTypes propertyType)
        {
            string returnValue = null;

            CustomDocumentProperty newProp = CreateCustomProperty(propertyName, propertyValue, propertyType);

            using (var document = WordprocessingDocument.Open(fileName, true))
            {
                var customProps = document.CustomFilePropertiesPart;
                if (customProps == null)
                {
                    // No custom properties? Add the part, and the collection of properties now.
                    customProps = document.AddCustomFilePropertiesPart();
                    customProps.Properties = new DocumentFormat.OpenXml.CustomProperties.Properties();
                }

                var props = customProps.Properties;
                if (props != null)
                {
                    var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();

                    // Does the property exist? If so, get the return value,  and then delete the property.
                    if (prop != null)
                    {
                        returnValue = prop.InnerText;
                        prop.Remove();
                    }

                    // Append the new property, and 
                    // fix all the property ID values. 
                    // The PropertyId value must start at 2.
                    props.AppendChild(newProp);
                    int pid = 2;
                    foreach (CustomDocumentProperty item in props)
                    {
                        item.PropertyId = pid++;
                    }

                    props.Save();
                }
            }

            return returnValue;
        }

        public static string WDSetCustomProperty(WordprocessingDocument document, string propertyName, object propertyValue, PropertyTypes propertyType)
        {
            string returnValue = null;

            CustomDocumentProperty newProp = CreateCustomProperty(propertyName, propertyValue, propertyType);


            var customProps = document.CustomFilePropertiesPart;
            if (customProps == null)
            {
                // No custom properties? Add the part, and the collection of properties now.
                customProps = document.AddCustomFilePropertiesPart();
                customProps.Properties = new DocumentFormat.OpenXml.CustomProperties.Properties();
            }

            var props = customProps.Properties;
            if (props != null)
            {
                var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();

                // Does the property exist? If so, get the return value,  and then delete the property.
                if (prop != null)
                {
                    returnValue = prop.InnerText;
                    prop.Remove();
                }

                // Append the new property, and 
                // fix all the property ID values. 
                // The PropertyId value must start at 2.
                props.AppendChild(newProp);
                int pid = 2;
                foreach (CustomDocumentProperty item in props)
                {
                    item.PropertyId = pid++;
                }

                props.Save();

            }

            return returnValue;
        }

        private static CustomDocumentProperty CreateCustomProperty(string propertyName, object propertyValue, PropertyTypes propertyType)
        {
            var newProp = new CustomDocumentProperty();
            bool propSet = false;

            // Calculate the correct type:
            switch (propertyType)
            {
                case PropertyTypes.DateTime:
                    // Verify that you were passed a real date, 
                    // and if so, format in the correct way. 
                    // The date/time value passed in should 
                    // represent a UTC date/time.
                    if (propertyValue is DateTime)
                    {
                        newProp.VTFileTime = new VTFileTime(string.Format(
                          "{0:s}Z", Convert.ToDateTime(propertyValue)));
                        propSet = true;
                    }

                    break;
                case PropertyTypes.NumberInteger:
                    if (propertyValue is int)
                    {
                        newProp.VTInt32 = new VTInt32(propertyValue.ToString());
                        propSet = true;
                    }

                    break;
                case PropertyTypes.NumberDouble:
                    if (propertyValue is double)
                    {
                        newProp.VTFloat = new VTFloat(propertyValue.ToString());
                        propSet = true;
                    }

                    break;
                case PropertyTypes.Text:
                    newProp.VTLPWSTR = new VTLPWSTR(propertyValue.ToString());
                    propSet = true;

                    break;
                case PropertyTypes.YesNo:
                    if (propertyValue is bool)
                    {
                        // Must be lowercase.
                        newProp.VTBool = new VTBool(
                          Convert.ToBoolean(propertyValue).ToString().ToLower());
                        propSet = true;
                    }

                    break;
            }

            if (!propSet)
            {
                // If the code could not convert the 
                // property to a valid value, throw an exception:
                throw new InvalidDataException("propertyValue");
            }

            // Now that you have handled the parameters,
            // work on the document.
            newProp.FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
            newProp.Name = propertyName;
            return newProp;
        }

        // Extract the styles or stylesWithEffects part from a 
        // word processing document as an XDocument instance.
        static XDocument ExtractStylesPart(Document document)
        {
            // Declare a variable to hold the XDocument.
            XDocument? styles = null;

            // Open the document for read access and get a reference.

            // Get a reference to the main document part.
            var docPart = document.MainDocumentPart;

            if (docPart is null)
            {
                throw new ArgumentNullException("MainDocumentPart is null.");
            }

            // Assign a reference to the appropriate part to the
            // stylesPart variable.
            StylesPart stylesPart;

            if (docPart.StyleDefinitionsPart is not null)
            {
                stylesPart = docPart.StyleDefinitionsPart;
            }
            else
            {
                throw new ArgumentNullException("StyleWithEffectsPart and StyleDefinitionsPart are undefined");
            }

            using (var reader = XmlNodeReader.Create(stylesPart.GetStream(FileMode.Open, FileAccess.Read)))
            {
                // Create the XDocument.
                styles = XDocument.Load(reader);
            }

            // Return the XDocument instance.
            return styles;
        }

        // Extract the styles or stylesWithEffects part from a 
        // word processing document as an XDocument instance.
        static XDocument ExtractStylesPart(string fileName, bool getStylesWithEffectsPart = true)
        {
            // Declare a variable to hold the XDocument.
            XDocument? styles = null;

            // Open the document for read access and get a reference.
            using (var document = WordprocessingDocument.Open(fileName, false))
            {
                // Get a reference to the main document part.
                var docPart = document.MainDocumentPart;

                if (docPart is null)
                {
                    throw new ArgumentNullException("MainDocumentPart is null.");
                }

                // Assign a reference to the appropriate part to the
                // stylesPart variable.
                StylesPart stylesPart;

                if (getStylesWithEffectsPart && docPart.StylesWithEffectsPart is not null)
                {
                    stylesPart = docPart.StylesWithEffectsPart;
                }
                else if (docPart.StyleDefinitionsPart is not null)
                {
                    stylesPart = docPart.StyleDefinitionsPart;
                }
                else
                {
                    throw new ArgumentNullException("StyleWithEffectsPart and StyleDefinitionsPart are undefined");
                }

                using (var reader = XmlNodeReader.Create(stylesPart.GetStream(FileMode.Open, FileAccess.Read)))
                {
                    // Create the XDocument.
                    styles = XDocument.Load(reader);
                }
            }
            // Return the XDocument instance.
            return styles;
        }

        /// <summary>
        /// The GetCustomProperty.
        /// </summary>
        /// <param name="cprops">The Custom Property set<see cref="DocumentFormat.OpenXml.CustomProperties.Properties"/>.</param>
        /// <param name="propertyName">The Property name <see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetCustomProperty(DocumentFormat.OpenXml.CustomProperties.Properties cprops, string propertyName)
        {
            string returnValue = null;
            var prop = cprops.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();

            // Does the property exist? If so, get the return value.
            if (prop != null)
            {
                returnValue = prop.InnerText;
            }

            return returnValue;
        }

        /// <summary>
        /// The IsNumeric.
        /// </summary>
        /// <param name="checkstring">The checkstring<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static bool IsNumeric(string checkstring)
        {
            bool retCheck = int.TryParse(checkstring, out int n);
            return retCheck;
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
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="newStyles">The new styles.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 13/12/2025 13/12/2025 </created>
        /// </remarks>
        private static void ReplaceStylesPart(Document document, XDocument newStyles)
        {
            // Open the document for write access and get a reference.

            if (document.MainDocumentPart != null)
            {
                if (document.MainDocumentPart.StyleDefinitionsPart is null)
                {
                    document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                    //throw new ArgumentNullException("MainDocumentPart and/or one or both of the Styles parts is null.");
                }

                // else remove styledefinitionpart and replace
                else if (document.MainDocumentPart.StyleDefinitionsPart is not null)
                {
                    document.MainDocumentPart.DeletePart(document.MainDocumentPart.StyleDefinitionsPart);
                    document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                }

                // Get a reference to the main document part.
                var docPart = document.MainDocumentPart;

                // Assign a reference to the appropriate part to the
                // stylesPart variable.

                StylesPart? stylesPart = null;


                stylesPart = docPart.StyleDefinitionsPart;


                // If the part exists, populate it with the new styles.
                if (stylesPart is not null)
                {
                    newStyles.Save(new StreamWriter(stylesPart.GetStream(FileMode.Create, FileAccess.Write)));
                }
            }
        }

        // Given a file and an XDocument instance that contains the content of 
        // a styles or stylesWithEffects part, replace the styles in the file 
        // with the styles in the XDocument.

        static void ReplaceStylesPart(string fileName, XDocument newStyles, bool setStylesWithEffectsPart = true)
        {

            // Open the document for write access and get a reference.
            using (var document = WordprocessingDocument.Open(fileName, true))
            {
                if (document.MainDocumentPart is null || (document.MainDocumentPart.StyleDefinitionsPart is null))
                {
                    document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                    //throw new ArgumentNullException("MainDocumentPart and/or one or both of the Styles parts is null.");
                }

                // Get a reference to the main document part.
                var docPart = document.MainDocumentPart;

                // Assign a reference to the appropriate part to the
                // stylesPart variable.

                StylesPart? stylesPart = null;

                if (setStylesWithEffectsPart)
                {
                    stylesPart = docPart.StylesWithEffectsPart;
                }
                else
                {
                    stylesPart = docPart.StyleDefinitionsPart;
                }

                // If the part exists, populate it with the new styles.
                if (stylesPart is not null)
                {
                    newStyles.Save(new StreamWriter(stylesPart.GetStream(FileMode.Create, FileAccess.Write)));
                }
            }
        }

        public static bool ConvertTextToDocx(string newStoryPath)
        {
            bool success = false;

            try
            {
                // first action is to read the text file
                string text = File.ReadAllText(newStoryPath);

                // create standard properties settings a set of WordProperties
                WordProperties props = new WordProperties()
                {
                    Title = "New Document",
                    Author = "Doug Taylor"
                };

                // second is to create a body from the text respect any formatting
                Body body = CreateBodyFromString(text, props, true);

                // third is to create a new document



                // create CustomProperties settings a set of WordProperties
                WordProperties customProps = new WordProperties()
                {

                };

                string testPath = newStoryPath.Replace(".text", ".docx", StringComparison.CurrentCultureIgnoreCase);

                // create the actual document
                WordXML.CreateNewDocument(testPath, body, props, customProps);


                // add styles to the document this seems to work best if done after creating the document
                WordXML.ReplaceStyles(WordXML.stylesPath, testPath);
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        #endregion
    }
}
