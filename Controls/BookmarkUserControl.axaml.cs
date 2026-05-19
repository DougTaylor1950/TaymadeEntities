using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaymadeEntities.ViewModels;

namespace TaymadeEntities.Controls
{
    public partial class BookmarkUserControl : UserControl
    {
        #region Public Constructors

        public BookmarkUserControl()
        {
            InitializeComponent();
        }

        public BookmarkUserControl(bool editing)
        {
            InitializeComponent();

            this.DelBookmark.IsVisible = editing;
        }

        #endregion Public Constructors

        //private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
    }
}