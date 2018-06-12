using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace WordSearch.Models
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
            TilePositions = null;
            Text = text;
            Row = 0;
            Column = 0;
        }       

        // test if word fits direction
        internal bool WordFits(int rows, int columns)
        {
            bool bOK = false;
            try
            {
                int wordLen = Text.Length;
                switch (Direction)
                {
                    case WordDirection.LeftToRight:
                        bOK = (Row + wordLen) <= rows;
                        break;
                    case WordDirection.TopToBottom:
                        bOK = (Column + wordLen) <= columns;
                        break;
                    case WordDirection.RightToLeft:
                        bOK = (Row - wordLen) >= 0;
                        break;
                    case WordDirection.BottomToTop:
                        bOK = (Column - wordLen) >= 0;
                        break;
                    case WordDirection.TopLeftToBottomRight:
                        bOK = (Row + wordLen) <= rows && (Column + wordLen) <= columns;
                        break;
                    case WordDirection.TopRightToBottomLeft:
                        bOK = (Row - wordLen) >= 0 && (Column + wordLen) <= columns;
                        break;
                    case WordDirection.BottomLeftToTopRight:
                        bOK = (Row + wordLen) <= rows && (Column - wordLen) >= 0;
                        break;
                    case WordDirection.BottomRightToTopLeft:
                        bOK = (Row - wordLen) >= 0 && (Column - wordLen) >= 0;
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
                    GetNextPosition(Direction, ref row, ref column);
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

        // get next position based on direction
        internal void GetNextPosition(WordDirection direction, ref int row, ref int column)
        {
            switch (direction)
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
                    Debug.Assert(false);
                    break;
            }
        }

        // test all values in text for collision
        internal bool HasCollision(Word word)
        {
            try
            {
                int row = Row;
                int column = Column;
                for (int n=0; n<Text.Length; n++)
                {
                    char ch1 = Text[n];
                    int row2 = word.Row;
                    int column2 = word.Column;
                    for (int n2=0; n2<word.Text.Length; n2++)
                    {
                        char ch2 = word.Text[n2];
                        // collision found
                        if (row == row2 && column == column2 && ch1 != ch2)
                        {
                            Debug.WriteLine($"Collision found for words {Text} and {word.Text} at position {row} x {column}");
                            return true;
                        }
                        GetNextPosition(word.Direction, ref row2, ref column2);
                    }
                    GetNextPosition(Direction, ref row, ref column);
                }
            }
            catch (Exception ex)
            {
                var error = $"HasCollision exception, {ex.Message}";
                Debug.WriteLine(error);
                return true;
            }
            return false;
        }
    }
}
