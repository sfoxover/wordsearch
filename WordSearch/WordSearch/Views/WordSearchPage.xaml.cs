using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WordSearch.Controls;
using WordSearch.Util;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WordSearchPage : ContentPage
    {
        // page width and height
        double PageWidth { get; set; }
        double PageHeight { get; set; }
        // lock tile creation
        private static object CalculateTilesLock = new object();

        IEventAggregator EventAggregator { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage (IEventAggregator eventAggregator)
		{
            EventAggregator = eventAggregator;
            InitializeComponent();
            Appearing += WordSearchPage_Appearing;
        }

        private void WordSearchPage_Appearing(object sender, EventArgs e)
        {
            WordManager.Instance.ListenForTileClicks(EventAggregator);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            if (PageWidth != width || PageHeight != height)
            {
                PageWidth = width;
                PageHeight = height;
                // calculate tiles for portrait mode orientation
                Task.Run(() =>
                {
                    bool bOK = CalculateTiles(width, height);
                    Debug.Assert(bOK);
                });
            }
        }

        // calculate for portrait mode orientation
        private bool CalculateTiles(double width, double height)
        {
            bool bOK = true;
            try
            {
                lock (CalculateTilesLock)
                {
                    // work out width and height based on page size and rows for difficulty level selected
                    int rows = WordManager.Instance.GetTileRows();
                    int tileWidth = (int)(width / rows);
                    int tileHeight = tileWidth;
                    int columns = (int)(height / tileHeight);
                    Debug.WriteLine($"CalculateTiles starting for {rows} x {columns}");
                    // initialize word array
                    WordManager.Instance.InitializeWordList(rows, columns);
                    // create titles
                    // add tiles on UI thread
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        // create titles
                        tilesView.Children.Clear();
                        for (int column = 0; column < columns; column++)
                        {
                            for (int row = 0; row < rows; row++)
                            {
                                Tile tile = null;
                                if (WordManager.Instance.GetTileAt(row, column, out tile))
                                {
                                    string letter = $"{tile.Letter}";
                                    tilesView.Children.Add(new TileControl(letter, row, column, tileWidth, tileHeight));
                                }
                                else
                                {
                                    bOK = false;
                                    Debug.WriteLine($"Failed to read tile {row} x {column}");
                                }
                            }
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("CalculateTiles exception, " + ex.Message);
                bOK = false;
            }
            return bOK;
        }
    }
}