//-----------------------------------------------------------------------
// <copyright file="SeasonPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>06/05/2022 13:44:43 06/05/2022 13:44:43 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Season" />.
    /// </summary>
    public partial class Season
    {
        #region Methods

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return SeasonNo.ToString() + " - " + Name + "." + Year.ToString();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        internal void Delete()
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            {
                Season? tempSeason = _context.Seasons.Find(this.Id);
                if (tempSeason != null)
                {
                    _context.Seasons.Remove(this);
                    _context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        public void Insert()
        {
            Save();
        }

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            using DBContext.SandboxEntities _context = new DBContext.SandboxEntities();
            {
                Season? tempSeason = _context.Seasons.Find(this.Id);
                if (tempSeason != null)
                {
                    MoveToTempSeason(tempSeason);
                }
                else
                {
                    _context.Seasons.Add(this);
                }
                _context.SaveChanges();
            }
        }

        public void CreateFromSeason(Season? season)
        {
            if (season == null) return;
            this.Id = season.Id;
            this.Year = season.Year;
            this.Series = season.Series;
            this.SeasonNo = season.SeasonNo;
            this.Path = season.Path;
            this.TMDBID = season.TMDBID;
            this.ShowId = season.ShowId;
            this.Name = season.Name;
            this.Description = season.Description;
        }

        private void MoveToTempSeason(Season tempSeason)
        {
            tempSeason.Year = this.Year;
            tempSeason.Series = this.Series;
            tempSeason.SeasonNo = this.SeasonNo;
            tempSeason.Path = this.Path;
            tempSeason.TMDBID = this.TMDBID;
            tempSeason.ShowId = this.ShowId;
            tempSeason.Name = this.Name;
            tempSeason.Description = this.Description;

        }

        #endregion
    }
}
