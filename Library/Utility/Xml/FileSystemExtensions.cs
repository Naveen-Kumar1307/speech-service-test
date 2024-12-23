using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Xml
{
    /// <summary>
    /// Extends some baseline file system classes.
    /// </summary>
    public static class FileSystemExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileSystemExtensions));

        #region folder extensions
        /// <summary>
        /// Creates this folder if it does not exist.
        /// </summary>
        /// <param name="folder">a file system folder</param>
        /// <returns>this folder (after creating it if needed)</returns>
        public static DirectoryInfo CreateIfMissing(this DirectoryInfo folder)
        {
            try
            {
                if (folder != null && !folder.Exists) folder.Create();
            }
            catch (Exception ex)
            {
                Logger.Error("Folder creation failed " + folder.FullName, ex);
            }

            return folder;
        }
        #endregion

    } // FileSystemExtensions
}
