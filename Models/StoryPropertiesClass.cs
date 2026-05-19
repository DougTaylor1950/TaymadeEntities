//-----------------------------------------------------------------------
// <copyright file="StoryPropertiesClass.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>18/08/2022 11:14:48 18/08/2022 11:14:48 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Newtonsoft.Json;
    //using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="PropertiesClass" />.
    /// </summary>
    public class PropertiesClass
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public string Value { get; set; }

        public List<string> ValueList { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="PropertiesCLassList" />.
    /// </summary>
    public class PropertiesCLassList : List<PropertiesClass>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesCLassList"/> class.
        /// </summary>
        public PropertiesCLassList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesCLassList"/> class.
        /// </summary>
        /// <param name="collection">The collection<see cref="IEnumerable{PropertiesClass}"/>.</param>
        public PropertiesCLassList(IEnumerable<PropertiesClass> collection) : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesCLassList"/> class.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        public PropertiesCLassList(string fileName)
        {
            PropertiesCLassList temp = LoadFromJson(fileName);

            foreach (var item in temp)
            {
                Add(item);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The LoadFromJson.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <returns>The <see cref="ReplaceCLassList"/>.</returns>
        public PropertiesCLassList LoadFromJson(string fileName)
        {
            string json = string.Empty;

            using (StreamReader streamReader = new StreamReader(fileName))
            {
                json = streamReader.ReadToEnd();
                streamReader.Close();
            }

            List<PropertiesClass> propertiesClasses = JsonConvert.DeserializeObject<List<PropertiesClass>>(json);

            return new PropertiesCLassList(propertiesClasses);
        }

        /// <summary>
        /// The SaveList.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        public void SaveList(string fileName)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="StoryPropertiesClass" />.
    /// </summary>
    public class StoryPropertiesClass
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Dk.
        /// </summary>
        public bool? Dk { get; set; }

        /// <summary>
        /// Gets or sets the Find.
        /// </summary>
        public string Find { get; set; }

        /// <summary>
        /// Gets or sets the Format.
        /// </summary>
        public bool? Format { get; set; }

        /// <summary>
        /// Gets or sets the MatchCase.
        /// </summary>
        public bool? MatchCase { get; set; }

        /// <summary>
        /// Gets or sets the Method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the Repeat.
        /// </summary>
        public bool? Repeat { get; set; }

        /// <summary>
        /// Gets or sets the Replace.
        /// </summary>
        public string Replace { get; set; }

        /// <summary>
        /// Gets or sets the Style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the WholeWords.
        /// </summary>
        public bool? WholeWords { get; set; }

        /// <summary>
        /// Gets or sets the WildCards.
        /// </summary>
        public bool? WildCards { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ReplaceCLassList" />.
    /// </summary>
    public class ReplaceCLassList : List<StoryPropertiesClass>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceCLassList"/> class.
        /// </summary>
        public ReplaceCLassList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceCLassList"/> class.
        /// </summary>
        /// <param name="collection">The collection<see cref="IEnumerable{ReplaceClass}"/>.</param>
        public ReplaceCLassList(IEnumerable<StoryPropertiesClass> collection) : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceCLassList"/> class.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        public ReplaceCLassList(string fileName)
        {
            ReplaceCLassList temp = LoadFromJson(fileName);

            foreach (var item in temp)
            {
                Add(item);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The LoadFromJson.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        /// <returns>The <see cref="ReplaceCLassList"/>.</returns>
        public ReplaceCLassList LoadFromJson(string fileName)
        {
            string json = string.Empty;

            using (StreamReader streamReader = new StreamReader(fileName))
            {
                json = streamReader.ReadToEnd();
                streamReader.Close();
            }

            List<StoryPropertiesClass> replaceClasses = JsonConvert.DeserializeObject<List<StoryPropertiesClass>>(json);

            return new ReplaceCLassList(replaceClasses);
        }

        /// <summary>
        /// The SaveList.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        public void SaveList(string fileName)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        #endregion
    }
}
