using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AvalonMVVM.Models
{
    public class MusicEntitiesContext : DbContext
    {
        public MusicEntitiesContext()
        {
            Database.SetCommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=TAYMADE-8\\sqlexpress;Initial Catalog=sandbox;Persist Security Info=True;User Id=sandbox;Password=sandbox;Encrypt=false");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>().HasKey(m => m.Id);
            modelBuilder.Entity<Album>().HasMany(a => a.AlbumTracks).WithOne(t => t.Album).HasForeignKey(t => t.AlbumID);
            modelBuilder.Entity<Album>().HasMany(a => a.ArtistAlbums).WithOne(a => a.Album).HasForeignKey(a => a.AlbumID);


            modelBuilder.Entity<Artist>().HasKey(a => a.Id);
            modelBuilder.Entity<Artist>().HasMany(a => a.ArtistAlbums).WithOne(a => a.Artist).HasForeignKey(a => a.ArtistID);
            modelBuilder.Entity<Artist>().HasMany(a => a.GroupMembers).WithOne(g => g.ArtistGroup).HasForeignKey(a => a.GroupId);
            modelBuilder.Entity<Artist>().HasMany(a => a.ArtistVideos).WithOne(g => g.Artist).HasForeignKey(a => a.ArtistID);
        }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<Artist> Artists { get; set; }

        public virtual DbSet<AlbumTrack> AlbumTracks { get; set; }

        public virtual DbSet<ArtistAlbum> ArtistAlbums { get; set; }

        public virtual DbSet<ArtistVideo> ArtistVideos { get; set; }

        public virtual DbSet<GroupMembers> GroupMembers { get; set; }


    }
}
