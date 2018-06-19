using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Controls;
using WordSearch.Models;
using WordSearch.ViewModels;
using Xamarin.Forms;

namespace WordSearch.Util
{
    public class WordManager
    {
        static Random Random = new Random();
        // game difficulty enum
        public enum GameDifficulty { easy, medium, hard};
        public GameDifficulty DifficultyLevel { get; set; }
        // tile sizes
        private const int TILE_ROWS_LEVEL_EASY = 8;
        private const int TILE_ROWS_LEVEL_MEDIUM = 12;
        private const int TILE_ROWS_LEVEL_HARD = 20;
        // words to solve per level
        private const int WORDS_LEVEL_EASY = 4;
        private const int WORDS_LEVEL_MEDIUM = 8;
        private const int WORDS_LEVEL_HARD = 16;

        // array of random words
        public List<Word> Words { get; set; }
        // word list array
        private TileControlViewModel[,] TileViewModels { get; set; }

        public WordManager()
        {
            TileViewModels = null;
            Words = null;
            DifficultyLevel = GameDifficulty.easy;
        }

        // create new word tile multi dimentional array
        public bool InitializeWordList(int maxWordLength, TileControlViewModel[,] viewModels)
        {
            bool bOK = true;
            try
            {
                // store tile array
                TileViewModels = viewModels;
                // put words in tiles
                bOK = PlaceWordsInTiles(maxWordLength);
                Debug.Assert(bOK);
            }
            catch (Exception ex)
            {
                var error = $"InitializeWordList exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // put words in tiles
        private bool PlaceWordsInTiles(int maxWordLength)
        {
            bool bOK = true;
            try
            {
                Words = new List<Word>();
                // load words database
                var wordDb = new WordDatabase();
                int count = 0;
                int tries = 0;
                int total = GetLevelWordCount();
                var filterList = new List<string>();
                while (count < total && tries < Defines.MAX_RANDOM_TRIES)
                {
                    string text;
                    bOK = wordDb.GetNextRandomWord(maxWordLength, filterList, out text);
                    Debug.Assert(bOK);
                    if (bOK)
                    {
                        // create word object
                        Word word = new Word(text);
                        // select a random direction and position
                        bOK = SelectRandomPose(ref word);
                        Debug.Assert(bOK);
                        if (bOK)
                        {
                            bOK = word.CalculateTilePositions();
                            Debug.Assert(bOK);
                            if (bOK)
                            {
                                // found next word that fits ok
                                filterList.Add(text);
                                Debug.WriteLine($"PlaceWordsInTiles adding word {text}");
                                Words.Add(word);
                                count++;
                                tries = 0;
                            }
                        }
                    }
                    tries++;
                }
                if (bOK)
                {
                    // update tiles with words
                    bOK = RefreshWordTileStates();
                    Debug.Assert(bOK);
                }
            }
            catch (Exception ex)
            {
                var error = $"PlaceWordsInTiles exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // refresh all tiles that belong to a word
        private bool RefreshWordTileStates()
        {
            bool bOK = true;
            try
            {
                foreach (var word in Words) 
                {
                    for (int n = 0; n < word.Text.Length; n++)
                    {
                        char ch = word.Text[n];
                        int row = (int)word.TilePositions[n].X;
                        int column = (int)word.TilePositions[n].Y;
                        TileViewModels[row, column].Letter = $"{ch}";
                        TileViewModels[row, column].LetterSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var error = $"RefreshWordTileStates exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // select a random direction and position
        bool SelectRandomPose(ref Word word)
        {
            bool bOK = false;
            try
            {
                int rows = TileViewModels.GetLength(0);
                int columns = TileViewModels.GetLength(1);
                int tries = 0;
                while (tries < Defines.MAX_RANDOM_TRIES)
                {
                    int wordLen = word.Text.Length;
                    int row = Random.Next(rows);
                    int column = Random.Next(columns);
                    word.Row = row;
                    word.Column = column;
                    var directions = new List<Word.WordDirection>();
                    foreach (Word.WordDirection direction in Enum.GetValues(typeof(Word.WordDirection)))
                    {
                        // test if word fits
                        word.Direction = direction;
                        if (word.WordFits(rows, columns))
                        {
                            // test for collision with existing words
                            bool bHasCollision = false;
                            foreach(var existingWord in Words)
                            {
                                if(existingWord.HasCollision(word))
                                {
                                    bHasCollision = true;
                                    break;
                                }
                            }
                            if(!bHasCollision)
                                directions.Add(direction);
                        }
                    }
                    if (directions.Count > 0)
                    {
                        // select direction and set row and column for word object
                        word.Direction = directions[Random.Next(directions.Count)];                        
                        bOK = true;
                        break;
                    }
                    tries++;
                }
            }
            catch (Exception ex)
            {
                var error = $"SelectRandomPose exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // get tile from array
        public bool GetTileAt(int row, int column, out TileControlViewModel tile)
        {
            bool bOK = true;
            tile = null;
            try
            {
                tile = TileViewModels.GetValue(row, column) as TileControlViewModel;
            }
            catch (Exception ex)
            {
                var error = $"GetTileAt exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // get tile width based on game difficulty
        public int GetTileRows()
        {
            int result = 0;
            switch(DifficultyLevel)
            {
                case GameDifficulty.easy:
                    result = TILE_ROWS_LEVEL_EASY;
                    break;
                case GameDifficulty.medium:
                    result = TILE_ROWS_LEVEL_MEDIUM;
                    break;
                case GameDifficulty.hard:
                    result = TILE_ROWS_LEVEL_HARD;
                    break;
            }
            Debug.Assert(result != 0);
            return result;
        }

        // get number of words to find based on difficulty
        int GetLevelWordCount()
        {
            int count = 0;
            switch(DifficultyLevel)
            {
                case GameDifficulty.easy:
                    count = WORDS_LEVEL_EASY;
                    break;
                case GameDifficulty.medium:
                    count = WORDS_LEVEL_MEDIUM;
                    break;
                case GameDifficulty.hard:
                    count = WORDS_LEVEL_HARD;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return count;
        }

        public bool ListenForTileClicks(IEventAggregator eventAggregator)
        {
            bool bOK = true;
            try
            {
                if (eventAggregator == null)
                    return false;
                eventAggregator.GetEvent<TileSelectionEvent<TileControlViewModel>>().Subscribe((tile)=> {
                    Debug.WriteLine($"tile clicked {tile.Letter}");
                });
            }
            catch(Exception ex)
            {
                var error = $"ListenForTileClicks exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        public bool StopListenForTileClicks(IEventAggregator eventAggregator)
        {
            bool bOK = true;
            try
            {
                if (eventAggregator == null)
                    return false;
                eventAggregator.GetEvent<TileSelectionEvent<TileControlViewModel>>().Unsubscribe((tile) => {
                    Debug.WriteLine($"tile clicked {tile.Letter}");
                });
            }
            catch (Exception ex)
            {
                var error = $"StopListenForTileClicks exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }
    }
}
