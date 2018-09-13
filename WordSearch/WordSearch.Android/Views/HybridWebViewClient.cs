using System;
using Android.Webkit;
using WordSearch.Helpers;

namespace WordSearch.Droid.Views
{
    public class HybridWebViewClient : WebViewClient
    {
        readonly HybridWebViewRenderer Parent;

        public HybridWebViewClient(HybridWebViewRenderer parent)
        {
            Parent = parent;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            try
            {
                if (Parent == null)
                    return;
                Parent.OnPageFinished();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebViewClient::OnPageFinished exception, {ex.Message}");
            }
        }
    }
}