using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordSearch.Models;
using WordSearch.Helpers;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HighScoresPage : ContentPage
    {
        // get access to ViewModel
        private HighScoresPageViewModel ViewModel
        {
            get { return BindingContext as HighScoresPageViewModel; }
        }

        public HighScoresPage(double width, double height)
        {
            InitializeComponent();
            BindingContext = new HighScoresPageViewModel(Navigation, webViewHighScores);
            ViewModel.HtmlPageWidth = width;
            ViewModel.HtmlPageHeight = height;
            webViewHighScores.AddLocalCallback("scoresJSCallback", ScoresJSCallback);
            ViewModel.LoadHighScoreData();
        }

        // callback from JS body html page
        void ScoresJSCallback(string message)
        {
            ViewModel.HasHtmlPageSignalled = true;
            System.Diagnostics.Debug.WriteLine($"Got local callback: {message}");
            MessageJson msg = new MessageJson(message);
            switch (msg.Message)
            {
                case "ping":
                    break;
                case "closeWindow":
                    // back to main page
                    ViewModel.CloseWindow();
                    break;
                case "clearScores":
                    ViewModel.ClearHighScores();
                    break;
                case "Error":
                    if (msg.Data != null)
                    {
                        Logger.Instance.Error(msg.Data.ToString());
                    }
                    break;
                default:
                    Debug.Assert(false, $"ScoresJSCallback unexpected message {message}");
                    break;
            }
        }
    }
}