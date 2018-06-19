using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Controls;
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
        // Prism events interface
        IEventAggregator EventAggregator { get; set; }
        // resize counter to avoid flicker
        int PageSizedCounter { get; set; }

        // get access to ViewModel
        private WordSearchPageViewModel ViewModel
        {
            get { return BindingContext as WordSearchPageViewModel; }
        }

        public WordSearchPage (IEventAggregator eventAggregator)
		{
            Manager = null;
            EventAggregator = eventAggregator;
            InitializeComponent();
            Appearing += WordSearchPage_Appearing;
        }

        private void WordSearchPage_Appearing(object sender, EventArgs e)
        {
            double height = Height - FlexWordsList.Height;
            bool bOK = CalculateTiles(Width, height);
            Debug.Assert(bOK);
            bOK = ResizeTiles(Width, height);
            Debug.Assert(bOK);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); // must be called
            // use size counter to avoid resize flicker
            PageSizedCounter++;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 50), () =>
            {
                PageSizedCounter--;
                if (PageSizedCounter == 0)
                {
                    bool bOK = ResizeTiles(Width, Height - FlexWordsList.Height);
                    Debug.Assert(bOK);
                }
                return false;
            });
        }

        // calculate for portrait mode orientation
        private bool CalculateTiles(double width, double height)
        {
            bool bOK = true;
            try
            {
                Manager = new WordManager();
                Manager.ListenForTileClicks(EventAggregator);
                // work out width and height based on page size and rows for difficulty level selected
                int rows = Manager.GetTileRows();
                int tileWidth = (int)(width / rows);
                int tileHeight = tileWidth;
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
                        TileControlViewModel viewModel = new TileControlViewModel(EventAggregator);
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
                // place words in top of page
                foreach(var word in Manager.Words)
                {
                    Label label = new Label();
                    label.Text = word.Text;
                    FlexWordsList.Children.Add(label);
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
    }
}