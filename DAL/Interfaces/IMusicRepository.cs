using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IMusicRepository : IDisposable
    {
        bool AddAlbum(Album album);
        bool AddAlbumTrack(AlbumTrack albumTrack);
        bool AddArtist(Artist artist);
        bool AddArtistAlbum(ArtistAlbum artistAlbum);

        #region Public Methods

        void Delete(int id);
        ObservableCollection<Album>? GetAlbumsByArtistId(int? value);
        Album? GetAlbumsByName(string findText);
        List<AlbumTrack>? GetAlbumTracksByAlbumId(int id);
        List<ArtistAlbum>? GetArtistAlbumsByAlbumId(int id);
        Artist? GetArtistById(int artistID);
        List<Artist>? GetArtists();
        List<GroupMembers>? GetGroupMembersByArtistId(int id);
        bool Save();
        bool UpdateAlbum(Album album);
        bool UpdateTrack(AlbumTrack albumTrack);

        #endregion Public Methods

    }
}
