using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
    {
        private INavigation Navigation { get; set; }

        // notify Xaml that a bound property has changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // new game command click
        public ICommand NewGameClickCommand;

        public MainPageViewModel(INavigation value)
        {
            Navigation = value;
            NewGameClickCommand = new Command(NewGameClickAsync);
        }

        private async void NewGameClickAsync()
        {
            await Navigation.PushAsync(new WordSearchPage());
        }
    }
}
