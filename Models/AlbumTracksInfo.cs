//using MusicBrainzSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaymadeEntities.Models
{
    public class AlbumTracksInfo
    {
        public Album? Album { get; set; }

        //public MBMedia? Media { get; set; }

       // public DCAlbumDetails DCAlbumDetails { get; set; }

    }

    public class ArtistGroupInfo
    {
        public Artist? Artist { get; set; }

       // public List<MBArtistRelationship>? BandMembers { get; set; }

    }
}