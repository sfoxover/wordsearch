using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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

        private Grid.IGridList<View> _tiles = null;
        public Grid.IGridList<View> Tiles
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

        private RowDefinitionCollection _tileRowDefinition = null;
        public RowDefinitionCollection TileRowDefinition
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

        private ColumnDefinitionCollection _tileColumnDefinition = null;
        public ColumnDefinitionCollection TileColumnDefinition
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


