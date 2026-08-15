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
        private int? zoomDuration;

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

        public int? ZoomDuration
        {
            get => zoomDuration;
            set => this.RaiseAndSetIfChanged(ref zoomDuration, value);
        }

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
                MoviePath = this.MoviePath,
                FrameSetHeaderId = this.FrameSetHeaderId,
                ZoomDuration = this.ZoomDuration
            };
            return clone;
        }

        internal static void RenameMovieFile(FrameSet currentFrameSet, FrameSet last)
        {
            string frameSetName = "FrameSet" + currentFrameSet.Index.ToString("000").Trim() + ".mp4";
            string newFramesetName = "FrameSet" + last.Index.ToString("000").Trim() + ".mp4";

            string oldPath = currentFrameSet.MoviePath;
            string newPath = currentFrameSet.MoviePath.Replace(frameSetName, newFramesetName);

            if (File.Exists(oldPath) && !File.Exists(newPath))
            {
                // move old to new
                File.Move(oldPath, newPath);
                last.MoviePath = newPath;
                last.HasMovie = true;
                last.Save();
            }
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