using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WordSearch.Helpers;
using WordSearch.Models;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class LogFilesPageViewModel : BindableBase
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

        public LogFilesPageViewModel(INavigation navigation, FormsWebView webView)
            : base(navigation)
        {
            HasHtmlPageSignalled = false;
            WebView = webView;
            SourceHtml = "html/logFiles.html";
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
                string script = $"logFiles.handleMsgFromApp('{json}')";
                await WebView.InjectJavascriptAsync(script).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SignalHtmlPage exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // clear logs from database
        internal async void ClearLogs()
        {
            bool bOK = await Logger.Instance.DeleteAllRecords();
            if (bOK)
            {
                await SignalHtmlPage("LoadLogs", null);
            }
        }

        // load logs from database
        public async void LoadData()
        {
            bool bOK = Logger.Instance.LoadAllRecords(out List<DebugLog> results);
            if (bOK)
            {
                await SignalHtmlPage("LoadLogs", results);
            }
        }
    }
}
