//using static TaymadeEntities.Support.MissingFileFinder;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaymadeEntities.Models
{
    /// <summary>
    /// </summary>
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 01/08/2026 10:30 </created>
    /// </remarks>
    public class FrameSetHeader
    {
        private List<FrameSet>? frameSetList;

        public int Id { get; set; }

        [NotMapped]
        public List<FrameSet>? FrameSetList
        {
            get => frameSetList;
            set => frameSetList = value;
        }

        public int MovieImageId { get; set; } = 0;

        [JsonPropertyName("SplitIntoMovies")]
        public bool SplitIntoMovies { get; set; } = false;
        [JsonPropertyName("MaxXSize")]
        public int MaxXSize { get; internal set; }
        [JsonPropertyName("MaxYSize")]
        public int MaxYSize { get; internal set; }
    }
    //public class FolderProperties
    //{
    //    public FolderProperties(string? path)
    //    {
    //        if (!string.IsNullOrEmpty(path))
    //        {
    //            Path = path;
    //            Load();
    //        }
    //        else Save();
    //    }

    //    public FolderProperties()
    //    {
    //    }

    //    public void Load(string path = "")
    //    {
    //        if (string.IsNullOrEmpty(path)) path = PropertiesFileName();
    //        string json = string.Empty;

    //        if (File.Exists(path))
    //        {
    //            using StreamReader reader = new StreamReader(path);
    //            json = reader.ReadToEnd();

    //            FolderProperties? props = JsonConvert.DeserializeObject<FolderProperties>(json);

    //            if (props != null)
    //            {
    //                Speed = props.Speed;
    //                Path = props.Path;
    //                Comments = props.Comments;
    //                MovieId = props.MovieId;
    //            }
    //        }
    //    }

    //    public FolderProperties(double speed, string path)
    //    {
    //        Speed = speed;
    //        Path = path;

    //        if (!File.Exists(PropertiesFileName()))
    //        {
    //            this.Save();
    //        }
    //    }

    //    public string? Comments { get; set; } = "<comment>";

    //    public double? Speed { get; set; } = 5;

    //    public string? Path { get; set; }

    //    public int? MovieId { get; set; }

    //    #region Methods

    //    public string PropertiesFileName()
    //    {
    //        string returnVal = "";
    //        returnVal = Path + @"\Properties.json";

    //        return returnVal;
    //    }
    //    public void Save()
    //    {
    //        string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);

    //        using System.IO.StreamWriter writer = new StreamWriter(PropertiesFileName(), false);
    //        writer.WriteLine(json);
    //        writer.Flush();
    //        writer.Close();
    //    }
    //    #endregion
    //}
}