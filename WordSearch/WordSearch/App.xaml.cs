using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WordSearch.Views;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WordSearch
{
	public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
