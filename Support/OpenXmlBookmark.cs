//-----------------------------------------------------------------------
// <copyright file="openxml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>02/03/2018 09:50:26 02/03/2018 09:50:26 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support.Word
{
    using DocumentFormat.OpenXml.Wordprocessing;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="OpenXmlBookmark" />
    /// </summary>
    public class OpenXmlBookmark
    {
        #region Fields

        /// <summary>
        /// Defines the bookmarkIndex
        /// </summary>
        private int bookmarkIndex;

        /// <summary>
        /// Defines the myEnd
        /// </summary>
        private BookmarkEnd myEnd = null;

        /// <summary>
        /// Defines the mySectionType
        /// </summary>
        private OpenXML.DocumentSection mySectionType;

        /// <summary>
        /// Defines the myStart
        /// </summary>
        private BookmarkStart myStart = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BookmarkIndex
        /// </summary>
        public int BookmarkIndex
        {
            get => bookmarkIndex;
            set => bookmarkIndex = value;
        }

        /// <summary>
        /// Gets or sets the End
        /// </summary>
        public BookmarkEnd End
        {
            get => myEnd;
            set => myEnd = value;
        }

        /// <summary>
        /// Gets the Id
        /// </summary>
        public String Id => Start?.Id;

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => Start?.Name;

        /// <summary>
        /// Gets or sets the SectionType
        /// </summary>
        public OpenXML.DocumentSection SectionType
        {
            get => mySectionType;
            set => mySectionType = value;
        }

        /// <summary>
        /// Gets or sets the Start
        /// </summary>
        public BookmarkStart Start
        {
            get => myStart;
            set => myStart = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The AddTextAfter
        /// </summary>
        /// <param name="newText">The <see cref="string"/></param>
        public void AddTextAfter(string newText)
        {
            if (End != null)
            {
                IEnumerable<Paragraph> paras = End.Ancestors<Paragraph>();

                if (paras != null && paras.Count() > 0)
                {
                    Paragraph para = paras.First();
                    if (para != null)
                    {

                        var runElement = new Run(new Text(newText));
                        para.InsertAfter(runElement, End);
                    }
                }
            }
        }

        /// <summary>
        /// The BookmarkText
        /// </summary>
        /// <returns>The <see cref="Text"/></returns>
        public Text BookmarkText()
        {
            var run = Start.NextSibling<Run>();

            if (run != null)
                // I've found a run and suppose it has a Text
                return run.GetFirstChild<Text>();
            else
            {
                // I will go through all the siblings and try to find any Text
                Text text = null;
                var nextSibling = Start.NextSibling();
                while (text == null && nextSibling != null)
                {
                    if (nextSibling.IsEndBookmark(Start))
                        // I've reached the end of the bookmark and couldn't find any Text
                        return null;

                    text = nextSibling.GetFirstDescendant<Text>();
                    nextSibling = nextSibling.NextSibling();
                }

                return text;
            }
        }

        /// <summary>
        /// The FindTextInColumn
        /// </summary>
        /// <returns>The <see cref="Text"/></returns>
        public Text FindTextInColumn()
        {
            var cell = Start.GetParent<TableRow>().GetFirstChild<TableCell>();

            for (int i = 0; i < Start.ColumnFirst; i++)
            {
                cell = cell.NextSibling<TableCell>();
            }

            return cell.GetFirstDescendant<Text>();
        }

        /// <summary>
        /// The RemoveBookmark
        /// </summary>
        public void RemoveBookmark()
        {
            if (Start != null)
            {
                Start.Remove();
            }

            if (End != null)
            {
                End.Remove();
            }
        }

        /// <summary>
        /// The ReplaceText
        /// </summary>
        /// <param name="newText">The <see cref="string"/></param>
        public void ReplaceText(string newText)
        {
            Run bookmarkText = Start.NextSibling<Run>();
            if (bookmarkText != null)
            {
                bookmarkText.GetFirstChild<Text>().Text = newText;
            }
        }

        #endregion
    }
}
