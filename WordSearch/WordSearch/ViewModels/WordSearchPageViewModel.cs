using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordSearch.Models;

namespace WordSearch.ViewModels
{
    public class WordSearchPageViewModel : BindableBase, INavigationAware
    {
        // prism NavigationService
        protected INavigationService NavigationService { get; private set; }

        // header text labels
        private string _text1;
        public string Text1
        {
            get { return _text1; }
            set { SetProperty(ref _text1, value); }
        }
        private string _text2;
        public string Text2
        {
            get { return _text2; }
            set { SetProperty(ref _text2, value); }
        }
        private string _text3;
        public string Text3
        {
            get { return _text3; }
            set { SetProperty(ref _text3, value); }
        }
        private string _text4;
        public string Text4
        {
            get { return _text4; }
            set { SetProperty(ref _text4, value); }
        }
        private string _text5;
        public string Text5
        {
            get { return _text5; }
            set { SetProperty(ref _text5, value); }
        }
        private string _text6;
        public string Text6
        {
            get { return _text6; }
            set { SetProperty(ref _text6, value); }
        }
        private string _text7;
        public string Text7
        {
            get { return _text7; }
            set { SetProperty(ref _text7, value); }
        }
        private string _text8;
        public string Text8
        {
            get { return _text8; }
            set { SetProperty(ref _text8, value); }
        }

        public WordSearchPageViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        // load words in header and strike out completed words
        public bool LoadWordsHeader(List<Word> words)
        {
            bool bOK = true;
            try
            {
                // place words in grid layout header
                if (words.Count >= 4)
                {
                    Text1 = words[0].Text;
                    Text2 = words[1].Text;
                    Text3 = words[2].Text;
                    Text4 = words[3].Text;
                }
                if (words.Count >= 8)
                {
                    Text5 = words[4].Text;
                    Text6 = words[5].Text;
                    Text7 = words[6].Text;
                    Text8 = words[7].Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadWordsHeader exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}


