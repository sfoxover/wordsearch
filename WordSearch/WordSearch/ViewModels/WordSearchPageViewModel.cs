﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Models;
using WordSearch.Util;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : BindableBase
    {
        private INavigation Navigation { get; set; }

        // header text labels
        private string _text1;
        public string Text1
        {
            get { return _text1; }
            set { SetProperty(ref _text1, value); }
        }
        private string _text2;
        public string Text2
        {
            get { return _text2; }
            set { SetProperty(ref _text2, value); }
        }
        private string _text3;
        public string Text3
        {
            get { return _text3; }
            set { SetProperty(ref _text3, value); }
        }
        private string _text4;
        public string Text4
        {
            get { return _text4; }
            set { SetProperty(ref _text4, value); }
        }
        private string _text5;
        public string Text5
        {
            get { return _text5; }
            set { SetProperty(ref _text5, value); }
        }
        private string _text6;
        public string Text6
        {
            get { return _text6; }
            set { SetProperty(ref _text6, value); }
        }
        private string _text7;
        public string Text7
        {
            get { return _text7; }
            set { SetProperty(ref _text7, value); }
        }
        private string _text8;
        public string Text8
        {
            get { return _text8; }
            set { SetProperty(ref _text8, value); }
        }
        private string _text9;
        public string Text9
        {
            get { return _text9; }
            set { SetProperty(ref _text9, value); }
        }
        private string _text10;
        public string Text10
        {
            get { return _text10; }
            set { SetProperty(ref _text10, value); }
        }
        private string _text11;
        public string Text11
        {
            get { return _text11; }
            set { SetProperty(ref _text11, value); }
        }
        private string _text12;
        public string Text12
        {
            get { return _text12; }
            set { SetProperty(ref _text12, value); }
        }
        private string _text13;
        public string Text13
        {
            get { return _text13; }
            set { SetProperty(ref _text13, value); }
        }
        private string _text14;
        public string Text14
        {
            get { return _text14; }
            set { SetProperty(ref _text14, value); }
        }
        private string _text15;
        public string Text15
        {
            get { return _text15; }
            set { SetProperty(ref _text15, value); }
        }
        private string _text16;
        public string Text16
        {
            get { return _text16; }
            set { SetProperty(ref _text16, value); }
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
            WordSearchHeaderSourceHtml = DependencyService.Get<IDependencyHelper>().GetLocalHtmlPath() + "wordSearchHeader.html";
            StartGameTimer();
        }    

        // load words in header and strike out completed words
        public bool LoadWordsHeader(List<Word> words)
        {
            bool bOK = true;
            try
            {
                // place words in grid layout header
                if (words.Count >= 4)
                {
                    Text1 = words[0].Text;
                    Text2 = words[1].Text;
                    Text3 = words[2].Text;
                    Text4 = words[3].Text;
                }
                if (words.Count >= 8)
                {
                    Text5 = words[4].Text;
                    Text6 = words[5].Text;
                    Text7 = words[6].Text;
                    Text8 = words[7].Text;
                }
                if (words.Count >= 16)
                {
                    Text9 = words[8].Text;
                    Text10 = words[9].Text;
                    Text11 = words[10].Text;
                    Text12 = words[11].Text;
                    Text13 = words[12].Text;
                    Text14 = words[13].Text;
                    Text15 = words[14].Text;
                    Text16 = words[15].Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadWordsHeader exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
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


