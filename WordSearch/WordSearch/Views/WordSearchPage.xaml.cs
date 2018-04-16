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
			InitializeComponent();
            BindingContext = new WordSearchPageViewModel();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            if (PageWidth != width || PageHeight != height)
            {
                PageWidth = width;
                PageHeight = height;
                bool bOK = CalculateTiles();
                Debug.Assert(bOK);
            }
        }

        private bool CalculateTiles()
        {
            bool bOK = true;
            try
            {
                TilesPerRow = (int)PageWidth / Defines.TILE_WIDTH;
                TilesPerColumn = (int)PageHeight / Defines.TILE_HEIGHT;
            }
            catch(Exception ex)
            {
                bOK = false;
            }
            return bOK;
        }

    }
}