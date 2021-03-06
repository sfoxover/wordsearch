﻿using Serilog;
using System;
using WordSearch.Helpers;
using WordSearch.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel
        {
            get { return BindingContext as MainPageViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(Navigation);
            Initialize();
        }

        public void Initialize()
        {
            try
            {
                // Verify words db exists
                DependencyService.Get<IDependencyHelper>().CheckWordsDBFileExists("words.db3");

                // xamarin essentials screen lock off
                if (ScreenLock.IsActive)
                    ScreenLock.RequestRelease();

                // Test window scrolling
                //for (int n = 0; n < 300; n++)
                //  Logger.Instance.Error($"Error test {n}");


            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Initialize exception, {ex.Message}");
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            try
            {
                base.OnSizeAllocated(width, height); // must be called
                ViewModel.ScreenWidth = width;
                ViewModel.ScreenHeight = height;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"OnSizeAllocated exception, {ex.Message}");
            }
        }
    }
}
