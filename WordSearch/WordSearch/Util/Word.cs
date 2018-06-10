using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace WordSearch.Util
{
    public class Word
    {
        // word direction enum
        public enum WordDirection { LeftToRight, TopToBottom, RightToLeft, BottomToTop, TopLeftToBottomRight, TopRightToBottomLeft, BottomLeftToTopRight, BottomRightToTopLeft };
        // random word
        public string Text { get; set; }
        // start row position
        public int Row { get; set; }
        // start column position
        public int Column { get; set; }
        // word bearing
        public WordDirection Direction { get; set; }
        // reference to tile objects used in this word
        public List<Point> TilePositions { get; set; }

        public Word(string text)
        {
            Text = text;
            Row = 0;
            Column = 0;
        }       

        // test if word fits direction
        internal bool WordFits(WordDirection direction, int row, int column, int rows, int columns)
        {
            bool bOK = false;
            try
            {
                int wordLen = Text.Length;
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

        // add point array of tile positions
        internal bool CalculateTilePositions()
        {
            bool bOK = true;
            try
            {
                TilePositions = new List<Point>();
                int wordLen = Text.Length;
                int row = Row;
                int column = Column;
                TilePositions.Add(new Point(row, column));
                for (int n = 1; n < wordLen; n++)
                {
                    switch (Direction)
                    {
                        case WordDirection.LeftToRight:
                            row++;
                            break;
                        case WordDirection.TopToBottom:
                            column++;
                            break;
                        case WordDirection.RightToLeft:
                            row--;
                            break;
                        case WordDirection.BottomToTop:
                            column--;
                            break;
                        case WordDirection.TopLeftToBottomRight:
                            row++;
                            column++;
                            break;
                        case WordDirection.TopRightToBottomLeft:
                            row--;
                            column++;
                            break;
                        case WordDirection.BottomLeftToTopRight:
                            row++;
                            column--;
                            break;
                        case WordDirection.BottomRightToTopLeft:
                            row--;
                            column--;
                            break;
                        default:
                            bOK = false;
                            Debug.Assert(false);
                            break;
                    }
                    TilePositions.Add(new Point(row, column));
                }
            }
            catch (Exception ex)
            {
                var error = $"CalculateTilePositions exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

    }
}
