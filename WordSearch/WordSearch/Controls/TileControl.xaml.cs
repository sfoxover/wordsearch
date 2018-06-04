using Prism.Mvvm;
using System;
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

        public TileControl(string letter)
		{
            InitializeComponent();
            ViewModel.Letter = letter;
        }
    }
}