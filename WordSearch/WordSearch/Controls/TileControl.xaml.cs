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

        public TileControl(int row, int column, int tileWidth, int tileHeight)
		{
            InitializeComponent();
            Debug.Assert(ViewModel != null);
            ViewModel.Letter = $"{TileControlViewModel.GetRandomLetter()}";
            ViewModel.TileRow = row;
            ViewModel.TileColum = column;
            ViewModel.TileWidth = tileWidth - 2;
            ViewModel.TileHeight = tileHeight - 2;
            ViewModel.LetterSelected = false;
        }
    }
}