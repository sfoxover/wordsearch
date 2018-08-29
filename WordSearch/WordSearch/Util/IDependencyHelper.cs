using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WordSearch.Util
{
    public interface IDependencyHelper
    {
        string GetLocalHtmlPath();
        // get base path to button images
        string GetResourceImagesPath();
        // Get database file path.
        string GetLocalDatabaseFilePath(string filename);
        // Get database file path from Assets folder.
        string GetAssetsDatabaseFilePath(string filename);
        // Check for words db and copy from resource if needed.
        Task<Tuple<bool, string>> CheckWordsDBFileExists(string filename);
    }
}
