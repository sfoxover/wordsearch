using Prism.Events;
using System;
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
        double PageWidth { get; set; }
        double PageHeight { get; set; }
        // portrait tile size
        int TilesX { get; set; }
        int TilesY { get; set; }

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
                    bool bOK = CalculateTiles();
                    Debug.Assert(bOK);
                });
            }
        }

        // calculate for portrait mode orientation
        private bool CalculateTiles()
        {
            bool bOK = true;
            try
            {
                TilesX = (int)PageWidth / Defines.TILE_WIDTH; 
                TilesY = (int)PageHeight / Defines.TILE_HEIGHT;

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    // create titles
                    tilesView.Children.Clear();
                    for (int row = 0; row < TilesX; row++)
                    {
                        for (int column = 0; column < TilesY; column++)
                        {
                            string letter = $"{row}{column}";
                            tilesView.Children.Add(new TileControl(letter, row, column));
                        }
                    }
                });
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