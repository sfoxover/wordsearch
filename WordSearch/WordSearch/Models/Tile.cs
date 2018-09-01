using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Models
{
    public class Tile
    {
        static Random Random = new Random();
        // Character
        public string Letter { get; set; }
        // store row, column that this tile belongs to
        public int TileRow { get; set; }
        public int TileColumn { get; set; }

        // is tile clicked
        public bool LetterSelected { get; set; }
        // is this tile part of cmpleted word
        public bool IsPartOfCompletedWord { get; set; }

        public Tile()
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
