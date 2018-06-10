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
        int Row { get; set; }
        int Column { get; set; }
        WordDirection Direction { get; set; }

        public Word(string text, int row, int column)
        {
            Text = text;
            Row = row;
            Column = column;
        }

        bool SelectDirection()
        {
            bool bOK = false;
            try
            {
                Random rand = new Random();
                int safeNum = selectedWords.Count * 100;
                int item = 0;
                int tries = 0;
                int rows = tiles.GetLength(0);
                int columns = tiles.GetLength(1);
                while (item < selectedWords.Count && tries < safeNum)
                {
                    string word = selectedWords[item];
                    int wordLen = word.Length;
                    int row = rand.Next(rows);
                    int column = rand.Next(columns);
                    var directions = new List<WordDirection>();
                    foreach (WordDirection direction in Enum.GetValues(typeof(WordDirection)))
                    {
                        if (WordFits(direction, word.Length, row, column, rows, columns))
                        {
                            directions.Add(direction);
                        }
                    }
                    if (directions.Count > 0)
                    {
                        WordDirection directionSelected = directions[rand.Next(directions.Count)];
                        item++;
                    }
                    tries++;
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

        // test if word fits direction
        private bool WordFits(WordDirection direction, int wordLen, int row, int column, int rows, int columns)
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
