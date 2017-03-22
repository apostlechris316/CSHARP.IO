/********************************************************************************
 * CSHARP IO Library - General utility to manipulate files in on the Windows 
 * file system 
 * 
 * NOTE: Adapted from Clinch.IO
 * 
 * LICENSE: Free to use provided details on fixes and/or extensions emailed to 
 *          chris.williams@readwatchcreate.com
 ********************************************************************************/

using System;
using System.IO;

namespace CSHARP.IO
{
    /// <summary>
    /// C# Class containing Static Methods to help working with files and file paths.
    /// </summary>
    public static class FileHelper
    {
        #region Copy Directory

        /// <summary>
        /// Copies a directory 
        /// </summary>
        /// <param name="sourceFullPath">Full path to directory to copy from</param>
        /// <param name="destinationFullPath">Full path to copy directory to</param>
        /// <remarks>NEW in v1.0.0.8</remarks>
        public static void CopyDirectory(string sourceFullPath, string destinationFullPath)
        {
            var sourceDir = new DirectoryInfo(sourceFullPath);
            var destinationDir = new DirectoryInfo(destinationFullPath);

            CopyDirectory(sourceDir, destinationDir);
        }

        /// <summary>
        /// Copies a directory 
        /// </summary>
        /// <param name="source">Directory to copy from</param>
        /// <param name="destination">Directory to</param>
        /// <remarks>NEW in v1.0.0.8</remarks>
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            var files = source.GetFiles();
            foreach (var file in files) file.CopyTo(Path.Combine(destination.FullName, file.Name));

            // Process subdirectories.
            var dirs = source.GetDirectories();
            foreach (var dir in dirs)
            {
                // Get destination directory.
                var destinationDirectory = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDirectory));
            }
        }

        #endregion

        #region GetSubDirectories 

        /// <summary>
        /// returns a list of directories beneath a given directory
        /// </summary>
        /// <param name="directoryFullPath"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetSubDirectories(string directoryFullPath)
        {
            var source = new DirectoryInfo(directoryFullPath);
            return source.GetDirectories();
        }

        #endregion

        /// <summary>
        /// Directory Separator (may be different depending on operation system
        /// </summary>
        /// <remarks>V1.0.0.2 - Proper Case now used</remarks>
        public const string DirectorySeparator = "\\";

        #region FilePath Parsing Related

        /// <summary>
        /// Returns the originally passed in directoryPath but includes Directory seperator if not already at the end.
        /// </summary>
        /// <param name="directoryPath">Full path to directory</param>
        /// <returns>Directory with a slash on the end</returns>
        /// <remarks>NEW IN 1.0.0.1</remarks>
        public static string EnsureTrailingDirectorySeparator(string directoryPath)
        {
            return directoryPath + (directoryPath.EndsWith(DirectorySeparator) ? string.Empty : DirectorySeparator);
        }

        /// <summary>
        /// Converts a local absolute file path to Uri
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        /// <returns></returns>
        public static Uri ConvertAbsoluteFilePathToUri(string absoluteFilePath)
        {
            return new Uri(absoluteFilePath, UriKind.Absolute);
        }

        /// <summary>
        /// Converts local Uri to Absolute File Path
        /// </summary>
        /// <param name="localUri"></param>
        /// <returns></returns>
        /// <remarks>New In v1.0.0.9</remarks>
        public static string ConvertLocalUriToAbsoluteFilePath(Uri localUri)
        {
            string localFilePath = localUri.LocalPath;
            if (localFilePath.StartsWith("\\")) throw new Exception("ConvertLocalUriToAbsoluteFilePath - ERROR: Local Uri is not actual local Uri (" + localFilePath  + ")");

            return localFilePath;
        }

        /// <summary>
        /// Returns Directory Portion of FullPath
        /// </summary>
        /// <param name="fullPath">full path to a file</param>
        /// <returns>Directory portion of file path</returns>
        public static string GetDirectoryFromFilePath(string fullPath)
        {
            var fileDirectory = string.Empty;
            var fileParts = fullPath.Split(DirectorySeparator[0]);
            for (var partNdx = 1; partNdx < fileParts.Length - 1; partNdx++)
            {
                fileDirectory = fileDirectory + DirectorySeparator + fileParts[partNdx];
            }

            return fileDirectory;
        }

        /// <summary>
        /// Returns Last DirectoryFolderName from of DirectoryPath
        /// </summary>
        /// <param name="fullPath">full path to a directory folder</param>
        /// <returns>Directory folder name from full path</returns>
        public static string GetDirectoryFolderNameFromDirectoryPath(string fullPath)
        {
            var fileParts = fullPath.Split(DirectorySeparator[0]);
            return fileParts.Length > 0 ? fileParts[fileParts.Length - 1] : "";
        }

        /// <summary>
        /// Returns FileName Portion of FullPath
        /// </summary>
        /// <param name="fullPath">full path to a file</param>
        /// <returns>Filename from full path</returns>
        public static string GetFileNameFromFilePath(string fullPath)
        {
            var fileParts = fullPath.Split(DirectorySeparator[0]);
            return fileParts.Length > 0 ? fileParts[fileParts.Length - 1] : "";
        }

        #endregion

        #region File List for Directory 

        /// <summary>
        /// Returns a list of FileInfo for files in a given directory
        /// </summary>
        /// <param name="fileSystemDirectoryPath">Full path to directory containing the files</param>
        /// <returns>An array of FileInfo object for each file in the directory</returns>
        public static FileInfo [] GetFileListForDirectory(string fileSystemDirectoryPath)
        {
            // Get the list of files
            var di = new DirectoryInfo(fileSystemDirectoryPath);
            return di.GetFiles();
        }

        /// <summary>
        /// Returns a list of FileInfo for files in a given directory
        /// </summary>
        /// <param name="fileSystemDirectoryPath">Full path to directory containing the files</param>
        /// <param name="searchPattern">Filter used to determine which files get returned</param>
        /// <param name="recursive">If true, returns all files for all subdirectories as well</param>
        /// <returns>An array of FileInfo object for each file in the directory matching the filter</returns>
        public static FileInfo[] GetFilteredFileListForDirectory(string fileSystemDirectoryPath, string searchPattern, bool recursive)
        {
            // Get the list of files
            var di = new DirectoryInfo(fileSystemDirectoryPath);
            return di.GetFiles(searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
        
        #endregion

        /// <summary>
        /// Returns true if file size is greater than maximum file size
        /// </summary>
        /// <param name="fileInfo">File Information Object</param>
        /// <param name="maxFileSize">Maximum File Size</param>
        /// <returns>True, if the file size is greater than the max file size passed in</returns>
        /// <remarks>NEW IN V1.0.0.4</remarks>
        public static bool IsFileOverMaxSize(FileInfo fileInfo, long maxFileSize)
        {
            return fileInfo.Length < maxFileSize;
        }
    }
}
