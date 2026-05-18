using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class MovieIntResult
    {
        public int Id { get; set; }

        public static List<int> GetMovieIds(List<MovieIntResult> actorMovies)
        {
            return  actorMovies.Select(am => am.Id).ToList();
        }
    }
}
