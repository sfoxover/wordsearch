using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Runtime.CompilerServices;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : BindableBase, INavigationAware
    {
        // prism NavigationService
        protected INavigationService NavigationService { get; private set; }

        public WordSearchPageViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
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


