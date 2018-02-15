/********************************************************************************
 * CSHARP IO Library - General utility to manipulate files in on the Windows 
 * file system 
 * 
 * NOTE: Adapted from Clinch.IO
 * 
 * LICENSE: Free to use provided details on fixes and/or extensions emailed to 
 *          chris.williams@readwatchcreate.com
 ********************************************************************************/

using System.IO;

namespace CSHARPStandard.IO
{
    /// <summary>
    /// Text Stream Helper Functions
    /// </summary>
    public static class TextStreamHelper
    {
        /// <summary>
        /// Reads the full contents of the file to a string
        /// </summary>
        /// <param name="fileInfo">FileInfo object used to open the stream for read</param>
        /// <returns>Contents of the stream in a string</returns>
        /// <remarks>NEW in v1.0.0.9</remarks>
        public static string ReadContents(FileInfo fileInfo)
        {
            var streamReader = new StreamReader(fileInfo.OpenRead());
            var returnValue = streamReader.ReadToEnd();
            streamReader.Close();
            return returnValue;
        }

        /// <summary>
        /// Reads the full contents of the file to a string
        /// </summary>
        /// <param name="stream">Stream to get contents from</param>
        /// <returns>Contents of the stream in a string</returns>
        /// <remarks>Beware as stream passsed in is passed to StreamReader so likely it will be disposed when returned</remarks>
        public static string ReadContents(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var returnValue = streamReader.ReadToEnd();
            streamReader.Close();
            return returnValue;
        }

        /// <summary>
        /// Writes the contents supplied to a file. (Always Overwrites the file. If you wish to append use WriteContents with file path)
        /// </summary>
        /// <param name="fileInfo">FileInfo object used to open the stream for write</param>
        /// <param name="contents">contents to write</param>
        /// <remarks>NEW in v1.0.0.9</remarks>
        public static void WriteContents(FileInfo fileInfo, string contents)
        {
            // ensure the directory exists
            if (fileInfo.DirectoryName != null) Directory.CreateDirectory(fileInfo.DirectoryName);

            // write the contents of the file
            var streamWriter = new StreamWriter(fileInfo.OpenWrite());
            streamWriter.Write(contents);
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// Writes the contents supplied to a file.
        /// </summary>
        /// <param name="fileName">file to write contents to</param>
        /// <param name="contents">contents to write</param>
        /// <param name="overwrite">if true overwrites existing file else appends to end</param>
        /// <remarks>Beware as stream passsed in is passed to StreamWriter so likely it will be disposed when returned</remarks>
        public static void WriteContents(string fileName, string contents, bool overwrite)
        {
            // ensure the directory exists
            Directory.CreateDirectory(FileHelper.GetDirectoryFromFilePath(fileName));

            using (
                var streamWriter = (File.Exists(fileName))
                    ? overwrite ? File.CreateText(fileName) : File.AppendText(fileName)
                    : File.CreateText(fileName))
            {
                streamWriter.Write(contents);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }
    }
}
