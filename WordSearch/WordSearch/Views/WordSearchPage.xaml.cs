using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage ()
		{
            Manager = null;
            HasBeenInitialized = false;            
            InitializeComponent();
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
                    bool bOK;
                    double wordHeight = Height - FlexWordsList.Height;
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
                Manager = new WordManager();
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
                // work out width and height based on page size and rows for difficulty level selected
                int rows = Manager.GetTileRows();
                int tileWidth = (int)(width / rows);
                int tileHeight = (int)(height / rows);
                // use min of height or width to ensure min tiles required
                tileWidth = tileHeight = Math.Min(tileWidth, tileHeight);
                int columns = (int)(height / tileHeight);
                Debug.WriteLine($"CalculateTiles starting for {rows} x {columns}");
                // add titles on UI thread
                FlexTilesView.Children.Clear();
                List<TileControl> controls = new List<TileControl>();
                var viewModels = new TileControlViewModel[rows, columns];
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        // create tile view model
                        TileControlViewModel viewModel = new TileControlViewModel(Manager.TileClickedCallback);
                        viewModel.Letter = $"{TileControlViewModel.GetRandomLetter()}";
                        viewModel.TileRow = row;
                        viewModel.TileColum = column;
                        viewModel.TileWidth = tileWidth - 2;
                        viewModel.TileHeight = tileHeight - 2;
                        viewModels.SetValue(viewModel, row, column);
                        // create tile control passing view model to constructor
                        var control = new TileControl(viewModel);
                        controls.Add(control);
                        // add control to flex view
                        FlexTilesView.Children.Add(control);
                    }
                }
                // initialize word array
                int maxWordLength = rows > columns ? rows : columns;
                bOK = Manager.InitializeWordList(maxWordLength, viewModels);
                Debug.Assert(bOK);
                // load words in header and strike out completed words
                if (bOK)
                {
                    bOK = LoadWordsHeader();
                    Debug.Assert(bOK);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalculateTiles exception, " + ex.Message);
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
                    int rows = Manager.GetTileRows();
                    int tileWidth = (int)(width / rows);
                    int tileHeight = tileWidth;
                    foreach (TileControl tileView in FlexTilesView.Children)
                    {
                        tileView.ViewModel.TileWidth = tileWidth - 2;
                        tileView.ViewModel.TileHeight = tileHeight - 2;
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

        // load words in header and highlight completed words
        public bool LoadWordsHeader()
        {
            bool bOK = true;
            try
            {
                // call view model to bind text
                ViewModel.LoadWordsHeader(Manager.Words);
                // dynamically update style
                for (int n=0; n<Manager.Words.Count; n++)
                {
                    var word = Manager.Words[n];
                    var label = FlexWordsList.Children[n] as Label;
                    if(word.IsWordCompleted)
                    {
                        label.BackgroundColor = Color.Yellow;
                        label.TextColor = Color.Red;
                    }
                    else
                    {
                        label.BackgroundColor = Color.LightGray;
                        label.TextColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadWordsHeader exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        // delegate callback to update header text
        private async void OnWordCompletedCallbackAsync(Word word)
        {
            LoadWordsHeader();
            await DisplayAlert("Great job", $"You found the word {word.Text}!", "OK");
        }

        // delegate for game completed callback
        private async void OnGameCompletedCallbackAsync()
        {
            await DisplayAlert("Winner", "Game completed!", "OK");   
        }
    }
}

