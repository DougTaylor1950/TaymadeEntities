using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using TaymadeEntities.Models;

namespace TaymadeEntities.DBContext
{
    public partial class MovieImageEntity : DbContext
    {
        public MovieImageEntity()
        {
            Database.SetCommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
        }

        public MovieImageEntity(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<MovieImage> MovieImage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=TAYMADE-8\\sqlexpress;Initial Catalog=sandbox;Persist Security Info=True;User Id=sandbox;Password=sandbox;Encrypt=false");
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieImage>().HasKey(mf => mf.Id);
        }
    }
}
