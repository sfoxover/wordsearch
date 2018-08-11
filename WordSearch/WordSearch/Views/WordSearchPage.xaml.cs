﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WordSearch.Controls;
using WordSearch.Models;
using WordSearch.Util;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WordSearchPage : ContentPage
    {
        // word manager object
        WordManager Manager { get; set; }
        // resize counter to avoid flicker
        int PageSizedCounter { get; set; }
        // rows and columns
        int Rows { get; set; }
        int Columns { get; set; }
        private bool HasCaculatedTiles { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage()
        {
            HasCaculatedTiles = false;
            Rows = 0;
            Columns = 0;
            InitializeComponent();
            Manager = new WordManager();
            BindingContext = new WordSearchPageViewModel(Navigation, 300, 20, webViewHeader, webViewTiles);
            webViewHeader.AddLocalCallback("headerJSCallback", HeaderJSCallback);
            webViewTiles.AddLocalCallback("tilesJSCallback", TilesJSCallback);
            InitalizeDelegates();
        }

        public WordSearchPage(WordManager.GameDifficulty level)
        {
            HasCaculatedTiles = false;
            Rows = 0;
            Columns = 0;
            Manager = new WordManager();
            Manager.DifficultyLevel = level;
            int secondsRemaining = Manager.GetStartSecondsRemaining();
            int points = Manager.GetPointsPerLetter();
            InitializeComponent();
            BindingContext = new WordSearchPageViewModel(Navigation, secondsRemaining, points, webViewHeader, webViewTiles);
            webViewHeader.AddLocalCallback("headerJSCallback", HeaderJSCallback);
            webViewTiles.AddLocalCallback("tilesJSCallback", TilesJSCallback);
            InitalizeDelegates();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            // use size counter to avoid resize flicker
            PageSizedCounter++;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 50), () =>
            {
                PageSizedCounter--;
                if (PageSizedCounter == 0 && height > 0)
                {
                    ViewModel.HtmlPageWidth = width;
                    ViewModel.HtmlHeaderPageHeight = 100;
                    double wordHeight = height - 100;
                    ViewModel.HtmlTilePageHeight = wordHeight;
                    // calculate tile row and column count
                    if (!HasCaculatedTiles)
                    {
                        HasCaculatedTiles = CalculateTiles(width, wordHeight);
                    }
                }
                return false;
            });
        }

        // set up callbacks
        private bool InitalizeDelegates()
        {
            bool bOK = true;
            try
            {                
                // set up WordManager delegate events                 
                Manager.GameCompletedCallback += OnGameCompletedCallbackAsync;
                Manager.WordCompletedCallback += OnWordCompletedCallbackAsync;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("InitalizeDelegates exception, " + ex.Message);
                bOK = false;
            }
            return bOK;
        }       

        // calculate for portrait mode orientation
        private bool CalculateTiles(double width, double height)
        {
            bool bOK = true;
            try
            {
                if (height <= 0)
                    return false;
                // work out tile width and height and count based on orientation and difficulty level selected
                bOK = CalculateTileWidthHeight(width, height, out int tileWidth, out int tileHeight);
                Debug.Assert(bOK);
                // add titles on UI thread
                var viewModels = new TileControlViewModel[Rows, Columns];
                for (int column = 0; column < Columns; column++)
                {
                    for (int row = 0; row < Rows; row++)
                    {
                        // create tile view model
                        TileControlViewModel viewModel = new TileControlViewModel();
                        viewModel.Letter = $"{TileControlViewModel.GetRandomLetter()}";
                        viewModel.TileRow = row;
                        viewModel.TileColumn = column;
                        viewModels.SetValue(viewModel, row, column);
                    }
                }
                // initialize word array
                int maxWordLength = Rows > Columns ? Rows : Columns;
                bOK = Manager.InitializeWordList(maxWordLength, viewModels);
                Debug.Assert(bOK);
                // send tiles to html page
                ViewModel.SignalTilesHtmlPage("LoadTiles", Manager.TileViewModels);
                LoadWordsHeader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalculateTiles exception, " + ex.Message);
                bOK = false;
            }
            return bOK;
        }

        // work out tile width and height and count based on orientation and difficulty level selected
        private bool CalculateTileWidthHeight(double width, double height, out int tileWidth, out int tileHeight)
        {
            bool bOK = true;
            tileWidth = 0;
            tileHeight = 0;
            try
            {
                if (height <= 0)
                    return false;
                int tiles = Manager.GetMinRequiredTiles();
                tileWidth = (int)(width / tiles);
                tileHeight = (int)(height / tiles);
                // use min of height or width to ensure min tiles required
                tileWidth = tileHeight = Math.Min(tileWidth, tileHeight);
                Rows = (int)(width / tileWidth);
                Columns = (int)(height / tileHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalculateTileWidthHeight exception, " + ex.Message);
                bOK = false;
            }
            return bOK;
        }

        // load words in header and highlight completed words
        public void LoadWordsHeader()
        {
            if(Manager.Words != null && Manager.Words.Count > 0)
                ViewModel.SignalHeaderHtmlPage("LoadWordsHeader", Manager.Words);
        }

        // delegate callback to update header text
        private async void OnWordCompletedCallbackAsync(Word word)
        {
            ViewModel.UpdateScore(word.Text.Length);
            // signal html page with word complete
            var textPos = Manager.GetTextPos(word);            
            await ViewModel.SignalHeaderHtmlPage("OnWordComplete", textPos);
        }

        // delegate for game completed callback
        private async void OnGameCompletedCallbackAsync()
        {
            ViewModel.GameCompleted = true;
            await ViewModel.SignalTilesHtmlPage("OnGameCompleted", null);
        }       

        // callback from JS header html page
        void HeaderJSCallback(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Got local callback: {message}");
            MessageJson msg = new MessageJson(message);
            switch(msg.Message)
            {
                case "LogMsg":
                    Debug.WriteLine(msg.Data.ToString());
                    break;
                case "Error":
                    if (msg.Data != null)
                    {
                        Debug.WriteLine(msg.Data.ToString());
                    }
                    break;
                case "LoadWordsHeader":
                    LoadWordsHeader();
                    break;
                default:
                    Debug.Assert(false, $"HeaderJSCallback unexpected message {message}");
                    break;
            }
        }

        // callback from JS body html page
        void TilesJSCallback(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Got local callback: {message}");
            MessageJson msg = new MessageJson(message);
            switch (msg.Message)
            {
                case "tileClick":
                    if (!ViewModel.GameCompleted)
                    {
                        Manager.CheckForSelectedWord(msg.Data as JObject);
                        // reload tiles
                        ViewModel.SignalTilesHtmlPage("UpdateTileSelectedSates", Manager.TileViewModels);
                    }
                    break;
                case "LogMsg":
                    Debug.WriteLine(msg.Data.ToString());
                    break;
                case "Error":
                    if (msg.Data != null)
                    {
                        Debug.WriteLine(msg.Data.ToString());
                    }
                    break;             
                default:
                    Debug.Assert(false, $"TilesJSCallback unexpected message {message}");
                    break;
            }
        }
    }
}

