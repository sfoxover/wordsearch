using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel
        {
            get { return BindingContext as MainPageViewModel; }
        }

        public MainPage()
        {
            BindingContext = new MainPageViewModel(Navigation);
            InitializeComponent();
        }
    }
}
