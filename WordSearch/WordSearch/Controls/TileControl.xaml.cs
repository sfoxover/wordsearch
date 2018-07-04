using System.Diagnostics;
using WordSearch.Models;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TileControl : ContentView
    {
        public TileControlViewModel ViewModel
        {
            get { return BindingContext as TileControlViewModel; }
        }

        // test live rendering
        public TileControl()
        {
            BindingContext = new TileControlViewModel(null);
            ViewModel.Letter = "a";
            ViewModel.TileRow = 0;
            ViewModel.TileColum = 0;
            ViewModel.TileWidth = 90 - 2;
            ViewModel.TileHeight = 90 - 2;
            ViewModel.LetterSelected = true;
            InitializeComponent();
        }

        public TileControl(TileControlViewModel viewModel)
        {
            Debug.Assert(viewModel != null);
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}