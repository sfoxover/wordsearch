using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Util
{
    public interface IDependencyHelper
    {
        string GetLocalHtmlPath();
        // get base path to button images
        string GetResourceImagesPath();
        // Get database file path.
        string GetLocalDatabaseFilePath(string filename);
    }
}
