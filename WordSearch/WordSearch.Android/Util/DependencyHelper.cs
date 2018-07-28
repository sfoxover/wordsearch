using System;
using WordSearch.Util;
using WordSearch.Droid.Util;

[assembly: Xamarin.Forms.Dependency(typeof(DependencyHelper))]
namespace WordSearch.Droid.Util
{
    public class DependencyHelper : IDependencyHelper
    {
        public string GetLocalHtmlPath()
        {
            return "file:///android_asset/";
        }
    }
}