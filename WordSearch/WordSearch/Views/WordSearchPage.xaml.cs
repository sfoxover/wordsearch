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

        int FlagSize = 0;

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
                    int tilesX = (int)width / Defines.TILE_WIDTH; 
                    int tilesY = (int)height / Defines.TILE_HEIGHT;
                    Debug.WriteLine($"CalculateTiles starting for {tilesX} x {tilesY}");
                    var tiles = new List<TileControl>();
                    // create titles
                    for (int column = 0; column < tilesY; column++)
                    {
                        for (int row = 0; row < tilesX; row++)
                        {
                            string letter = $"{row}{column}";
                            tiles.Add(new TileControl(letter, row, column));
                            Debug.WriteLine(letter);
                        }
                    }
                    // add tiles on UI thread
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        // create titles
                        tilesView.Children.Clear();
                        foreach (var tile in tiles)
                        {
                            tilesView.Children.Add(tile);
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