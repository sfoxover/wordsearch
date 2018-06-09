using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Util
{
    // class used to encapsulate a tile as part of a word puzzle
    public class Tile
    {
        static Random RandomGenerator = new Random();
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
            int num = RandomGenerator.Next(0, 26); 
            char let = (char)('a' + num);
            return let;
        }
    }
}
