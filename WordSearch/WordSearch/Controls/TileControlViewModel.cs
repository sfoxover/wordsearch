using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace WordSearch.Controls
{
	public class TileControlViewModel : BindableBase, INavigationAware
    {
        protected INavigationService NavigationService { get; private set; }

        // letter displayed in control
        private string _letter;
        public string Letter
        {
            get { return _letter; }
            set { SetProperty(ref _letter, value); }
        }

        // Tile Width request
        private int _tileWidth;
        public int TileWidth
        {
            get { return _tileWidth; }
            set { SetProperty(ref _tileWidth, value); }
        }

        // Tile height request
        private int _tileHeight;
        public int TileHeight
        {
            get { return _tileHeight; }
            set { SetProperty(ref _tileHeight, value); }
        }

        // Tile border color
        private Color _titleBorderColor;
        public Color TitleBorderColor
        {
            get { return _titleBorderColor; }
            set { SetProperty(ref _titleBorderColor, value); }
        }

        // frame click handler
        public DelegateCommand TileClickCommand { get; set; }

        public TileControlViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            SetDefaultValues();
        }

        public TileControlViewModel(string letter)
        {
            Letter = letter;
            SetDefaultValues();
        }

        void SetDefaultValues()
        {
            TitleBorderColor = Color.Blue;
            TileWidth = Defines.TILE_WIDTH - 1;
            TileHeight = Defines.TILE_HEIGHT - 1;
            TileClickCommand = new DelegateCommand(HandleTitleClick);
        }

        private void HandleTitleClick()
        {
            TitleBorderColor = Color.Red;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
