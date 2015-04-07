using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;

namespace TestAgent.Services.FileService
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        UseSynchronizationContext = false,
        AddressFilterMode = AddressFilterMode.Any)]
    public class FileService : IFileService
    {
        public FileService()
        {
            TokenFiles = new Dictionary<string, string>();
        }

        public string UploadDirectory { get; set; }

        public Dictionary<string, string> TokenFiles;

        public UploadAcknowledgement UploadFile(UploadRequest request)
        {
            var token = Guid.NewGuid().ToString("N");

            try
            {
                // Try to create the upload directory if it does not yet exist
                var fullDirectory = Path.Combine(UploadDirectory, token);
                if (!Directory.Exists(fullDirectory))
                {
                    Directory.CreateDirectory(fullDirectory);
                }

                // Check if a file with the same filename is already 
                // present in the upload directory. If this is the case 
                // then delete this file
                string path = Path.Combine(fullDirectory, Path.GetFileName(request.FileName));
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // Read the incoming stream and save it to file
                const int bufferSize = 2048;
                var buffer = new byte[bufferSize];
                using (var outputStream = new FileStream(path,FileMode.Create, FileAccess.Write))
                {
                    int bytesRead = request.Stream.Read(buffer, 0, bufferSize);
                    while (bytesRead > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                        bytesRead = request.Stream.Read(buffer, 0, bufferSize);
                    }
                    outputStream.Close();
                }

                // Extract the files in the directory
                ZipFile.ExtractToDirectory(path, fullDirectory);

                // Delete the original file
                File.Delete(path);

                TokenFiles.Add(token, path);

                return new UploadAcknowledgement
                {
                    UploadResult = true,
                    Token = token
                };
            }
            catch (Exception)
            {
                return new UploadAcknowledgement
                {
                    UploadResult = false
                };
            }
        }
    }
}