using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Controls;
using WordSearch.ViewModels;

namespace WordSearch.Util
{
    public class WordManager
    {
        private static readonly Lazy<WordManager> _instance = new Lazy<WordManager>(() => { return new WordManager(); });
        public static WordManager Instance { get { return _instance.Value; } }
        // word list array
        private Tile[,] TileLetterArray { get; set; }
        private static object TileLetterArrayLock = new object();
        // game difficulty enum
        public enum GameDifficulty { easy, medium, hard};
        public GameDifficulty DifficultyLevel { get; set; }
        // tile sizes
        private const int TILE_ROWS_LEVEL_EASY = 8;
        private const int TILE_ROWS_LEVEL_MEDIUM = 12;
        private const int TILE_ROWS_LEVEL_HARD = 20;
        // word database
        private WordDatabase WordsDb { get; set; }
        // array of random words
        public List<Word> SelectedWords { get; set; }
       
        private WordManager()
        {
            TileLetterArray = null;
            WordsDb = null;
            SelectedWords = null;
            DifficultyLevel = GameDifficulty.medium;
        }

        // create new word tile multi dimentional array
        public bool InitializeWordTile(int rows, int columns)
        {
            bool bOK = true;
            try
            {
                var tiles = new Tile[rows, columns];
                // create titles
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        var tile = new Tile();
                        tiles.SetValue(tile, row, column);
                    }
                }
                // load words database
                WordsDb = new WordDatabase();
                bOK = WordsDb.CreateWordList(5);
                Debug.Assert(bOK);
                if (bOK)
                {
                    // put words in tiles
                    bOK = PlaceWordsInTiles(tiles, WordsDb.SelectedWords);
                    lock (TileLetterArrayLock)
                    {
                        TileLetterArray = tiles;
                    }
                }
            }
            catch (Exception ex)
            {
                var error = $"InitializeWordTile exception, {ex.Message}";
                Debug.WriteLine(error);
                bOK = false;
            }
            return bOK;
        }

        // put words in tiles
        private bool PlaceWordsInTiles(Tile[,] tiles, List<string> selectedWords)
        {
            bool bOK = true;
            try
            {
                SelectedWords = new List<Word>();
                foreach (var text in selectedWords)
                {
                    Word word = new Word();
                    SelectedWords.Add(word);
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

        // get tile from array
        public bool GetTileAt(int row, int column, out Tile tile)
        {
            bool bOK = true;
            tile = null;
            try
            {
                lock (TileLetterArrayLock)
                {
                    tile = TileLetterArray.GetValue(row, column) as Tile;
                }
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
