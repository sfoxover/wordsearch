using System;
using WordSearch.Util;
using WordSearch.Droid.Util;
using System.IO;
using System.Threading.Tasks;

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
        public string GetLocalDatabaseFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        // Get database file path from Assets folder.
        public string GetAssetsDatabaseFilePath(string filename)
        {
            return "file:///android_asset/" + filename;
        }

        // Check for words db and copy from resource if needed.
        public async Task<Tuple<bool, string>> CheckWordsDBFileExists(string filename)
        {
            return new Tuple<bool, string>(true, "");
        }
    }
}