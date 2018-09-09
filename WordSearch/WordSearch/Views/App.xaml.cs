using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WordSearch.Views;
using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WordSearch
{
	public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }
       
        protected override void OnStart()
        {
            AppCenter.Start("android=a65979bc-328c-45c3-a3c8-1d7f8fd12a13;" + "uwp=562aeb1d-adf4-4f7a-b562-6badd9fc9ea8;", typeof(Analytics), typeof(Crashes));
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
