using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WordSearch.Util
{
    public class Word
    {
        // word direction enum
        public enum WordDirection { LeftToRight, TopToBottom, RightToLeft, BottomToTop, TopLeftToBottomRight, TopRightToBottomLeft, BottomLeftToTopRight, BottomRightToTopLeft };
        // random word
        public string Text { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public WordDirection Direction { get; set; }

        public Word(string text)
        {
            Text = text;
            Row = 0;
            Column = 0;
        }       

        // test if word fits direction
        internal bool WordFits(WordDirection direction, int wordLen, int row, int column, int rows, int columns)
        {
            bool bOK = false;
            try
            {
                switch (direction)
                {
                    case WordDirection.LeftToRight:
                        bOK = (row + wordLen) <= rows;
                        break;
                    case WordDirection.TopToBottom:
                        bOK = (column + wordLen) <= columns;
                        break;
                    case WordDirection.RightToLeft:
                        bOK = (row - wordLen) >= 0;
                        break;
                    case WordDirection.BottomToTop:
                        bOK = (column - wordLen) >= 0;
                        break;
                    case WordDirection.TopLeftToBottomRight:
                        bOK = (row + wordLen) <= rows && (column + wordLen) <= columns;
                        break;
                    case WordDirection.TopRightToBottomLeft:
                        bOK = (row - wordLen) >= 0 && (column + wordLen) <= columns;
                        break;
                    case WordDirection.BottomLeftToTopRight:
                        bOK = (row + wordLen) <= rows && (column - wordLen) >= 0;
                        break;
                    case WordDirection.BottomRightToTopLeft:
                        bOK = (row - wordLen) >= 0 && (column - wordLen) >= 0;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var error = $"WordFits exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

    }
}
