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
            BindingContext = new TileControlViewModel();
            ViewModel.Letter = "a";
            ViewModel.TileRow = 0;
            ViewModel.TileColumn = 0;
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