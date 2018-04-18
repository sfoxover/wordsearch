using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public WordSearchPage ()
		{
            BindingContext = new WordSearchPageViewModel();
            InitializeComponent();
        }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
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
                // create grid Row Definition
                var rows = new RowDefinitionCollection();
                for(int n=0;n<TilesPerRow;n++)
                {
                    rows.Add(new RowDefinition { Height = Defines.TILE_HEIGHT });
                }
                ViewModel.TileRowDefinition = rows;
                // create grid Column Definition
                var columns = new ColumnDefinitionCollection();
                for (int n = 0; n < TilesPerColumn; n++)
                {
                    columns.Add(new ColumnDefinition { Width = Defines.TILE_WIDTH });
                }
                ViewModel.TileColumnDefinition = columns;
            }
            catch(Exception ex)
            {
                bOK = false;
            }
            return bOK;
        }

    }
}