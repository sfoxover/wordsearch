using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WordSearch.Controls;
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
        int TilesPerRow { get; set; }
        int TilesPerColumn { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage ()
		{
            BindingContext = new WordSearchPageViewModel();
            InitializeComponent();
        }       

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            if (PageWidth != width || PageHeight != height)
            {
                PageWidth = width;
                PageHeight = height;
                // calculate tiles for portrait mode orientation
                bool bOK = CalculateTiles();
                Debug.Assert(bOK);
            }
        }

        // calculate for portrait mode orientation
        private bool CalculateTiles()
        {
            bool bOK = true;
            try
            {
                TilesPerRow = (int)PageWidth / Defines.TILE_WIDTH;
                TilesPerColumn = (int)PageHeight / Defines.TILE_HEIGHT;
                ViewModel.ColumnCount = TilesPerColumn;
                int totalTiles = TilesPerRow * TilesPerColumn;
                var items = new List<object>();
                for (int n = 0; n < totalTiles; n++)
                {
                    items.Add(new TileView());
                }
                ViewModel.Tiles = new FlowObservableCollection<object>(items);
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