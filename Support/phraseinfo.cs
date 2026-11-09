using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using ReactiveUI;
using Avalonia.Media;
using Color = Avalonia.Media.Color;

namespace TaymadeEntities.Support
{
    /// <summary>
    /// Defines the <see cref="PhraseInfo" />
    /// </summary>
    public class PhraseInfo : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// Defines the BackColour
        /// </summary>
        public string backColour = Colors.White.ToString();

        /// <summary>
        /// Defines the ForeColour
        /// </summary>
        public string foreColour = Colors.Black.ToString();

        public string ForeColour { get; set; }

        public string BackColour { get; set; }

        /// <summary>
        /// Defines the mybackcolour
        /// </summary>
        private Avalonia.Media.Color mybackcolour = Colors.White;

        /// <summary>
        /// Defines the myforecolor
        /// </summary>
        private Avalonia.Media.Color myforecolor = Colors.Black;

        /// <summary>
        /// Defines the mySeries
        /// </summary>
        private int mySeries;
        private string path;

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

        public string Path { get => path; set => this.RaiseAndSetIfChanged(ref path, value); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the backColour
        /// </summary>
        [XmlIgnore]
        public Avalonia.Media.Color BackColor
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BackColour))
                {
                    Color temp = Colors.White;

                    if (Color.TryParse(BackColour, out temp))
                    {
                        mybackcolour = temp;
                    }
                }

                return mybackcolour;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref mybackcolour, value);
                BackColour = value.ToString();

            }
        }

        /// <summary>
        /// Gets or sets the foreColour
        /// </summary>
        [XmlIgnore]
        public Color ForeColor
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ForeColour))
                {
                    Color temp = Colors.White;

                    if (Color.TryParse(ForeColour, out temp))
                    {
                        myforecolor = temp;
                    }
                }
                return myforecolor;
            }
            set
            {
                myforecolor = value;
                ForeColour = value.ToString();
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
            if (!string.IsNullOrEmpty(xml))
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

                BackColour = BackColor.ToString();
                ForeColor = i.ForeColor;
                ForeColour = i.ForeColour;
                foreColour = ForeColor.ToString();
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
