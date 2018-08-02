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
        // is initialized
        private bool HasBeenInitialized { get; set; }
        // rows and columns
        int Rows { get; set; }
        int Columns { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage()
        {
            Rows = 0;
            Columns = 0;
            HasBeenInitialized = false;
            InitializeComponent();
            Manager = new WordManager();
            BindingContext = new WordSearchPageViewModel(Navigation, 300, 20, webViewHeader);
            webViewHeader.AddLocalCallback("headerJSCallback", HeaderJSCallback);
        }

        public WordSearchPage(WordManager.GameDifficulty level)
        {
            Rows = 0;
            Columns = 0;
            HasBeenInitialized = false;            
            Manager = new WordManager();
            Manager.DifficultyLevel = level;
            int secondsRemaining = Manager.GetStartSecondsRemaining();
            int points = Manager.GetPointsPerLetter();
            BindingContext = new WordSearchPageViewModel(Navigation, secondsRemaining, points, webViewHeader);
            InitializeComponent();
            webViewHeader.AddLocalCallback("headerJSCallback", HeaderJSCallback);
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
                    ViewModel.HtmlHeaderPageHeight = 120;
                    double wordHeight = height - 120;
                    ViewModel.HtmlTilePageHeight = wordHeight;
                    bool bOK;
                    if (!HasBeenInitialized)
                    {
                        bOK = InitalizeDelegates();
                        Debug.Assert(bOK);
                        HasBeenInitialized = CalculateTiles(Width, wordHeight);
                        Debug.Assert(HasBeenInitialized);
                    }
                    bOK = ResizeTiles(width, wordHeight);
                    Debug.Assert(bOK);
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
                Manager.ListenForTileClicks();
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
                FlexTilesView.Children.Clear();
                List<TileControl> controls = new List<TileControl>();
                var viewModels = new TileControlViewModel[Rows, Columns];
                for (int column = 0; column < Columns; column++)
                {
                    for (int row = 0; row < Rows; row++)
                    {
                        // create tile view model
                        TileControlViewModel viewModel = new TileControlViewModel(Manager.TileClickedCallback);
                        viewModel.Letter = $"{TileControlViewModel.GetRandomLetter()}";
                        viewModel.TileRow = row;
                        viewModel.TileColum = column;
                        viewModel.TileWidth = tileWidth;
                        viewModel.TileHeight = tileHeight;
                        viewModels.SetValue(viewModel, row, column);
                        // create tile control passing view model to constructor
                        var control = new TileControl(viewModel);
                        controls.Add(control);
                        // add control to flex view
                        FlexTilesView.Children.Add(control);
                    }
                }
                // initialize word array
                int maxWordLength = Rows > Columns ? Rows : Columns;
                bOK = Manager.InitializeWordList(maxWordLength, viewModels);
                Debug.Assert(bOK);
                // load words in header and strike out completed words
                if (bOK)
                {
                    LoadWordsHeader();
                }
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

        // resize tile width and height to match page size
        private bool ResizeTiles(double width, double height)
        {
            bool bOK = true;
            try
            {
                Debug.Assert(Manager != null);
                if (Manager != null)
                {
                    // work out tile width and height and count based on orientation and difficulty level selected
                    bOK = CalculateResizedTileWidthHeight(width, height, out int tileWidth, out int tileHeight);
                    Debug.Assert(bOK);
                    foreach (TileControl tileView in FlexTilesView.Children)
                    {
                        tileView.ViewModel.TileWidth = tileWidth;
                        tileView.ViewModel.TileHeight = tileHeight;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"ResizeTiles exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // calculate tile size needs to maintain correct number of rows and columns in flex layout
        private bool CalculateResizedTileWidthHeight(double width, double height, out int tileWidth, out int tileHeight)
        {
            bool bOK = true;
            tileWidth = 0;
            tileHeight = 0;
            try
            {
                if (height <= 0)
                    return false;
                tileWidth = (int)(width / Rows);
                tileHeight = (int)(height / Columns);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalculateResizedTileWidthHeight exception, " + ex.Message);
                bOK = false;
            }
            return bOK;
        }

        // load words in header and highlight completed words
        public async Task<bool> LoadWordsHeader()
        {
            return await ViewModel.SignalHtmlPage("LoadWordsHeader", Manager.Words);
        }

        // delegate callback to update header text
        private async void OnWordCompletedCallbackAsync(Word word)
        {
            await LoadWordsHeader();
            ViewModel.UpdateScore(word.Text.Length);
            // signal html page with word complete
            await ViewModel.SignalHtmlPage("OnWordComplete", word);
        }

        // delegate for game completed callback
        private async void OnGameCompletedCallbackAsync()
        {
            ViewModel.GameCompleted = true;
            await DisplayAlert("Winner", "Game completed!", "OK");   
        }       

        // callback from JS html page
        void HeaderJSCallback(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Got local callback: {message}");
            MessageJson msg = new MessageJson(message);
            switch(msg.Message)
            {
                case "OnReady":
                    Debug.WriteLine("Header html DOM ready.");
                    break;
                case "LogMsg":
                    Debug.WriteLine(msg.Data.ToString());
                    break;
                case "Error":
                    Debug.WriteLine(msg.Data.ToString());
                    break;
            }
        }
    }
}

