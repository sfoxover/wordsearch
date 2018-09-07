using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WordSearch.Helpers;
using WordSearch.Views;
using Xamarin.Essentials;
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
        // Sound on or off
        private string _soundOnImgPath;
        public string SoundOnImgPath
        {
            get { return _soundOnImgPath; }
            set { SetProperty(ref _soundOnImgPath, value); }
        }
        // Text for sound on or off
        private string _soundOnText;
        public string SoundOnText
        {
            get { return _soundOnText; }
            set { SetProperty(ref _soundOnText, value); }
        }
        // Sound is on or off
        private bool SoundSettingIsOn = true;
        // Screen width and height
        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }

        // new game command click
        public Command NewEasyGameCommand { get; }
        public Command NewMediumGameCommand { get; }
        public Command NewHardGameCommand { get; }
        public Command ShowHighScoresCommand { get; }
        public Command SoundOnCommand { get; }

        public MainPageViewModel(INavigation navigation)
             : base(navigation)
        {
            NewEasyGameCommand = new Command(OnNewEasyGameClick);
            NewMediumGameCommand = new Command(OnNewMediumGameClick);
            NewHardGameCommand = new Command(OnNewHardGameClick);
            ShowHighScoresCommand = new Command(OnShowHighScoresClick);
            SoundOnCommand = new Command(SoundOnClick);
            // main logo image
            string basePath = DependencyService.Get<IDependencyHelper>().GetResourceImagesPath();
            LogoPath = basePath + "mainlogo.png";
            // new game image button paths
            EasyGameImgPath = basePath + "baby.png";
            MediumGameImgPath = basePath + "manwalk.png";
            HardGameImgPath = basePath + "mansuitcasefast.png";
            HighScoresImgPath = basePath + "leaderboard.png";
            ScreenWidth = 0;
            ScreenHeight = 0;
            SoundSettingIsOn = Preferences.Get("soundOn", true);
            SetSoundIcon(basePath);
        }

        // Set sound icon and text
        private void SetSoundIcon(string basePath)
        {
            if (SoundSettingIsOn)
            {
                SoundOnText = "Sound is on";
                SoundOnImgPath = basePath + "ic_volume_up_black_48dp.png";
            }
            else
            {
                SoundOnText = "Sound is off";
                SoundOnImgPath = basePath + "ic_volume_off_black_48dp.png";
            }
        }

        public async void OnNewEasyGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.easy, ScreenWidth, ScreenHeight));
        }

        public async void OnNewMediumGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.medium, ScreenWidth, ScreenHeight));
        }

        public async void OnNewHardGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(Defines.GameDifficulty.hard, ScreenWidth, ScreenHeight));
        }

        public async void OnShowHighScoresClick()
        {
            await Navigation.PushAsync(new HighScoresPage(ScreenWidth, ScreenHeight));
        }

        // Toggle sound on or off
        public void SoundOnClick()
        {
            SoundSettingIsOn = !SoundSettingIsOn;
            Preferences.Set("soundOn", SoundSettingIsOn);
            string basePath = DependencyService.Get<IDependencyHelper>().GetResourceImagesPath();
            SetSoundIcon(basePath);
        }
    }
}
