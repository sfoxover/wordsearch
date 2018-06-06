using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TileControl : ContentView
    {
        private TileControlViewModel ViewModel
        {
            get { return BindingContext as TileControlViewModel; }
        }

        public TileControl(string letter, int row, int column)
		{
            InitializeComponent();
            ViewModel.Letter = letter;
            ViewModel.TileRow = row;
            ViewModel.TileColum = column;
        }
    }
}