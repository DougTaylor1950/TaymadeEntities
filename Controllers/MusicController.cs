using System.Collections.ObjectModel;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class MusicController : IDisposable
    {

        #region Private Fields

        private bool disposedValue;
        private IMusicRepository musicRepository;

        #endregion Private Fields

        #region Public Constructors

        public MusicController()
        {
            this.musicRepository = new MusicRepository(new DBContext.MusicEntitiesContext());
        }

        public MusicController(IMusicRepository repository)
        {
            this.musicRepository = repository;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public ObservableCollection<Album>? GetAlbumsByArtistId(int? value)
        {
            return musicRepository.GetAlbumsByArtistId(value);
        }

        public Album? GetAlbumsByName(string findText)
        {
            return musicRepository.GetAlbumsByName(findText);
        }

        public List<ArtistAlbum>? GetAllArtistAlbums(int artistId)
        {
            return musicRepository.GetAllArtistAlbums(artistId);
        }

        public ObservableCollection<Artist>? GetArtists()
        {
            List<Artist>? temp = musicRepository.GetArtists();
            if (temp != null)
                return new ObservableCollection<Artist>(temp);
            else return new ObservableCollection<Artist>();
        }

        public Artist? GetArtistsByMBID(string? id)
        {
            return musicRepository.GetArtistByMBID(id);
        }

        public List<Artist>? GetArtistsByName(string findText, string artistType = "Person")
        {
            return musicRepository.GetArtistsByName(findText, artistType);
        }


        public IEnumerable<ArtistVideo> GetArtistVideos()
        {
            return musicRepository.GetArtistVideos();
        }

        public ObservableCollection<Artist>? GetGroups()
        {
            List<Artist>? temp = musicRepository.GetGroups();
            if (temp != null)
                return new ObservableCollection<Artist>(temp);
            else return new ObservableCollection<Artist>();
        }
        public List<Artist>? GetGroupsByName(string name)
        {
            return musicRepository.GetGroupsByName(name);
        }

        public bool Save()
        {
            return musicRepository.Save();
        }

        #endregion Public Methods

        #region Internal Methods

        //public void Delete(int id)
        //{
        //    templateRepository.DeleteMovie(id);
        //}
        internal bool AddAlbum(Album album)
        {
            return musicRepository.AddAlbum(album);
        }

        internal bool AddArtist(Artist artist)
        {
            return musicRepository.AddArtist(artist);
        }

        internal bool AddArtistAlbum(ArtistAlbum artistAlbum)
        {
            return musicRepository.AddArtistAlbum(artistAlbum);
        }

        internal bool AddArtistVideo(ArtistVideo artistVideo)
        {
            return musicRepository.AddArtistVideo(artistVideo);
        }

        internal bool AddTrack(AlbumTrack albumTrack)
        {
            return musicRepository.AddAlbumTrack(albumTrack);
        }

        internal bool DeleteArtist(Artist artist)
        {
            return musicRepository.DeleteArtist(artist);
        }

        internal bool DeleteArtistVideo(ArtistVideo artistVideo)
        {
            return musicRepository.DeleteArtistVideo(artistVideo);
        }

        internal bool DeleteGroupMember(GroupMembers groupMembers)
        {
            return musicRepository.DeleteGroupMember(groupMembers);
        }

        internal List<AlbumTrack>? GetAlbumTracksByAlbumId(int id)
        {
            return musicRepository.GetAlbumTracksByAlbumId(id);
        }

        internal List<ArtistAlbum>? GetArtistAlbumsByAlbumId(int id)
        {
            return musicRepository.GetArtistAlbumsByAlbumId(id);
        }
        internal List<ArtistAlbum>? GetArtistAlbumsByArtistId(int id)
        {
            return musicRepository.GetArtistAlbumsByArtistId(id);
        }

        internal Artist? GetArtistById(int artistID)
        {
            return musicRepository.GetArtistById(artistID);
        }

        internal List<GroupMembers>? GetGroupMembersByArtistId(int id)
        {
            return musicRepository.GetGroupMembersByArtistId(id);
        }

        internal bool UpdateAlbum(Album album)
        {
            return musicRepository.UpdateAlbum(album);
        }

        internal bool UpdateArtist(Artist artist)
        {
            return musicRepository.UpdateArtist(artist);
        }

        internal bool UpdateArtistVideo(ArtistVideo artistVideo)
        {
            return musicRepository.UpdateArtistVideo(artistVideo);
        }

        internal bool UpdateTrack(AlbumTrack albumTrack)
        {
            return musicRepository.UpdateTrack(albumTrack);
        }

        #endregion Internal Methods

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


        public bool DeleteArtistAlbum(ArtistAlbum artistAlbum)
        {
            return musicRepository.DeleteArtistAlbum(artistAlbum);
        }

        internal bool DeleteAlbumTrack(AlbumTrack albumTrack)
        {
            return musicRepository.DeleteAlbumTrack(albumTrack);
        }

        #endregion Protected Methods

    }
}