using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;

namespace TaymadeEntities.Comparators
{
    public class MovieComparerByChapters : Comparer<Movies>
    {
        public override int Compare(Movies? x, Movies? y)
        {
            if (x == null || y == null || x.HasChapters == null || y.HasChapters == null)
                return 0;
            return x.CompareToByChapters(y);
        }
    }

}
