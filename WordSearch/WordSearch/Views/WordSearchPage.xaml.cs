﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WordSearch.Models;
using WordSearch.Helpers;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

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
        // Flag to only start timer 1 time
        private bool HasStartedHardModeTimer { get; set; }
        // Screen width and height
        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }
        public double TilesScreenHeight { get; set; }
        // Sound on or off
        public bool SoundSettingIsOn { get; private set; }
        // Has warned of score penalty
        public bool ScorePenaltyWarned { get; private set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage(Defines.GameDifficulty level, double width, double height)
        {
            InitializeComponent();
            Rows = 0;
            Columns = 0;
            InitalizeHeaderAndTiles(level, width, height);
        }

        // Initialize tiles
        private bool InitalizeHeaderAndTiles(Defines.GameDifficulty level, double width, double height)
        {
            bool bOK = true;
            try
            {
                // Check if sound is on
                SoundSettingIsOn = Preferences.Get("soundOn", true);
                ScorePenaltyWarned = false;
                // Create word manager
                Manager = new WordManager();
                Manager.DifficultyLevel = level;
                // set up WordManager delegate events                 
                Manager.GameCompletedCallback += OnGameCompletedCallbackAsync;
                Manager.WordCompletedCallback += OnWordCompletedCallbackAsync;
                // Databinding
                int secondsRemaining = Manager.GetStartSecondsRemaining();
                int points = Manager.GetPointsPerLetter();
                BindingContext = new WordSearchPageViewModel(Navigation, secondsRemaining, points, webViewHeader, webViewTiles);
                // Html callbacks
                webViewHeader.RegisterAction(HeaderJSCallback);
                webViewTiles.RegisterAction(TilesJSCallback);
                // Set sizes
                ScreenWidth = width;
                ScreenHeight = height;
                TilesScreenHeight = height - Defines.HEADER_HTML_HEIGHT;
                ViewModel.HtmlPageWidth = width;
                ViewModel.HtmlHeaderPageHeight = 100;
                double wordHeight = height - 100;
                ViewModel.HtmlTilePageHeight = wordHeight;
                Task.Run(() =>
                {
                    CalculateTiles();
                    LoadWordsTiles();
                    LoadWordsHeader();
                });
                // xamarin essentials screen lock
                if (!ScreenLock.IsActive)
                    ScreenLock.RequestActive();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"InitalizeHeaderAndTiles exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }       

        // calculate for portrait mode orientation
        private bool CalculateTiles()
        {
            bool bOK = true;
            try
            {
                if (TilesScreenHeight <= 0)
                    return false;
                // work out tile width and height and count based on orientation and difficulty level selected
                bOK = CalculateTileWidthHeight(out int tileWidth, out int tileHeight);
                Debug.Assert(bOK);
                Rows = (int)(ScreenWidth / tileWidth);
                Columns = (int)(TilesScreenHeight / tileHeight);
                // add titles on UI thread
                var viewModels = new Tile[Rows, Columns];
                for (int column = 0; column < Columns; column++)
                {
                    for (int row = 0; row < Rows; row++)
                    {
                        // create tile view model
                        Tile viewModel = new Tile();
                        viewModel.Letter = $"{Tile.GetRandomLetter()}";
                        viewModel.TileRow = row;
                        viewModel.TileColumn = column;
                        viewModels.SetValue(viewModel, row, column);
                    }
                }
                // initialize word array
                int maxWordLength = Rows > Columns ? Rows : Columns;
                bOK = Manager.InitializeWordList(maxWordLength, viewModels);
                Debug.Assert(bOK);              
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"CalculateTiles exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // work out tile width and height and count based on orientation and difficulty level selected
        private bool CalculateTileWidthHeight(out int tileWidth, out int tileHeight)
        {
            bool bOK = true;
            tileWidth = 0;
            tileHeight = 0;
            try
            {
                if (TilesScreenHeight <= 0)
                    return false;
                int tiles = Manager.GetMinRequiredTiles();
                tileWidth = (int)(ScreenWidth / tiles);
                tileHeight = (int)(TilesScreenHeight / tiles);
                // use min of height or width to ensure min tiles required
                tileWidth = tileHeight = Math.Min(tileWidth, tileHeight);                
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"CalculateTileWidthHeight exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // Send tiles array to html page
        public void LoadWordsTiles()
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ViewModel.SignalTilesHtmlPage("LoadTiles", Manager.TileViewModels);
                });
                // speak completed word
                if (SoundSettingIsOn && Manager.DifficultyLevel == Defines.GameDifficulty.hard)
                {
                    bool hasBeenPlayed = Preferences.Get("playedHardWarning", false);
                    if (!hasBeenPlayed)
                    {
                        Preferences.Set("playedHardWarning", true);
                        TextToSpeech.SpeakAsync("Warning. On hard level the words you need to find are hidden. You will see 1 word to find every 5 seconds.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"LoadWordsTiles exception, {ex.Message}");
            }
        }

        // load words in header and highlight completed words
        public void LoadWordsHeader()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Randomly hide words in hard level
                if (Manager.DifficultyLevel == Defines.GameDifficulty.hard && !HasStartedHardModeTimer)
                {
                    HasStartedHardModeTimer = true;
                    LoadHiddenHardModeHeader();
                }
                Manager.GetWordsList(out List<Word> words);
                if (words != null && words.Count > 0)
                    ViewModel.SignalHeaderHtmlPage("LoadWordsHeader", words);
            });
        }

        // Selected random visable word for hard mode
        public void LoadHiddenHardModeHeader()
        {
            // Randomly hide words in hard level
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (!ViewModel.GameCompleted && ViewModel.SecondsRemaining > 0)
                {
                    Manager.HideHardLevelWords();                   
                    LoadHiddenHardModeHeader();
                    Debug.WriteLine($"LoadHiddenHardModeHeader() {DateTime.Now.ToString()}");
                }
                else
                {
                    Manager.ShowAllHardLevelWords();
                }
                // Refresh words header
                Manager.GetWordsList(out List<Word> words);
                if (words != null && words.Count > 0)
                    ViewModel.SignalHeaderHtmlPage("LoadWordsHeader", words);
                return false;
            });
        }

        // delegate callback to update header text
        private async void OnWordCompletedCallbackAsync(Word word)
        {
            ViewModel.UpdateScore(word.Text.Length);
            // signal html page with word complete
            var textPos = Manager.GetTextPos(word);            
            ViewModel.SignalHeaderHtmlPage("OnWordComplete", textPos);
            // speak completed word
            if(SoundSettingIsOn)
                await TextToSpeech.SpeakAsync("You found " + word.Text);
        }

        // delegate for game completed callback
        private async void OnGameCompletedCallbackAsync()
        {
            ViewModel.GameCompleted = true;
            // Game over if less than or equal to zero points
            if (ViewModel.GameScore <= 0)
            {
                ViewModel.SecondsRemaining = 0;
                if (SoundSettingIsOn)
                {
                    await TextToSpeech.SpeakAsync("Sorry but you did not score any points. Please try again.");
                }
                return;
            }
           
            // get ranking
            bool bOK = Score.GetScoreRank(ViewModel.GameScore, out int ranking);
            var rank = new { Score = ViewModel.GameScore, Rank = ranking };
            ViewModel.SignalTilesHtmlPage("OnGameCompleted", rank);
            ViewModel.SignalHeaderHtmlPage("OnGameCompleted", rank);
            // speak completed word
            if (SoundSettingIsOn)
            {
                switch (Manager.DifficultyLevel)
                {
                    case Defines.GameDifficulty.easy:
                    case Defines.GameDifficulty.medium:
                        await TextToSpeech.SpeakAsync("Excellent job. You are a winner!");
                        break;
                    case Defines.GameDifficulty.hard:
                        await TextToSpeech.SpeakAsync("Wow. You are a legand!");
                        break;
                }
            }
        }       

        // callback from JS header html page
        void HeaderJSCallback(string message)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ViewModel.HasHeaderPageSignalled = true;
                    MessageJson msg = new MessageJson(message);
                    switch (msg.Message)
                    {
                        case "LogMsg":
                            Debug.WriteLine(msg.Data.ToString());
                            break;
                        case "Error":
                            if (msg.Data != null)
                            {
                                Logger.Instance.Error(msg.Data.ToString());
                            }
                            break;
                        case "LoadWordsHeader":
                            LoadWordsHeader();
                            break;
                        default:
                            Debug.Assert(false, $"HeaderJSCallback unexpected message {message}");
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HeaderJSCallback exception, {ex.Message}");
            }
        }

        // callback from JS body html page
        void TilesJSCallback(string message)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ViewModel.HasTilesPageSignalled = true;
                    MessageJson msg = new MessageJson(message);
                    switch (msg.Message)
                    {
                        case "ping":
                            break;
                        case "tilePageReady":
                            ViewModel.IsLoading = false;
                            FlexScoreHeader.BackgroundColor = Color.LightBlue;
                            break;
                        case "tileClick":
                            if (!ViewModel.GameCompleted && ViewModel.SecondsRemaining > 0)
                            {
                                if(Manager.CheckForSelectedWord(msg.Data as JObject, out bool isPartOfWord))
                                {
                                    if(!isPartOfWord && Manager.DifficultyLevel == Defines.GameDifficulty.hard)
                                    {
                                        // Remove score on missed tile hit
                                        ViewModel.SubtractPenaltyScore();
                                        if (SoundSettingIsOn && !ScorePenaltyWarned)
                                        {
                                            ScorePenaltyWarned = true;
                                            TextToSpeech.SpeakAsync($"You lost {Defines.PENALTY_POINTS} points. Only click on tiles that are part of a word.");
                                        }
                                    }
                                }
                                // reload tiles
                                ViewModel.SignalTilesHtmlPage("UpdateTileSelectedSates", Manager.TileViewModels);
                            }
                            break;
                        case "hightscoreName":
                            SaveHighScore(msg.Data.ToString());
                            break;
                        case "LogMsg":
                            Debug.WriteLine(msg.Data.ToString());
                            break;
                        case "Error":
                            if (msg.Data != null)
                            {
                                Logger.Instance.Error(msg.Data.ToString());
                            }
                            break;
                        default:
                            Debug.Assert(false, $"TilesJSCallback unexpected message {message}");
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"TilesJSCallback exception, {ex.Message}");
            }
        }

        // save user score
        private async void SaveHighScore(string name)
        {
            try
            {
                var score = new Score(name, Manager, ViewModel);
                bool bOK = await score.SaveRecord();
                Debug.Assert(bOK);
                // back to main page
                ViewModel.CloseWindow();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"SaveHighScore exception, {ex.Message}");
            }
        }
    }
}

