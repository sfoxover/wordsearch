using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordSearch.Util;
using WordSearch.ViewModels;

namespace WordSearch.Models
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Score 
    {
        [JsonProperty]
        [Key]
        public int Id { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int Points { get; set; }
        [JsonProperty]
        public int Level { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(DateFormatConverter), "dd MMMM yyyy h:mm tt")]
        public DateTimeOffset Date { get; set; }

        public Score()
        {
        }

        public Score(string name, WordManager manager, WordSearchPageViewModel viewModel)
        {
            Name = name;
            Points = viewModel.GameScore;
            Level = (int)manager.DifficultyLevel;
            Date = DateTimeOffset.Now;
        }

        public async Task<bool> SaveRecord()
        {
            bool bOK = true;
            try
            {
                using (var db = new DbContextScores())
                {
                    db.Scores.Add(this);
                    await db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"SaveRecord exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // load all score records
        public static bool LoadAllRecords(out List<Score> results)
        {
            bool bOK = true;
            results = new List<Score>();
            try
            {
                using (var db = new DbContextScores())
                {
                    db.Database.EnsureCreated();
                    results = db.Scores.OrderByDescending(item => item.Points).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadAllRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // delete all scores
        internal static async Task<bool> DeleteAllRecords()
        {
            bool bOK = true;
            try
            {
                using (var db = new DbContextScores())
                {
                    foreach (var score in db.Scores)
                        db.Scores.Remove(score);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAllRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // get new score rank
        public static bool GetScoreRank(int score, out int ranking)
        {
            bool bOK = true;
            ranking = 1;
            try
            {
                using (var db = new DbContextScores())
                {
                    db.Database.EnsureCreated();
                    var count = db.Scores.Where(item => item.Points > score).Count();
                    ranking = count + 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetScoreRank exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}
