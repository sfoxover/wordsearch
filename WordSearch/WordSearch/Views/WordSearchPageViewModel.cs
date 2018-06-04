using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Runtime.CompilerServices;

namespace WordSearch
{
    public class WordSearchPageViewModel : BindableBase
    {
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
            set { SetProperty(ref _columnCount, value); }
        }

        public WordSearchPageViewModel()
        {
            //Tiles = new FlowObservableCollection<object>();
        }
    }
}


