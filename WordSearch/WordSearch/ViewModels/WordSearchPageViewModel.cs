using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        /*
        private FlowObservableCollection<object> _tiles = null;
        public FlowObservableCollection<object> Tiles
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
        */

        private int? _columnCount = 0;
        public int? ColumnCount
        {
            get { return _columnCount; }
            set
            {
                if (_columnCount != value)
                {
                    _columnCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public WordSearchPageViewModel()
        {
            //Tiles = new FlowObservableCollection<object>();
        }
    }
}


