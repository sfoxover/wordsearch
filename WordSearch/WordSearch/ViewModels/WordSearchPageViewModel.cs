using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Models;
using WordSearch.Util;
using Xamarin.Forms;
using System.Linq;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : BindableBase
    {
        private INavigation Navigation { get; set; }

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
        private double SecondsRemaining { get; set; }
        private double StartingSeconds { get; set; }
        // points per letter
        private double PointsPerLetter { get; set; }
        // game score
        private int GameScore { get; set; }
        // is game completed
        public bool GameCompleted { get; set; }
        // header local html path
        private string _wordSearchHeaderSourceHtml;
        public string WordSearchHeaderSourceHtml
        {
            get { return _wordSearchHeaderSourceHtml; }
            set { SetProperty(ref _wordSearchHeaderSourceHtml, value); }
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

        public WordSearchPageViewModel(INavigation value, int secondsRemaining, int pointsPerLetter)
        {
            Navigation = value;
            StartingSeconds = secondsRemaining;
            SecondsRemaining = secondsRemaining;
            PointsPerLetter = pointsPerLetter;
            Debug.Assert(SecondsRemaining > 0);
            Debug.Assert(PointsPerLetter > 0);
            GameScore = 0;
            GameCompleted = false;
            ScoreBoard = $"Score: {GameScore}";
            WordSearchHeaderSourceHtml = "html/wordSearchHeader.html";
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
                    GameTimer = $"Time remaining: {(int)SecondsRemaining} seconds";
                    return !GameCompleted;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"StartGameTimer exception, {ex.Message}");
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateScore exception, {ex.Message}");
            }
        }
    }
}


