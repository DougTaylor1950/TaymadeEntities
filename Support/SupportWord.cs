namespace TaymadeEntities.Support.Word
{
    using TaymadeEntities.Support;
    using TaymadeEntities.Support.Word;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.CustomProperties;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using OpenXmlPowerTools;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO.Packaging;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;
    
    using Color = DocumentFormat.OpenXml.Wordprocessing.Color;

    /// <summary>
    /// Defines the <see cref="BookmarkList" />
    /// </summary>
    public class BookmarkList : List<OpenXmlBookmark>
    {
    }

    /// <summary>
    /// Defines the <see cref="BreakList" />
    /// </summary>
    public class BreakList : List<OpenXMLBreak>
    {
    }

    /// <summary>
    /// Defines the <see cref="DocumentStyle" />
    /// </summary>
    public class DocumentStyle
    {
        #region Fields

        /// <summary>
        /// Defines the Bold
        /// </summary>
        public bool Bold = false;

        /// <summary>
        /// Defines the Caps
        /// </summary>
        public bool Caps = false;

        /// <summary>
        /// Defines the color
        /// </summary>
        public Color color = null;

        /// <summary>
        /// Defines the Italic
        /// </summary>
        public bool Italic = false;

        /// <summary>
        /// Defines the Strike
        /// </summary>
        public bool Strike = false;

        /// <summary>
        /// Defines the UnderLine
        /// </summary>
        public bool UnderLine = false;

        /// <summary>
        /// Defines the AnsiFont
        /// </summary>
        internal string AnsiFont = string.Empty;

        /// <summary>
        /// Defines the AsciiFont
        /// </summary>
        internal string AsciiFont = string.Empty;

        /// <summary>
        /// Defines the BasedOn
        /// </summary>
        internal string BasedOn = string.Empty;

        /// <summary>
        /// Defines the ComplexFont
        /// </summary>
        internal string ComplexFont = string.Empty;

        /// <summary>
        /// Defines the FontSize
        /// </summary>
        internal float FontSize = 10;

        /// <summary>
        /// Defines the runFonts
        /// </summary>
        internal RunFonts runFonts = null;

        /// <summary>
        /// Defines the fontStyle
        /// </summary>
        private FontStyle fontStyle = System.Drawing.FontStyle.Regular;

        /// <summary>
        /// Defines the myFont
        /// </summary>
        private System.Drawing.Font myFont = null;

        /// <summary>
        /// Defines the myParent
        /// </summary>
        private StyleList myParent;

        /// <summary>
        /// Defines the myStyleRunProperties
        /// </summary>
        private StyleRunProperties myStyleRunProperties;

        /// <summary>
        /// Defines the style
        /// </summary>
        private Style style;

        public bool InUse = false;

        internal XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the Font
        /// </summary>
        public System.Drawing.Font Font
        {
            get
            {
                if (myFont == null && StyleRunProperties != null)
                {
                    if (AsciiFont != null && AsciiFont != string.Empty)
                    {
                        myFont = new System.Drawing.Font(AsciiFont, FontSize, fontStyle);
                    }
                    else
                    {
                        if (BasedOn != null && BasedOn != string.Empty && Parent != null)
                        {
                            DocumentStyle parentStyle = Parent.Find(x => x.style.StyleId == BasedOn);
                            if (parentStyle.Font != null)
                            {
                                myFont = new System.Drawing.Font(parentStyle.Font.FontFamily, FontSize, parentStyle.Font.Style | fontStyle);
                            }
                        }
                        else
                        {
                            myFont = new System.Drawing.Font("Arial", FontSize, fontStyle);
                        }
                    }
                }
                return myFont;
            }
            set => myFont = value;
        }

        /// <summary>
        /// Gets or sets the Parent
        /// </summary>
        public StyleList Parent
        {
            get => myParent;
            set => myParent = value;
        }

        /// <summary>
        /// Gets or sets the Style
        /// </summary>
        public Style Style
        {
            get => style;
            set
            {
                style = value;

                if (value != null)
                {
                    StyleRunProperties = value.StyleRunProperties;
                    if (value.BasedOn != null) BasedOn = value.BasedOn.Val;
                }
            }
        }

        private ParagraphProperties paragraphProperties;

        public ParagraphProperties ParagraphProperties
        {
            get => paragraphProperties;
            set => paragraphProperties = value;
        }


        /// <summary>
        /// Gets or sets the StyleRunProperties
        /// </summary>
        public StyleRunProperties StyleRunProperties
        {
            get => myStyleRunProperties;
            set
            {
                myStyleRunProperties = value;
                if (value != null)
                {
                    color = value.Color;
                    Bold = value.Bold != null;
                    Italic = value.Italic != null;
                    Caps = value.Caps != null;
                    Strike = value.Strike != null;
                    UnderLine = value.Underline != null;
                    if (Bold) fontStyle |= System.Drawing.FontStyle.Bold;
                    if (Italic) fontStyle |= System.Drawing.FontStyle.Italic;
                    if (UnderLine) fontStyle |= System.Drawing.FontStyle.Underline;
                    if (Strike) fontStyle |= System.Drawing.FontStyle.Strikeout;

                    runFonts = value.RunFonts;
                    if (runFonts != null)
                    {
                        AsciiFont = runFonts.Ascii;
                        AnsiFont = runFonts.HighAnsi;
                        ComplexFont = runFonts.ComplexScript;
                        if (value.FontSize != null) float.TryParse(value.FontSize.Val, out FontSize);
                        if (AsciiFont != null && AsciiFont != string.Empty)
                        {
                            // Font = new System.Drawing.Font(AsciiFont, FontSize, fontStyle);
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods


        public void ParseXML(XElement item)
        {
            if (item != null)
            {
                XElement runproperties = item.Element(w + "pPr");
                if (runproperties != null)
                {
                    ParagraphProperties = new ParagraphProperties(runproperties.ToString());



                }
            }
        }

        /// <summary>
        /// The GetColour
        /// </summary>
        /// <returns>The <see cref="System.Drawing.Color"/></returns>
        public System.Drawing.Color GetColour()
        {
            System.Drawing.Color retcolour = System.Drawing.Color.Black;
            if (color != null)
            {
                string stColor = color.Val;
                retcolour = System.Drawing.ColorTranslator.FromHtml("#" + stColor);
            }

            return retcolour;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="OpenXML" />
    /// </summary>
    public class OpenXML
    {
        #region Enums

        public enum DocumentSection { Main, Header, Footer };

        #endregion

        #region Methods

        public static void SetCustomProperties(WordProperties props, string fileName)
        {
            if (!string.IsNullOrEmpty(props.Codes))
            {
                WordXML.WDSetCustomProperty(fileName, "Codes", props.Codes, WordXML.PropertyTypes.Text);
            }
            else if (props.Keywords != null)
            {
                WordXML.WDSetCustomProperty(fileName, "Codes", props.Keywords, WordXML.PropertyTypes.Text);
            }

            if (props.Age != null)
            {
                WordXML.WDSetCustomProperty(fileName, "Age", props.Age, WordXML.PropertyTypes.Text);
            }

            if (props.LowestAge != null)
            {
                WordXML.WDSetCustomProperty(fileName, "LowestAge", props.LowestAge, WordXML.PropertyTypes.Text);
            }

            if (props.Published != null)
            {
                WordXML.WDSetCustomProperty(fileName, "Published", props.Published, WordXML.PropertyTypes.Text);
            }

            if (props.Percent != null)
            {
                WordXML.WDSetCustomProperty(fileName, "percent", props.Percent, WordXML.PropertyTypes.Text);
            }

            if (props.DocumentId > 0)
            {
                WordXML.WDSetCustomProperty(fileName, "DocumentId", props.DocumentId, WordXML.PropertyTypes.NumberInteger);
            }
        }
        public static void SetStandardProperties(WordProperties props, string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    using (WordprocessingDocument document = WordprocessingDocument.Open(fileName, true))
                    {
                        PackageProperties packageProps = (PackageProperties)document.PackageProperties;

                        if (!string.IsNullOrEmpty(props.Author))
                        {
                            document.PackageProperties.Creator = props.Author.TrimEnd('\r', '\n');
                        }

                        if (!string.IsNullOrEmpty(props.Title))
                        {
                            document.PackageProperties.Title = props.Title.TrimEnd('\r', '\n');
                        }

                        if (!string.IsNullOrEmpty(props.Keywords))
                        {
                            document.PackageProperties.Keywords = props.Keywords;
                        }

                        document.PackageProperties.Modified = DateTime.Now;
                        document.PackageProperties.Revision = document.PackageProperties.Revision + 1;
                        document.Save();
                        document.Dispose();
                    }
                }
                catch (Exception)
                {


                }
            }
        }
        /// <summary>
        /// The ProcessBookmarksPart
        /// </summary>
        /// <param name="values">The <see cref="BookmarkList"/></param>
        /// <param name="documentSection">The <see cref="DocumentSection"/></param>
        /// <param name="section">The <see cref="object"/></param>
        public static void ProcessBookmarksPart(out BookmarkList values, DocumentSection documentSection, object section)
        {
            IEnumerable bookmarks = null;
            BookmarkList openBookmarks = new BookmarkList();
            switch (documentSection)
            {
                case DocumentSection.Main:
                    {
                        bookmarks = ((MainDocumentPart)section).Document.Body.Descendants<BookmarkStart>();
                        break;
                    }
                case DocumentSection.Header:
                    {
                        bookmarks = ((HeaderPart)section).RootElement.Descendants<BookmarkStart>();
                        break;
                    }
                case DocumentSection.Footer:
                    {
                        bookmarks = ((FooterPart)section).RootElement.Descendants<BookmarkStart>();
                        break;
                    }
            }
            if (bookmarks != null)
            {
                // now process bookmarks
                foreach (OpenXmlElement bmStart in bookmarks)
                {
                    BookmarkEnd bmEnd = null;
                    BookmarkStart bookmarkStart = null;

                    bookmarkStart = bmStart as BookmarkStart;

                    OpenXmlBookmark newBookmark = openBookmarks.Find(x => x.Start.Name.ToString().ToUpper() == bookmarkStart.Name.ToString().ToUpper());


                    ////If the bookmark name is not in our list. Just continue with the loop


                    if (newBookmark == null)
                    {
                        newBookmark = new OpenXmlBookmark
                        {
                            Start = bookmarkStart,
                            SectionType = documentSection,
                            BookmarkIndex = openBookmarks.Count + 1
                        };
                        openBookmarks.Add(newBookmark);

                        //if (!values.ContainsKey(bmStart.LocalName))
                        //    continue;
                        //var bmText = values[bmStart.LocalName];
                        //BookmarkEnd bmEnd = null;
                        switch (documentSection)
                        {
                            case DocumentSection.Main:
                                {
                                    bmEnd = (((MainDocumentPart)section).Document.Body.Descendants<BookmarkEnd>().Where(b => b.Id == bookmarkStart.Id.ToString())).FirstOrDefault();
                                    break;
                                }
                            case DocumentSection.Header:
                                {
                                    bmEnd = (((HeaderPart)section).RootElement.Descendants<BookmarkEnd>().Where(b => b.Id == bookmarkStart.Id.ToString())).FirstOrDefault();
                                    break;
                                }
                            case DocumentSection.Footer:
                                {
                                    bmEnd = (((FooterPart)section).RootElement.Descendants<BookmarkEnd>().Where(b => b.Id == bookmarkStart.Id.ToString())).FirstOrDefault();
                                    break;
                                }
                        }
                        ////If we did not find anything just continue with the loop
                        if (bmEnd != null)
                        {
                            newBookmark.End = bmEnd;
                        }
                    }

                }
            }
            values = openBookmarks;
        }

        public static WordProperties GetProperties(string fileName)
        {
            WordProperties returnProps = new WordProperties();
            try
            {
                WordprocessingDocument document = WordprocessingDocument.Open(fileName, false);
                {
                    DocumentFormat.OpenXml.ExtendedProperties.Properties? props = null;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (document.ExtendedFilePropertiesPart != null && document.ExtendedFilePropertiesPart.Properties != null)
                    {
                        props = document.ExtendedFilePropertiesPart.Properties;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
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
                        // make keywords of consistent format and get age list 
                        TidyKeywords(returnProps, tempKeywords);
                    }

                    // now get extended properties
                    if (props != null)
                    {
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
                            if (string.IsNullOrEmpty(returnProps.Codes)) returnProps.Codes = codes;
                        }

                        if (string.IsNullOrEmpty(returnProps.Age))
                        {
                            string age = GetCustomProperty(cprops, "Age");
                            if (!string.IsNullOrEmpty(age))
                            {
                                returnProps.Age = age;
                            }
                        }

                        string published = GetCustomProperty(cprops, "Published");
                        if (!string.IsNullOrEmpty(published))
                        {
                            returnProps.Published = published;
                        }

                        string percent = GetCustomProperty(cprops, "percent");
                        if (!string.IsNullOrEmpty(percent))
                        {
                            returnProps.Percent = percent;
                        }
                    }
                }

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
        /// The TidyKeywords. makes keywords consistent and splits out ages
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
                // tempKeywords = tempKeywords.Replace(" - ", ",");
                tempKeywords = tempKeywords.Replace("  ", " "); //.Replace(" ", ",");
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
                tempKeywords = tempKeywords.Replace(",-,", " - ");
                tempKeywords = tempKeywords.Replace('\n', ' ');
                tempKeywords = tempKeywords.Replace('\r', ' ');
                returnProps.Codes = tempKeywords;
                returnProps.Keywords = tempKeywords;
                FindAges(returnProps, tempKeywords);
            }

        }

        private static void FindAges(WordProperties returnProps, string tempKeywords)
        {
            // check on ages
            string agestring = string.Empty;
            string[] codes = tempKeywords.Split(new char[] { ',', '-', ' ' });
            string localCode = string.Empty;
            string tempAge = string.Empty;

            // go through codes looking for year old
            foreach (string code in codes)
            {
                if (code.Contains("y"))
                {
                    localCode = code;
                    // check for ';'
                    if (localCode.Contains(";"))
                    {
                        int semiPos = localCode.IndexOf(";");
                        if (semiPos > 0)
                        {
                            // we need the code stub from this point on
                            localCode = localCode.Substring(semiPos + 1);
                        }
                    }
                    int pos = localCode.IndexOf("y");
                    tempAge = localCode.Substring(0, pos).Replace(".", "").Trim();
                    //if (agestring != string.Empty)
                    //{
                    //    agestring += ",";
                    //}
                    // need to check string length

                    if (tempAge.Length > 2)
                    {
                        int i = tempAge.Length - 1;
                        string temp = tempAge;
                        tempAge = string.Empty;
                        while (char.IsNumber(temp[i]))
                        {
                            tempAge = temp[i] + tempAge;
                            i -= 1;
                        }


                    }
                    //agestring += tempAge;


                    if (Support.IsNumeric(tempAge) && tempAge != "69")
                    {
                        int.TryParse(tempAge, out int n);
                        if (agestring != string.Empty)
                        {
                            agestring += ",";
                        }
                        agestring += n.ToString().Trim();
                    }

                    //Regex regex = new Regex("\\d+\\b");
                    //Match match = regex.Match(tempAge);
                    //if (match.Success == true)
                    //{
                    //    if (match.Value != "69")
                    //    {
                    //        if (agestring != string.Empty)
                    //        {
                    //            agestring += ",";
                    //        }
                    //        agestring += match.Value;
                    //    }
                    //}
                }
            }
            if (agestring != string.Empty)
            {
                try
                {
                    List<int> intList;
                    string output = SortAgeList(agestring, out intList);
                    returnProps.Age = output;
                    //string[] ageArray = output.Split(',');
                    if (intList.Count > 0) returnProps.LowestAge = intList.FirstOrDefault().ToString();
                }
                catch (Exception)
                {
                    returnProps.Age = agestring;

                }
            }

        }

        public static string SortAgeList(string agestring, out List<int> intList)
        {

            intList = Support.StringIntListToIntList(agestring);
            return String.Join(",", intList);
        }

        /// <summary>
        /// The GetCustomProperty.
        /// </summary>
        /// <param name="cprops">The cprops<see cref="DocumentFormat.OpenXml.CustomProperties.Properties"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetCustomProperty(DocumentFormat.OpenXml.CustomProperties.Properties cprops, string propertyName)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string returnValue = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var prop = cprops.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Does the property exist? If so, get the return value.
            if (prop != null)
            {
                returnValue = prop.InnerText;
            }

            return returnValue;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="OpenXMLBreak" />
    /// </summary>
    public class OpenXMLBreak
    {
        #region Fields

        /// <summary>
        /// Defines the myID
        /// </summary>
        private int myID;

        /// <summary>
        /// Defines the myStringID
        /// </summary>
        private string myStringID;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BreakID
        /// </summary>
        public int BreakID
        {
            get => myID;
            set => myID = value;
        }

        /// <summary>
        /// Gets or sets the StringId
        /// </summary>
        public string StringId
        {
            get => myStringID;
            set => myStringID = value;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="OpenXMLHeading" />
    /// </summary>
    public class OpenXMLHeading
    {
        #region Fields

        /// <summary>
        /// Defines the myIndex
        /// </summary>
        private int myIndex;

        /// <summary>
        /// Defines the myLevel
        /// </summary>
        private int myLevel;

        /// <summary>
        /// Defines the myStringId
        /// </summary>
        private string myStringId;

        /// <summary>
        /// Defines the myText
        /// </summary>
        private string myText;
        private int pageNumber;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the HeadingLevel
        /// </summary>
        public int HeadingLevel
        {
            get => myLevel;
            set => myLevel = value;
        }

        /// <summary>
        /// Gets or sets the Index
        /// </summary>
        public int Index
        {
            get => myIndex;
            set => myIndex = value;
        }

        /// <summary>
        /// Gets or sets the StringID
        /// </summary>
        public string StringID
        {
            get => myStringId;
            set => myStringId = value;
        }

        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public string Text
        {
            get => myText;
            set => myText = value;
        }
        public int PageNumber { get => pageNumber; set => pageNumber = value; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="OpenXMLHeadingList" />
    /// </summary>
    public class OpenXMLHeadingList : List<OpenXMLHeading>
    {
    }

    public class WPDocumentList : List<WordprocessingDocument>
    {

    }


    /// <summary>
    /// Defines the <see cref="ParagraphElement" />
    /// </summary>
    public class ParagraphElement
    {
        #region Fields

        /// <summary>
        /// Defines the EndPosition
        /// </summary>
        public int EndPosition = 0;

        /// <summary>
        /// Defines the Paragraph
        /// </summary>
        public Paragraph Paragraph = null;

        /// <summary>
        /// Defines the rawtext
        /// </summary>
        public string rawtext = string.Empty;

        /// <summary>
        /// Defines the Runs
        /// </summary>
        public RunElementList Runs = new RunElementList();

        /// <summary>
        /// Defines the StartPosition
        /// </summary>
        public int StartPosition = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Length
        /// </summary>
        public int Length => Runs.Sum(item => item.Length);

        #endregion

        #region Methods

        public void AddChar(int Position, string newChar)
        {
            RunElement myRun = GetRun(Position);
            if (myRun != null)
            {
                myRun.EndPosition = myRun.StartPosition + myRun.Length - 1;
                if (myRun.EndPosition < myRun.StartPosition) myRun.EndPosition = myRun.StartPosition;
                Text text = myRun.Run.Descendants<Text>().FirstOrDefault();
                int pos = Position - StartPosition;
                if (myRun.StartPosition > 1)
                {
                    pos -= myRun.StartPosition;
                    pos -= 1;
                }
                if (pos >= myRun.EndPosition || Position == StartPosition)
                {
                    string innerText = myRun.Run.InnerText + newChar;
                    if (text != null)
                    {
                        myRun.Run.ReplaceChild(new Text(innerText), text);
                    }
                    else myRun.Run.AppendChild(new Text(innerText));
                }
                else if (pos >= 0 && pos <= myRun.Length)
                {
                    if (text != null)
                    {
                        string start = text.Text.Substring(0, pos);
                        string end = text.Text.Substring(pos);
                        string innerText = start + newChar + end;
                        myRun.Run.ReplaceChild(new Text(innerText), text);
                    }
                }
                myRun.RawText = myRun.Run.InnerText;
                myRun.EndPosition = myRun.StartPosition + myRun.Length - 1;
            }
        }

        /// <summary>
        /// Delete the character at the indicated position, 1 based.
        /// </summary>
        /// <param name="position">The <see cref="int"/></param>
        public void DeleteChar(int position, bool left)
        {
            RunElement myRun;
            if (left)
            {
                myRun = GetRun(position);
            }
            else
                myRun = GetRun(position);


            Run oldRun = myRun.Run;
            int step = position;
            if (myRun.StartPosition > 0)
            {
                step -= myRun.StartPosition;
            }

            step += 1;

            myRun.DeleteChar(step);

            Runs.MoveEndPoints(myRun, -1);
            EndPosition = StartPosition + Runs.MaxEndPosition();


            // check we still need a run
            if (myRun.RawText.Length == 0)
            {
                if (oldRun.Parent is Hyperlink)
                {
                    Hyperlink hyper = oldRun.Parent as Hyperlink;
                    hyper.RemoveChild(myRun.Run);
                    if (hyper.ChildElements.Count == 0)
                    {
                        Paragraph.RemoveChild(hyper);
                    }
                }
                else
                    Paragraph.RemoveChild(oldRun);
                Runs.Remove(myRun);
            }
            else
            {
                //oldRun.Parent.ReplaceChild(myRun.run, oldRun);
            }

            rawtext = GetRawText();
        }

        /// <summary>
        /// </summary>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 07/03/2018 07/03/2018 </created>
        /// </remarks>
        public void FixTrailingSpaces()
        {
            foreach (RunElement item in Runs)
            {
                if (item.RawText.Length > 0 && item.RawText[item.Length - 1] == ' ')
                {
                    Text oldText = item.Run.Descendants<Text>().FirstOrDefault();
                    Text newText = new Text(item.RawText.Substring(0, item.Length - 1));
                    item.Run.ReplaceChild<Text>(newText, oldText);
                    item.RawText = item.Run.InnerText;
                    item.EndPosition = item.StartPosition + item.Length - 1;
                    RunElement newElem = RunElement.SingleSpaceRunElement();
                    Paragraph.InsertAfter<Run>(newElem.Run, item.Run);
                    newElem.StartPosition = item.EndPosition + 1;
                    newElem.EndPosition = newElem.StartPosition;
                    Runs.Insert(Runs.IndexOf(item) + 1, newElem);
                    break;
                }
            }
        }

        /// <summary>
        /// The GetRawText
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string GetRawText()
        {

            string retValue = string.Empty;
            foreach (RunElement item in Runs)
            {
                item.RawText = item.Run.Descendants<Text>().FirstOrDefault().Text;
                retValue += item.RawText;
            }
            return retValue;
        }

        /// <summary>
        /// The GetRun
        /// </summary>
        /// <param name="position">The <see cref="int"/></param>
        /// <returns>The <see cref="Run"/></returns>
        public RunElement GetRun(int position)
        {
            RunElement returnValue = Runs.FirstOrDefault();

            // int progress = 0;
            int step = position;

            RunElement lastRun = null;
            foreach (RunElement item in Runs)
            {
                // check and if necessary fix start position
                if (lastRun != null)
                {
                    if (item.StartPosition <= lastRun.EndPosition)
                    {
                        item.StartPosition = lastRun.EndPosition + 1;
                    }
                }
                // check and if neccesary fix end position
                if (item.EndPosition < item.StartPosition)
                {
                    item.EndPosition = item.StartPosition;
                }
                // adjust end position
                item.EndPosition = item.StartPosition + item.Length - 1;
                if (item.StartPosition <= step && item.EndPosition >= step)
                {
                    returnValue = item;
                    break;
                }
                lastRun = item;

            }
            if (returnValue == null && Runs.Count == 0)
            {
                returnValue = new RunElement
                {
                    Run = Paragraph.Descendants<Run>().FirstOrDefault()
                };

                if (returnValue.Run == null)
                {
                    returnValue.Run = new Run();
                }
                returnValue.RawText = returnValue.Run.InnerText;
                returnValue.StartPosition = 1;
                returnValue.EndPosition = 1;
                Runs.Add(returnValue);
            }
            if (returnValue == null && Runs.Count > 0)
            {
                returnValue = Runs.Last();
            }
            return returnValue;
        }

        /// <summary>
        /// The GetStyle
        /// </summary>
        /// <returns>The <see cref="ParagraphStyleId"/></returns>
        public ParagraphStyleId GetStyle()
        {
            ParagraphProperties props = Paragraph.ParagraphProperties;
            ParagraphStyleId style1 = props?.ParagraphStyleId;
            return style1;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ParagraphElementList" />
    /// </summary>
    public class ParagraphElementList : List<ParagraphElement>
    {
        #region Methods

        public ParagraphElement FindByPosition(int pos)
        {
            return Find(x => x.StartPosition <= pos && x.EndPosition >= pos);
        }

        public ParagraphElement FindByPositionPlusOne(int pos)
        {
            return Find(x => x.StartPosition <= pos && x.EndPosition + 1 >= pos);
        }

        /// <summary>
        /// The CreateElement
        /// </summary>
        /// <param name="para">The <see cref="Paragraph"/></param>
        /// <returns>The <see cref="ParagraphElement"/></returns>
        public ParagraphElement CreateElement(Paragraph para)
        {
            int Start = MaxEndPosition();
            if (Start == 0) Start = 1;
            else Start += 1;
            return CreateElement(para, Start);
        }

        /// <summary>
        /// The CreateElement
        /// </summary>
        /// <param name="para">The <see cref="Paragraph"/></param>
        /// <param name="start">The <see cref="int"/></param>
        /// <returns>The <see cref="ParagraphElement"/></returns>
        public ParagraphElement CreateElement(Paragraph para, int start)
        {
            // add to list
            ParagraphElement paragraphElement = new ParagraphElement
            {
                Paragraph = para,
                StartPosition = start
            };
            Add(paragraphElement);

            // get run info
            IEnumerable<Run> RunList = para.Descendants<Run>();
            foreach (Run run in RunList)
            {

                RunElement newRun = new RunElement()
                {
                    Run = run,

                    RawText = run.InnerText
                };
                newRun.StartPosition = paragraphElement.Runs.MaxEndPosition() + 1;
                newRun.EndPosition = paragraphElement.Runs.MaxEndPosition() + run.InnerText.Length - 1;
                if (run.RunProperties != null)
                {
                    RunProperties rpr = run.RunProperties;
                    if (rpr.Bold != null)
                    {
                        newRun.Bold = true;
                    }
                    if (rpr.Italic != null)
                    {
                        newRun.Italic = true;
                    }
                    if (rpr.RunStyle != null)
                    {
                        RunStyle rstyle = rpr.RunStyle;
                        newRun.RunStyle = rstyle.Val.InnerText;
                    }
                }
                paragraphElement.rawtext += newRun.RawText;
                paragraphElement.Runs.Add(newRun);
                paragraphElement.EndPosition = paragraphElement.StartPosition + paragraphElement.rawtext.Length - 1;
            }

            return paragraphElement;
        }

        /// <summary>
        /// The CreateParagraph
        /// </summary>
        /// <param name="body">The <see cref="Body"/></param>
        /// <returns>The <see cref="ParagraphElement"/></returns>
        public ParagraphElement CreateParagraph(Body body)
        {
            return CreateParagraph(body, "PlainText");
        }

        /// <summary>
        /// The CreateParagraph
        /// </summary>
        /// <param name="body">The <see cref="Body"/></param>
        /// <param name="style">The <see cref="string"/></param>
        /// <returns>The <see cref="ParagraphElement"/></returns>
        public ParagraphElement CreateParagraph(Body body, string style)
        {
            ParagraphElement anElement = null;
            Paragraph newPara = new Paragraph();
            ParagraphProperties prp = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId() { Val = style }
            };
            newPara.AppendChild(prp);
            Run newRun = new Run();
            newRun.AppendChild(new Text(""));
            newPara.AppendChild(newRun);
            if (anElement == null)
            {
                anElement = this[Count - 1];
            }
            body.InsertAfter(newPara, anElement.Paragraph);

            int endPos = anElement.EndPosition;
            anElement = CreateElement(newPara, MaxEndPosition() + 2);
            return anElement;
        }

        /// <summary>
        /// The MaxEndPosition
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int MaxEndPosition()
        {
            int maxEnd = 0;
            foreach (ParagraphElement item in this)
            {
                if (item.EndPosition > maxEnd)
                { maxEnd = item.EndPosition; }
            }
            return maxEnd;
        }

        /// <summary>
        /// The MoveEndPoints
        /// </summary>
        /// <param name="elem">The <see cref="ParagraphElement"/></param>
        /// <param name="move">The <see cref="int"/></param>
        public void MoveEndPoints(ParagraphElement elem, int move)
        {
            int index = IndexOf(elem);
            elem.EndPosition += move;

            for (int i = index + 1; i < Count; i++)
            {
                ParagraphElement e = this[i];
                e.StartPosition += move;
                e.EndPosition += move;
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="body">The <see cref="Body"/></param>
        public void Save(Body body)
        {
            body.RemoveAllChildren<Paragraph>();


            foreach (ParagraphElement item in this)
            {
                body.AppendChild<Paragraph>(item.Paragraph);
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="RunElement" />
    /// </summary>
    public class RunElement
    {
        #region Fields

        /// <summary>
        /// Defines the Bold
        /// </summary>
        public bool Bold = false;

        /// <summary>
        /// Defines the EndPosition
        /// </summary>
        public int EndPosition = 0;

        /// <summary>
        /// Defines the italic
        /// </summary>
        public bool Italic = false;

        /// <summary>
        /// Defines the RawText
        /// </summary>
        public string RawText = string.Empty;

        private Run myRun = null;

        /// <summary>
        /// Defines the run
        /// </summary>
        public Run Run
        {
            get => myRun;
            set
            {
                myRun = value;
                if (value != null)
                {
                    if (myRun.RunProperties != null)
                    {
                        Bold = myRun.RunProperties.Bold != null;
                        Italic = myRun.RunProperties.Italic != null;
                    }
                }
            }
        }


        /// <summary>
        /// Defines the RunStyle
        /// </summary>
        public string RunStyle = string.Empty;

        /// <summary>
        /// Defines the StartPosition
        /// </summary>
        public int StartPosition = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Length
        /// </summary>
        public int Length => RawText.Length;

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 07/03/2018 07/03/2018 </created>
        /// </remarks>
        public static Text PreserveSpaceText(string text)
        {
            Text t = new Text(text)
            {
                Space = SpaceProcessingModeValues.Preserve
            };
            return t;
        }

        public static Run SingleSpaceRun()
        {
            Run runSingleSpace = new Run();
            runSingleSpace.AppendChild(PreserveSpaceText(" "));
            return runSingleSpace;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 07/03/2018 07/03/2018 </created>
        /// </remarks>
        public static RunElement SingleSpaceRunElement()
        {
            RunElement singleSpaceRunElement = new RunElement();
            Run runSingleSpace = SingleSpaceRun();
            singleSpaceRunElement.Run = runSingleSpace;
            return singleSpaceRunElement;
        }

        /// <summary>
        /// The DeleteChar
        /// </summary>
        /// <param name="position">The <see cref="int"/></param>
        public void DeleteChar(int position)
        {
            Text myText = Run.Descendants<Text>().FirstOrDefault();
            if (myText != null && myText.Text.Length >= position)
            {
                string startText = string.Empty;
                string endText = string.Empty;
                if (position > 0)
                {
                    startText = myText.Text.Substring(0, position - 1);
                }

                if (position + 1 <= myText.Text.Length)
                {
                    endText = myText.Text.Substring(position);
                }

                Text newText = new Text(startText + endText);
                Run.ReplaceChild(newText, myText);
                RawText = newText.Text;
                EndPosition = StartPosition + RawText.Length - 1;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="RunElementList" />
    /// </summary>
    public class RunElementList : List<RunElement>
    {
        #region Methods

        public void MoveEndPoints(RunElement elem, int move)
        {
            int index = IndexOf(elem);
            elem.EndPosition += move;

            for (int i = index + 1; i < Count; i++)
            {
                RunElement e = this[i];
                e.StartPosition += move;
                e.EndPosition += move;
            }
        }

        /// <summary>
        /// The MaxEndPosition
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int MaxEndPosition()
        {
            int maxStart = 0;
            foreach (RunElement item in this)
            {
                if (item.EndPosition > maxStart)
                {
                    maxStart = item.EndPosition;
                }
            }
            return maxStart;
        }

        /// <summary>
        /// The MaxStartPosition
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int MaxStartPosition()
        {
            int maxStart = 0;
            foreach (RunElement item in this)
            {
                if (item.StartPosition > maxStart)
                {
                    maxStart = item.StartPosition;
                }
            }
            return maxStart;
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{DocumentFormat.OpenXml.Wordprocessing.Style}" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 01/03/2018 10:36 </created>
    /// </remarks>
    public class StyleList : List<DocumentStyle>
    {
    }

    /// <summary>
    /// Defines the <see cref="OpenXMLElement" />
    /// </summary>
    internal static class OpenXMLElement
    {
        #region Methods

        /// <summary>
        /// The GetFirstDescendant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The <see cref="OpenXmlElement"/></param>
        /// <returns>The <see cref="T"/></returns>
        public static T GetFirstDescendant<T>(this OpenXmlElement parent) where T : OpenXmlElement
        {
            var descendants = parent.Descendants<T>();

            if (descendants != null)
                return descendants.FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// The GetParent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The <see cref="OpenXmlElement"/></param>
        /// <returns>The <see cref="T"/></returns>
        public static T GetParent<T>(this OpenXmlElement child) where T : OpenXmlElement
        {
            while (child != null)
            {
                child = child.Parent;

                if (child is T)
                    return (T)child;
            }

            return null;
        }

        /// <summary>
        /// The IsEndBookmark
        /// </summary>
        /// <param name="endBookmark">The <see cref="BookmarkEnd"/></param>
        /// <param name="startBookmark">The <see cref="BookmarkStart"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsEndBookmark(this BookmarkEnd endBookmark, BookmarkStart startBookmark)
        {
            if (endBookmark == null)
                return false;

            return endBookmark.Id == startBookmark.Id;
        }

        /// <summary>
        /// The IsEndBookmark
        /// </summary>
        /// <param name="element">The <see cref="OpenXmlElement"/></param>
        /// <param name="startBookmark">The <see cref="BookmarkStart"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsEndBookmark(this OpenXmlElement element, BookmarkStart startBookmark)
        {
            return IsEndBookmark(element as BookmarkEnd, startBookmark);
        }

        #endregion
    }
}