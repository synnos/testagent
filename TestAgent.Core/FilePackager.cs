using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace TestAgent.Core
{
    /// <summary>
    /// Contains methods to package files together
    /// </summary>
    public class FilePackager
    {
        /// <summary>
        /// Takes a list of files and creates a zip file with those contents
        /// </summary>
        /// <param name="outputFile">The resulting zip filename</param>
        /// <param name="filenamesToCompress">The files to add to the zip package</param>
        public static void CompressFiles(string outputFile, params string[] filenamesToCompress)
        {
            // Make sure that at least one filename is specified
            if (filenamesToCompress.Length == 0)
            {
                throw new ArgumentException("At least one file to compress should be specified!", "filenamesToCompress");
            }

            // Make sure that all the files specified exist
            var filesThatDoNotExist = filenamesToCompress.Where(f => !File.Exists(f)).ToArray();
            if (filesThatDoNotExist.Length > 0)
            {
                string missingFiles = filesThatDoNotExist.Aggregate(string.Empty, (current, file) => current + (file + Environment.NewLine));

                throw new ArgumentException("Some of the files specified do not exist: " + Environment.NewLine + missingFiles, "filenamesToCompress");
            }

            // Make sure that the output file specified is valid
            if (string.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentException("The output file specified cannot be empty", "outputFile");
            }

            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TemporaryForCompression");

            // Delete any existing files in temporary directory
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            // Recreate the directory
            Directory.CreateDirectory(directoryPath);

            // Copy all files specified to a temporary directory
            foreach (var file in filenamesToCompress)
            {
                string filename = Path.GetFileName(file);

                if (string.IsNullOrEmpty(filename))
                {
                    throw new Exception("Could not get the full filename of " + file);
                }

                File.Copy(file, Path.Combine(directoryPath, filename));
            }

            // If the output file already exists then delete it (overwrite)
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            // Compress the directory contents
            ZipFile.CreateFromDirectory(directoryPath, outputFile);
        }
    }
}
