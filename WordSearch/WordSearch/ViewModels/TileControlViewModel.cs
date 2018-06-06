using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using WordSearch.Util;
using Xamarin.Forms;
using WordSearch.Controls;

namespace WordSearch.ViewModels
{
	public class TileControlViewModel : BindableBase 
    {
        // event aggregator
        protected IEventAggregator EventAggregator { get; private set; }

        // letter displayed in control
        private string _letter;
        public string Letter
        {
            get { return _letter; }
            set { SetProperty(ref _letter, value); }
        }

        // store row, column that this tile belongs to
        public int TileRow { get; set; }
        public int TileColum { get; set; }

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

        // Tile text color
        private Color _letterTextColor;
        public Color LetterTextColor
        {
            get { return _letterTextColor; }
            set { SetProperty(ref _letterTextColor, value); }
        }

        // Tile border color
        private Color _letterTextBkgColor;
        public Color LetterTextBkgColor
        {
            get { return _letterTextBkgColor; }
            set { SetProperty(ref _letterTextBkgColor, value); }
        }

        // is tile clicked
        private bool _letterClicked;
        public bool LetterSelected
        {
            get { return _letterClicked; }
            set { SetProperty(ref _letterClicked, value); }
        }

        // frame click handler
        public DelegateCommand TileClickCommand { get; set; }

        public TileControlViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SetDefaultValues();
        }

        // set default values from constructor
        void SetDefaultValues()
        {
            TitleBorderColor = Color.Blue;
            LetterTextColor = Color.Black;
            LetterTextBkgColor = Color.White;
            TileWidth = Defines.TILE_WIDTH - 1;
            TileHeight = Defines.TILE_HEIGHT - 1;
            TileRow = -1;
            TileColum = -1;
            LetterSelected = false;
            TileClickCommand = new DelegateCommand(HandleTitleClick);
        }

        // tile was clicked
        private void HandleTitleClick()
        {
            if (!LetterSelected)
            {
                TitleBorderColor = Color.Red;
                LetterTextColor = Color.Black;
                LetterTextBkgColor = Color.Yellow;
            }
            else
            {
                TitleBorderColor = Color.Blue;
                LetterTextColor = Color.Black;
                LetterTextBkgColor = Color.White;
            }
            LetterSelected = !LetterSelected;
            // publish event
            EventAggregator.GetEvent<TileSelectionEvent<TileControlViewModel>>().Publish(this);
        }
    }
}
