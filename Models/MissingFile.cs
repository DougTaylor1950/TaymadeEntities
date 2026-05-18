using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class MissingFile : ModelBase
    {
        private string? path;
        private bool? isSelected;
        private int? durationSeconds;
        private TimeSpan movieDuration;

        public string? Path { get => path; set => this.RaiseAndSetIfChanged(ref path, value); }

        public bool? IsSelected { get => isSelected; set => this.RaiseAndSetIfChanged(ref isSelected, value); }

        /// <summary>
        /// Gets or sets the CreationTime.
        /// </summary>
        public string CreationTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the DurationSeconds.
        /// </summary>
        public Nullable<int> DurationSeconds
        {
            get
            {
                if (durationSeconds == null || durationSeconds == 0)
                {
                    SetError("Duration:Must be greater than Zero");
                    if (durationSeconds == null) durationSeconds = 0;
                }

                return durationSeconds;
            }

            set
            {
                if (value != null)
                {
                    this.RaiseAndSetIfChanged(ref durationSeconds, value);
                    MovieDuration = Movies.SetMovieDuration(value);
                    //this.SetPercentUnmarked();
                }
                else
                {
                    SetError("Duration:Must be greater than Zero");
                }
            }
        }

        private void SetMovieDurationValue()
        {
            if (movieDuration == TimeSpan.MinValue && DurationSeconds != null)
            {
                MovieDuration = Movies.SetMovieDuration(DurationSeconds);

                //this.RaiseAndSetIfChanged(ref movieDuration, value);
            }
            else if ((int)movieDuration.TotalSeconds != DurationSeconds && DurationSeconds != null)
            {
                MovieDuration = Movies.SetMovieDuration(DurationSeconds);
                // this.RaiseAndSetIfChanged(ref movieDuration, value);
                // this.RaisePropertyChanged("MovieDuration");
            }
            else if (movieDuration == TimeSpan.MinValue)
            {
                MovieDuration = Movies.SetMovieDuration(0);
                //movieDuration = TimeSpan.FromSeconds((Double)0);
            }
        }

        public TimeSpan MovieDuration
        {
            get
            {
                SetMovieDurationValue();
                return movieDuration;
            }

            set =>
                //movieDuration = value;
                this.RaiseAndSetIfChanged(ref movieDuration, value);
        }

        /// <summary>
        /// Gets or sets the FileInfo.
        /// </summary>
        public System.IO.FileInfo? FileInfo { get; set; }

        /// <summary>
        /// Gets or sets the FileLength.
        /// </summary>
        public long FileLength { get; set; }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(Path))
                    return System.IO.Path.GetFileName(Path);
                else return "";
            }
        }

        public override string ToString()
        {
            return this.Path + "-" + FileLength.ToString();
        }
    }

    public class MissingFileCollection : ObservableCollection<MissingFile>
    {
        public MissingFileCollection() { }

        public MissingFileCollection(IEnumerable<MissingFile> collection) : base(collection)
        {
        }

        public void DeselectAll()
        {
            foreach (var item in this)
            {
                item.IsSelected = false;
            }
        }

        public List<string> GetSelected()
        {
            List<string> returnList = new List<string>();

            foreach (var item in this)
            {
                if (item.IsSelected != null && item.IsSelected.Value) returnList.Add(item.Path);
            }

            returnList.Sort((x, y) => x.CompareTo(y));

            return returnList;
        }

        
    }
}
