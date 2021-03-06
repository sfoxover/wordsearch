﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WordSearch.Helpers;
using WordSearch.Models;
using WordSearch.Views;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class HighScoresPageViewModel : BindableBase
    {
        // reference to web page
        private HybridWebView WebView { get; set; }
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

        public HighScoresPageViewModel(INavigation navigation, HybridWebView webView)
            : base(navigation)
        {
            HasHtmlPageSignalled = false;
            WebView = webView;
            SourceHtml = "highScores.html";
        }

        // send message to html header page
        public bool SignalHtmlPage(string message, object data)
        {
            bool bOK = true;
            try
            {
                var msg = new MessageJson();
                msg.Message = message;
                msg.Data = data;
                string json = msg.GetJsonString();
                string script = $"highScores.handleMsgFromApp('{json}')";
                WebView.RunJSScript(script);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SignalHtmlPage exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // clear high scores from database
        internal async void ClearHighScores()
        {
            bool bOK = await Score.DeleteAllRecords();
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


