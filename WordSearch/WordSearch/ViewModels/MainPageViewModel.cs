using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WordSearch.Util;
using WordSearch.Views;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
	public class MainPageViewModel : BindableBase
    {
        // main logo image path
        private string _logoPath;
        public string LogoPath
        {
            get { return _logoPath; }
            set { SetProperty(ref _logoPath, value); }
        }
        // new easy game button image
        private string _easyGameImgPath;
        public string EasyGameImgPath
        {
            get { return _easyGameImgPath; }
            set { SetProperty(ref _easyGameImgPath, value); }
        }
        // new medium game button image
        private string _mediumGameImgPath;
        public string MediumGameImgPath
        {
            get { return _mediumGameImgPath; }
            set { SetProperty(ref _mediumGameImgPath, value); }
        }
        // new hard game button image
        private string _hardGameImgPath;
        public string HardGameImgPath
        {
            get { return _hardGameImgPath; }
            set { SetProperty(ref _hardGameImgPath, value); }
        }
        // high scores button image
        private string _highScoresImgPath;
        public string HighScoresImgPath
        {
            get { return _highScoresImgPath; }
            set { SetProperty(ref _highScoresImgPath, value); }
        }

        // new game command click
        public Command NewEasyGameCommand { get; }
        public Command NewMediumGameCommand { get; }
        public Command NewHardGameCommand { get; }
        public Command ShowHighScoresCommand { get; }

        public MainPageViewModel(INavigation navigation)
             : base(navigation)
        {
            NewEasyGameCommand = new Command(OnNewEasyGameClick);
            NewMediumGameCommand = new Command(OnNewMediumGameClick);
            NewHardGameCommand = new Command(OnNewHardGameClick);
            ShowHighScoresCommand = new Command(OnShowHighScoresClick);
            // main logo image
            string basePath = DependencyService.Get<IDependencyHelper>().GetResourceImagesPath();
            LogoPath = basePath + "mainlogo.png";
            // new game image button paths
            EasyGameImgPath = basePath + "baby.png";
            MediumGameImgPath = basePath + "manwalk.png";
            HardGameImgPath = basePath + "mansuitcasefast.png";
            HighScoresImgPath = basePath + "leaderboard.png";
        }

        public async void OnNewEasyGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.easy));
        }

        public async void OnNewMediumGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.medium));
        }

        public async void OnNewHardGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.hard));
        }

        public async void OnShowHighScoresClick()
        {
            await Navigation.PushAsync(new HighScoresPage());
        }
    }
}
