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

        public WordDatabase()
        {
        }

        // get random list of words
        public bool GetRandomWords(int numWords, int maxWordLength, out List<string> wordList)
        {
            wordList = new List<string>();
            try
            {
                Random rnd = new Random();
                if (numWords > WordList.Length)
                    numWords = WordList.Length;
                int safeNum = numWords * 100;
                int words = 0;
                int tries = 0;
                while(words < numWords && tries < safeNum)
                {
                    int num = rnd.Next(WordList.Length);
                    string text = WordList[num].ToLower();
                    if(text.Length <= maxWordLength && !wordList.Contains(text))
                    {
                        wordList.Add(text);
                        words++;
                    }
                    tries++;
                }
                return wordList.Count == numWords;
            }
            catch (Exception ex)
            {
                var error = $"GetRandomWords exception, {ex.Message}";
                Debug.WriteLine(error);
            }
            return false;
        }
    }
}
