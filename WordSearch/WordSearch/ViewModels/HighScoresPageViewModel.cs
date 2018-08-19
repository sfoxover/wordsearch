using System;
using System.Collections.Generic;
using System.Text;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class HighScoresPageViewModel : BindableBase
    {
        private INavigation Navigation { get; set; }
        // reference to web page
        private FormsWebView WebView { get; set; }
        private string _sourceHtml;
        public string SourceHtml
        {
            get { return _sourceHtml; }
            set { SetProperty(ref _sourceHtml, value); }
        }      
        // local html page width
        private double _htmlPageWidth;
        public double HtmlPageWidth
        {
            get { return _htmlPageWidth; }
            set { SetProperty(ref _htmlPageWidth, value); }
        }
        // local header html page height
        private double _htmlPageHeight;
        public double HtmlPageHeight
        {
            get { return _htmlPageHeight; }
            set { SetProperty(ref _htmlPageHeight, value); }
        }

        public HighScoresPageViewModel(INavigation value, FormsWebView webView)
        {
            WebView = webView;
            Navigation = value;
            SourceHtml = "html/highScores.html";
        }
    }
}


