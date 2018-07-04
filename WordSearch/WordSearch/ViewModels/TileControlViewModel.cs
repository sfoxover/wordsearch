using WordSearch.Util;
using Xamarin.Forms;
using System;
using System.Windows.Input;

namespace WordSearch.ViewModels
{
	public class TileControlViewModel : BindableBase
    {
        static Random Random = new Random();

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
        private double _tileWidth;
        public double TileWidth
        {
            get { return _tileWidth; }
            set { SetProperty(ref _tileWidth, value); }
        }

        // Tile height request
        private double _tileHeight;
        public double TileHeight
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
        private bool _letterSelected;
        public bool LetterSelected
        {
            get { return _letterSelected; }
            set
            {
                SetProperty(ref _letterSelected, value);
                SetLetterColors();
            }
        }
        // is this tile part of cmpleted word
        public bool IsPartOfCompletedWord { get; set; }

        // frame click handler
        public ICommand TileClickCommand;
        private WordManager.TileClickedDelegate TileClickedCallBack { get; set; }

        public TileControlViewModel(WordManager.TileClickedDelegate callback)
        {
            IsPartOfCompletedWord = false;
            TileClickedCallBack = callback;            
            SetDefaultValues();
        }

        // set default values from constructor
        void SetDefaultValues()
        {
            TitleBorderColor = Color.Blue;
            LetterTextColor = Color.Black;
            LetterTextBkgColor = Color.White;
            TileWidth = 0;
            TileHeight = 0;
            TileRow = -1;
            TileColum = -1;
            LetterSelected = false;
            TileClickCommand = new Command(HandleTitleClick);
        }

        // choose a random lower case letter
        public static char GetRandomLetter()
        {
            int num = Random.Next(26);
            char let = (char)('a' + num);
            return let;
        }

        private void SetLetterColors()
        {
            if (LetterSelected)
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
        }

        // tile was clicked
        private void HandleTitleClick()
        {
            if (!IsPartOfCompletedWord)
            {
                LetterSelected = !LetterSelected;
                // publish event
                TileClickedCallBack?.Invoke(this);
            }
        }
    }
}
