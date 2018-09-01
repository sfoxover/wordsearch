using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using WordSearch.Helpers;
using WordSearch.UWP.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(DependencyHelper))]
namespace WordSearch.UWP.Helpers
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

        // Get database file path for UWP app.
        public string GetLocalDatabaseFilePath(string filename)
        {
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, filename);
            return dbPath;
        }

        // Get database file path from Assets folder.
        public string GetAssetsDatabaseFilePath(string filename)
        {
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, filename);
            return dbPath;
        }

        // Check for words db and copy from resource if needed.
        public async Task<Tuple<bool, string>> CheckWordsDBFileExists(string filename)
        {
            string error = "";
            try
            {
                var filePath = GetAssetsDatabaseFilePath(filename);
                if(File.Exists(filePath))
                    return new Tuple<bool, string>(true, "");
                // Copy file from Assets folder
                var dbFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + filename)).AsTask();
                var file = await dbFile.CopyAsync(ApplicationData.Current.LocalFolder);
                if (file.IsAvailable)
                    return new Tuple<bool, string>(true, "");
                else
                    error = $"CheckWordsDBFileExists failed to copy file {filename}";
            }
            catch(Exception ex)
            {
                error = $"CheckWordsDBFileExists exception, {ex.Message}";
                Debug.WriteLine(error);                
            }
            return new Tuple<bool, string>(false, error);
        }
    }
}
