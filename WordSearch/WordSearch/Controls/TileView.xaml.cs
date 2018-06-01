using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TileView : ContentView, INotifyPropertyChanged
    {
        // Notify Xaml that a bound property has changed
        protected event PropertyChangedEventHandler PropertyChanged;
        protected override void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // letter displayed in control
        private string _letter;
        public string Letter
        {
            get { return _letter; }
            set
            {
                if (_letter != value)
                {
                    _letter = value;
                    OnPropertyChanged();
                }
            }
        }

        public TileView ()
		{
            BindingContext = this;
            InitializeComponent();
		}

        public TileView(string letter)
        {
            BindingContext = this;
            Letter = letter;
            InitializeComponent();
        }
    }
}