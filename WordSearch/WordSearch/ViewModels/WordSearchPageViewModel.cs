using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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

        private Array _tiles = null;
        public Array Tiles
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

        private Array _tileRowDefinition = null;
        public Array TileRowDefinition
        {
            get { return _tileRowDefinition; }
            set
            {
                if (_tileRowDefinition != value)
                {
                    _tileRowDefinition = value;
                    OnPropertyChanged();
                }
            }
        }

        private Array _tileColumnDefinition = null;
        public Array TileColumnDefinition
        {
            get { return _tileColumnDefinition; }
            set
            {
                if (_tileColumnDefinition != value)
                {
                    _tileColumnDefinition = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}


