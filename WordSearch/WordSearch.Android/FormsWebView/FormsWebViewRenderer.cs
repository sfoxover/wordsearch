﻿using Android.Content;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xam.Plugin.WebView.Abstractions;
using Xam.Plugin.WebView.Abstractions.Enumerations;
using Xam.Plugin.WebView.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FormsWebView), typeof(FormsWebViewRenderer))]
namespace Xam.Plugin.WebView.Droid
{
    public class FormsWebViewRenderer : ViewRenderer<FormsWebView, Android.Webkit.WebView>
    {

        public static string MimeType = "text/html";

        public static string EncodingType = "UTF-8";

        public static string HistoryUri = "";

        public static string BaseUrl { get; set; } = "file:///android_asset/";

        public static bool IgnoreSSLGlobally { get; set; }

        public static event EventHandler<Android.Webkit.WebView> OnControlChanged;

        JavascriptValueCallback _callback;

        private Context LocalContext { get; set; }

        public FormsWebViewRenderer(Context context) 
            : base(context)
        {
            LocalContext = context;
        }

        public static void Initialize()
        {
            var dt = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FormsWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && Element != null)
                SetupControl();

            if (e.NewElement != null)
                SetupElement(e.NewElement);

            if (e.OldElement != null)
                DestroyElement(e.OldElement);
        }

        void SetupElement(FormsWebView element)
        {
            element.PropertyChanged += OnPropertyChanged;
            element.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
            element.OnBackRequested += OnBackRequested;
            element.OnForwardRequested += OnForwardRequested;
            element.OnRefreshRequested += OnRefreshRequested;

            SetSource();
        }

        void DestroyElement(FormsWebView element)
        {
            element.PropertyChanged -= OnPropertyChanged;
            element.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
            element.OnBackRequested -= OnBackRequested;
            element.OnForwardRequested -= OnForwardRequested;
            element.OnRefreshRequested -= OnRefreshRequested;

            element.Dispose();
        }

        void SetupControl()
        {
            var webView = new Android.Webkit.WebView(LocalContext);
            _callback = new JavascriptValueCallback(this);

            // https://github.com/SKLn-Rad/Xam.Plugin.WebView.Webview/issues/11
            webView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            // Defaults
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.AddJavascriptInterface(new FormsWebViewBridge(this), "bridge");
            webView.SetWebViewClient(new FormsWebViewClient(this));
            webView.SetWebChromeClient(new FormsWebViewChromeClient(this));
            webView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            FormsWebView.CallbackAdded += OnCallbackAdded;

            SetNativeControl(webView);
            OnControlChanged?.Invoke(this, webView);
        }

        async void OnCallbackAdded(object sender, string e)
        {
            if (Element == null || string.IsNullOrWhiteSpace(e)) return;

            if ((sender == null && Element.EnableGlobalCallbacks) || sender != null)
                await OnJavascriptInjectionRequest(FormsWebView.GenerateFunctionScript(e));
        }

        void OnForwardRequested(object sender, EventArgs e)
        {
            if (Control == null) return;

            if (Control.CanGoForward())
                Control.GoForward();
        }

        void OnBackRequested(object sender, EventArgs e)
        {
            if (Control == null) return;

            if (Control.CanGoBack())
                Control.GoBack();
        }

        void OnRefreshRequested(object sender, EventArgs e)
        {
            if (Control == null) return;

            Control.LoadUrl("about:blank");
            SetSource();
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Source":
                    SetSource();
                    break;
            }
        }

        internal async Task<string> OnJavascriptInjectionRequest(string js)
        {
            // fire!
            _callback.Reset();
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Element != null && Control != null)
                {
                    Control.EvaluateJavascript(js, _callback);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"OnJavascriptInjectionRequest null value for webview with command, {js}");
                }
            });
            return "";
        }

        internal void SetSource()
        {
            if (Element == null || Control == null || string.IsNullOrWhiteSpace(Element.Source)) return;

            switch (Element.ContentType)
            {
                case WebViewContentType.Internet:
                    LoadFromInternet();
                    break;

                case WebViewContentType.LocalFile:
                    LoadFromFile();
                    break;

                case WebViewContentType.StringData:
                    LoadFromString();
                    break;
            }
        }

        void LoadFromString()
        {
            if (Element == null || Control == null || Element.Source == null) return;

            // Check cancellation
            var handler = Element.HandleNavigationStartRequest(Element.Source);
            if (handler.Cancel) return;

            // Load
            Control.LoadDataWithBaseURL(Element.BaseUrl ?? BaseUrl, Element.Source, MimeType, EncodingType, HistoryUri);
        }

        void LoadFromFile()
        {
            if (Element == null || Control == null || Element.Source == null) return;

            Control.LoadUrl(Path.Combine(Element.BaseUrl ?? BaseUrl, Element.Source));
        }

        void LoadFromInternet()
        {
            if (Element == null || Control == null || Element.Source == null) return;

            var headers = new Dictionary<string, string>();

            // Add Local Headers
            foreach (var header in Element.LocalRegisteredHeaders)
            {
                if (!headers.ContainsKey(header.Key))
                    headers.Add(header.Key, header.Value);
            }

            // Add Global Headers
            if (Element.EnableGlobalHeaders)
            {
                foreach (var header in FormsWebView.GlobalRegisteredHeaders)
                {
                    if (!headers.ContainsKey(header.Key))
                        headers.Add(header.Key, header.Value);
                }
            }
            
            Control.LoadUrl(Element.Source, headers);
        }
    }
}