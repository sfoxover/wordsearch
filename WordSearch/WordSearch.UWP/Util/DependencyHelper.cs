using System;
using WordSearch.Util;
using WordSearch.UWP.Util;

[assembly: Xamarin.Forms.Dependency(typeof(DependencyHelper))]
namespace WordSearch.UWP.Util
{
    public class DependencyHelper : IDependencyHelper
    {
        public string GetLocalHtmlPath()
        {
            return "ms-appx-web:///Assets/";
        }

        // get base path to button images
        public string GetResourceImagesPath()
        {
            return "html/images/";
        }
    }
}
