using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;

namespace TaymadeEntities
{
    public class BookmarkEventArgs : System.EventArgs
    {
        public BookmarkEventArgs()
        {
        }

        public BookmarkEventArgs(Bookmark bookmark)
        {
            Bookmark = bookmark;
        }

        public Bookmark Bookmark { get; set; }

    }

    public class ViewingCompletedEventArgs : System.EventArgs
    {
        public ViewingCompletedEventArgs()
        {
        }

        public ViewingCompletedEventArgs(Bookmark bookmark)
        {
            Bookmark = bookmark;
        }

        public Bookmark? Bookmark { get; set; }

    }
}
