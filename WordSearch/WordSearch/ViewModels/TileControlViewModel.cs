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
        public int TileColumn { get; set; }

        // is tile clicked
        private bool _letterSelected;
        public bool LetterSelected
        {
            get { return _letterSelected; }
            set
            {
                SetProperty(ref _letterSelected, value);
            }
        }
        // is this tile part of cmpleted word
        public bool IsPartOfCompletedWord { get; set; }

        public TileControlViewModel()
        {
            IsPartOfCompletedWord = false;
            SetDefaultValues();
        }

        // set default values from constructor
        void SetDefaultValues()
        {
            TileRow = -1;
            TileColumn = -1;
            LetterSelected = false;
        }

        // choose a random lower case letter
        public static char GetRandomLetter()
        {
            int num = Random.Next(26);
            char let = (char)('a' + num);
            return let;
        }
    }
}
