//-----------------------------------------------------------------------
// <copyright file="openxml.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>02/03/2018 09:50:26 02/03/2018 09:50:26 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace Support
{
    using OpenXmlPowerTools;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="WPSection" />.
    /// </summary>
    public class WPSection
    {
        #region Fields

        /// <summary>
        /// Defines the sectionHTML.
        /// </summary>
        private string sectionHTML;

        /// <summary>
        /// Defines the sectionHTMLX.
        /// </summary>
        private XElement sectionHTMLX;

        /// <summary>
        /// Defines the sectionName.
        /// </summary>
        private string sectionName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WPSection"/> class.
        /// </summary>
        /// <param name="wmlDocument">The wmlDocument<see cref="WmlDocument"/>.</param>
        /// <param name="sectionNo">The sectionNo<see cref="int"/>.</param>
        public WPSection(WmlDocument wmlDocument, int sectionNo)
        {
            WmlDocument = wmlDocument;
            SectionNo = sectionNo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SectionHTML.
        /// </summary>
        public string SectionHTML
        {
            get
            {
                if (string.IsNullOrEmpty(sectionHTML))
                {
                    if (SectionHTMLX != null) sectionHTML = SectionHTMLX.ToString();
                }
                return sectionHTML;
            }

            set => sectionHTML = value;
        }

        /// <summary>
        /// Gets or sets the SectionHTMLX.
        /// </summary>
        public XElement SectionHTMLX
        {
            get
            {
                if (sectionHTMLX == null)
                {
                    sectionHTMLX = WmlDocument.ConvertToHtml(new OpenXmlPowerTools.WmlToHtmlConverterSettings());
                }
                return sectionHTMLX;
            }
            set => sectionHTMLX = value;
        }

        /// <summary>
        /// Gets the SectionName.
        /// </summary>
        public string SectionName
        {
            get
            {
                if (string.IsNullOrEmpty(sectionName))
                {
                    sectionName = "Section " + SectionNo.ToString();
                }
                return sectionName;
            }
        }

        public string SectionNameID
        {
            get
            {
                return SectionName.Replace(" ", "_");
            }
        }
        public string SectionNameIDHash
        {
            get
            {
                return "#" + SectionName.Replace(" ", "_");
            }
        }

        /// <summary>
        /// Gets or sets the SectionNo.
        /// </summary>
        public int SectionNo { get; set; }

        /// <summary>
        /// Gets or sets the WmlDocument.
        /// </summary>
        public WmlDocument WmlDocument { get; set; }

        #endregion
    }
}
