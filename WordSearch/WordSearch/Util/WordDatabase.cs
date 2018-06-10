using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WordSearch.Util
{
    public class WordDatabase
    {
        // source of words
        private string[] WordList = { "Africa", "Air", "Aladdin", "Alaska", "America", "Apple", "Appleseed", "April", "As", "Asia", "Atari", "August" };
        static Random Random = new Random();
        public WordDatabase()
        {
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
                    int num = Random.Next(WordList.Length);
                    string text = WordList[num].ToLower();
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
                var error = $"GetNextRandomWord exception, {ex.Message}";
                Debug.WriteLine(error);
            }
            return bOK;
        }
    }
}
