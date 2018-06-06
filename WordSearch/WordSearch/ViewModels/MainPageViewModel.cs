using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace WordSearch.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
    {
        // prism NavigationService
        protected INavigationService NavigationService { get; private set; }

        // new game command click
        public DelegateCommand NewGameClickCommand { get; set; }

        public MainPageViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            NewGameClickCommand = new DelegateCommand(NewGameClickAsync);
        }

        private async void NewGameClickAsync()
        {
            await NavigationService.NavigateAsync("NavigationPage/WordSearchPage");
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
