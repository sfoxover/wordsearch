using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WordSearch.Helpers;
using Xamarin.Forms;

namespace WordSearch.Views
{
    public class HybridWebView : View
    {
        Action<string> action;

        // Url
        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri", returnType: typeof(string), declaringType: typeof(HybridWebView), defaultValue: default(string));
        // JS scripts to run in web page
        public static readonly BindableProperty ScriptsProperty = BindableProperty.Create(propertyName: "Scripts", returnType: typeof(ObservableCollection<string>), declaringType: typeof(HybridWebView), defaultValue: default(ObservableCollection<string>));
        internal static object ScriptsPropertyLock = new object();

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public ObservableCollection<string> Scripts
        {
            get { return (ObservableCollection<string>)GetValue(ScriptsProperty); }
            set { SetValue(ScriptsProperty, value); }
        }

        public HybridWebView() : base()
        {
            Scripts = new ObservableCollection<string>();
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        // Call from Javascript to C#
        public void InvokeAction(string data)
        {
            try
            {
                if (action == null || data == null)
                return;
                action.Invoke(data);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebView::InvokeAction exception, {ex.Message}");
            }
        }

        // Queue script for call in HybridWebViewRenderer
        public void RunJSScript(string script)
        {
            try
            { 
                Scripts.Add(script);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"HybridWebView::RunJSScript exception, {ex.Message}");
            }
        }

        public static string InjectedFunction
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "function csharp(data){bridge.invokeAction(data);}";
                    case Device.iOS:
                        return "function csharp(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
                    default:
                        return "function csharp(data){window.external.notify(data);}";
                }
            }
        }
    }
}
