using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Util
{
    // class used to encapsulate a tile as part of a word puzzle
    public class Tile
    {
        public bool Selected { get; set; }
        public char Letter { get; set; }

        public Tile()
        {
            Selected = false;
            Letter = GetRandomLetter();
        }

        // choose a random lower case letter
        private char GetRandomLetter()
        {
            Random rnd = new Random();
            int num = rnd.Next(0, 26); 
            char let = (char)('a' + num);
            return let;
        }
    }
}
