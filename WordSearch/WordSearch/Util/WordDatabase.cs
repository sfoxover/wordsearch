using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Util
{
    public class WordDatabase
    {
        // source of words
        private string[] WordList = { "Africa", "Air", "Aladdin", "Alaska", "America", "Apple", "Appleseed", "April", "As", "Asia", "Atari", "August" };
        // list of selected words
        private List<string> SelectedWords { get; set; }
        static Random RandomGenerator = new Random();

        public WordDatabase()
        {
            SelectedWords = new List<string>();
        }

        public bool CreateWordList(int numWords)
        {
            SelectedWords.Clear();
            if (numWords > WordList.Length)
                numWords = WordList.Length;
            int safeNum = numWords * 100;
            int words = 0;
            int tries = 0;
            while(words < numWords && tries < safeNum)
            {
                int num = RandomGenerator.Next(WordList.Length);
                string value = WordList[num].ToLower();
                if(!SelectedWords.Contains(value))
                {
                    SelectedWords.Add(value);
                    words++;
                }
                tries++;
            }
            return SelectedWords.Count == numWords;
        }
    }
}
