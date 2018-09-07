using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Helpers
{
    public class Defines
    {
        public static string TAG = "Super Word Search";
        // maximum attempts to find a random value that fits constraints 
        public const int MAX_RANDOM_TRIES = 100;
        // game difficulty enum
        public enum GameDifficulty { easy, medium, hard };
        // word direction enum
        public enum WordDirection { LeftToRight, TopToBottom, RightToLeft, BottomToTop, TopLeftToBottomRight, TopRightToBottomLeft, BottomLeftToTopRight, BottomRightToTopLeft };
        // Header html page height
        public const int HEADER_HTML_HEIGHT = 100;
        // Minus points for hard level penalty
        public const int PENALTY_POINTS = 25;
    }
}
