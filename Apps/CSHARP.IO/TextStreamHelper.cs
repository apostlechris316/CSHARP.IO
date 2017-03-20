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

namespace CSHARP.IO
{
    /// <summary>
    /// Text Stream Helper Functions
    /// </summary>
    public static class TextStreamHelper
    {
        /// <summary>
        /// Reads the full contents of the file to a string
        /// </summary>
        /// <param name="stream">Stream to get contents from</param>
        /// <returns>Contents of the stream in a string</returns>
        /// <remarks>Beware as stream passsed in is passed to StreamReader so likely it will be disposed when returned</remarks>
        public static string ReadContents(Stream stream)
        {
            string returnValue;

            using (var streamReader = new StreamReader(stream))
            {
                returnValue = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return returnValue;
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
