using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace TaymadeEntities.Support
{
    /// <summary>
    /// Defines the <see cref="PhraseInfo" />
    /// </summary>
    public class PhraseInfo
    {
        #region Fields

        /// <summary>
        /// Defines the BackColour
        /// </summary>
        public string backColour = System.Drawing.ColorTranslator.ToHtml(Color.White);

        /// <summary>
        /// Defines the ForeColour
        /// </summary>
        public string foreColour = System.Drawing.ColorTranslator.ToHtml(Color.Black);

        public string ForeColour { get; set; }

        public string BackColour { get; set; }

        /// <summary>
        /// Defines the mybackcolour
        /// </summary>
        private System.Drawing.Color mybackcolour = Color.White;

        /// <summary>
        /// Defines the myforecolor
        /// </summary>
        private System.Drawing.Color myforecolor = Color.Black;

        /// <summary>
        /// Defines the mySeries
        /// </summary>
        private int mySeries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseInfo"/> class.
        /// </summary>
        public PhraseInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseInfo"/> class.
        /// </summary>
        /// <param name="xml">The <see cref="string"/></param>
        public PhraseInfo(string xml)
        {
            Deserialize(xml);
        }

        public string Path { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the backColour
        /// </summary>
        [XmlIgnore]
        public System.Drawing.Color BackColor
        {
            get
            {
                mybackcolour = System.Drawing.ColorTranslator.FromHtml(BackColour);
                return mybackcolour;
            }
            set
            {
                mybackcolour = value;
                backColour = System.Drawing.ColorTranslator.ToHtml(value);
            }
        }

        /// <summary>
        /// Gets or sets the foreColour
        /// </summary>
        [XmlIgnore]
        public System.Drawing.Color ForeColor
        {
            get
            {
                myforecolor = System.Drawing.ColorTranslator.FromHtml(ForeColour);
                return myforecolor;
            }
            set
            {
                myforecolor = value;
                ForeColour = System.Drawing.ColorTranslator.ToHtml(value);
            }
        }

        /// <summary>
        /// Gets or sets the Series
        /// </summary>
        public int Series
        {
            get => mySeries;
            set => mySeries = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Deserialize
        /// </summary>
        /// <param name="xml">The <see cref="string"/></param>
        public void Deserialize(string xml)
        {
            if ( !string.IsNullOrEmpty(xml) )
            {
                // Create an instance of the XmlSerializer specifying type and namespace.
                XmlSerializer serializer = new
                XmlSerializer(typeof(PhraseInfo));

                // A FileStream is needed to read the XML document.
                System.IO.StringReader fs = new System.IO.StringReader(xml);
                XmlReader reader = XmlReader.Create(fs);

                // Declare an object variable of the type to be deserialized.
                PhraseInfo i;

                // Use the Deserialize method to restore the object's state.
                i = (PhraseInfo)serializer.Deserialize(reader);
                fs.Close();

                BackColor = i.BackColor;
                
                BackColour = System.Drawing.ColorTranslator.ToHtml(BackColor);
                ForeColor = i.ForeColor;
                ForeColour = i.ForeColour;
                foreColour = System.Drawing.ColorTranslator.ToHtml(ForeColor);
                Series = i.Series;
                Path = i.Path;
            }
        }

        /// <summary>
        /// The Serialise
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string Serialise()
        {
            string retValue = "";

            XmlSerializer ser = new XmlSerializer(typeof(PhraseInfo));

            System.IO.StringWriter sw = new System.IO.StringWriter();
            ser.Serialize(sw, this);
            retValue = sw.ToString();

            return retValue;
        }

        #endregion
    }
}
