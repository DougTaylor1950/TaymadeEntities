using Newtonsoft.Json;

//using static TaymadeEntities.Support.MissingFileFinder;
using ReactiveUI;

namespace TaymadeEntities.Models
{
    public class FrameSet : ModelBase
    {
        #region Private Fields

        private bool hasMovie = false;
        private string? moviePath;

        #endregion Private Fields

        #region Public Properties

        [JsonProperty(PropertyName = "EndImage")]
        public int EndImage { get; set; }

        [JsonProperty(PropertyName = "FrameRate")]
        public double FrameRate { get; set; } = 1.0;

        [JsonIgnore()]
        public FrameSetHeader? FrameSetHeader { get; internal set; }

        public int FrameSetHeaderId { get; set; }

        [JsonProperty(PropertyName = "HasMovie")]
        public bool HasMovie
        {
            get => hasMovie;
            set
            {
                this.RaiseAndSetIfChanged(ref hasMovie, value);
            }
        }

        [JsonIgnore()]
        public new int Id { get; set; }

        [JsonProperty(PropertyName = "Index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "MoviePath")]
        public string? MoviePath
        {
            get => moviePath;
            set
            {
                this.RaiseAndSetIfChanged(ref moviePath, value);
            }
        }

        [JsonProperty(PropertyName = "StartImage")]
        public int StartImage { get; set; }

        [JsonProperty(PropertyName = "StartImageName")]
        public string? StartImageName { get; set; }

        #endregion Public Properties

        #region Internal Methods

        internal FrameSet Clone()
        {
            FrameSet clone = new FrameSet()
            {
                Id = this.Id,
                StartImage = this.StartImage,
                EndImage = this.EndImage,
                StartImageName = this.StartImageName,
                Index = this.Index,
                FrameRate = this.FrameRate,
                HasMovie = this.HasMovie,
                MoviePath = this.MoviePath
            };
            return clone;
        }

        internal void Delete()
        {
            DataController.MovieController.DeleteFrameSet(this);
        }

        internal void Save()
        {
            DataController.MovieController.UpdateFrameSet(this);
        }

        internal void SetStartAndEnd(int start, int end)
        {
            StartImage = start;
            EndImage = end;
        }

        #endregion Internal Methods
    }
}