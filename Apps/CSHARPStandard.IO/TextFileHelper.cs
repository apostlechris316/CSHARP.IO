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
using System.Text;

namespace CSHARPStandard.IO
{
    /// <summary>
    /// Text File Helper Functions
    /// </summary>
    public class TextFileHelper
    {
        /// <summary>
        /// Opens the input file stream
        /// </summary>
        /// <param name="fileName">Full path to file</param>
        /// <returns>Stream to read from</returns>
        /// <remarks>NEW in V1.0.0.5
        /// V1.0.0.7 - Notice that CloseInputFileStream was removed.  Use "Using" instead as it will handle Close and Dispose automatically for you. 
        /// </remarks>
        public StreamReader OpenInputFileStream(string fileName)
        {
            return File.OpenText(fileName);
        }

        /// <summary>
        /// Gets the line count for a single file (even blank ones)
        /// </summary>
        /// <param name="fileName">Full path to the file to count lines</param>
        /// <returns></returns>
        /// <remarks>
        /// NEW in v1.0.0.15
        /// </remarks>
        public int GetLineCount(string fileName)
        {
            return GetLineCount(fileName, false);
        }

        /// <summary>
        /// Gets the line count for a single file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ignoreBlankLines">If true, will only count lines with text</param>
        /// <returns></returns>
        /// <remarks>
        /// NEW in v1.0.0.15
        /// Ported from https://www.codeproject.com/Articles/9105/Counting-Lines-of-Code-in-C to .NET Standard Library
        /// </remarks>
        public int GetLineCount(string fileName, bool ignoreBlankLines)
        { 
            if (File.Exists(fileName) == false)
                throw new FileNotFoundException("The file (" + fileName + ") does not exist");

            int lines = 0;
            try
            {
                using (var streamReader = File.OpenText(fileName))
                {
                    var lineRead = streamReader.ReadLine();
                    while (lineRead != null)
                    {
                        // if we are counting all lines or line is not empty
                        if (ignoreBlankLines == false || (string.IsNullOrEmpty(lineRead.Trim()) == false))
                        {
                            lines++;
                        }

                        // Get the next line
                        lineRead = streamReader.ReadLine();
                    }
                }
            }
            catch
            {
            }

            return lines;
        }

        /// <summary>
        /// Reads the full contents of the file to a string
        /// </summary>
        /// <param name="fileName">name of file to get contents from</param>
        /// <returns>full contents of the file as a string</returns>
        /// <remarks>v1.0.0.7 Converted to using and removed redundant Close and Dispose</remarks>
        public string ReadContents(string fileName)
        {
            if (File.Exists(fileName) == false)
                throw new FileNotFoundException("The file (" + fileName + ") does not exist");

            using (var streamReader = File.OpenText(fileName))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the full contents of the file to a string using specific encoding
        /// </summary>
        /// <param name="fileName">name of file to get contents from</param>
        /// <param name="encoding"></param>
        /// <returns>full contents of the file as a string</returns>
        /// <remarks>v1.0.0.7 Converted to using and removed redundant Close and Dispose<br/>
        /// v1.0.0.8 Now supports passing in encoding</remarks>
        public string ReadContents(string fileName, string encoding)
        {
            if (File.Exists(fileName) == false)
                throw new FileNotFoundException("The file (" + fileName + ") does not exist");

            switch (encoding.ToUpperInvariant())
            {
                case "ASCII":
                    return File.ReadAllText(fileName, Encoding.ASCII);
                case "UTF7":
                    return File.ReadAllText(fileName, Encoding.UTF7);
                case "UTF8":
                    return File.ReadAllText(fileName, Encoding.UTF8);
                case "UTF32":
                    return File.ReadAllText(fileName, Encoding.UTF32);
                case "UNICODE":
                    return File.ReadAllText(fileName, Encoding.Unicode);
                default:
                    return File.ReadAllText(fileName);
            }
        }

        /// <summary>
        /// Writes the contents supplied to a file.
        /// </summary>
        /// <param name="fileName">file to write contents to</param>
        /// <param name="contents">contents to write</param>
        /// <param name="overwrite">if true overwrites existing file else appends to end</param>
        /// <remarks>v1.0.0.7 Converted to using and removed redundant Close and Dispose.
        /// v1.0.0.12 Optimized by getting directory path once instead of twice</remarks>
        public void WriteContents(string fileName, string contents, bool overwrite)
        {
            // ensure the directory exists
            var fileHelper = new FileHelper();
            var directory = fileHelper.GetDirectoryFromFilePath(fileName);
            if (string.IsNullOrEmpty(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            using (
                var streamWriter = (File.Exists(fileName))
                    ? overwrite ? File.CreateText(fileName) : File.AppendText(fileName)
                    : File.CreateText(fileName))
            {
                streamWriter.Write(contents);
                streamWriter.Flush();
            }
        }

        /// <summary>
        /// Writes the contents supplied to a file.
        /// </summary>
        /// <param name="fileName">file to write contents to</param>
        /// <param name="contents">contents to write</param>
        /// <param name="overwrite">if true overwrites existing file else appends to end</param>
        /// <param name="encoding">Determines how contents is encoded on write possible values are (ASCII,UTF7,UTF8,UTF32, or empty string). If empty string it uses the default encoding.</param>
        /// <remarks>NEW in v1.0.0.8 </remarks>
        public void WriteContents(string fileName, string contents, bool overwrite, string encoding)
        {
            // ensure the directory exists
            var fileHelper = new FileHelper();
            if (string.IsNullOrEmpty(fileHelper.GetDirectoryFromFilePath(fileName)) == false)
            {
                Directory.CreateDirectory(fileHelper.GetDirectoryFromFilePath(fileName));
            }

            if (overwrite)
            {
                switch (encoding.ToUpperInvariant())
                {
                    case "ASCII":
                        File.WriteAllText(fileName, contents, Encoding.ASCII);
                        return;
                    case "UTF7":
                        File.WriteAllText(fileName, contents, Encoding.UTF7);
                        return;
                    case "UTF8":
                        File.WriteAllText(fileName, contents, Encoding.UTF8);
                        return;
                    case "UTF32":
                        File.WriteAllText(fileName, contents, Encoding.UTF32);
                        return;
                    case "UNICODE":
                        File.WriteAllText(fileName, contents, Encoding.Unicode);
                        return;
                    default:
                        File.WriteAllText(fileName, contents);
                        return;
                }
            }
            else
            {
                switch (encoding.ToUpperInvariant())
                {
                    case "ASCII":
                        File.AppendAllText(fileName, contents, Encoding.ASCII);
                        return;
                    case "UTF7":
                        File.AppendAllText(fileName, contents, Encoding.UTF7);
                        return;
                    case "UTF8":
                        File.AppendAllText(fileName, contents, Encoding.UTF8);
                        return;
                    case "UTF32":
                        File.AppendAllText(fileName, contents, Encoding.UTF32);
                        return;
                    case "UNICODE":
                        File.AppendAllText(fileName, contents, Encoding.Unicode);
                        return;
                    default:
                        File.AppendAllText(fileName, contents);
                        return;
                }
            }
        }

        /// <summary>
        /// Opens a file, replaces a string with another string then saves the file
        /// </summary>
        /// <param name="fileName">Full path to file</param>
        /// <param name="find">Text to find</param>
        /// <param name="replace">Text to replace</param>
        /// <remarks>NEW in 1.0.0.8 </remarks>
        public void ReplaceInFile(string fileName, string find, string replace)
        {
            var contents = ReadContents(fileName);
            WriteContents(fileName, contents.Replace(find, replace), true);
        }
    }
}
