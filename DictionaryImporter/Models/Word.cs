using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using DictionaryImporter.Helpers;
using System.Diagnostics;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace DictionaryImporter.Models
{
    // Word model for database import tool
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

        public Word()
        {
        }

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

        public static async Task<bool> SaveRecords(List<Word> words)
        {
            bool bOK = true;
            try
            {
                using (var db = new DbContextWords())
                {
                    await db.Words.AddRangeAsync(words);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // Load all records with a difficulty filter.
        public static bool LoadRecords(Defines.GameDifficulty difficulty, out List<Word> results)
        {
            results = null;
            bool bOK = true;
            try
            {
                using (var db = new DbContextWords())
                {
                    db.Database.EnsureCreated();
                    results = (from item in db.Words
                              where item.WordDifficulty >= difficulty
                              orderby item ascending
                              select item).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}
