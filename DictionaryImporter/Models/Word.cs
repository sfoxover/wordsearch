using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using DictionaryImporter.Helpers;
using Realms;
using System.Diagnostics;

namespace DictionaryImporter.Models
{
    // Word model for database import tool
    [JsonObject(MemberSerialization.OptIn)]
    public class Word : RealmObject
    {
        // word direction enum
        public enum WordDirection { LeftToRight, TopToBottom, RightToLeft, BottomToTop, TopLeftToBottomRight, TopRightToBottomLeft, BottomLeftToTopRight, BottomRightToTopLeft };
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
        public WordDirection Direction { get; set; }
        // reference to tile objects used in this word
        public List<Point> TilePositions { get; set; }
        // flag when whole word is completed
        [JsonProperty]
        public bool IsWordCompleted { get; set; }
        // Hide some words in hard level
        [JsonProperty]
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
            WordDifficulty = Defines.GameDifficulty.easy;
        }

        public bool SaveRecords(List<Word> data)
        {
            bool bOK = true;
            try
            {
                var realm = Realm.GetInstance();
                realm.Write(() =>
                {
                    realm.Add(data);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}
