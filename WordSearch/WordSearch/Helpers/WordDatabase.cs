using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WordSearch.Models;

namespace WordSearch.Helpers
{
    public class WordDatabase
    {
        static Random Random = new Random();
        List<Word> WordList { get; set; }
        public WordDatabase()
        {
            WordList = null;
        }

        // Load words database into memory
        public bool LoadWordsDB(Defines.GameDifficulty difficulty)
        {
            bool bOK = false;
            try
            {
                bOK = Word.LoadRecords(difficulty, out List<Word> results);
                Debug.Assert(bOK);
                if (bOK)
                    WordList = new List<Word>(results);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"LoadWordsDB exception, {ex.Message}");
            }
            return bOK;
        }

        // get next random word with filter list
        public bool GetNextRandomWord(int maxWordLength, List<string> wordListFilter, out string result)
        {
            bool bOK = false;
            result = "";
            try
            {
                int tries = 0;
                while (tries < Defines.MAX_RANDOM_TRIES)
                {
                    int num = Random.Next(WordList.Count);
                    string text = WordList[num].Text.ToLower();
                    if (text.Length <= maxWordLength && !wordListFilter.Contains(text))
                    {
                        result = text;
                        bOK = true;
                        break;
                    }
                    tries++;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"GetNextRandomWord exception, {ex.Message}");
            }
            return bOK;
        }
    }
}
