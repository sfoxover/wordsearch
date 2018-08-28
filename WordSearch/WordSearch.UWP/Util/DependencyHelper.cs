using System;
using System.IO;
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

        // Get database file path.
        string GetLocalDatabaseFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}
