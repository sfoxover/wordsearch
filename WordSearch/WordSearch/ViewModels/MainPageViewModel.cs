using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WordSearch.Util;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
	public class MainPageViewModel : BindableBase
    {
        private INavigation Navigation { get; set; }
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

        // new game command click
        public Command NewEasyGameCommand { get; }
        public Command NewMediumGameCommand { get; }
        public Command NewHardGameCommand { get; }

        public MainPageViewModel(INavigation value)
        {
            Navigation = value;
            LogoPath = "html/images/mainlogo.png";
            NewEasyGameCommand = new Command(OnNewEasyGameClick);
            NewMediumGameCommand = new Command(OnNewMediumGameClick);
            NewHardGameCommand = new Command(OnNewHardGameClick);
            // new game image button paths
            string basePath = DependencyService.Get<IDependencyHelper>().GetLocalHtmlPath();
            EasyGameImgPath = basePath + "html/images/appbar.baby.png";
            MediumGameImgPath = basePath + "html/images/appbar.man.walk.png";
            HardGameImgPath = basePath + "html/images/appbar.man.suitcase.fast.png";
        }

        public async void OnNewEasyGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(WordManager.GameDifficulty.easy));
        }

        public async void OnNewMediumGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(WordManager.GameDifficulty.medium));
        }

        public async void OnNewHardGameClick()
        {
            await Navigation.PushAsync(new WordSearchPage(WordManager.GameDifficulty.hard));
        }
    }
}
