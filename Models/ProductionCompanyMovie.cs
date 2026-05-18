using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class ProductionCompanyMovie
    {
        #region Public Properties

        public int CompanyId { get; set; }
        public int Id { get; set; }

        [NotMapped]
        public Movies? Movie { get; set; }

        public int MovieId { get; set; }
        [NotMapped]
        public ProductionCompany? ProductionCompany { get; set; }

        #endregion Public Properties
    }
}
