using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WordSearch.Models;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class HighScoresPageViewModel : BindableBase
    {
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
        // has the html page loaded
        public bool HasHtmlPageSignalled { get; set; }

        public HighScoresPageViewModel(INavigation navigation, FormsWebView webView)
            : base(navigation)
        {
            HasHtmlPageSignalled = false;
            WebView = webView;
            SourceHtml = "html/highScores.html";
        }

        // send message to html header page
        public async Task<bool> SignalHtmlPage(string message, object data)
        {
            bool bOK = true;
            try
            {
                // wait for page load
                int waited = 0;
                while (!HasHtmlPageSignalled && waited < 20)
                {
                    await Task.Delay(100);
                    waited++;
                }
                Debug.Assert(HasHtmlPageSignalled);
                var msg = new MessageJson();
                msg.Message = message;
                msg.Data = data;
                string json = msg.GetJsonString();
                string script = $"highScores.handleMsgFromApp('{json}')";
                await WebView.InjectJavascriptAsync(script).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SignalHtmlPage exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // clear high scores from database
        internal void ClearHighScores()
        {
            bool bOK = Score.DeleteAllRecords();
            if (bOK)
            {
                SignalHtmlPage("LoadHighScores", null);
            }
        }

        // load high scores from database
        public void LoadHighScoreData()
        {
            bool bOK = Score.LoadAllRecords(out List<Score> results);
            if(bOK)
            {
                SignalHtmlPage("LoadHighScores", results);
            }
        }
    }
}


