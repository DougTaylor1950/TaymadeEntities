//-----------------------------------------------------------------------
// <copyright file="BookmarkPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>28/06/2020 13:47:06 28/06/2020 13:47:06 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Avalonia.Media.Imaging;
    using TaymadeEntities.Support;
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Bookmark" />.
    /// </summary>
    public partial class Bookmark : ModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the bookmarkTime.
        /// </summary>
        internal readonly ObservableAsPropertyHelper<string> bookmarkTime;

        /// <summary>
        /// Defines the imageBMP.
        /// </summary>
        internal Bitmap? imageBMP;

        /// <summary>
        /// Defines the bMPVisible.
        /// </summary>
        private bool? bMPVisible;

        /// <summary>
        /// Defines the formattedTime.
        /// </summary>
        private string? formattedTime = string.Empty;

        /// <summary>
        /// Defines the hours.
        /// </summary>
        private double hours = 0;

        /// <summary>
        /// Defines the milli.
        /// </summary>
        private double milli;

        /// <summary>
        /// Defines the minutes.
        /// </summary>
        private double minutes = 0;

        /// <summary>
        /// Defines the seconds.
        /// </summary>
        private double seconds = 0;

        /// <summary>
        /// Defines the timeSpan.
        /// </summary>
        private TimeSpan timeSpan;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BMPVisible
        /// Gets or sets a value indicating whether BMPVisible...
        /// </summary>
        [NotMapped]
        public bool? BMPVisible { get => bMPVisible; set => this.RaiseAndSetIfChanged(ref bMPVisible, value); }

        /// <summary>
        /// Gets the BookmarkTime.
        /// </summary>
        public string? BookmarkTime => bookmarkTime.Value;

        /// <summary>
        /// Gets or sets the FormattedTime
        /// Gets the FormattedTime.....
        /// </summary>
        [NotMapped]
        public string? FormattedTime
        {
            get
            {
                if ((string.IsNullOrEmpty(formattedTime) || formattedTime == "00:00:00") && Time != null) SetFormattedTime();
                return formattedTime;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref formattedTime, value);
                SetTime();
            }
        }

        /// <summary>
        /// Gets or sets the Hours.
        /// </summary>
        [NotMapped]
        public double Hours
        {
            get
            {
                GetSplitTime();
                return hours;

            }

            set
            {
                hours = value;
                SetSplitTime();
            }
        }

        /// <summary>
        /// Gets or sets the ImageBMP.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.Imaging.Bitmap? ImageBMP
        {
            get
            {
                if (imageBMP == null)
                {
                    SetImageBMP();
                }

                BMPVisible = (imageBMP != null);

                return imageBMP;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref imageBMP, value);
                BMPVisible = (imageBMP != null);
            }
        }

        /// <summary>
        /// Gets or sets the Milli.
        /// </summary>
        [NotMapped]
        public double Milli
        {
            get
            {
                GetSplitTime();
                return milli;

            }

            set
            {
                milli = value;
                SetSplitTime();
            }
        }

        /// <summary>
        /// Gets or sets the Minutes.
        /// </summary>
        [NotMapped]
        public double Minutes
        {
            get
            {
                GetSplitTime();
                return minutes;

            }

            set
            {
                minutes = value;
                SetSplitTime();
            }
        }

        /// <summary>
        /// Gets or sets the Seconds.
        /// </summary>
        [NotMapped]
        public double Seconds
        {
            get
            {
                GetSplitTime();
                return seconds;

            }

            set
            {
                seconds = value;
                SetSplitTime();
            }
        }

        /// <summary>
        /// Gets or sets the TimeSpan.
        /// </summary>
        [NotMapped]
        public TimeSpan TimeSpan { get => timeSpan; set => this.RaiseAndSetIfChanged(ref timeSpan, value); }

        #endregion

        #region Methods

        /// <summary>
        /// The Redisplay.
        /// </summary>
        public void Redisplay()
        {
            FormattedTime = FormattedTime;
            this.RaisePropertyChanged();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        internal void Delete()
        {
            Movies = null;
            var local = DataController.SandboxEntities.Set<Bookmark>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }

            DataController.SandboxEntities.Bookmarks.Remove(this);
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        internal void Insert()
        {
            DataController.SandboxEntities.Bookmarks.Add(this);
            DataController.SandboxEntities.SaveChanges();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            var local = DataController.SandboxEntities.Set<Bookmark>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
               // DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DataController.SandboxEntities.SaveChanges();
        }

        internal async Task<bool> SaveAsync()
        {
            bool success = false;
            var local = DataController.SandboxEntities.Set<Bookmark>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

            // check if local is not null
            if (local != null)
            {
                // detach
                // DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
            }
            DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
           int count = await DataController.SandboxEntities.SaveChangesAsync();
            success = (count == 1);
            return success;
        }

        /// <summary>
        /// The SetImageBMP.
        /// </summary>
        internal void SetImageBMP()
        {
            if (!string.IsNullOrEmpty(Support.FixImagePath(ImagePath)))
            {
                string fileName = Support.FixImagePath(ImagePath);
                if (System.IO.File.Exists(fileName) && imageBMP == null)
                {
                    ImageBMP = Support.GetBMP(fileName);
                }
            }
        }

        /// <summary>
        /// The GetSplitTime.
        /// </summary>
        private void GetSplitTime()
        {
            if (Time != null)
            {
                TimeSpan dt = TimeSpan.Parse(FormattedTime);
                hours = dt.Hours;
                minutes = dt.Minutes;
                seconds = dt.Seconds;
                milli = dt.Milliseconds;
            }
        }

        /// <summary>
        /// The SetFormattedTime.
        /// </summary>
        private void SetFormattedTime()
        {
            CultureInfo info = new CultureInfo("en-UK");
            if (Time != null && Time > 0)
            {
                //TimeSpan time;//= new TimeSpan(0, 0, (int)Time.Value);
                TimeSpan = TimeSpan.FromSeconds(Time.Value);
                FormattedTime = TimeSpan.ToString(@"hh\:mm\:ss", info);
            }
            else
            {
                FormattedTime = "00:00:00";
            }
        }

        /// <summary>
        /// The SetSplitTime.
        /// </summary>
        private void SetSplitTime()
        {
            TimeSpan ts = new TimeSpan(0, (int)hours, (int)minutes, (int)seconds, (int)milli);
            Time = ts.TotalSeconds;
        }

        /// <summary>
        /// The SetTime.
        /// </summary>
        private void SetTime()
        {
            if (TimeSpan.TryParse(formattedTime, out TimeSpan ts))
            {
                //time = ts.TotalSeconds;
                this.RaiseAndSetIfChanged(ref time, ts.TotalSeconds, "Time");
            }
            else if (double.TryParse(formattedTime, out double intvalue))
            {
                //time = intvalue;
                this.RaiseAndSetIfChanged(ref time, intvalue, "Time");

            }
        }

        #endregion
    }
}
