using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
	public class MainPageViewModel : BindableBase
    {
        private INavigation Navigation { get; set; }

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
