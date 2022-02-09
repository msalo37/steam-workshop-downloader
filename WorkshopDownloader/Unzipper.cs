using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopDownloader
{
    public static class Unzipper
    {
        public static async void UnzipFileAsync(string filePath)
        {
            string extractPath = filePath.Replace(".zip", "");

            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            Directory.CreateDirectory(extractPath);
            await Task.Run(() => ZipFile.ExtractToDirectory(filePath, extractPath));

            File.Delete(filePath);
        }
    }
}
