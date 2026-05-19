using Microsoft.EntityFrameworkCore;

namespace TaymadeEntities.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext()
        {
            
        }

        public DbSet<Movies> MovieDB {get; set;}

   

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           optionsBuilder.UseSqlServer("Data Source=TAYMADE-8\\sqlexpress;Initial Catalog=sandbox;Persist Security Info=True;User Id=sandbox;Password=sandbox");
       }
    }
}