//using static TaymadeEntities.Support.MissingFileFinder;
using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaymadeEntities.Models
{
    
    public class FrameSetCollection : List<FrameSet>
    {
        

        private FrameSet? currentFrameSet;

        public FrameSetCollection()
        {
        }


        public FrameSetCollection(IEnumerable<FrameSet> collection) : base(collection)
        {
        }

        

       

        public FrameSetHeader? Parent { get; set; }

        //public FrameSet? CurrentFrameSet
        //{
        //    get => currentFrameSet;
        //    set
        //    {
        //        currentFrameSet = value;

        //    }
        //}        

        public FrameSet? NextItem(FrameSet currentFrameSet)
        {
            FrameSet? returned = null;
            if (currentFrameSet != null)
            {
                int ind = this.IndexOf(currentFrameSet);
                if (ind < this.Count-1)
                {
                    returned = this[ind + 1];
                }
            }

            return returned;
        }


        public FrameSet? PreviousItem(FrameSet currentFrameSet)
        {
            FrameSet? returned = null;
            if (currentFrameSet != null )
            {
                int ind = this.IndexOf(currentFrameSet);
                if (ind > 0)
                {
                    returned = this[ind-1];
                }
            }
            return returned;
        }

        //public void RaisePropertyChanging(PropertyChangingEventArgs args)
        //{
        //    ((IReactiveObject)CurrentFrameSet).RaisePropertyChanging(args);
        //}

        //public void RaisePropertyChanged(PropertyChangedEventArgs args)
        //{
        //    ((IReactiveObject)CurrentFrameSet).RaisePropertyChanged(args);
        //}
    }
}