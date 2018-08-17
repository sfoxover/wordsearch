using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public class DesignMainPageViewModel : BindableBase
    {
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

        public DesignMainPageViewModel()
        {
            LogoPath = "html/images/mainlogo.png";
            NewEasyGameCommand = new Command(OnNewEasyGameClick);
            NewMediumGameCommand = new Command(OnNewMediumGameClick);
            NewHardGameCommand = new Command(OnNewHardGameClick);
        }

        public async void OnNewEasyGameClick()
        {
        }

        public async void OnNewMediumGameClick()
        {
        }

        public async void OnNewHardGameClick()
        {
        }
    }
}
