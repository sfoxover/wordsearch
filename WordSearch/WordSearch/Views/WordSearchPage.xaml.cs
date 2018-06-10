using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WordSearch.Controls;
using WordSearch.Models;
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
        private SemaphoreSlim CalculateTilesSemaphore { get; set; }

        IEventAggregator EventAggregator { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage (IEventAggregator eventAggregator)
		{
            EventAggregator = eventAggregator;
            CalculateTilesSemaphore = new SemaphoreSlim(1);
            InitializeComponent();
            Appearing += WordSearchPage_Appearing;
        }

        private void WordSearchPage_Appearing(object sender, EventArgs e)
        {
            WordManager.Instance.ListenForTileClicks(EventAggregator);
        }

        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            if (PageWidth != width || PageHeight != height)
            {
                PageWidth = width;
                PageHeight = height;
                // calculate tiles for portrait mode orientation
                bool bOK = await CalculateTiles(width, height);
                Debug.Assert(bOK);
            }
        }

        // calculate for portrait mode orientation
        private async Task<bool> CalculateTiles(double width, double height)
        {
            bool bOK = true;
            try
            {
                bOK = await CalculateTilesSemaphore.WaitAsync(-1);
                // work out width and height based on page size and rows for difficulty level selected
                int rows = WordManager.Instance.GetTileRows();
                int tileWidth = (int)(width / rows);
                int tileHeight = tileWidth;
                int columns = (int)(height / tileHeight);
                Debug.WriteLine($"CalculateTiles starting for {rows} x {columns}");
                // add titles on UI thread
                FlexTilesView.Children.Clear();
                List<TileControl> controls = new List<TileControl>();
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        var control = new TileControl(row, column, tileWidth, tileHeight);
                        controls.Add(control);
                        FlexTilesView.Children.Add(control);
                    }
                }
                // initialize word array
                bOK = WordManager.Instance.InitializeWordList(rows, columns, controls);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("CalculateTiles exception, " + ex.Message);
                bOK = false;
            }
            finally
            {
                CalculateTilesSemaphore.Release();
            }
            return bOK;
        }
    }
}