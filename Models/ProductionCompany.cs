using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class ProductionCompany:ReactiveObject
    {
        #region Private Fields

        private string? name;

        #endregion Private Fields

        #region Public Properties

       
        public int Id { get; set; }

        public int TMDBID { get; set; }

        public string? CompanyName { get => name; set => this.RaiseAndSetIfChanged( ref name, value); }

        //public int MoviesId { get; set; }
   
        #endregion Public Properties
    }
}
