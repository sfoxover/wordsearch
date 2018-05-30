using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using WordSearch.Controls;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : INotifyPropertyChanged
    {
        // notify Xaml that a bound property has changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<TileView> _tiles = null;
        public ObservableCollection<TileView> Tiles
        {
            get { return _tiles; }
            set
            {
                if (_tiles != value)
                {
                    _tiles = value;
                    OnPropertyChanged();
                }
            }
        }

        public WordSearchPageViewModel()
        {
            Tiles = new ObservableCollection<TileView>();
        }
    }
}


