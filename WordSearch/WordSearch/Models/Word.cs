using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace WordSearch.Models
{
    // Word model for game tiles
    [JsonObject(MemberSerialization.OptIn)]
    public class Word
    {
        [Key]
        public int Id { get; set; }
        // random word
        [JsonProperty]
        public string Text { get; set; }
        // start row position
        [JsonProperty]
        public int Row { get; set; }
        // start column position
        [JsonProperty]
        public int Column { get; set; }
        // word bearing
        [NotMapped]
        public Defines.WordDirection Direction { get; set; }
        // reference to tile objects used in this word
        [NotMapped]
        public List<Point> TilePositions { get; set; }
        // flag when whole word is completed
        [JsonProperty]
        [NotMapped]
        public bool IsWordCompleted { get; set; }
        // Hide some words in hard level
        [JsonProperty]
        [NotMapped]
        public bool IsWordHidden { get; set; }
        // Word difficulty level
        [JsonProperty]
        public Defines.GameDifficulty WordDifficulty { get; set; }

        public Word(string text)
        {
            TilePositions = null;
            Text = text;
            Row = 0;
            Column = 0;
            IsWordCompleted = false;
            IsWordHidden = false;
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
                    case Defines.WordDirection.LeftToRight:
                        bOK = (Row + wordLen) <= rows;
                        break;
                    case Defines.WordDirection.TopToBottom:
                        bOK = (Column + wordLen) <= columns;
                        break;
                    case Defines.WordDirection.RightToLeft:
                        bOK = (Row - wordLen) >= 0;
                        break;
                    case Defines.WordDirection.BottomToTop:
                        bOK = (Column - wordLen) >= 0;
                        break;
                    case Defines.WordDirection.TopLeftToBottomRight:
                        bOK = (Row + wordLen) <= rows && (Column + wordLen) <= columns;
                        break;
                    case Defines.WordDirection.TopRightToBottomLeft:
                        bOK = (Row - wordLen) >= 0 && (Column + wordLen) <= columns;
                        break;
                    case Defines.WordDirection.BottomLeftToTopRight:
                        bOK = (Row + wordLen) <= rows && (Column - wordLen) >= 0;
                        break;
                    case Defines.WordDirection.BottomRightToTopLeft:
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
        internal void GetNextPosition(Defines.WordDirection direction, ref int row, ref int column)
        {
            switch (direction)
            {
                case Defines.WordDirection.LeftToRight:
                    row++;
                    break;
                case Defines.WordDirection.TopToBottom:
                    column++;
                    break;
                case Defines.WordDirection.RightToLeft:
                    row--;
                    break;
                case Defines.WordDirection.BottomToTop:
                    column--;
                    break;
                case Defines.WordDirection.TopLeftToBottomRight:
                    row++;
                    column++;
                    break;
                case Defines.WordDirection.TopRightToBottomLeft:
                    row--;
                    column++;
                    break;
                case Defines.WordDirection.BottomLeftToTopRight:
                    row++;
                    column--;
                    break;
                case Defines.WordDirection.BottomRightToTopLeft:
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
