using System;
using WordSearch.Droid.Helpers;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Android.Content;
using WordSearch.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(DependencyHelper))]
namespace WordSearch.Droid.Helpers
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
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);            
        }

        // Check for words db and copy from resource if needed.
        public async Task<Tuple<bool, string>> CheckWordsDBFileExists(string filename)
        {
            string error = "";
            try
            {
                var filePath = GetAssetsDatabaseFilePath(filename);
                if (File.Exists(filePath))
                    return new Tuple<bool, string>(true, "");

                // Copy file from Assets folder
                var dbFile = "file:///android_asset/" + filename;
                var assetManager = Android.App.Application.Context.Assets;
                using (var streamReader = new StreamReader(assetManager.Open(filename)))
                {
                    using (Stream outStream = File.Create(filePath))
                    {
                        streamReader.BaseStream.CopyTo(outStream);
                    }
                }
                if (File.Exists(filePath))
                {
                    return new Tuple<bool, string>(true, "");
                }
                else
                {
                    error = $"CheckWordsDBFileExists failed to copy file {filename}";
                    Logger.Instance.Error(error);
                }
            }
            catch (Exception ex)
            {
                error = $"CheckWordsDBFileExists exception, {ex.Message}";
                Logger.Instance.Error(error);
            }
            return new Tuple<bool, string>(false, error);
        }
    }
}