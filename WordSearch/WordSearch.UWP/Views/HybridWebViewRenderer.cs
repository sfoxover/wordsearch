using System;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using WordSearch.Views;

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

        private void Scripts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsScriptReady)
            {
                // Inject JS script
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (Element != null && Element.Scripts != null && Element.Scripts.Count > 0)
                    {
                        foreach (var script in Element.Scripts)
                        {
                            Control.InvokeScriptAsync("eval", new[] { script });
                        }
                        Element.Scripts.Clear();
                    }
                });
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
                foreach (var script in Element.Scripts)
                {
                    await Control.InvokeScriptAsync("eval", new[] { script });
                }
                Element.Scripts.Clear();

                IsScriptReady = true;
            }
        }

        // Javascript call from html page to C#
        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.InvokeAction(e.Value);
        }
    }
}
