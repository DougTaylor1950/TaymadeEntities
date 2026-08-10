//using static TaymadeEntities.Support.MissingFileFinder;

using ReactiveUI;
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
    public class FrameSetHeader : ModelBase
    {
        #region Private Fields

        private int? defaultZoomFrames = 50;
        private int? frameRate = 20;
        private List<FrameSet>? frameSetList;
        private int movieImageId = 0;

        #endregion Private Fields

        #region Public Properties

        public int? DefaultZoomFrames
        {
            get => defaultZoomFrames;
            set => this.RaiseAndSetIfChanged(ref defaultZoomFrames, value);
        }

        public double? FPS { get; set; }
        public int? FrameRate
        {
            get => frameRate;
            set => this.RaiseAndSetIfChanged(ref frameRate, value);
        }

        [NotMapped]
        public List<FrameSet>? FrameSetList
        {
            get
            {
                if (frameSetList == null || frameSetList.Count == 0)
                {
                    frameSetList = DataController.MovieController.GetFrameSetsByHeaderId(Id);
                    if (frameSetList == null) frameSetList = new List<FrameSet>();
                }
                return frameSetList;
            }

            set => this.RaiseAndSetIfChanged(ref frameSetList, value);
        }

        public new int Id { get; set; }
        public int MaxXSize { get; internal set; }

        public int MaxYSize { get; internal set; }

        public int MovieImageId
        {
            get => movieImageId;
            set => this.RaiseAndSetIfChanged(ref movieImageId, value);
        }

        public bool SplitIntoMovies { get; set; } = false;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="imageItems">The image items.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 06/08/2026 06/08/2026 </created>
        /// </remarks>
        public void SetImageItemsFrameSets(ImageItemsCollection? imageItems)
        {
            // check we have a set to process
            if (imageItems == null) return;

            // go through each frameset
            foreach (FrameSet frameSet in FrameSetList)
            {
                // and set the image items for that frame set
                for (int i = frameSet.StartImage - 1; i < imageItems.Count; i++)
                {
                    var item = imageItems[i];
                    item.FrameSetIndex = frameSet.Index;
                }
            }

            // check last frame set ends correctly
            FrameSet? last = FrameSetList.LastOrDefault();
            if (last != null)
            {
                last.EndImage = imageItems.Count;
                last.Save();
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 06/08/2026 06/08/2026 </created>
        /// </remarks>
        internal FrameSet CreateFrameSet(int count)
        {
            if (this.FrameSetList == null)
            {
                this.FrameSetList = DataController.MovieController.GetFrameSetsByHeaderId(Id);
                if (this.FrameSetList == null)
                {
                    this.FrameSetList = new List<FrameSet>();
                }
            }

            FrameSet newFrameset = new FrameSet
            {
                Index = this.FrameSetList.Count + 1,
                FrameSetHeaderId = Id,
                FrameSetHeader = this,
                EndImage = count,
                ZoomDuration = 5,
                FrameRate = 0.2
            };
            newFrameset.Save();
            return newFrameset;
        }

        internal void Save()
        {
            DataController.MovieController.UpdateFrameSetHeader(this);
        }

        #endregion Internal Methods
    }
}