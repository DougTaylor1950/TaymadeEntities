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
        #region Private Fields

        private readonly DBContext.MusicEntitiesContext _context;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public MusicRepository(MusicEntitiesContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

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

        public List<ArtistAlbum>? GetAllArtistAlbums(int artistId)
        {
            var artistIds =
                _context.GroupMembers
                    .Where(gm => gm.ArtistId == artistId)
                    .Select(gm => gm.GroupId)
                .Union(
                _context.Artists
                    .Where(a => a.Id == artistId)
                    .Select(a => a.Id)
                    );

            var albumIds =
                _context.ArtistAlbums
                    .Where(aa => artistIds.Contains(aa.ArtistID)).ToList();
            //        .Select(aa => aa.AlbumID);

            //var albums =
            //    _context.Albums
            //        .Where(a => albumIds.Contains(a.Id));
            return albumIds.ToList();
        }

        public bool AddArtistVideo(ArtistVideo? artistVideo)
        {
            _context.ArtistVideos.Add(artistVideo);
            bool success = _context.SaveChanges() > 0;
            artistVideo = _context.ArtistVideos.Find(artistVideo.Id);
            return success; ;
        }

        public bool DeleteAlbumTrack(AlbumTrack albumTrack)
        {
            _context.AlbumTracks.Remove(albumTrack);
            return Save();
        }
        public bool DeleteArtist(Artist artist)
        {
            _context.Artists.Remove(artist);
            return Save();
        }

        public bool DeleteArtistAlbum(ArtistAlbum artistAlbum)
        {
            _context.ArtistAlbums.Remove(artistAlbum);
            return Save();
        }
        public bool DeleteArtistVideo(ArtistVideo artistVideo)
        {

            _context.ArtistVideos.Remove(artistVideo);
            return Save();
        }

        public bool DeleteGroupMember(GroupMembers groupMembers)
        {
            _context.GroupMembers.Remove(groupMembers);
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ActorRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public ObservableCollection<Album>? GetAlbumsByArtistId(int? value)
        {
            List<Album> tempList = new List<Album>();
            if (value == null)
                tempList = _context.Albums.Include(t => t.AlbumTracks)
                    .Include(a => a.ArtistAlbums)
                    .Include(v => v.ArtistVideos).ToList();
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
            List<ArtistAlbum>? tempList = null;
            tempList = _context.ArtistAlbums.Where(a => a.AlbumID == id).OrderBy(a => a.ArtistID).ToList();
            return tempList;
        }

        public List<ArtistAlbum>? GetArtistAlbumsByArtistId(int id)
        {
            List<ArtistAlbum>? tempList = null;
            tempList = _context.ArtistAlbums.Where(a => a.ArtistID == id).OrderBy(a => a.ArtistID).ToList();
            return tempList;
        }

        public Artist? GetArtistById(int artistID)
        {
            return _context.Artists.Find(artistID);
        }

        public Artist? GetArtistByMBID(string? id)
        {
            return _context.Artists.Where(a => a.MusicBrainzID == id).FirstOrDefault();
        }

        public List<Artist>? GetArtists()
        {
            return _context.Artists

                .Include(v => v.ArtistVideos)
                .Include(al => al.ArtistAlbums)
                .Include(a => a.GroupMembers)
                .Where(a => a.ArtistType == "Person")
                .OrderBy(a => a.Name)
                .ToList();
        }

        public List<Artist>? GetGroups()
        {
            return _context.Artists

                .Include(v => v.ArtistVideos)
                .Include(al => al.ArtistAlbums)
                .Include(a => a.GroupMembers)
                .Where(a => a.ArtistType == "Group")
                .OrderBy(a => a.Name)
                .ToList();
        }

        public List<ArtistVideo> GetArtistVideos()
        {
            
            return _context.ArtistVideos.ToList();
        }

        public List<Artist>? GetGroupsByName(string name)
        {
            List<Artist>? tempList = _context.Artists
               .Include(v => v.ArtistVideos)
               .Include(al => al.ArtistAlbums)
               .Include(a => a.GroupMembers)
               .Where(x => x.Name.ToLower().Contains(name.ToLower())
                        && x.ArtistType == "Group")
               .OrderBy(a => a.Name)
               .ToList();
            return tempList;
        }
        public List<Artist>? GetArtistsByName(string findText, string artistType = "Person")
        {
            return _context.Artists

               .Include(v => v.ArtistVideos)
               .Include(al => al.ArtistAlbums)
               .Include(a => a.GroupMembers)
               .Where(x => x.Name.ToLower().Contains(findText.ToLower())
                        && x.ArtistType == artistType)
               .OrderBy(a => a.Name)
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

        public bool UpdateArtist(Artist artist)
        {
            _context.Artists.Update(artist);
            return Save();
        }

        public bool UpdateArtistVideo(ArtistVideo artistVideo)
        {
            _context.ArtistVideos.Update(artistVideo);
            return Save();
        }

        public bool UpdateTrack(AlbumTrack albumTrack)
        {
            _context.AlbumTracks.Update(albumTrack);
            return Save();
        }

        #endregion Public Methods

        #region Protected Methods

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

        #endregion Protected Methods
    }
}