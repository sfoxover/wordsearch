using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
                TilesPerRow = (int)PageWidth / Defines.TILE_WIDTH; 
                TilesPerColumn = (int)PageHeight / Defines.TILE_HEIGHT;

                // create grid Row Definition
                var rows = new RowDefinitionCollection();
                for (int n = 0; n < TilesPerRow; n++)
                {
                    rows.Add(new RowDefinition { Height = GridLength.Star });
                }
                // create grid Column Definition
                var columns = new ColumnDefinitionCollection();
                for (int n = 0; n < TilesPerColumn; n++)
                {
                    columns.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    tilesView.RowDefinitions = rows;
                    tilesView.ColumnDefinitions = columns;
                    // create titles
                    tilesView.Children.Clear();
                    for (int row = 0; row < TilesPerRow; row++)
                    {
                        for (int column = 0; column < TilesPerColumn; column++)
                        {
                            string letter = $"{row}{column}";
                            tilesView.Children.Add(new TileView(letter), row, column);
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