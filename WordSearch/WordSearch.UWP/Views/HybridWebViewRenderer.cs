using System;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using WordSearch.Views;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(WordSearch.UWP.Views.HybridWebViewRenderer))]
namespace WordSearch.UWP.Views
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Windows.UI.Xaml.Controls.WebView>
    {
        bool IsScriptReady = false;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && Element != null)
            {
                SetNativeControl(new Windows.UI.Xaml.Controls.WebView());
            }
            if (Control != null)
            {
                if (e.OldElement != null)
                {
                    Control.NavigationCompleted -= OnWebViewNavigationCompleted;
                    Control.ScriptNotify -= OnWebViewScriptNotify;
                }
                if (e.NewElement != null)
                {
                    Control.NavigationCompleted += OnWebViewNavigationCompleted;
                    Control.ScriptNotify += OnWebViewScriptNotify;
                    Control.Source = new Uri(string.Format("ms-appx-web:///html//{0}", Element.Uri));

                    Element.Scripts.CollectionChanged += Scripts_CollectionChanged;
                }
            }
        }      

        async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Add Injection Function
                string jsFunction = "function invokeCSharpAction(data){window.external.notify(data);}";
                await Control.InvokeScriptAsync("eval", new[] { jsFunction });

                // Inject JS script
                Element.GetScripts(out List<string> results);
                foreach (var script in results)
                {
                    await Control.InvokeScriptAsync("eval", new[] { script });
                }
                Element.ClearScripts(results);

                IsScriptReady = true;
            }
        }

        // Javascript call from html page to C#
        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.InvokeAction(e.Value);
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
                            Control.InvokeScriptAsync("eval", new[] { script });
                        }
                        Element.ClearScripts(results);
                    }
                });
            }
        }
    }
}
