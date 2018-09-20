using System;
using System.Collections.Generic;
using Android.Content;
using WordSearch.Droid.Helpers;
using WordSearch.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WordSearch.Helpers;
using Xamarin.Essentials;
using Android.Webkit;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(WordSearch.Droid.Views.HybridWebViewRenderer))]
namespace WordSearch.Droid.Views
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        Context _context;
        bool IsScriptReady = false;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (Control == null && Element != null)
                {
                    var webView = new Android.Webkit.WebView(_context);
                    webView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.Settings.DomStorageEnabled = true;
                    webView.SetWebViewClient(new HybridWebViewClient(this));
                    webView.SetWebChromeClient(new WebChromeClient());
                    webView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    SetNativeControl(webView);
                }
                if (Control != null)
                {
                    if (e.OldElement != null)
                    {
                        Control.RemoveJavascriptInterface("jsBridge");
                        Element.Scripts.CollectionChanged -= Scripts_CollectionChanged;
                        var hybridWebView = e.OldElement as HybridWebView;
                        hybridWebView.Cleanup();
                    }
                    if (e.NewElement != null)
                    {
                        Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                        var url = $"file:///android_asset/html/{Element.Uri}";
                        Control.LoadUrl(url);                      
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebViewRenderer::OnElementChanged exception, {ex.Message}");
            }
        }

        public void OnPageFinished()
        {
            try
            { 
                // Inject JS script
                Element.GetScripts(out List<string> results);
                foreach (var script in results)
                {
                    InjectJS(script);
                }
                Element.ClearScripts(results);
                Element.Scripts.CollectionChanged += Scripts_CollectionChanged;
                IsScriptReady = true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebViewRenderer::OnPageFinished exception, {ex.Message}");
            }
        }

        void InjectJS(string script)
        {
            try
            {
                System.Diagnostics.Debug.Assert(MainThread.IsMainThread);
                if (Control != null)
                {
                    Control.EvaluateJavascript(script, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebViewRenderer::InjectJS exception, {ex.Message}");
            }
        }

        private void Scripts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsScriptReady)
            {
                System.Diagnostics.Debug.Assert(MainThread.IsMainThread);
                // Inject JS script
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (Element != null) 
                        {
                            Element.GetScripts(out List<string> results);
                            foreach (var script in results)
                            {
                                InjectJS(script);
                            }
                            Element.ClearScripts(results);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error($"HybridWebViewRenderer::Scripts_CollectionChanged exception, {ex.Message}");
                    }
                });
            }
        }
    }
}
