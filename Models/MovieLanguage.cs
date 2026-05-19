using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaymadeEntities.Models
{
    public class MovieLanguage
    {
        public int Id { get; set; }

        public int? MovieId { get; set; }

        public string? Iso_639_1 { get; set; }

        public string? LanguageName { get; set; }

        [NotMapped]
        public Movies Movie { get; set; }
    }
}
