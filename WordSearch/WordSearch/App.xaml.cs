using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.DryIoc;
using WordSearch.Views;
using Prism.Mvvm;
using WordSearch.ViewModels;
using WordSearch.Controls;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WordSearch
{
	public partial class App : PrismApplication
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<WordSearchPage>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(AlternateResolver);
        }

        private static Type AlternateResolver(Type type)
        {
            var viewName = type.Name;
            if (String.IsNullOrEmpty(viewName))
                return null;

            var viewModel = "WordSearch.ViewModels." + viewName + "ViewModel";

            return Type.GetType(viewModel);
        }
    }
}
