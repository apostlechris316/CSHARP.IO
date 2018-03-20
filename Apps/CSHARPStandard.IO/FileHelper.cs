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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSHARPStandard.IO
{
    /// <summary>
    /// C# Class containing Static Methods to help working with files and file paths.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Look at all files in a given folder to see if they could have been new or edited.
        /// </summary>
        /// <param name="firstFolder"></param>
        /// <param name=""></param>
        /// <param name="recursive"></param>
        /// <param name="checkContents">If selected compares the actual contents of the file if the lengths match otherwise just does quick check of date, file meta and size check.</param>
        /// <returns></returns>
        /// <remarks> NEW IN v1.0.0.13
        /// If you pass checkContents of false, then the results may not be totally accurrate</remarks>
        public Dictionary<FileInfo, string> GetPossibleNewOrEditedFilesInFolders(string firstFolder, string secondFolder, bool recursive, bool checkContents)
        {
            var results = new Dictionary<FileInfo, string>();

            var filesFirstInDirectory = GetFileListForDirectory(firstFolder);
            var filesInSecondDirectory = GetFileListForDirectory(secondFolder);

            foreach (FileInfo fileInfo in filesFirstInDirectory)
            {
                var secondFileInfo = filesInSecondDirectory.FirstOrDefault(f => f.Name == fileInfo.Name);
                if (secondFileInfo == null)
                {
                    // TO DO: Check if file already in results
                    results.Add(fileInfo, "NEW");
                }
                else
                {
                    // if the size is different no need to check there is a difference
                    if (secondFileInfo.Length != fileInfo.Length)
                    {
                        results.Add(fileInfo, "EDITED");
                    }
                    else
                    {
                        if (checkContents == false)
                        {
                            // if we need to compare length
                            if (FileCompare(fileInfo.FullName, secondFileInfo.FullName) == false)
                            {
                                results.Add(fileInfo, "EDITED");
                            }
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Compares two files to see if they are equal
        /// </summary>
        /// <param name="firstFilePath">Path to the first file to compare</param>
        /// <param name="secondFilePath">Path to the second file to compare</param>
        /// <returns></returns>
        /// <remarks> NEW IN v1.0.0.13
        public bool FileCompare(string firstFilePath, string secondFilePath)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (firstFilePath == secondFilePath)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            using (fs1 = new FileStream(firstFilePath, FileMode.Open))
            {
                using (fs2 = new FileStream(secondFilePath, FileMode.Open))
                {
                    // Check the file sizes. If they are not the same, the files 
                    // are not the same.
                    if (fs1.Length != fs2.Length)
                    {
                        // Return false to indicate files are different
                        return false;
                    }

                    // Read and compare a byte from each file until either a
                    // non-matching set of bytes is found or until the end of
                    // file1 is reached.
                    do
                    {
                        // Read one byte from each file.
                        file1byte = fs1.ReadByte();
                        file2byte = fs2.ReadByte();
                    }
                    while ((file1byte == file2byte) && (file1byte != -1));

                    // Return the success of the comparison. "file1byte" is 
                    // equal to "file2byte" at this point only if the files are 
                    // the same.
                    return ((file1byte - file2byte) == 0);
                }
            }
        }

        /// <summary>
        /// Create a directory if it does not already exist
        /// </summary>
        /// <param name="directory"></param>
        /// <remarks>NEW in v1.0.0.10</remarks>
        public void EnsureDirectoryExists(string directory)
        {
            if(Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

        }

        #region Copy Directory

        /// <summary>
        /// Copies a directory 
        /// </summary>
        /// <param name="sourceFullPath">Full path to directory to copy from</param>
        /// <param name="destinationFullPath">Full path to copy directory to</param>
        /// <remarks>NEW in v1.0.0.8</remarks>
        public void CopyDirectory(string sourceFullPath, string destinationFullPath)
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
        public void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
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
        public DirectoryInfo[] GetSubDirectories(string directoryFullPath)
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
        public string EnsureTrailingDirectorySeparator(string directoryPath)
        {
            return directoryPath + (directoryPath.EndsWith(DirectorySeparator) ? string.Empty : DirectorySeparator);
        }

        /// <summary>
        /// Converts a local absolute file path to Uri
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        /// <returns></returns>
        public Uri ConvertAbsoluteFilePathToUri(string absoluteFilePath)
        {
            return new Uri(absoluteFilePath, UriKind.Absolute);
        }

        /// <summary>
        /// Converts local Uri to Absolute File Path
        /// </summary>
        /// <param name="localUri"></param>
        /// <returns></returns>
        /// <remarks>New In v1.0.0.9</remarks>
        public string ConvertLocalUriToAbsoluteFilePath(Uri localUri)
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
        /// <remarks>Fixed in v1.0.0.11 - Did not return the drive portion</remarks>
        public string GetDirectoryFromFilePath(string fullPath)
        {
            var fileParts = fullPath.Split(DirectorySeparator[0]);

            // (FIXED in v1.0.0.11) if fileparts > 0 then we need to append the first part
            var fileDirectory = (fileParts.Length > 0) ? fileParts[0] : string.Empty;

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
        public string GetDirectoryFolderNameFromDirectoryPath(string fullPath)
        {
            var fileParts = fullPath.Split(DirectorySeparator[0]);
            return fileParts.Length > 0 ? fileParts[fileParts.Length - 1] : "";
        }

        /// <summary>
        /// Returns FileName Portion of FullPath
        /// </summary>
        /// <param name="fullPath">full path to a file</param>
        /// <returns>Filename from full path</returns>
        public string GetFileNameFromFilePath(string fullPath)
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
        public FileInfo [] GetFileListForDirectory(string fileSystemDirectoryPath)
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
        public FileInfo[] GetFilteredFileListForDirectory(string fileSystemDirectoryPath, string searchPattern, bool recursive)
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
        public bool IsFileOverMaxSize(FileInfo fileInfo, long maxFileSize)
        {
            return fileInfo.Length < maxFileSize;
        }
    }
}
