using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace WordSearch.Views
{
    public class HybridWebView : View
    {
        Action<string> action;

        // Url
        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri", returnType: typeof(string), declaringType: typeof(HybridWebView), defaultValue: default(string));
        // JS scripts to run in web page
        public static readonly BindableProperty ScriptsProperty = BindableProperty.Create(propertyName: "Scripts", returnType: typeof(List<string>), declaringType: typeof(HybridWebView), defaultValue: default(List<string>));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public List<string> Scripts
        {
            get { return (List<string>)GetValue(ScriptsProperty); }
            set { SetValue(ScriptsProperty, value); }
        }

        public HybridWebView() : base()
        {
            Scripts = new List<string>();
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
            if (action == null || data == null)
                return;
            action.Invoke(data);
        }

        // Queue script for call in HybridWebViewRenderer
        public void RunJSScript(string script)
        {
            Scripts.Add(script);
        }
    }
}
