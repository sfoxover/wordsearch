﻿using System;
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

        // new game command click
        public Command NewEasyGameCommand { get; }
        public Command NewMediumGameCommand { get; }
        public Command NewHardGameCommand { get; }

        public MainPageViewModel(INavigation value)
        {
            Navigation = value;
            LogoPath = "html/mainlogo.png";
            NewEasyGameCommand = new Command(OnNewEasyGameClick);
            NewMediumGameCommand = new Command(OnNewMediumGameClick);
            NewHardGameCommand = new Command(OnNewHardGameClick);
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
