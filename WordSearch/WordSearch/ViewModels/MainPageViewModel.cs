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
        public Command NewEasyGameCommand { get; }

        public MainPageViewModel(INavigation value)
        {
            Navigation = value;
            NewEasyGameCommand = new Command(OnNewGameClick);
        }

        public async void OnNewGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage());
        }
    }
}
