using Prism.Events;
using System;
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

        private WordManager()
        {
            TileLetterArray = null;
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
                lock(TileLetterArrayLock)
                {
                    TileLetterArray = tiles;
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
