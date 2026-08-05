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
    public class FrameSetHeader:ModelBase
    {
        private List<FrameSet>? frameSetList;
        private int movieImageId = 0;

        public int Id { get; set; }

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

            set => this.RaiseAndSetIfChanged(ref  frameSetList, value);
        }

        public int MovieImageId 
        {
            get => movieImageId; 
            set => this.RaiseAndSetIfChanged(ref movieImageId, value); 
        }
        public bool SplitIntoMovies { get; set; } = false;

        public int MaxXSize { get; internal set; }
        public int MaxYSize { get; internal set; }

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
                Index = this.FrameSetList.Count +1,
                FrameSetHeaderId = Id,
                FrameSetHeader = this,
                EndImage = count
            };
            newFrameset.Save();
            return newFrameset;
        }
    }
    
}