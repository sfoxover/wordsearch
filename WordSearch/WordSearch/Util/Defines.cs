using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch
{
    public class Defines
    {
        // maximum attempts to find a random value that fits constraints 
        public const int MAX_RANDOM_TRIES = 100;
        // game difficulty enum
        public enum GameDifficulty { easy, medium, hard };
        // word direction enum
        public enum WordDirection { LeftToRight, TopToBottom, RightToLeft, BottomToTop, TopLeftToBottomRight, TopRightToBottomLeft, BottomLeftToTopRight, BottomRightToTopLeft };
    }
}
