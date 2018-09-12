using System;
using System.Collections.Generic;
using Android.Content;
using WordSearch.Droid.Helpers;
using WordSearch.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

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
            base.OnElementChanged(e);

            if (Control == null && Element != null)
            {
                var webView = new Android.Webkit.WebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                SetNativeControl(webView);
            }
            if (Control != null)
            {
                if (e.OldElement != null)
                {
                    Control.RemoveJavascriptInterface("jsBridge");
                    var hybridWebView = e.OldElement as HybridWebView;
                    hybridWebView.Cleanup();
                }
                if (e.NewElement != null)
                {
                    Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                    Control.LoadUrl(string.Format("file:///android_asset/html/{0}", Element.Uri));

                    // Add Injection Function
                    string jsFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
                    InjectJS(jsFunction);

                    // Inject JS script
                    Element.GetScripts(out List<string> results);
                    foreach (var script in results)
                    {
                        InjectJS(script);
                    }
                    Element.ClearScripts(results);

                    IsScriptReady = true;
                }
            }
        }

        void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        private void Scripts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsScriptReady)
            {
                // Inject JS script
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (Element != null) // && Element.Scripts != null && Element.Scripts.Count > 0)
                    {
                        Element.GetScripts(out List<string> results);
                        foreach (var script in results)
                        {
                            InjectJS(script);
                        }
                        Element.ClearScripts(results);
                    }
                });
            }
        }
    }
}
