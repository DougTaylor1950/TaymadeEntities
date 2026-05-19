using System;
using System.Collections.Generic;
using TaymadeEntities.Models;
using ReactiveUI;

namespace TaymadeEntities.ViewModels
{
	public class BookmarkViewModel : ReactiveObject
	{
        private Bookmark currentBookmark;

        public Bookmark CurrentBookmark
        { 
            get => currentBookmark; 
            set => this.RaiseAndSetIfChanged(ref  currentBookmark, value); 
        }
    }
}