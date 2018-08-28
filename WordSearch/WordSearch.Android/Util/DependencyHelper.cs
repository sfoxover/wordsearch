using System;
using WordSearch.Util;
using WordSearch.Droid.Util;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(DependencyHelper))]
namespace WordSearch.Droid.Util
{
    public class DependencyHelper : IDependencyHelper
    {
        public string GetLocalHtmlPath()
        {
            return "file:///android_asset/";
        }

        // get base path to button images
        public string GetResourceImagesPath()
        {
            return "";
        }

        // Get database file path.
        string IDependencyHelper.GetLocalDatabaseFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}