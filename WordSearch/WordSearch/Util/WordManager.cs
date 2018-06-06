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

        private WordManager()
        {
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
