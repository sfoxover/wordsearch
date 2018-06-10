using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Controls;
using WordSearch.ViewModels;
using Xamarin.Forms;

namespace WordSearch.Util
{
    public class WordManager
    {
        private static readonly Lazy<WordManager> _instance = new Lazy<WordManager>(() => { return new WordManager(); });
        public static WordManager Instance { get { return _instance.Value; } }       
        // game difficulty enum
        public enum GameDifficulty { easy, medium, hard};
        public GameDifficulty DifficultyLevel { get; set; }
        // tile sizes
        private const int TILE_ROWS_LEVEL_EASY = 8;
        private const int TILE_ROWS_LEVEL_MEDIUM = 12;
        private const int TILE_ROWS_LEVEL_HARD = 20;
        // array of random words
        public List<Word> Words { get; set; }
        private static object WordsLock = new object();
        // word list array
        private Tile[,] Tiles { get; set; }
        private static object TilesLock = new object();

        private WordManager()
        {
            Tiles = null;
            Words = null;
            DifficultyLevel = GameDifficulty.easy;
        }

        // create new word tile multi dimentional array
        public bool InitializeWordList(int rows, int columns)
        {
            bool bOK = true;
            try
            {
                // create titles
                var tiles = new Tile[rows, columns];
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        var tile = new Tile();
                        tiles.SetValue(tile, row, column);
                    }
                }
                // store tile array
                lock (TilesLock)
                {
                    Tiles = tiles;
                }
                // put words in tiles
                int maxWordLength = rows > columns? rows : columns;
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
                lock (WordsLock)
                {
                    Words = new List<Word>();
                }
                // load words database
                var wordDb = new WordDatabase();
                List<string> wordList;
                bOK = wordDb.GetRandomWords(5, maxWordLength, out wordList);
                Debug.Assert(bOK);
                if (bOK)
                {
                    foreach (var text in wordList)
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
                                lock (WordsLock)
                                {
                                    Words.Add(word);
                                }
                            }
                        }
                    }
                    if (bOK)
                    {
                        // update tiles with words
                        bOK = RefreshWordTileStates();
                        Debug.Assert(bOK);
                    }
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
                lock (WordsLock)
                {
                    foreach (var word in Words) 
                    {
                        for (int n = 0; n < word.Text.Length; n++)
                        {
                            char ch = word.Text[n];
                            int row = (int)word.TilePositions[n].X;
                            int column = (int)word.TilePositions[n].Y;
                            lock (TilesLock)
                            {
                                Tiles[row, column].Letter = ch;
                            }
                        }
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
                Random rand = new Random();
                int rows = 0;
                int columns = 0;
                lock (TilesLock)
                {
                    rows = Tiles.GetLength(0);
                    columns = Tiles.GetLength(1);
                }
                int tries = 0;
                while (tries < 100)
                {
                    int wordLen = word.Text.Length;
                    int row = rand.Next(rows);
                    int column = rand.Next(columns);
                    var directions = new List<Word.WordDirection>();
                    foreach (Word.WordDirection direction in Enum.GetValues(typeof(Word.WordDirection)))
                    {
                        if (word.WordFits(direction, row, column, rows, columns))
                        {
                            directions.Add(direction);
                        }
                    }
                    if (directions.Count > 0)
                    {
                        // select direction and set row and column for word object
                        word.Direction = directions[rand.Next(directions.Count)];
                        word.Row = row;
                        word.Column = column;
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
        public bool GetTileAt(int row, int column, out Tile tile)
        {
            bool bOK = true;
            tile = null;
            try
            {
                lock (TilesLock)
                {
                    tile = Tiles.GetValue(row, column) as Tile;
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
