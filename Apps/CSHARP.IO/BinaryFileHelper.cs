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
    /// Binary File Helper Functions
    /// </summary>
    public static class BinaryFileHelper
    {
        /// <summary>
        /// Opens the file and returns the stream to be read from
        /// </summary>
        /// <param name="fileName">name of file to get contents from</param>
        /// <returns>FileStream object</returns>
        /// <remarks>V1.0.0.3 - New method</remarks>
        public static FileStream ReadStream(string fileName)
        {
            if (File.Exists(fileName) == false) throw new FileNotFoundException("The file (" + fileName + ") does not exist");

            return File.OpenRead(fileName);
        }


        /// <summary>
        /// Reads the full contents of the file to a byte array
        /// </summary>
        /// <param name="fileName">name of file to get contents from</param>
        /// <returns>Array of bytes containing the content</returns>
        public static byte [] ReadContents(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("The file (" + fileName + ") does not exist");

            byte[] array = null;
            using (var reader = File.OpenRead(fileName))
            {
                if (reader.Read(array, 0, Convert.ToInt32(reader.Length)) != reader.Length) return null;
            }
            return array;
        }

        /// <summary>
        /// Writes the contents supplied to a file.
        /// </summary>
        /// <param name="fileName">file to write contents to</param>
        /// <param name="contents">contents to write</param>
        public static void WriteContents(string fileName, byte [] contents)
        {
            // ensure the directory exists
            var fileDirectory = string.Empty;
            var fileParts = fileName.Split('\\');
            for(var partNdx = 1; partNdx < fileParts.Length-1; partNdx++)
            {
                fileDirectory = fileDirectory + "\\" + fileParts[partNdx];
            }
            Directory.CreateDirectory(fileDirectory);

            File.WriteAllBytes(fileName, contents);
        }
    }
}
