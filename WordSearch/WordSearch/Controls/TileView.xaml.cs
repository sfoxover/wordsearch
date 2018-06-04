using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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

        // Tile Width request
        private int _tileWidth;
        public int TileWidth
        {
            get { return _tileWidth; }
            set
            {
                if (_tileWidth != value)
                {
                    _tileWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        // Tile height request
        private int _tileHeight;
        public int TileHeight
        {
            get { return _tileHeight; }
            set
            {
                if (_tileHeight != value)
                {
                    _tileHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        // frame click handler
        public ICommand TileClickCommand { get; set; }

        public TileView ()
		{
            BindingContext = this;
            SetDefaultValues();
            InitializeComponent();
		}

        public TileView(string letter)
        {
            BindingContext = this;
            Letter = letter;
            SetDefaultValues();
            InitializeComponent();
        }

        void SetDefaultValues()
        {
            TileWidth = Defines.TILE_WIDTH - 1;
            TileHeight = Defines.TILE_HEIGHT - 1;
            TileClickCommand = new Command(() => HandleTitleClick());
        }

        private void HandleTitleClick()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                TileFrame.BorderColor = Color.Red;
            });
        }
    }
}