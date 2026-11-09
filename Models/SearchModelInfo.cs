using System;
using System.Collections.Generic;
using System.Text;

namespace TaymadeEntities.Models
{
    public class SearchModelInfo
    {
        #region Public Properties

        public string? ActorName { get; set; }
        public string? BookmarkText { get; set; }
        public string? InfoText { get; set; }
        public int? MovieId { get; set; }
        public string? Title { get; set; }
        public List<TaymadeEntities.Models.Movies>? MovieList { get;  set; }

        #endregion Public Properties
    }
}