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
        #region Public Methods

        bool AddAlbum(Album album);
        bool AddAlbumTrack(AlbumTrack albumTrack);
        bool AddArtist(Artist artist);
        bool AddArtistAlbum(ArtistAlbum artistAlbum);
        bool AddArtistVideo(ArtistVideo artistVideo);

        #endregion Public Methods

        #region Public Methods

        void Delete(int id);
        bool DeleteAlbumTrack(AlbumTrack albumTrack);
        bool DeleteArtist(Artist artist);
        bool DeleteArtistAlbum(ArtistAlbum artistAlbum);
        bool DeleteArtistVideo(ArtistVideo artistVideo);
        bool DeleteGroupMember(GroupMembers groupMembers);

        ObservableCollection<Album>? GetAlbumsByArtistId(int? value);

        Album? GetAlbumsByName(string findText);

        List<AlbumTrack>? GetAlbumTracksByAlbumId(int id);

        List<ArtistAlbum>? GetAllArtistAlbums(int artistId);
        List<ArtistAlbum>? GetArtistAlbumsByAlbumId(int id);
        List<ArtistAlbum>? GetArtistAlbumsByArtistId(int id);
        Artist? GetArtistById(int artistID);
        Artist? GetArtistByMBID(string? id);

        List<Artist>? GetArtists();
        List<Artist>? GetArtistsByName(string findText, string artistType = "Person");
        List<ArtistVideo> GetArtistVideos();
        List<GroupMembers>? GetGroupMembersByArtistId(int id);

        List<Artist>? GetGroups();
        List<Artist>? GetGroupsByName(string name);
        bool Save();
        bool UpdateAlbum(Album album);
        bool UpdateArtist(Artist artist);
        bool UpdateArtistVideo(ArtistVideo artistVideo);
        bool UpdateTrack(AlbumTrack albumTrack);
        #endregion Public Methods

    }
}
