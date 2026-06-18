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

        public bool Save()
        {
            return musicRepository.Save();
        }

        #endregion Public Methods

        //public void Delete(int id)
        //{
        //    templateRepository.DeleteMovie(id);
        //}

        #region Internal Methods

        internal List<AlbumTrack>? GetAlbumTracksByAlbumId(int id)
        {
            return musicRepository.GetAlbumTracksByAlbumId(id);
        }

        internal List<ArtistAlbum>? GetArtistAlbumsByAlbumId(int id)
        {
            return musicRepository.GetArtistAlbumsByAlbumId(id);
        }

        internal Artist? GetArtistById(int artistID)
        {
            return musicRepository.GetArtistById(artistID);
        }

        internal List<GroupMembers>? GetGroupMembersByArtistId(int id)
        {
            return musicRepository.GetGroupMembersByArtistId(id);
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

        public Album? GetAlbumsByName(string findText)
        {
            return musicRepository.GetAlbumsByName(findText);
        }

        public ObservableCollection<Artist>? GetArtists()
        {
            List<Artist>? temp = musicRepository.GetArtists();
            if (temp != null)
                return new ObservableCollection<Artist>(temp);
            else return new ObservableCollection<Artist>();
        }

        internal bool AddArtistAlbum(ArtistAlbum artistAlbum)
        {
           return  musicRepository.AddArtistAlbum(artistAlbum);
        }

        internal bool AddArtist(Artist artist)
        {
            return musicRepository.AddArtist(artist);
        }

        internal bool AddTrack(AlbumTrack albumTrack)
        {
            return musicRepository.AddAlbumTrack(albumTrack);
        }

        internal bool AddAlbum(Album album)
        {
            return musicRepository.AddAlbum(album);
        }

        #endregion Protected Methods
    }
}