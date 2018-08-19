using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WordSearch.Util;
using WordSearch.ViewModels;

namespace WordSearch.Models
{
    public class Score : RealmObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
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
    }
}
