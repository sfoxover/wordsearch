using System;
using System.Diagnostics;
using WordSearch.Models;
using Xamarin.Forms;
using System.Threading.Tasks;
using WordSearch.Helpers;
using WordSearch.Views;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : BindableBase
    {
        private HybridWebView WebViewHeader { get; set; }
        private HybridWebView WebViewTiles { get; set; }

        // Loading screen visible.
        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        // countdown timer
        private string _gameTimer;
        public string GameTimer
        {
            get { return _gameTimer; }
            set { SetProperty(ref _gameTimer, value); }
        }
        // score board
        private string _scoreBoard;
        public string ScoreBoard
        {
            get { return _scoreBoard; }
            set { SetProperty(ref _scoreBoard, value); }
        }
        // countdown timer
        internal double SecondsRemaining { get; set; }
        private double StartingSeconds { get; set; }
        // points per letter
        private double PointsPerLetter { get; set; }
        // game score
        public int GameScore { get; set; }
        // is game completed
        public bool GameCompleted { get; set; }
        // header local html path
        private string _wordSearchHeaderSourceHtml;
        public string WordSearchHeaderSourceHtml
        {
            get { return _wordSearchHeaderSourceHtml; }
            set { SetProperty(ref _wordSearchHeaderSourceHtml, value); }
        }
        // tile body local html path
        private string _wordSearchTilesSourceHtml;
        public string WordSearchTilesSourceHtml
        {
            get { return _wordSearchTilesSourceHtml; }
            set { SetProperty(ref _wordSearchTilesSourceHtml, value); }
        }
        // local html page width
        private double _htmlPageWidth;
        public double HtmlPageWidth
        {
            get { return _htmlPageWidth; }
            set { SetProperty(ref _htmlPageWidth, value); }
        }
        // local header html page height
        private double _htmlHeaderPageHeight;
        public double HtmlHeaderPageHeight
        {
            get { return _htmlHeaderPageHeight; }
            set { SetProperty(ref _htmlHeaderPageHeight, value); }
        }
        // local tile html page height
        private double _htmlTilePageHeight;
        public double HtmlTilePageHeight
        {
            get { return _htmlTilePageHeight; }
            set { SetProperty(ref _htmlTilePageHeight, value); }
        }
        // has the html page loaded
        public bool HasTilesPageSignalled { get; set; }
        public bool HasHeaderPageSignalled { get; set; }

        public WordSearchPageViewModel(INavigation navigation, int secondsRemaining, int pointsPerLetter, HybridWebView webViewHeader, HybridWebView webViewTiles)
             : base(navigation)
        {
            IsLoading = true;
            HasTilesPageSignalled = false;
            HasHeaderPageSignalled = false;
            WebViewHeader = webViewHeader;
            WebViewTiles = webViewTiles;
            StartingSeconds = secondsRemaining;
            SecondsRemaining = secondsRemaining;
            PointsPerLetter = pointsPerLetter;
            Debug.Assert(SecondsRemaining > 0);
            Debug.Assert(PointsPerLetter > 0);
            GameScore = 0;
            GameCompleted = false;
            ScoreBoard = $"Score: {GameScore}";
            WordSearchHeaderSourceHtml = "wordSearchHeader.html";
            WordSearchTilesSourceHtml = "wordSearchTiles.html";
            StartGameTimer();
        }    

        private void StartGameTimer()
        {
            try
            {
                Device.StartTimer(new TimeSpan(0, 0, 0, 1), () =>
                {
                    SecondsRemaining -= 1;
                    if (SecondsRemaining < 0)
                        SecondsRemaining = 0;
                    if (SecondsRemaining > 0)
                    {
                        var ts = new TimeSpan(0, 0, 0, (int)SecondsRemaining);
                        GameTimer = $"Time: {ts.Minutes} MIN {ts.Seconds} SEC";
                    }
                    else
                    {
                        GameTimer = "GAME OVER.";
                    }
                    SignalHeaderHtmlPage("OnUpdateTime", GameTimer);
                    return !GameCompleted;
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"StartGameTimer exception, {ex.Message}");
            }
        }

        internal void UpdateScore(int length)
        {
            try
            {
                if (SecondsRemaining > 0)
                {
                    double multiplier = SecondsRemaining / StartingSeconds * PointsPerLetter;
                    GameScore += (int)(length * multiplier);
                    ScoreBoard = $"Score: {GameScore}";
                    SignalHeaderHtmlPage("OnUpdateScore", ScoreBoard);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"UpdateScore exception, {ex.Message}");
            }
        }

        // send message to html header page
        public bool SignalHeaderHtmlPage(string message, object data)
        {
            bool bOK = true;
            try
            {
                var msg = new MessageJson();
                msg.Message = message;
                msg.Data = data;
                string json = msg.GetJsonString();
                string script = $"header.handleMsgFromApp('{json}')";
                WebViewHeader.RunJSScript(script);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SignalHeaderHtmlPage exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // send message to html tiles page
        public bool SignalTilesHtmlPage(string message, object data)
        {
            bool bOK = true;
            try
            {
                var msg = new MessageJson();
                msg.Message = message;
                msg.Data = data;
                string json = msg.GetJsonString();
                string script = $"tiles.handleMsgFromApp('{json}')";
                WebViewTiles.RunJSScript(script);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SignalTilesHtmlPage exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // Remove score on missed tile hit
        internal void SubtractPenaltyScore()
        {
            try
            {
                GameScore -= Defines.PENALTY_POINTS;
                ScoreBoard = $"Score: {GameScore}";
                SignalHeaderHtmlPage("OnUpdateScore", ScoreBoard);
                SignalHeaderHtmlPage("SubtractPenaltyScore", $"-{Defines.PENALTY_POINTS}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SubtractPenaltyScore exception, {ex.Message}");
            }
        }
    }
}


