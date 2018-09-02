﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordSearch.Models;
using WordSearch.Helpers;
using WordSearch.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordSearch.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogFilesPage : ContentPage
    {
        // get access to ViewModel
        private LogFilesPageViewModel ViewModel
        {
            get { return BindingContext as LogFilesPageViewModel; }
        }

        public LogFilesPage(double width, double height)
        {
            InitializeComponent();
            BindingContext = new LogFilesPageViewModel(Navigation, webViewLogs);
            ViewModel.HtmlPageWidth = width;
            ViewModel.HtmlPageHeight = height;
            webViewLogs.AddLocalCallback("logsJSCallback", LogsJSCallback);
            ViewModel.LoadData();
        }

        // callback from JS body html page
        void LogsJSCallback(string message)
        {
            ViewModel.HasHtmlPageSignalled = true;
            System.Diagnostics.Debug.WriteLine($"Got local callback: {message}");
            MessageJson msg = new MessageJson(message);
            switch (msg.Message)
            {
                case "ping":
                    break;
                case "closeWindow":
                    // back to main page
                    ViewModel.CloseWindow();
                    break;
                case "clearLogs":
                    ViewModel.ClearLogs();
                    break;
                case "Error":
                    if (msg.Data != null)
                    {
                        Logger.Instance.Error(msg.Data.ToString());
                    }
                    break;
                default:
                    Debug.Assert(false, $"logsJSCallback unexpected message {message}");
                    break;
            }
        }
    }
}