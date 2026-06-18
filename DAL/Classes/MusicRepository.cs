using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Classes
{
    public class MusicRepository : IMusicRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.MusicEntitiesContext _context;

        public MusicRepository(MusicEntitiesContext context)
        {
            _context = context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ActorRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool AddAlbum(Album album)
        {
            _context.Albums.Add(album);
            return Save();
        }
        public bool AddAlbumTrack(AlbumTrack albumTrack)
        {
            _context.AlbumTracks.Add(albumTrack);
            return Save();
        }

        public bool AddArtist(Artist artist)
        {
            _context.Artists.Add(artist);
            return Save();
        }
        public bool AddArtistAlbum(ArtistAlbum artistAlbum)
        {
            _context.ArtistAlbums.Add(artistAlbum);
            return Save();
        }

        public void Delete(int id)
        {
            //Actor actorToDelete = _context.Actors.Find(id);
            //if (actorToDelete != null)
            //{
            //    _context.Actors.Remove(actorToDelete);
            //    Save();
            //}
        }

        public ObservableCollection<Album>? GetAlbumsByArtistId(int? value)
        {
            List<Album> tempList = new List<Album>();
            if (value == null)
                tempList = _context.Albums.Include(t=>t.AlbumTracks)
                    .Include(a=> a.ArtistAlbums)
                    .Include(v=>v.ArtistVideos).ToList();
            else
            {
                //tempList = _context.Albums.Where(a=> a.A)
            }

            return new ObservableCollection<Album>(tempList);
        }

        public Album? GetAlbumsByName(string findText)
        {
            return _context.Albums.Where(a => a.AlbumName.ToLower().Contains(findText.ToLower())).FirstOrDefault();
        }

        public List<AlbumTrack>? GetAlbumTracksByAlbumId(int id)
        {
            List<AlbumTrack>? tempList = null;
            tempList = _context.AlbumTracks.Where(a => a.AlbumID == id).OrderBy(a => a.TrackNo).ToList();
            return tempList;
        }

        public List<ArtistAlbum>? GetArtistAlbumsByAlbumId(int id)
        {
            List<ArtistAlbum> tempList = null;
            tempList = _context.ArtistAlbums.Where(a => a.AlbumID == id).OrderBy(a => a.ArtistID).ToList();
            return tempList;
        }

        public Artist? GetArtistById(int artistID)
        {
            return _context.Artists.Find(artistID);
        }

        public List<Artist>? GetArtists()
        {
            return _context.Artists

                .Include(v => v.ArtistVideos)
                .Include(al => al.ArtistAlbums)
                .ToList();
        }

        public List<GroupMembers>? GetGroupMembersByArtistId(int id)
        {
            return _context.GroupMembers.Where(g => g.ArtistId == id).OrderBy(a => a.ArtistGroup).ToList();
        }

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        public bool UpdateAlbum(Album album)
        {
            _context.Albums.Update(album);
            return Save();
        }
        public bool UpdateTrack(AlbumTrack albumTrack)
        {
            _context.AlbumTracks.Update(albumTrack);
            return Save();
        }
    }
}
