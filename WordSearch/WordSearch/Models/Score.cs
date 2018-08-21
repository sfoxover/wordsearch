using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WordSearch.Util;
using WordSearch.ViewModels;

namespace WordSearch.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Score : RealmObject
    {
        [JsonProperty]
        public int Id { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int Points { get; set; }
        [JsonProperty]
        public int Level { get; set; }
        [JsonProperty]
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

        public bool SaveRecord()
        {
            bool bOK = true;
            try
            {
                var realm = Realm.GetInstance();
                realm.Write(() =>
                {
                    realm.Add(this);
                });
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
                var realm = Realm.GetInstance();
                results = realm.All<Score>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadAllRecords exception {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}
