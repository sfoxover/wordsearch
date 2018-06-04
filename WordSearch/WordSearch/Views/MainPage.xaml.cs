﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WordSearch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnNewGame_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WordSearchPage());
        }
    }
}