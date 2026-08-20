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
        private FrameSetCollection? frameSetList;
        private int? movieImageId = 0;
        private int? lastFrameSetIndex;
        private FrameSet? titleFrameSet;

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
        public FrameSetCollection? FrameSetList
        {
            get
            {
                if (frameSetList == null || frameSetList.Count == 0)
                {
                    frameSetList = DataController.MovieController.GetFrameSetsByHeaderId(Id);
                    if (frameSetList == null)
                    {
                        frameSetList = new FrameSetCollection();
                        frameSetList.Parent = this;
                    }
                }
                return frameSetList;
            }

            set => this.RaiseAndSetIfChanged(ref frameSetList, value);
        }

        public new int Id { get; set; }

        public int? LastFrameSetIndex
        {
            get => lastFrameSetIndex;
            set => this.RaiseAndSetIfChanged(ref lastFrameSetIndex, value);
        }
        public int MaxXSize { get; internal set; }

        public int MaxYSize { get; internal set; }

        public int? MovieImageId
        {
            get => movieImageId;
            set => this.RaiseAndSetIfChanged(ref movieImageId, value);
        }

        public bool SplitIntoMovies { get; set; } = false;

        [NotMapped]
        public FrameSet? TitleFrameSet
        {
            get => titleFrameSet;
            set => this.RaiseAndSetIfChanged(ref titleFrameSet, value);
        }

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
                if (frameSet.Index > 0)
                {
                    if (frameSet.StartImage < 1) frameSet.StartImage = 1;
                    // and set the image items for that frame set

                    bool first = true; // set startimage name
                    for (int i = frameSet.StartImage - 1; i < imageItems.Count; i++)
                    {
                        var item = imageItems[i];
                        if (first)
                        {
                            frameSet.StartImageName = item.ImageName;
                            frameSet.Save();
                            first = false;
                        }
                        item.FrameSetIndex = frameSet.Index;
                    }
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
        internal FrameSet CreateFrameSet(int count = 0)
        {
            CheckFrameSetList();
            FrameSet newFrameset = MakeFrameSet();
            newFrameset.EndImage = count;
            return newFrameset;
        }

        internal FrameSet CreateFrameSetBefore(FrameSet? frameSet)
        {
            CheckFrameSetList();
            FrameSet newFrameset = MakeFrameSet();

            if (frameSet != null)
            {
                int indx = this.FrameSetList.IndexOf(frameSet);
                if (indx != -1)
                {
                    // this should insert before the supplied item
                    if (indx > 0)
                        this.FrameSetList.Insert(indx - 1, newFrameset);
                    else
                        this.FrameSetList.Insert(indx, newFrameset);
                    newFrameset.StartImage = frameSet.StartImage;
                    newFrameset.EndImage = frameSet.EndImage;
                    frameSet.StartImage += 1;
                }

                // if we have only one item exit
                if (this.FrameSetList.Count <= 1) return newFrameset;



                FrameSet? last = this.FrameSetList.LastOrDefault();
                for (int i = FrameSetList.Count - 2; i >= indx; i--)
                {
                    FrameSet? currentFrameSet = this.FrameSetList[i];
                    // move current to last
                    last.StartImage = currentFrameSet.StartImage;
                    last.EndImage = currentFrameSet.EndImage;
                    last.MoviePath = currentFrameSet.MoviePath;
                    last.StartImageName = currentFrameSet.StartImageName;
                    if (!string.IsNullOrEmpty(currentFrameSet.MoviePath))
                    {
                        FrameSet.RenameMovieFile(currentFrameSet, last);
                    }
                    last.Save();
                    last = currentFrameSet;
                }

                Reindex();
            }

            return newFrameset;
        }

        public void Refresh()
        {
            this.FrameSetList = DataController.MovieController.GetFrameSetsByHeaderId(this.Id);
        }

        internal void Reindex()
        {
            int indexer = 1;
            // go through list and reindex
            foreach (var item in this.frameSetList)
            {
                item.Index = indexer;
                item.Save();
                indexer += 1;
            }
        }

        internal void Save()
        {
            if (MovieImageId == 0) return; 
            if (Id == 0)
                DataController.MovieController.InsertFrameSetHeader(this);
            else

                DataController.MovieController.UpdateFrameSetHeader(this);
        }

        #endregion Internal Methods

        #region Private Methods

        private void CheckFrameSetList()
        {
            if (this.FrameSetList == null)
            {
                this.FrameSetList = DataController.MovieController.GetFrameSetsByHeaderId(Id);
                if (this.FrameSetList == null)
                {
                    this.FrameSetList = new FrameSetCollection();
                }
                this.FrameSetList.Parent = this;
            }
        }

        private FrameSet MakeFrameSet()
        {
            FrameSet newFrameset = new FrameSet
            {
                Index = this.FrameSetList.Count + 1,
                FrameSetHeaderId = Id,
                FrameSetHeader = this,
                ZoomDuration = 5,
                FrameRate = 0.2
            };
            newFrameset.Save(); // adds to framesetlist
            return newFrameset;
        }

        #endregion Private Methods
    }
}